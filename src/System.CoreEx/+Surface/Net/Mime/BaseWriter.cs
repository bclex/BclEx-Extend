#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;
using System.Runtime;

namespace System.Net.Mime
{
    internal abstract class BaseWriter
    {
        protected BufferBuilder _bufferBuilder;
        protected Stream _contentStream;
        protected static byte[] CRLF = new byte[] { 13, 10 };
        private static int DefaultLineLength = 0x4c;
        protected bool _isInContent;
        private int _lineLength;
        private EventHandler _onCloseHandler;
        private static AsyncCallback _onWrite = new AsyncCallback(BaseWriter.OnWrite);
        private bool _shouldEncodeLeadingDots;
        protected Stream _stream;

        protected BaseWriter(Stream stream, bool shouldEncodeLeadingDots)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            _stream = stream;
            _shouldEncodeLeadingDots = shouldEncodeLeadingDots;
            _onCloseHandler = new EventHandler(OnClose);
            _bufferBuilder = new BufferBuilder();
            _lineLength = DefaultLineLength;
        }

        internal IAsyncResult BeginGetContentStream(AsyncCallback callback, object state)
        {
            var multiResult = new MultiAsyncResult(this, callback, state);
            var contentStream = GetContentStream(multiResult);
            if (!(multiResult.Result is Exception))
                multiResult.Result = contentStream;
            multiResult.CompleteSequence();
            return multiResult;
        }

        protected virtual void CheckBoundary() { }

        internal abstract void Close();
        internal Stream EndGetContentStream(IAsyncResult result)
        {
            object obj2 = MultiAsyncResult.End(result);
            if (obj2 is Exception)
                throw ((Exception) obj2);
            return (Stream) obj2;
        }

        protected void Flush(MultiAsyncResult multiResult)
        {
            if (_bufferBuilder.Length > 0)
            {
                if (multiResult != null)
                {
                    multiResult.Enter();
                    var asyncResult = _stream.BeginWrite(_bufferBuilder.GetBuffer(), 0, _bufferBuilder.Length, _onWrite, multiResult);
                    if (asyncResult.CompletedSynchronously)
                    {
                        _stream.EndWrite(asyncResult);
                        multiResult.Leave();
                    }
                }
                else
                    _stream.Write(_bufferBuilder.GetBuffer(), 0, _bufferBuilder.Length);
                _bufferBuilder.Reset();
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal Stream GetContentStream()
        {
            return GetContentStream(null);
        }

        private Stream GetContentStream(MultiAsyncResult multiResult)
        {
            if (_isInContent)
                throw new InvalidOperationException(SR.GetString("MailWriterIsInContent"));
            _isInContent = true;
            CheckBoundary();
            _bufferBuilder.Append(CRLF);
            Flush(multiResult);
            var stream = new EightBitStream(_stream, _shouldEncodeLeadingDots);
            var stream2 = new ClosableStream(stream, _onCloseHandler);
            _contentStream = stream2;
            return stream2;
        }

        protected abstract void OnClose(object sender, EventArgs args);
        protected static void OnWrite(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                var asyncState = (MultiAsyncResult)result.AsyncState;
                var context = (BaseWriter) asyncState.Context;
                try
                {
                    context._stream.EndWrite(result);
                    asyncState.Leave();
                }
                catch (Exception exception) { asyncState.Leave(exception); }
            }
        }

        private void WriteAndFold(string value, int charsAlreadyOnLine, bool allowUnicode)
        {
            var num = 0;
            var offset = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (MailBnfHelper.IsFWSAt(value, i))
                {
                    i += 2;
                    _bufferBuilder.Append(value, offset, i - offset, allowUnicode);
                    offset = i;
                    num = i;
                    charsAlreadyOnLine = 0;
                }
                else if ((i - offset) > (_lineLength - charsAlreadyOnLine) && num != offset)
                {
                    _bufferBuilder.Append(value, offset, num - offset, allowUnicode);
                    _bufferBuilder.Append(CRLF);
                    offset = num;
                    charsAlreadyOnLine = 0;
                }
                else if (value[i] == MailBnfHelper.Space || value[i] == MailBnfHelper.Tab)
                    num = i;
            }
            if ((value.Length - offset) > 0)
                _bufferBuilder.Append(value, offset, value.Length - offset, allowUnicode);
        }

        internal void WriteHeader(string name, string value, bool allowUnicode)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            if (_isInContent)
                throw new InvalidOperationException(SR.GetString("MailWriterIsInContent"));
            CheckBoundary();
            _bufferBuilder.Append(name);
            _bufferBuilder.Append(": ");
            WriteAndFold(value, name.Length + 2, allowUnicode);
            _bufferBuilder.Append(CRLF);
        }

        internal abstract void WriteHeaders(NameValueCollection headers, bool allowUnicode);
    }
}

