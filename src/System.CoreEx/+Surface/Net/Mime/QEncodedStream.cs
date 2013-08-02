#region Foreign-License
// .Net40 Surface
#endregion
using System.IO;

namespace System.Net.Mime
{
    internal class QEncodedStream : IEncodableStream
    {
        private WriteStateInfoBase _writeState;

        internal QEncodedStream(WriteStateInfoBase wsi)
        {
            _writeState = wsi;
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

