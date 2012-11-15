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
	internal struct BitTreeEncoder
	{
		private BitEncoder[] _models;
		private int _numBitLevels;

		public BitTreeEncoder(int numBitLevels)
		{
			_numBitLevels = numBitLevels;
			_models = new BitEncoder[1 << numBitLevels];
		}

		public void Init()
		{
			for (uint i = 1; i < (1 << _numBitLevels); i++)
				_models[i].Init();
		}

		public void Encode(Encoder rangeEncoder, uint symbol)
		{
			uint m = 1;
			for (int bitIndex = _numBitLevels; bitIndex > 0; )
			{
				bitIndex--;
				uint bit = (symbol >> bitIndex) & 1;
				_models[m].Encode(rangeEncoder, bit);
				m = (m << 1) | bit;
			}
		}

		public void ReverseEncode(Encoder rangeEncoder, uint symbol)
		{
			uint m = 1;
			for (uint i = 0; i < _numBitLevels; i++)
			{
				uint bit = symbol & 1;
				_models[m].Encode(rangeEncoder, bit);
				m = (m << 1) | bit;
				symbol >>= 1;
			}
		}

		public uint GetPrice(uint symbol)
		{
			uint price = 0;
			uint m = 1;
			for (int bitIndex = _numBitLevels; bitIndex > 0; )
			{
				bitIndex--;
				uint bit = (symbol >> bitIndex) & 1;
				price += _models[m].GetPrice(bit);
				m = (m << 1) + bit;
			}
			return price;
		}

		public uint ReverseGetPrice(uint symbol)
		{
			uint price = 0;
			uint m = 1;
			for (int i = _numBitLevels; i > 0; i--)
			{
				uint bit = symbol & 1;
				symbol >>= 1;
				price += _models[m].GetPrice(bit);
				m = (m << 1) | bit;
			}
			return price;
		}

		public static uint ReverseGetPrice(BitEncoder[] models, uint startIndex, int numBitLevels, uint symbol)
		{
			uint price = 0;
			uint m = 1;
			for (int i = numBitLevels; i > 0; i--)
			{
				uint bit = symbol & 1;
				symbol >>= 1;
				price += models[startIndex + m].GetPrice(bit);
				m = (m << 1) | bit;
			}
			return price;
		}

		public static void ReverseEncode(BitEncoder[] Models, uint startIndex, Encoder rangeEncoder, int numBitLevels, uint symbol)
		{
			uint m = 1;
			for (int i = 0; i < numBitLevels; i++)
			{
				uint bit = symbol & 1;
				Models[startIndex + m].Encode(rangeEncoder, bit);
				m = (m << 1) | bit;
				symbol >>= 1;
			}
		}
	}
}
