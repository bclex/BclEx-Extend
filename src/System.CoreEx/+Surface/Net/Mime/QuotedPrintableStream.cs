#region Foreign-License
// .Net40 Surface
#endregion
using System.IO;

namespace System.Net.Mime
{
    internal class QuotedPrintableStream : DelegatedStream, IEncodableStream
    {
        private bool _encodeCRLF;
        private int _lineLength;

        internal QuotedPrintableStream(Stream stream, bool encodeCRLF)
            : this(stream, EncodedStreamFactory.DefaultMaxLineLength)
        {
            _encodeCRLF = encodeCRLF;
        }

        internal QuotedPrintableStream(Stream stream, int lineLength)
            : base(stream)
        {
            if (lineLength < 0)
                throw new ArgumentOutOfRangeException("lineLength");
            _lineLength = lineLength;
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

