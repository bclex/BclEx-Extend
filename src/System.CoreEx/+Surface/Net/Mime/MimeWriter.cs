#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace System.Net.Mime
{
    internal class MimeWriter : BaseWriter
    {
        private byte[] _boundaryBytes;
        private static byte[] DASHDASH = new byte[] { 0x2d, 0x2d };
        private bool _writeBoundary;

        internal MimeWriter(Stream stream, string boundary)
            : base(stream, false)
        {
            _writeBoundary = true;
            if (boundary == null)
                throw new ArgumentNullException("boundary");
            _boundaryBytes = Encoding.ASCII.GetBytes(boundary);
        }

        internal IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            var multiResult = new MultiAsyncResult(this, callback, state);
            Close(multiResult);
            multiResult.CompleteSequence();
            return multiResult;
        }

        protected override void CheckBoundary()
        {
            if (_writeBoundary)
            {
                base._bufferBuilder.Append(BaseWriter.CRLF);
                base._bufferBuilder.Append(DASHDASH);
                base._bufferBuilder.Append(_boundaryBytes);
                base._bufferBuilder.Append(BaseWriter.CRLF);
                _writeBoundary = false;
            }
        }

        internal override void Close()
        {
            Close(null);
            base._stream.Close();
        }

        private void Close(MultiAsyncResult multiResult)
        {
            base._bufferBuilder.Append(BaseWriter.CRLF);
            base._bufferBuilder.Append(DASHDASH);
            base._bufferBuilder.Append(_boundaryBytes);
            base._bufferBuilder.Append(DASHDASH);
            base._bufferBuilder.Append(BaseWriter.CRLF);
            base.Flush(multiResult);
        }

        internal void EndClose(IAsyncResult result)
        {
            MultiAsyncResult.End(result);
            base._stream.Close();
        }

        protected override void OnClose(object sender, EventArgs args)
        {
            if (base._contentStream == sender)
            {
                base._contentStream.Flush();
                base._contentStream = null;
                _writeBoundary = true;
                base._isInContent = false;
            }
        }

        internal override void WriteHeaders(NameValueCollection headers, bool allowUnicode)
        {
            if (headers == null)
                throw new ArgumentNullException("headers");
            foreach (string str in headers)
                base.WriteHeader(str, headers[str], allowUnicode);
        }
    }
}

