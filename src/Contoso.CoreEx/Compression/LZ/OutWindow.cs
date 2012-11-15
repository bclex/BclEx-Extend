#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.IO;
namespace Contoso.Compression.LZ
{
    /// <summary>
    /// OutWindow
    /// </summary>
	public class OutWindow
	{
        private byte[] _buffer;
        private uint _pos;
        private uint _windowSize;
        private uint _streamPos;
		private Stream _stream;
        /// <summary>
        /// 
        /// </summary>
		public uint TrainSize;

        /// <summary>
        /// Creates the specified window size.
        /// </summary>
        /// <param name="windowSize">Size of the window.</param>
		public void Create(uint windowSize)
		{
			if (_windowSize != windowSize)
			{
				// System.GC.Collect();
				_buffer = new byte[windowSize];
			}
			_windowSize = windowSize;
			_pos = 0;
			_streamPos = 0;
		}

        /// <summary>
        /// Inits the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="solid">if set to <c>true</c> [solid].</param>
		public void Init(Stream stream, bool solid)
		{
			ReleaseStream();
			_stream = stream;
			if (!solid)
			{
				_streamPos = 0;
				_pos = 0;
				TrainSize = 0;
			}
		}

        /// <summary>
        /// Trains the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
		public bool Train(Stream stream)
		{
			long len = stream.Length;
			uint size = (len < _windowSize) ? (uint)len : _windowSize;
			TrainSize = size;
			stream.Position = len - size;
			_streamPos = _pos = 0;
			while (size > 0)
			{
				uint curSize = _windowSize - _pos;
				if (size < curSize)
					curSize = size;
				int numReadBytes = stream.Read(_buffer, (int)_pos, (int)curSize);
				if (numReadBytes == 0)
					return false;
				size -= (uint)numReadBytes;
				_pos += (uint)numReadBytes;
				_streamPos += (uint)numReadBytes;
				if (_pos == _windowSize)
					_streamPos = _pos = 0;
			}
			return true;
		}

        /// <summary>
        /// Releases the stream.
        /// </summary>
		public void ReleaseStream()
		{
			Flush();
			_stream = null;
		}

        /// <summary>
        /// Flushes this instance.
        /// </summary>
		public void Flush()
		{
			uint size = _pos - _streamPos;
			if (size == 0)
				return;
			_stream.Write(_buffer, (int)_streamPos, (int)size);
			if (_pos >= _windowSize)
				_pos = 0;
			_streamPos = _pos;
		}

        /// <summary>
        /// Copies the block.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="len">The len.</param>
		public void CopyBlock(uint distance, uint len)
		{
			uint pos = _pos - distance - 1;
			if (pos >= _windowSize)
				pos += _windowSize;
			for (; len > 0; len--)
			{
				if (pos >= _windowSize)
					pos = 0;
				_buffer[_pos++] = _buffer[pos++];
				if (_pos >= _windowSize)
					Flush();
			}
		}

        /// <summary>
        /// Puts the byte.
        /// </summary>
        /// <param name="b">The b.</param>
		public void PutByte(byte b)
		{
			_buffer[_pos++] = b;
			if (_pos >= _windowSize)
				Flush();
		}

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
		public byte GetByte(uint distance)
		{
			uint pos = _pos - distance - 1;
			if (pos >= _windowSize)
				pos += _windowSize;
			return _buffer[pos];
		}
	}
}
