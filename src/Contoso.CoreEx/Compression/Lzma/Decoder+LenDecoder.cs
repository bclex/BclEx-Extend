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
    /// <summary>
    /// Decoder
    /// </summary>
    public partial class Decoder
    {
        private class LenDecoder
        {
            private BitDecoder _choice = new BitDecoder();
            private BitDecoder _choice2 = new BitDecoder();
            private BitTreeDecoder[] _lowCoder = new BitTreeDecoder[Base.kNumPosStatesMax];
            private BitTreeDecoder[] _midCoder = new BitTreeDecoder[Base.kNumPosStatesMax];
            private BitTreeDecoder _highCoder = new BitTreeDecoder(Base.kNumHighLenBits);
            private uint _numPosStates = 0;

            /// <summary>
            /// Creates the specified num pos states.
            /// </summary>
            /// <param name="numPosStates">The num pos states.</param>
            public void Create(uint numPosStates)
            {
                for (uint posState = _numPosStates; posState < numPosStates; posState++)
                {
                    _lowCoder[posState] = new BitTreeDecoder(Base.kNumLowLenBits);
                    _midCoder[posState] = new BitTreeDecoder(Base.kNumMidLenBits);
                }
                _numPosStates = numPosStates;
            }

            /// <summary>
            /// Inits this instance.
            /// </summary>
            public void Init()
            {
                _choice.Init();
                for (uint posState = 0; posState < _numPosStates; posState++)
                {
                    _lowCoder[posState].Init();
                    _midCoder[posState].Init();
                }
                _choice2.Init();
                _highCoder.Init();
            }

            /// <summary>
            /// Decodes the specified range decoder.
            /// </summary>
            /// <param name="rangeDecoder">The range decoder.</param>
            /// <param name="posState">State of the pos.</param>
            /// <returns></returns>
            public uint Decode(RangeCoder.Decoder rangeDecoder, uint posState)
            {
                if (_choice.Decode(rangeDecoder) == 0)
                    return _lowCoder[posState].Decode(rangeDecoder);
                else
                {
                    uint symbol = Base.kNumLowLenSymbols;
                    if (_choice2.Decode(rangeDecoder) == 0)
                        symbol += _midCoder[posState].Decode(rangeDecoder);
                    else
                    {
                        symbol += Base.kNumMidLenSymbols;
                        symbol += _highCoder.Decode(rangeDecoder);
                    }
                    return symbol;
                }
            }
        }
    }
}
