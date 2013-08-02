#region Foreign-License
// .Net40 Surface
#endregion
using System.IO;
using System.Text;

namespace System.Net.Mime
{
    internal class EncodedStreamFactory
    {
        //private const int _defaultMaxLineLength = 70;
        //private const int _initialBufferSize = 0x400;

        protected byte[] CreateFooter()
        {
            return new byte[] { 0x3f, 0x3d };
        }

        protected byte[] CreateHeader(Encoding encoding, bool useBase64Encoding)
        {
            var s = string.Format("=?{0}?{1}?", encoding.HeaderName, useBase64Encoding ? "B" : "Q");
            return Encoding.ASCII.GetBytes(s);
        }

        internal IEncodableStream GetEncoder(TransferEncoding encoding, Stream stream)
        {
            if (encoding == TransferEncoding.Base64)
                return new Base64Stream(stream, new Base64WriteStateInfo());
            if (encoding == TransferEncoding.QuotedPrintable)
                return new QuotedPrintableStream(stream, true);
#if CLR45
            if (encoding != TransferEncoding.SevenBit && encoding != TransferEncoding.EightBit)
                throw new NotSupportedException("Encoding Stream");
#endif
            return new EightBitStream(stream);

        }

        internal IEncodableStream GetEncoderForHeader(Encoding encoding, bool useBase64Encoding, int headerTextLength)
        {
            var header = CreateHeader(encoding, useBase64Encoding);
            var footer = CreateFooter();
            if (useBase64Encoding)
            {
                var base2 = new Base64WriteStateInfo(0x400, header, footer, DefaultMaxLineLength, headerTextLength);
                return new Base64Stream((Base64WriteStateInfo)base2);
            }
            return new QEncodedStream(new WriteStateInfoBase(0x400, header, footer, DefaultMaxLineLength, headerTextLength));
        }

        internal static int DefaultMaxLineLength
        {
            get { return 70; }
        }
    }
}

