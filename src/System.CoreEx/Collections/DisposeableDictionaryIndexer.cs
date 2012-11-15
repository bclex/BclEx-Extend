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
using System.Collections.Generic;
namespace System.Collections
{
    /// <summary>
    /// Provides hash-based collection and indexer functionality using generic types.
    /// </summary>
    /// <typeparam name="TKey">Generic type of a key</typeparam>
    /// <typeparam name="TValue">Generic type of a value in the collection</typeparam>
#if !COREINTERNAL
    public
#endif
 class DisposeableDictionaryIndexer<TKey, TValue> : DictionaryIndexer<TKey, TValue>
        where TValue : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeableDictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public DisposeableDictionaryIndexer()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeableDictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DisposeableDictionaryIndexer(IDictionary<TKey, TValue> dictionary)
            : base(dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeableDictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="factory">The factory.</param>
        public DisposeableDictionaryIndexer(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> factory)
            : base(dictionary, factory) { }

        /// <summary>
        /// Implements the <see cref="System.IDisposable.Dispose"/> method of
        /// <see cref="System.IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Dispose(true);
            }
        }
        /// <summary>
        /// Disposes this instance of the object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if ((!disposing) && (_dictionary != null))
            {
                foreach (TValue value in _dictionary.Values)
                    value.Dispose();
                _dictionary.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the value (of generic parameter type TValue) associated with the specified key.
        /// </summary>
        /// <value>Value associate with key provided.</value>
        public override TValue this[TKey key]
        {
            set
            {
                if (_dictionary == null)
                    throw new InvalidOperationException("_dictionary is null");
                TValue lastValue;
                if (_dictionary.TryGetValue(key, out lastValue))
                    lastValue.Dispose();
                _dictionary[key] = value;
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            if (_dictionary != null)
            {
                foreach (TValue value in _dictionary.Values)
                    value.Dispose();
                _dictionary.Clear();
            }
        }

        /// <summary>
        /// Removes the item with the specified key.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns></returns>
        public override bool Remove(TKey key)
        {
            if (_dictionary != null)
            {
                TValue lastValue;
                if (_dictionary.TryGetValue(key, out lastValue))
                    lastValue.Dispose();
                return _dictionary.Remove(key);
            }
            return false;
        }
    }
}
