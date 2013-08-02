#region Foreign-License
// .Net40 Polyfill
#endregion
using System.Runtime;
using System.Text;

namespace System.Net.Mail
{
    internal class BufferBuilder
    {
        private byte[] _buffer;
        private int _offset;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal BufferBuilder()
            : this(0x100) { }

        internal BufferBuilder(int initialSize)
        {
            _buffer = new byte[initialSize];
        }

        internal void Append(byte value)
        {
            EnsureBuffer(1);
            _buffer[_offset++] = value;
        }

        internal void Append(byte[] value)
        {
            Append(value, 0, value.Length);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal void Append(string value)
        {
            Append(value, false);
        }

        internal void Append(string value, bool allowUnicode)
        {
            if (!string.IsNullOrEmpty(value))
                Append(value, 0, value.Length, allowUnicode);
        }

        internal void Append(byte[] value, int offset, int count)
        {
            EnsureBuffer(count);
            Buffer.BlockCopy(value, offset, _buffer, _offset, count);
            _offset += count;
        }

        internal void Append(string value, int offset, int count)
        {
            EnsureBuffer(count);
            for (var i = 0; i < count; i++)
            {
                var ch = value[offset + i];
                if (ch > '\x00ff')
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { ch }));
                _buffer[_offset + i] = (byte)ch;
            }
            _offset += count;
        }

        internal void Append(string value, int offset, int count, bool allowUnicode)
        {
            if (allowUnicode)
            {
                var buffer = Encoding.UTF8.GetBytes(value.ToCharArray(), offset, count);
                Append(buffer);
            }
            else
                Append(value, offset, count);
        }

        private void EnsureBuffer(int count)
        {
            if (count > (_buffer.Length - _offset))
            {
                var dst = new byte[((_buffer.Length * 2) > (_buffer.Length + count)) ? (_buffer.Length * 2) : (_buffer.Length + count)];
                Buffer.BlockCopy(_buffer, 0, dst, 0, _offset);
                _buffer = dst;
            }
        }

        internal byte[] GetBuffer()
        {
            return _buffer;
        }

        internal void Reset()
        {
            _offset = 0;
        }

        internal int Length
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _offset; }
        }
    }
}

