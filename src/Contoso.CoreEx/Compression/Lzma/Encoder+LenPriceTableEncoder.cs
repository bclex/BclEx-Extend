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
    /// <summary>
    /// Encoder
    /// </summary>
    public partial class Encoder
    {
        private class LenPriceTableEncoder : LenEncoder
        {
            private uint[] _prices = new uint[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];
            private uint _tableSize;
            private uint[] _counters = new uint[Base.kNumPosStatesEncodingMax];

            /// <summary>
            /// Sets the size of the table.
            /// </summary>
            /// <param name="tableSize">Size of the table.</param>
            public void SetTableSize(uint tableSize) { _tableSize = tableSize; }

            public uint GetPrice(uint symbol, uint posState)
            {
                return _prices[posState * Base.kNumLenSymbols + symbol];
            }

            private void UpdateTable(uint posState)
            {
                SetPrices(posState, _tableSize, _prices, posState * Base.kNumLenSymbols);
                _counters[posState] = _tableSize;
            }

            /// <summary>
            /// Updates the tables.
            /// </summary>
            /// <param name="numPosStates">The num pos states.</param>
            public void UpdateTables(uint numPosStates)
            {
                for (uint posState = 0; posState < numPosStates; posState++)
                    UpdateTable(posState);
            }

            /// <summary>
            /// Encodes the specified range encoder.
            /// </summary>
            /// <param name="rangeEncoder">The range encoder.</param>
            /// <param name="symbol">The symbol.</param>
            /// <param name="posState">State of the pos.</param>
            public new void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--_counters[posState] == 0)
                    UpdateTable(posState);
            }
        }
    }
}
