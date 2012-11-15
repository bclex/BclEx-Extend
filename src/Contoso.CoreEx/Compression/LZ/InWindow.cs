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
using System;
using System.IO;
namespace Contoso.Compression.LZ
{
    /// <summary>
    /// InWindow
    /// </summary>
	public class InWindow
	{
        /// <summary>
        /// _bufferBase
        /// </summary>
		public Byte[] _bufferBase; // pointer to buffer with data
		private Stream _stream;
		private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done
		private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream
		private uint _pointerToLastSafePosition;
        /// <summary>
        /// _bufferOffset
        /// </summary>
		public uint _bufferOffset;
        /// <summary>
        /// _blockSize
        /// </summary>
		public uint _blockSize; // Size of Allocated memory block
        /// <summary>
        /// _pos
        /// </summary>
		public uint _pos; // offset (from _buffer) of curent byte
		private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos
		private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        /// <summary>
        /// _streamPos
        /// </summary>
		public uint _streamPos; // offset (from _buffer) of first not read byte from Stream

        /// <summary>
        /// Moves the block.
        /// </summary>
		public void MoveBlock()
		{
			uint offset = (uint)(_bufferOffset) + _pos - _keepSizeBefore;
			// we need one additional byte, since MovePos moves on 1 byte.
			if (offset > 0)
				offset--;
			uint numBytes = (uint)(_bufferOffset) + _streamPos - offset;
			// check negative offset ????
			for (uint i = 0; i < numBytes; i++)
				_bufferBase[i] = _bufferBase[offset + i];
			_bufferOffset -= offset;
		}

        /// <summary>
        /// Reads the block.
        /// </summary>
		public virtual void ReadBlock()
		{
			if (_streamEndWasReached)
				return;
			while (true)
			{
				int size = (int)((0 - _bufferOffset) + _blockSize - _streamPos);
				if (size == 0)
					return;
				int numReadBytes = _stream.Read(_bufferBase, (int)(_bufferOffset + _streamPos), size);
				if (numReadBytes == 0)
				{
					_posLimit = _streamPos;
					uint pointerToPostion = _bufferOffset + _posLimit;
					if (pointerToPostion > _pointerToLastSafePosition)
						_posLimit = (uint)(_pointerToLastSafePosition - _bufferOffset);

					_streamEndWasReached = true;
					return;
				}
				_streamPos += (uint)numReadBytes;
				if (_streamPos >= _pos + _keepSizeAfter)
					_posLimit = _streamPos - _keepSizeAfter;
			}
		}

		void Free() { _bufferBase = null; }

        /// <summary>
        /// Creates the specified keep size before.
        /// </summary>
        /// <param name="keepSizeBefore">The keep size before.</param>
        /// <param name="keepSizeAfter">The keep size after.</param>
        /// <param name="keepSizeReserv">The keep size reserv.</param>
		public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
		{
			_keepSizeBefore = keepSizeBefore;
			_keepSizeAfter = keepSizeAfter;
			uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
			if (_bufferBase == null || _blockSize != blockSize)
			{
				Free();
				_blockSize = blockSize;
				_bufferBase = new Byte[_blockSize];
			}
			_pointerToLastSafePosition = _blockSize - keepSizeAfter;
		}

        /// <summary>
        /// Sets the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
		public void SetStream(Stream stream) { _stream = stream; }
        /// <summary>
        /// Releases the stream.
        /// </summary>
		public void ReleaseStream() { _stream = null; }

        /// <summary>
        /// Inits this instance.
        /// </summary>
		public void Init()
		{
			_bufferOffset = 0;
			_pos = 0;
			_streamPos = 0;
			_streamEndWasReached = false;
			ReadBlock();
		}

        /// <summary>
        /// Moves the pos.
        /// </summary>
		public void MovePos()
		{
			_pos++;
			if (_pos > _posLimit)
			{
				uint pointerToPostion = _bufferOffset + _pos;
				if (pointerToPostion > _pointerToLastSafePosition)
					MoveBlock();
				ReadBlock();
			}
		}

        /// <summary>
        /// Gets the index byte.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public Byte GetIndexByte(int index) { return _bufferBase[_bufferOffset + _pos + index]; }

		// index + limit have not to exceed _keepSizeAfter;
        /// <summary>
        /// Gets the match len.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
		public uint GetMatchLen(int index, uint distance, uint limit)
		{
			if (_streamEndWasReached)
				if ((_pos + index) + limit > _streamPos)
					limit = _streamPos - (uint)(_pos + index);
			distance++;
			uint pby = _bufferOffset + _pos + (uint)index;
			uint i;
			for (i = 0; i < limit && _bufferBase[pby + i] == _bufferBase[pby + i - distance]; i++);
			return i;
		}

        /// <summary>
        /// Gets the num available bytes.
        /// </summary>
        /// <returns></returns>
		public uint GetNumAvailableBytes() { return _streamPos - _pos; }

        /// <summary>
        /// Reduces the offsets.
        /// </summary>
        /// <param name="subValue">The sub value.</param>
		public void ReduceOffsets(int subValue)
		{
			_bufferOffset += (uint)subValue;
			_posLimit -= (uint)subValue;
			_pos -= (uint)subValue;
			_streamPos -= (uint)subValue;
		}
	}
}
