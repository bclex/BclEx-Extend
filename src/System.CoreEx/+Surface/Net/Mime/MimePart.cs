#region Foreign-License
// .Net40 Surface
#endregion
using System.IO;
using System.Net.Mail;
using System.Runtime;

namespace System.Net.Mime
{
    internal class MimePart : MimeBasePart, IDisposable
    {
        private const int _maxBufferSize = 0x4400;
        private AsyncCallback _readCallback;
        private Stream _stream;
        private bool _streamSet;
        private bool _streamUsedOnce;
        private AsyncCallback _writeCallback;

        internal class MimePartContext
        {
            internal byte[] buffer;
            internal int bytesLeft;
            internal bool completed;
            internal bool completedSynchronously = true;
            internal Stream outputStream;
            internal LazyAsyncResult result;
            internal BaseWriter writer;

            internal MimePartContext(BaseWriter writer, LazyAsyncResult result)
            {
                this.writer = writer;
                this.result = result;
                this.buffer = new byte[0x4400];
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal MimePart() { }
        internal override IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
        {
            base.PrepareHeaders(allowUnicode);
            writer.WriteHeaders(base.Headers, allowUnicode);
            var result = new MimeBasePart.MimePartAsyncResult(this, state, callback);
            var context = new MimePartContext(writer, result);
            ResetStream();
            _streamUsedOnce = true;
            var result2 = writer.BeginGetContentStream(new AsyncCallback(ContentStreamCallback), context);
            if (result2.CompletedSynchronously)
                ContentStreamCallbackHandler(result2);
            return result;
        }

        internal void Complete(IAsyncResult result, Exception e)
        {
            MimePartContext asyncState = (MimePartContext)result.AsyncState;
            if (asyncState.completed)
                throw e;
            try
            {
                if (asyncState.outputStream != null)
                    asyncState.outputStream.Close();
            }
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

        internal void ContentStreamCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            var stream = asyncState.writer.EndGetContentStream(result);
            asyncState.outputStream = GetEncodedStream(stream);
            _readCallback = new AsyncCallback(ReadCallback);
            _writeCallback = new AsyncCallback(WriteCallback);
            var result2 = Stream.BeginRead(asyncState.buffer, 0, asyncState.buffer.Length, _readCallback, asyncState);
            if (result2.CompletedSynchronously)
                ReadCallbackHandler(result2);
        }

        public void Dispose()
        {
            if (_stream != null)
                _stream.Close();
        }

        internal Stream GetEncodedStream(Stream stream)
        {
            var stream2 = stream;
            if (TransferEncoding == TransferEncoding.Base64)
                return new Base64Stream(stream2, new Base64WriteStateInfo());
            if (TransferEncoding == TransferEncoding.QuotedPrintable)
                return new QuotedPrintableStream(stream2, true);
#if CLR45
            if (TransferEncoding != TransferEncoding.SevenBit && TransferEncoding != TransferEncoding.EightBit)
                return stream2;
            return new EightBitStream(stream2);
#else
            return stream2;
#endif
        }

        internal void ReadCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((MimePartContext)result.AsyncState).completedSynchronously = false;
                try { ReadCallbackHandler(result); }
                catch (Exception exception) { Complete(result, exception); }
            }
        }

        internal void ReadCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            asyncState.bytesLeft = Stream.EndRead(result);
            if (asyncState.bytesLeft > 0)
            {
                var result2 = asyncState.outputStream.BeginWrite(asyncState.buffer, 0, asyncState.bytesLeft, _writeCallback, asyncState);
                if (result2.CompletedSynchronously)
                    WriteCallbackHandler(result2);
            }
            else
                Complete(result, null);
        }

        internal void ResetStream()
        {
            if (_streamUsedOnce)
            {
                if (!Stream.CanSeek)
                    throw new InvalidOperationException(SR.GetString("MimePartCantResetStream"));
                Stream.Seek(0L, SeekOrigin.Begin);
                _streamUsedOnce = false;
            }
        }

        internal override void Send(BaseWriter writer, bool allowUnicode)
        {
            if (Stream != null)
            {
                int num;
                var buffer = new byte[0x4400];
                base.PrepareHeaders(allowUnicode);
                writer.WriteHeaders(base.Headers, allowUnicode);
                var contentStream = writer.GetContentStream();
                contentStream = GetEncodedStream(contentStream);
                ResetStream();
                _streamUsedOnce = true;
                while ((num = Stream.Read(buffer, 0, 0x4400)) > 0)
                    contentStream.Write(buffer, 0, num);
                contentStream.Close();
            }
        }

        internal void SetContent(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (_streamSet)
            {
                _stream.Close();
                _stream = null;
                _streamSet = false;
            }
            _stream = stream;
            _streamSet = true;
            _streamUsedOnce = false;
            TransferEncoding = TransferEncoding.Base64;
        }

        internal void SetContent(Stream stream, ContentType contentType)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            base._contentType = contentType;
            SetContent(stream);
        }

        internal void SetContent(Stream stream, string name, string mimeType)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (mimeType != null && mimeType != string.Empty)
                base._contentType = new ContentType(mimeType);
            if (name != null && name != string.Empty)
                base.ContentType.Name = name;
            SetContent(stream);
        }

        internal void WriteCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((MimePartContext)result.AsyncState).completedSynchronously = false;
                try { WriteCallbackHandler(result); }
                catch (Exception exception) { Complete(result, exception); }
            }
        }

        internal void WriteCallbackHandler(IAsyncResult result)
        {
            var asyncState = (MimePartContext)result.AsyncState;
            asyncState.outputStream.EndWrite(result);
            var result2 = Stream.BeginRead(asyncState.buffer, 0, asyncState.buffer.Length, _readCallback, asyncState);
            if (result2.CompletedSynchronously)
                ReadCallbackHandler(result2);
        }

        internal ContentDisposition ContentDisposition
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return base._contentDisposition; }
            set
            {
                base._contentDisposition = value;
                if (value == null)
                    ((HeaderCollection)base.Headers).InternalRemove(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition));
                else
                    base._contentDisposition.PersistIfNeeded((HeaderCollection)base.Headers, true);
            }
        }

        internal Stream Stream
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _stream; }
        }

        internal TransferEncoding TransferEncoding
        {
            get
            {
                var str = base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)];
                if (str.Equals("base64", StringComparison.OrdinalIgnoreCase))
                    return TransferEncoding.Base64;
                if (str.Equals("quoted-printable", StringComparison.OrdinalIgnoreCase))
                    return TransferEncoding.QuotedPrintable;
                if (str.Equals("7bit", StringComparison.OrdinalIgnoreCase))
                    return TransferEncoding.SevenBit;
#if CLR45
                if (str.Equals("8bit", StringComparison.OrdinalIgnoreCase))
                    return TransferEncoding.EightBit;
#endif
                return TransferEncoding.Unknown;
            }
            set
            {
                if (value == TransferEncoding.Base64)
                    base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "base64";
                else if (value == TransferEncoding.QuotedPrintable)
                    base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "quoted-printable";
                else if (value == TransferEncoding.SevenBit)
                    base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "7bit";
#if CLR45
                else
                {
                    if (value != TransferEncoding.EightBit)
                        throw new NotSupportedException(SR.GetString("MimeTransferEncodingNotSupported", new object[] { value }));
                    base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "8bit";
                }
#endif
            }
        }
    }
}

