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
namespace Contoso.Compression.RangeCoder
{
    internal struct BitDecoder
    {
        public const int kNumBitModelTotalBits = 11;
        public const uint kBitModelTotal = (1 << kNumBitModelTotalBits);
        private const int kNumMoveBits = 5;
        private uint _prob;

        public void UpdateModel(int numMoveBits, uint symbol)
        {
            if (symbol == 0)
                _prob += (kBitModelTotal - _prob) >> numMoveBits;
            else
                _prob -= (_prob) >> numMoveBits;
        }

        public void Init()
        {
            _prob = kBitModelTotal >> 1;
        }

        public uint Decode(RangeCoder.Decoder rangeDecoder)
        {
            var newBound = (uint)(rangeDecoder.Range >> kNumBitModelTotalBits) * (uint)_prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                _prob += (kBitModelTotal - _prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 0;
            }
            else
            {
                rangeDecoder.Range -= newBound;
                rangeDecoder.Code -= newBound;
                _prob -= (_prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 1;
            }
        }
    }
}
