#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading;

namespace System.Net.Mime
{
    internal class MimeMultiPart : MimeBasePart
    {
        private bool _allowUnicode;
        private static int _boundary;
        private AsyncCallback _mimePartSentCallback;
        private Collection<MimeBasePart> _parts;

        internal class MimePartContext
        {
            internal bool completed;
            internal bool completedSynchronously = true;
            internal Stream outputStream;
            internal IEnumerator<MimeBasePart> partsEnumerator;
            internal LazyAsyncResult result;
            internal BaseWriter writer;

            internal MimePartContext(BaseWriter writer, LazyAsyncResult result, IEnumerator<MimeBasePart> partsEnumerator)
            {
                this.writer = writer;
                this.result = result;
                this.partsEnumerator = partsEnumerator;
            }
        }

        internal MimeMultiPart(MimeMultiPartType type)
        {
            MimeMultiPartType = type;
        }

        internal override IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
        {
            _allowUnicode = allowUnicode;
            base.PrepareHeaders(allowUnicode);
            writer.WriteHeaders(base.Headers, allowUnicode);
            var result = new MimeBasePart.MimePartAsyncResult(this, state, callback);
            var context = new MimePartContext(writer, result, Parts.GetEnumerator());
            var result2 = writer.BeginGetContentStream(new AsyncCallback(ContentStreamCallback), context);
            if (result2.CompletedSynchronously)
                ContentStreamCallbackHandler(result2);
            return result;
        }

        internal void Complete(IAsyncResult result, Exception e)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            if (asyncState.completed)
                throw e;
            try { asyncState.outputStream.Close(); }
            catch (Exception exception)
            {
                if (e == null)
                    e = exception;
            }
            asyncState.completed = true;
            asyncState.result.InvokeCallback(e);
        }

        internal void ContentStreamCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((MimePartContext)result.AsyncState).completedSynchronously = false;
                try { ContentStreamCallbackHandler(result); }
                catch (Exception exception) { Complete(result, exception); }
            }
        }

        private void ContentStreamCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            asyncState.outputStream = asyncState.writer.EndGetContentStream(result);
            asyncState.writer = new MimeWriter(asyncState.outputStream, base.ContentType.Boundary);
            if (asyncState.partsEnumerator.MoveNext())
            {
                var current = asyncState.partsEnumerator.Current;
                _mimePartSentCallback = new AsyncCallback(MimePartSentCallback);
                var result2 = current.BeginSend(asyncState.writer, _mimePartSentCallback, _allowUnicode, asyncState);
                if (result2.CompletedSynchronously)
                    MimePartSentCallbackHandler(result2);
            }
            else
            {
                var result3 = ((MimeWriter)asyncState.writer).BeginClose(new AsyncCallback(MimeWriterCloseCallback), asyncState);
                if (result3.CompletedSynchronously)
                    MimeWriterCloseCallbackHandler(result3);
            }
        }

        internal string GetNextBoundary()
        {
            var num = Interlocked.Increment(ref _boundary) - 1;
            return ("--boundary_" + num.ToString(CultureInfo.InvariantCulture) + "_" + Guid.NewGuid().ToString(null, CultureInfo.InvariantCulture));
        }

        internal void MimePartSentCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((MimePartContext)result.AsyncState).completedSynchronously = false;
                try { MimePartSentCallbackHandler(result); }
                catch (Exception exception) { Complete(result, exception); }
            }
        }

        private void MimePartSentCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            asyncState.partsEnumerator.Current.EndSend(result);
            if (asyncState.partsEnumerator.MoveNext())
            {
                var result2 = asyncState.partsEnumerator.Current.BeginSend(asyncState.writer, _mimePartSentCallback, _allowUnicode, asyncState);
                if (result2.CompletedSynchronously)
                    MimePartSentCallbackHandler(result2);
            }
            else
            {
                var result3 = ((MimeWriter)asyncState.writer).BeginClose(new AsyncCallback(MimeWriterCloseCallback), asyncState);
                if (result3.CompletedSynchronously)
                    MimeWriterCloseCallbackHandler(result3);
            }
        }

        internal void MimeWriterCloseCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((MimePartContext)result.AsyncState).completedSynchronously = false;
                try { MimeWriterCloseCallbackHandler(result); }
                catch (Exception exception) { Complete(result, exception); }
            }
        }

        private void MimeWriterCloseCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            ((MimeWriter)asyncState.writer).EndClose(result);
            Complete(result, null);
        }

        internal override void Send(BaseWriter writer, bool allowUnicode)
        {
            base.PrepareHeaders(allowUnicode);
            writer.WriteHeaders(base.Headers, allowUnicode);
            var contentStream = writer.GetContentStream();
            var writer2 = new MimeWriter(contentStream, base.ContentType.Boundary);
            foreach (var part in Parts)
                part.Send(writer2, allowUnicode);
            writer2.Close();
            contentStream.Close();
        }

        private void SetType(MimeMultiPartType type)
        {
            base.ContentType.MediaType = "multipart/" + type.ToString().ToLower(CultureInfo.InvariantCulture);
            base.ContentType.Boundary = GetNextBoundary();
        }

        internal MimeMultiPartType MimeMultiPartType
        {
            set
            {
                if (value > MimeMultiPartType.Related || value < MimeMultiPartType.Mixed)
                    throw new NotSupportedException(value.ToString());
                SetType(value);
            }
        }

        internal Collection<MimeBasePart> Parts
        {
            get
            {
                if (_parts == null)
                    _parts = new Collection<MimeBasePart>();
                return _parts;
            }
        }
    }
}

