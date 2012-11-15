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
    /// A generic interface providing a generically typed indexer. Allows passing of object instances implementing interface
    /// without explicit knowledge of the object implementation specifics.
    /// </summary>
    /// <typeparam name="TKey">Generic type of the key for the indexed object instance.</typeparam>
    /// <typeparam name="TValue">Generic type for the value in the indexed object instance.</typeparam>
#if !COREINTERNAL
    public
#endif
 interface IDictionaryIndexer<TKey, TValue> : IIndexer<TKey, TValue>
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
        /// <summary>
        /// Determines whether the specified key exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key exists; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsKey(TKey key);
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool Remove(TKey key);
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Tries the get value at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool TryGetValueAt(int index, out TValue value);

        /// <summary>
        /// Gets the ICollection instance containing the Keys for the collection.
        /// </summary>
        /// <value>The key enum.</value>
        ICollection<TKey> Keys { get; }
        /// <summary>
        /// Gets the ICollection instance containing the Values for the collection.
        /// </summary>
        /// <value>The value enum.</value>
        ICollection<TValue> Values { get; }
    }

    /// <summary>
    /// Provides dictionary-based collection and indexer functionality using generic types.
    /// </summary>
    /// <typeparam name="TKey">Generic type of a key</typeparam>
    /// <typeparam name="TValue">Generic type of a value in the collection</typeparam>
#if !COREINTERNAL
    public
#endif
 class DictionaryIndexer<TKey, TValue> : IDictionaryIndexer<TKey, TValue>
    {
        /// <summary>
        /// Protected Member variable that exposes the the IDictionary instance used as the underlying storage object for this type.
        /// </summary>
        protected IDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public DictionaryIndexer()
            : this(new Dictionary<TKey, TValue>(), null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DictionaryIndexer(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryIndexer&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The hash.</param>
        /// <param name="factory">The factory.</param>
        public DictionaryIndexer(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> factory)
        {
            _dictionary = dictionary;
            Factory = factory;
        }

        /// <summary>
        /// Gets or sets the value (of generic parameter type TValue) associated with the specified key.
        /// </summary>
        /// <value>Value associate with key provided.</value>
        public virtual TValue this[TKey key]
        {
            get
            {
                if (_dictionary == null)
                    throw new InvalidOperationException("_hash is null");
                TValue value;
                if (_dictionary.TryGetValue(key, out value))
                    return value;
                if (Factory != null)
                {
                    _dictionary.Add(key, value = Factory(key));
                    return value;
                }
                throw new ArgumentException(string.Format(Local.UndefinedItemAB, "Dictionary", key), "key");
            }
            set
            {
                if (_dictionary == null)
                    throw new InvalidOperationException("_hash is null");
                _dictionary[key] = value;
            }
        }

        /// <summary>
        /// Allows creation of instances of the type originally associated with keys used by this generic collection class.
        /// </summary>
        public Func<TKey, TValue> Factory { get; private set; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            if (_dictionary != null)
                _dictionary.Clear();
        }

        /// <summary>
        /// Gets the count of the items in collection.
        /// </summary>
        /// <value>The count.</value>
        public virtual int Count
        {
            get { return (_dictionary != null ? _dictionary.Count : 0); }
        }

        /// <summary>
        /// Exposes the the IDictionary instance used as the underlying storage object for this type.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public IDictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
        }

        /// <summary>
        /// Gets the generic <typeparamref name="TValue"/> value associated with the specified <c>key</c>.
        /// </summary>
        /// <param name="index">The index whose value to get or set.</param>
        /// <returns></returns>
        /// <value>Returns the generic <typeparamref name="TValue"/> value associated with the specified <c>key</c>.</value>
        public virtual TValue GetValueAt(int index)
        {
            if (_dictionary == null)
                throw new InvalidOperationException("_dictionary is null");
            if (index >= _dictionary.Count)
                throw new ArgumentOutOfRangeException("key");
            //return m_hash.GetValue(index);
            throw new NotSupportedException();
        }
        /// <summary>
        /// Gets the generic <typeparamref name="TValue"/> value associated with the specified <c>key</c>.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <param name="defaultValue">The default value to return if no value is found associated with <c>key</c>.</param>
        /// <returns></returns>
        /// <value>Returns the generic <typeparamref name="TValue"/> value associated with the specified <c>key</c>.</value>
        public virtual TValue GetValue(TKey key, TValue defaultValue)
        {
            if (_dictionary == null)
                throw new InvalidOperationException("_hash is null");
            TValue value = _dictionary[key];
            if (value != null)
                return value;
            throw new ArgumentNullException(string.Format(Local.UndefinedKeyA, key), "key");
        }

        /// <summary>
        /// Determines whether the item in collection with specified key exists.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified item exists; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsKey(TKey key)
        {
            return (_dictionary != null ? _dictionary.ContainsKey(key) : false);
        }

        /// <summary>
        /// Removes the item with the specified key.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns></returns>
        public virtual bool Remove(TKey key)
        {
            return (_dictionary != null ? _dictionary.Remove(key) : false);
        }

        /// <summary>
        /// Return an instance of <see cref="System.Collections.Generic.ICollection{TKey}"/> representing the collection of
        /// keys in the indexed collection.
        /// </summary>
        /// <value>
        /// The <see cref="System.Collections.Generic.ICollection{TKey}"/> instance containing the collection of keys.
        /// </value>
        public virtual ICollection<TKey> Keys
        {
            get { return (_dictionary != null ? _dictionary.Keys : null); }
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary != null)
                return _dictionary.TryGetValue(key, out value);
            value = default(TValue);
            return false;
        }
        /// <summary>
        /// Tries the get value at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual bool TryGetValueAt(int index, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Return an instance of <see cref="System.Collections.Generic.ICollection{TValue}"/> representing the collection of
        /// values in the indexed collection.
        /// </summary>
        /// <value>
        /// The <see cref="System.Collections.Generic.ICollection{TValue}"/> instance containing the collection of values.
        /// </value>
        public virtual ICollection<TValue> Values
        {
            get { return (_dictionary != null ? _dictionary.Values : null); }
        }
    }
}
