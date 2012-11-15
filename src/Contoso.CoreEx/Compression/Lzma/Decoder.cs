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
namespace Contoso.Compression.Lzma
{
    using RangeCoder;
    public partial class Decoder : ICoder, ISetDecoderProperties //, Stream
    {
        private LZ.OutWindow m_OutWindow = new LZ.OutWindow();
        private RangeCoder.Decoder m_RangeDecoder = new RangeCoder.Decoder();
        private BitDecoder[] m_IsMatchDecoders = new BitDecoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
        private BitDecoder[] m_IsRepDecoders = new BitDecoder[Base.kNumStates];
        private BitDecoder[] m_IsRepG0Decoders = new BitDecoder[Base.kNumStates];
        private BitDecoder[] m_IsRepG1Decoders = new BitDecoder[Base.kNumStates];
        private BitDecoder[] m_IsRepG2Decoders = new BitDecoder[Base.kNumStates];
        private BitDecoder[] m_IsRep0LongDecoders = new BitDecoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
        private BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[Base.kNumLenToPosStates];
        private BitDecoder[] m_PosDecoders = new BitDecoder[Base.kNumFullDistances - Base.kEndPosModelIndex];
        private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(Base.kNumAlignBits);
        private LenDecoder m_LenDecoder = new LenDecoder();
        private LenDecoder m_RepLenDecoder = new LenDecoder();
        private LiteralDecoder m_LiteralDecoder = new LiteralDecoder();
        private uint m_DictionarySize;
        private uint m_DictionarySizeCheck;
        private uint m_PosStateMask;
        private bool _solid = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder"/> class.
        /// </summary>
        public Decoder()
        {
            m_DictionarySize = 0xFFFFFFFF;
            for (int i = 0; i < Base.kNumLenToPosStates; i++)
                m_PosSlotDecoder[i] = new BitTreeDecoder(Base.kNumPosSlotBits);
        }

        private void SetDictionarySize(uint dictionarySize)
        {
            if (m_DictionarySize != dictionarySize)
            {
                m_DictionarySize = dictionarySize;
                m_DictionarySizeCheck = Math.Max(m_DictionarySize, 1);
                uint blockSize = Math.Max(m_DictionarySizeCheck, (1 << 12));
                m_OutWindow.Create(blockSize);
            }
        }

        private void SetLiteralProperties(int lp, int lc)
        {
            if (lp > 8)
                throw new ArgumentOutOfRangeException("lp");
            if (lc > 8)
                throw new ArgumentOutOfRangeException("lc");
            m_LiteralDecoder.Create(lp, lc);
        }

        private void SetPosBitsProperties(int pb)
        {
            if (pb > Base.kNumPosStatesBitsMax)
                throw new ArgumentOutOfRangeException("pb");
            uint numPosStates = (uint)1 << pb;
            m_LenDecoder.Create(numPosStates);
            m_RepLenDecoder.Create(numPosStates);
            m_PosStateMask = numPosStates - 1;
        }

        private void Init(System.IO.Stream inStream, System.IO.Stream outStream)
        {
            m_RangeDecoder.Init(inStream);
            m_OutWindow.Init(outStream, _solid);
            uint i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (uint j = 0; j <= m_PosStateMask; j++)
                {
                    uint index = (i << Base.kNumPosStatesBitsMax) + j;
                    m_IsMatchDecoders[index].Init();
                    m_IsRep0LongDecoders[index].Init();
                }
                m_IsRepDecoders[i].Init();
                m_IsRepG0Decoders[i].Init();
                m_IsRepG1Decoders[i].Init();
                m_IsRepG2Decoders[i].Init();
            }
            m_LiteralDecoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
                m_PosSlotDecoder[i].Init();
            // m_PosSpecDecoder.Init();
            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
                m_PosDecoders[i].Init();
            m_LenDecoder.Init();
            m_RepLenDecoder.Init();
            m_PosAlignDecoder.Init();
        }

        /// <summary>
        /// Codes streams.
        /// </summary>
        /// <param name="inStream">input Stream.</param>
        /// <param name="outStream">output Stream.</param>
        /// <param name="inSize">input Size. -1 if unknown.</param>
        /// <param name="outSize">output Size. -1 if unknown.</param>
        /// <param name="progress">callback progress reference.</param>
        public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
        {
            Init(inStream, outStream);
            Base.State state = new Base.State();
            state.Init();
            uint rep0 = 0, rep1 = 0, rep2 = 0, rep3 = 0;
            ulong nowPos64 = 0;
            ulong outSize64 = (ulong)outSize;
            if (nowPos64 < outSize64)
            {
                if (m_IsMatchDecoders[state.Index << Base.kNumPosStatesBitsMax].Decode(m_RangeDecoder) != 0)
                    throw new DataErrorException();
                state.UpdateChar();
                byte b = m_LiteralDecoder.DecodeNormal(m_RangeDecoder, 0, 0);
                m_OutWindow.PutByte(b);
                nowPos64++;
            }
            while (nowPos64 < outSize64)
            {
                // ulong next = Math.Min(nowPos64 + (1 << 18), outSize64);
                // while(nowPos64 < next)
                {
                    uint posState = (uint)nowPos64 & m_PosStateMask;
                    if (m_IsMatchDecoders[(state.Index << Base.kNumPosStatesBitsMax) + posState].Decode(m_RangeDecoder) == 0)
                    {
                        byte prevByte = m_OutWindow.GetByte(0);
                        byte b;
                        if (!state.IsCharState())
                            b = m_LiteralDecoder.DecodeWithMatchByte(m_RangeDecoder, (uint)nowPos64, prevByte, m_OutWindow.GetByte(rep0));
                        else
                            b = m_LiteralDecoder.DecodeNormal(m_RangeDecoder, (uint)nowPos64, prevByte);
                        m_OutWindow.PutByte(b);
                        state.UpdateChar();
                        nowPos64++;
                    }
                    else
                    {
                        uint len;
                        if (m_IsRepDecoders[state.Index].Decode(m_RangeDecoder) == 1)
                        {
                            if (m_IsRepG0Decoders[state.Index].Decode(m_RangeDecoder) == 0)
                            {
                                if (m_IsRep0LongDecoders[(state.Index << Base.kNumPosStatesBitsMax) + posState].Decode(m_RangeDecoder) == 0)
                                {
                                    state.UpdateShortRep();
                                    m_OutWindow.PutByte(m_OutWindow.GetByte(rep0));
                                    nowPos64++;
                                    continue;
                                }
                            }
                            else
                            {
                                uint distance;
                                if (m_IsRepG1Decoders[state.Index].Decode(m_RangeDecoder) == 0)
                                    distance = rep1;
                                else
                                {
                                    if (m_IsRepG2Decoders[state.Index].Decode(m_RangeDecoder) == 0)
                                        distance = rep2;
                                    else
                                    {
                                        distance = rep3;
                                        rep3 = rep2;
                                    }
                                    rep2 = rep1;
                                }
                                rep1 = rep0;
                                rep0 = distance;
                            }
                            len = m_RepLenDecoder.Decode(m_RangeDecoder, posState) + Base.kMatchMinLen;
                            state.UpdateRep();
                        }
                        else
                        {
                            rep3 = rep2;
                            rep2 = rep1;
                            rep1 = rep0;
                            len = Base.kMatchMinLen + m_LenDecoder.Decode(m_RangeDecoder, posState);
                            state.UpdateMatch();
                            uint posSlot = m_PosSlotDecoder[Base.GetLenToPosState(len)].Decode(m_RangeDecoder);
                            if (posSlot >= Base.kStartPosModelIndex)
                            {
                                int numDirectBits = (int)((posSlot >> 1) - 1);
                                rep0 = ((2 | (posSlot & 1)) << numDirectBits);
                                if (posSlot < Base.kEndPosModelIndex)
                                    rep0 += BitTreeDecoder.ReverseDecode(m_PosDecoders,
                                            rep0 - posSlot - 1, m_RangeDecoder, numDirectBits);
                                else
                                {
                                    rep0 += (m_RangeDecoder.DecodeDirectBits(
                                        numDirectBits - Base.kNumAlignBits) << Base.kNumAlignBits);
                                    rep0 += m_PosAlignDecoder.ReverseDecode(m_RangeDecoder);
                                }
                            }
                            else
                                rep0 = posSlot;
                        }
                        if (rep0 >= m_OutWindow.TrainSize + nowPos64 || rep0 >= m_DictionarySizeCheck)
                        {
                            if (rep0 == 0xFFFFFFFF)
                                break;
                            throw new DataErrorException();
                        }
                        m_OutWindow.CopyBlock(rep0, len);
                        nowPos64 += len;
                    }
                }
            }
            m_OutWindow.Flush();
            m_OutWindow.ReleaseStream();
            m_RangeDecoder.ReleaseStream();
        }

        /// <summary>
        /// Sets the decoder properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void SetDecoderProperties(byte[] properties)
        {
            if (properties.Length < 5)
                throw new InvalidParamException();
            int lc = properties[0] % 9;
            int remainder = properties[0] / 9;
            int lp = remainder % 5;
            int pb = remainder / 5;
            if (pb > Base.kNumPosStatesBitsMax)
                throw new InvalidParamException();
            uint dictionarySize = 0;
            for (int i = 0; i < 4; i++)
                dictionarySize += ((uint)(properties[1 + i])) << (i * 8);
            SetDictionarySize(dictionarySize);
            SetLiteralProperties(lp, lc);
            SetPosBitsProperties(pb);
        }

        /// <summary>
        /// Trains the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public bool Train(System.IO.Stream stream)
        {
            _solid = true;
            return m_OutWindow.Train(stream);
        }

        //public override bool CanRead { get { return true; }}
        //public override bool CanWrite { get { return true; }}
        //public override bool CanSeek { get { return true; }}
        //public override long Length { get { return 0; }}
        //public override long Position
        //{
        //    get { return 0;	}
        //    set { }
        //}
        //public override void Flush() { }
        //public override int Read(byte[] buffer, int offset, int count) 
        //{
        //    return 0;
        //}
        //public override void Write(byte[] buffer, int offset, int count)
        //{
        //}
        //public override long Seek(long offset, System.IO.SeekOrigin origin)
        //{
        //    return 0;
        //}
        //public override void SetLength(long value) {}
    }
}
