#region Foreign-License
// .Net40 Surface
#endregion
using System.IO;

namespace System.Net.Mime
{
    internal class EightBitStream : DelegatedStream, IEncodableStream
    {
        private bool _shouldEncodeLeadingDots;

        internal EightBitStream(Stream stream)
            : base(stream) { }
        internal EightBitStream(Stream stream, bool shouldEncodeLeadingDots)
            : this(stream)
        {
            _shouldEncodeLeadingDots = shouldEncodeLeadingDots;
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
