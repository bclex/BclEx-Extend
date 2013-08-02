#region Foreign-License
// .Net40 Surface
#endregion
using System.Runtime;

namespace System.Net.Mime
{
    internal class WriteStateInfoBase
    {
        protected int _currentBufferUsed;
        protected int _currentLineLength;
        protected byte[] _footer;
        protected byte[] _header;
        protected int _maxLineLength;
        protected byte[] _buffer;
        protected const int _defaultBufferSize = 0x400;

        public WriteStateInfoBase()
        {
            _buffer = new byte[0x400];
            _header = new byte[0];
            _footer = new byte[0];
            _maxLineLength = EncodedStreamFactory.DefaultMaxLineLength;
            _currentLineLength = 0;
            _currentBufferUsed = 0;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public WriteStateInfoBase(int bufferSize, byte[] header, byte[] footer, int maxLineLength)
            : this(bufferSize, header, footer, maxLineLength, 0) { }

        public WriteStateInfoBase(int bufferSize, byte[] header, byte[] footer, int maxLineLength, int mimeHeaderLength)
        {
            _buffer = new byte[bufferSize];
            _header = header;
            _footer = footer;
            _maxLineLength = maxLineLength;
            _currentLineLength = mimeHeaderLength;
            _currentBufferUsed = 0;
        }

        public void Append(byte aByte)
        {
            EnsureSpaceInBuffer(1);
            Buffer[_currentBufferUsed++] = aByte;
            _currentLineLength++;
        }

        public void Append(params byte[] bytes)
        {
            EnsureSpaceInBuffer(bytes.Length);
            bytes.CopyTo(_buffer, Length);
            _currentLineLength += bytes.Length;
            _currentBufferUsed += bytes.Length;
        }

        public void AppendCRLF(bool includeSpace)
        {
            AppendFooter();
            Append(new byte[] { 13, 10 });
            _currentLineLength = 0;
            if (includeSpace)
                Append((byte)0x20);
            AppendHeader();
        }

        public void AppendFooter()
        {
            if (Footer != null && Footer.Length != 0)
                Append(Footer);
        }

        public void AppendHeader()
        {
            if (Header != null && Header.Length != 0)
                Append(Header);
        }

        public void BufferFlushed()
        {
            _currentBufferUsed = 0;
        }

        private void EnsureSpaceInBuffer(int moreBytes)
        {
            var length = Buffer.Length;
            while ((_currentBufferUsed + moreBytes) >= length)
                length *= 2;
            if (length > Buffer.Length)
            {
                var array = new byte[length];
                _buffer.CopyTo(array, 0);
                _buffer = array;
            }
        }

        public void Reset()
        {
            _currentBufferUsed = 0;
            _currentLineLength = 0;
        }

        public byte[] Buffer
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _buffer; }
        }

        public int CurrentLineLength
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _currentLineLength; }
        }

        public byte[] Footer
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _footer; }
        }

        public int FooterLength
        {
            get { return _footer.Length; }
        }

        public byte[] Header
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _header; }
        }

        public int Length
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _currentBufferUsed; }
        }

        public int MaxLineLength
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _maxLineLength; }
        }
    }
}

