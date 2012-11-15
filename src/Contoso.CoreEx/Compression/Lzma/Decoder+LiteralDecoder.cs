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
namespace Contoso.Compression.Lzma
{
    using RangeCoder;
    public partial class Decoder
    {
        private class LiteralDecoder
        {
            private struct Decoder2
            {
                private BitDecoder[] _decoders;

                public void Create() { _decoders = new BitDecoder[0x300]; }
                
                public void Init() { for (int i = 0; i < 0x300; i++) _decoders[i].Init(); }

                public byte DecodeNormal(RangeCoder.Decoder rangeDecoder)
                {
                    uint symbol = 1;
                    do
                        symbol = (symbol << 1) | _decoders[symbol].Decode(rangeDecoder);
                    while (symbol < 0x100);
                    return (byte)symbol;
                }

                public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, byte matchByte)
                {
                    uint symbol = 1;
                    do
                    {
                        uint matchBit = (uint)(matchByte >> 7) & 1;
                        matchByte <<= 1;
                        uint bit = _decoders[((1 + matchBit) << 8) + symbol].Decode(rangeDecoder);
                        symbol = (symbol << 1) | bit;
                        if (matchBit != bit)
                        {
                            while (symbol < 0x100)
                                symbol = (symbol << 1) | _decoders[symbol].Decode(rangeDecoder);
                            break;
                        }
                    }
                    while (symbol < 0x100);
                    return (byte)symbol;
                }
            }

            private Decoder2[] m_Coders;
            private int m_NumPrevBits;
            private int m_NumPosBits;
            private uint m_PosMask;

            public void Create(int numPosBits, int numPrevBits)
            {
                if (m_Coders != null && m_NumPrevBits == numPrevBits &&
                    m_NumPosBits == numPosBits)
                    return;
                m_NumPosBits = numPosBits;
                m_PosMask = ((uint)1 << numPosBits) - 1;
                m_NumPrevBits = numPrevBits;
                uint numStates = (uint)1 << (m_NumPrevBits + m_NumPosBits);
                m_Coders = new Decoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Create();
            }

            public void Init()
            {
                uint numStates = (uint)1 << (m_NumPrevBits + m_NumPosBits);
                for (uint i = 0; i < numStates; i++)
                    m_Coders[i].Init();
            }

            private uint GetState(uint pos, byte prevByte)
            {
                return ((pos & m_PosMask) << m_NumPrevBits) + (uint)(prevByte >> (8 - m_NumPrevBits));
            }

            public byte DecodeNormal(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte)
            {
                return m_Coders[GetState(pos, prevByte)].DecodeNormal(rangeDecoder);
            }

            public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
            {
                return m_Coders[GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);
            }
        }
    }
}
