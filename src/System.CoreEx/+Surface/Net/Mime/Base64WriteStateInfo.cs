#region Foreign-License
// .Net40 Surface
#endregion
namespace System.Net.Mime
{
    internal class Base64WriteStateInfo : WriteStateInfoBase
    {
        internal Base64WriteStateInfo() { }
        internal Base64WriteStateInfo(int bufferSize, byte[] header, byte[] footer, int maxLineLength, int mimeHeaderLength)
            : base(bufferSize, header, footer, maxLineLength, mimeHeaderLength) { }

        internal byte LastBits { get; set; }
        internal int Padding { get; set; }
    }
}

