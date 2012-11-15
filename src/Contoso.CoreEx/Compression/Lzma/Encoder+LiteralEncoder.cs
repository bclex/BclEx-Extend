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
using Contoso.Compression.RangeCoder;
namespace Contoso.Compression.Lzma
{
    public partial class Encoder
    {
        private class LiteralEncoder
        {
            private Encoder2[] _coders;
            private int _numPrevBits;
            private int _numPosBits;
            private uint _posMask;

            public struct Encoder2
            {
                private BitEncoder[] _encoders;

                public void Create() { _encoders = new BitEncoder[0x300]; }

                public void Init() { for (int i = 0; i < 0x300; i++) _encoders[i].Init(); }

                public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
                {
                    uint context = 1;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint bit = (uint)((symbol >> i) & 1);
                        _encoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint context = 1;
                    bool same = true;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint bit = (uint)((symbol >> i) & 1);
                        uint state = context;
                        if (same)
                        {
                            uint matchBit = (uint)((matchByte >> i) & 1);
                            state += ((1 + matchBit) << 8);
                            same = (matchBit == bit);
                        }
                        _encoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint price = 0;
                    uint context = 1;
                    int i = 7;
                    if (matchMode)
                    {
                        for (; i >= 0; i--)
                        {
                            uint matchBit = (uint)(matchByte >> i) & 1;
                            uint bit = (uint)(symbol >> i) & 1;
                            price += _encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }
                    }
                    for (; i >= 0; i--)
                    {
                        uint bit = (uint)(symbol >> i) & 1;
                        price += _encoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }
                    return price;
                }
            }

            public void Create(int numPosBits, int numPrevBits)
            {
                if (_coders != null && _numPrevBits == numPrevBits && _numPosBits == numPosBits)
                    return;
                _numPosBits = numPosBits;
                _posMask = ((uint)1 << numPosBits) - 1;
                _numPrevBits = numPrevBits;
                uint numStates = (uint)1 << (_numPrevBits + _numPosBits);
                _coders = new Encoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                    _coders[i].Create();
            }

            public void Init()
            {
                uint numStates = (uint)1 << (_numPrevBits + _numPosBits);
                for (uint i = 0; i < numStates; i++)
                    _coders[i].Init();
            }

            public Encoder2 GetSubCoder(uint pos, byte prevByte)
            {
                return _coders[((pos & _posMask) << _numPrevBits) + (uint)(prevByte >> (8 - _numPrevBits))];
            }
        }
    }
}
