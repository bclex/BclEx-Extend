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
using System.Linq;
namespace System.Collections.Generic
{
    /// <summary>
    /// KeyEqualityComparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, object> _keyAccessor;

        // Allows us to simply specify the key to compare with: y => y.CustomerID
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="keyAccessor">The key accessor.</param>
        public KeyEqualityComparer(Func<T, object> keyAccessor)
            : this(keyAccessor, null) { }
        // Allows us to tell if two objects are equal: (x, y) => y.CustomerID == x.CustomerID
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public KeyEqualityComparer(Func<T, T, bool> comparer)
            : this(null, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="keyAccessor">The key accessor.</param>
        /// <param name="comparer">The comparer.</param>
        public KeyEqualityComparer(Func<T, object> keyAccessor, Func<T, T, bool> comparer)
        {
            _keyAccessor = keyAccessor;
            _comparer = comparer;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(T x, T y)
        {
            if (_comparer != null)
                return _comparer(x, y);
            else
            {
                var valX = _keyAccessor(x);
                if (valX is IEnumerable<object>) // The special case where we pass a list of keys
                    return ((IEnumerable<object>)valX).SequenceEqual((IEnumerable<object>)_keyAccessor(y));
                return valX.Equals(_keyAccessor(y));
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        ///   </exception>
        public int GetHashCode(T obj)
        {
            if (_keyAccessor == null)
                return obj.ToString().ToLower().GetHashCode();
            else
            {
                var val = _keyAccessor(obj);
                if (val is IEnumerable<object>) // The special case where we pass a list of keys
                    return (int)((IEnumerable<object>)val).Aggregate((x, y) => x.GetHashCode() ^ y.GetHashCode());
                return val.GetHashCode();
            }
        }
    }
}