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
using System.Collections.Generic;
using System.Collections;
namespace Contoso.Collections
{
    /// <summary>
    /// ForwardBlockedCollection
    /// </summary>
    public abstract class ForwardBlockedCollection<TBlock, TValue>
    {
        private LinkedList<Block> _blocks = new LinkedList<Block>();
        /// <summary>
        /// 
        /// </summary>
        protected int _blockSize = 0x10000;

        /// <summary>
        /// Block
        /// </summary>
        public class Block : IEnumerable<TValue>
        {
            /// <summary>
            /// 
            /// </summary>
            public int Count;
            /// <summary>
            /// 
            /// </summary>
            public TBlock[] Items;

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        /// <summary>
        /// Gets the blocks.
        /// </summary>
        public LinkedList<Block> Blocks
        {
            get { return _blocks; }
        }

        /// <summary>
        /// Gets or sets the size of the block.
        /// </summary>
        /// <value>
        /// The size of the block.
        /// </value>
        protected int BlockSize
        {
            get { return _blockSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _blockSize = value;
            }
        }

        /// <summary>
        /// Factories the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract TBlock Factory(ref TValue value);

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TValue Add(ref TValue value)
        {
            // complete existing block
            Block b;
            var last = _blocks.Last;
            if (last != null && (b = last.Value).Count < _blockSize)
            {
                b.Items[b.Count++] = Factory(ref value);
                return value;
            }
            // add new block
            var items = new TBlock[_blockSize];
            items[0] = Factory(ref value);
            _blocks.AddLast(new Block { Count = 1, Items = items });
            return value;
        }

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public TValue Add(IEnumerable<TValue> values)
        {
            int count;
            TBlock[] items, carryItems;
            // complete existing block
            Block b;
            var last = _blocks.Last;
            if (last != null && (b = last.Value).Count < _blockSize)
            {
                count = b.Count;
                items = carryItems = b.Items;
            }
            else
            {
                b = null;
                count = 0;
                items = new TBlock[_blockSize];
                carryItems = null;
            }
            // add new blocks
            TValue value = default(TValue);
            for (var e = values.GetEnumerator(); e.MoveNext(); )
            {
                value = e.Current;
                items[count++] = Factory(ref value);
                if (count != _blockSize)
                    continue;
                // add new block
                if (items != carryItems)
                    _blocks.AddLast(new Block { Count = count, Items = items });
                else
                    b.Count = count;
                count = 0;
                items = new TBlock[_blockSize];
            }
            if (count > 0)
                if (items != carryItems)
                    _blocks.AddLast(new Block { Count = count, Items = items });
                else
                    b.Count = count;
            return value;
        }
    }
}