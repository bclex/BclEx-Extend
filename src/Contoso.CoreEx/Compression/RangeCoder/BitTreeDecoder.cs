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
    internal struct BitTreeDecoder
    {
        private BitDecoder[] _models;
        private int _numBitLevels;

        public BitTreeDecoder(int numBitLevels)
        {
            _numBitLevels = numBitLevels;
            _models = new BitDecoder[1 << numBitLevels];
        }

        public void Init()
        {
            for (uint i = 1; i < (1 << _numBitLevels); i++)
                _models[i].Init();
        }

        public uint Decode(Decoder rangeDecoder)
        {
            uint m = 1;
            for (int bitIndex = _numBitLevels; bitIndex > 0; bitIndex--)
                m = (m << 1) + _models[m].Decode(rangeDecoder);
            return m - ((uint)1 << _numBitLevels);
        }

        public uint ReverseDecode(Decoder rangeDecoder)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < _numBitLevels; bitIndex++)
            {
                uint bit = _models[m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= (bit << bitIndex);
            }
            return symbol;
        }

        public static uint ReverseDecode(BitDecoder[] models, uint startIndex, RangeCoder.Decoder rangeDecoder, int numBitLevels)
        {
            uint m = 1;
            uint symbol = 0;
            for (int bitIndex = 0; bitIndex < numBitLevels; bitIndex++)
            {
                uint bit = models[startIndex + m].Decode(rangeDecoder);
                m <<= 1;
                m += bit;
                symbol |= (bit << bitIndex);
            }
            return symbol;
        }
    }
}
