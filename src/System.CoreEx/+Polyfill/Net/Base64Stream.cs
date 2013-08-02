#region Foreign-License
// .Net40 Polyfill
#endregion
using System.IO;
using System.Net.Mime;

namespace System.Net
{
    internal class Base64Stream : DelegatedStream, IEncodableStream
    {
        private int _lineLength;
        private Base64WriteStateInfo _writeState;

        internal Base64Stream(Base64WriteStateInfo writeStateInfo)
        {
            _lineLength = writeStateInfo.MaxLineLength;
            _writeState = writeStateInfo;
        }

        internal Base64Stream(Stream stream, int lineLength)
            : base(stream)
        {
            _lineLength = lineLength;
            _writeState = new Base64WriteStateInfo();
        }

        internal Base64Stream(Stream stream, Base64WriteStateInfo writeStateInfo)
            : base(stream)
        {
            _writeState = new Base64WriteStateInfo();
            _lineLength = writeStateInfo.MaxLineLength;
        }

        public int DecodeBytes(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public int EncodeBytes(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public string GetEncodedString()
        {
            throw new NotImplementedException();
        }

        public Stream GetStream()
        {
            throw new NotImplementedException();
        }
    }
}

