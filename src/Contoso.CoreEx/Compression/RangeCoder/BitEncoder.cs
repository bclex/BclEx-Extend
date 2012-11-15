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
    internal struct BitEncoder
    {
        public const int kNumBitModelTotalBits = 11;
        public const uint kBitModelTotal = (1 << kNumBitModelTotalBits);
        private const int kNumMoveBits = 5;
        private const int kNumMoveReducingBits = 2;
        public const int kNumBitPriceShiftBits = 6;
        private uint _prob;
        private static uint[] _probPrices = new uint[kBitModelTotal >> kNumMoveReducingBits];

        static BitEncoder()
        {
            const int kNumBits = (kNumBitModelTotalBits - kNumMoveReducingBits);
            for (int i = kNumBits - 1; i >= 0; i--)
            {
                uint start = (uint)1 << (kNumBits - i - 1);
                uint end = (uint)1 << (kNumBits - i);
                for (uint j = start; j < end; j++)
                    _probPrices[j] = ((uint)i << kNumBitPriceShiftBits) + (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
            }
        }

        public void Init()
        {
            _prob = kBitModelTotal >> 1;
        }

        public void UpdateModel(uint symbol)
        {
            if (symbol == 0)
                _prob += (kBitModelTotal - _prob) >> kNumMoveBits;
            else
                _prob -= (_prob) >> kNumMoveBits;
        }

        public void Encode(Encoder encoder, uint symbol)
        {
            // encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
            // UpdateModel(symbol);
            uint newBound = (encoder.Range >> kNumBitModelTotalBits) * _prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                _prob += (kBitModelTotal - _prob) >> kNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                _prob -= (_prob) >> kNumMoveBits;
            }
            if (encoder.Range < Encoder.kTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

        public uint GetPrice(uint symbol) { return _probPrices[(((_prob - symbol) ^ ((-(int)symbol))) & (kBitModelTotal - 1)) >> kNumMoveReducingBits]; }
        public uint GetPrice0() { return _probPrices[_prob >> kNumMoveReducingBits]; }
        public uint GetPrice1() { return _probPrices[(kBitModelTotal - _prob) >> kNumMoveReducingBits]; }
    }
}
