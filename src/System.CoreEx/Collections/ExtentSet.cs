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
using System.Reflection;
namespace System.Collections.Generic
{
    /// <summary>
    /// IExtentSet
    /// </summary>
    public interface IExtentSet
    {
        /// <summary>
        /// Gets a value indicating whether this instance has extents.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has extents; otherwise, <c>false</c>.
        /// </value>
        bool HasExtents { get; }
        /// <summary>
        /// Determines whether this instance has extent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///   <c>true</c> if this instance has extent; otherwise, <c>false</c>.
        /// </returns>
        bool HasExtent<T>();
        /// <summary>
        /// Determines whether the specified type has extent.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type has extent; otherwise, <c>false</c>.
        /// </returns>
        bool HasExtent(Type type);
        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void Set<T>(T value);
        /// <summary>
        /// Sets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void SetMany<T>(IEnumerable<T> value);
        /// <summary>
        /// Sets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        void Set(Type type, object value);
        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Clear<T>();
        /// <summary>
        /// Clears the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        void Clear(Type type);
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>();
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetMany<T>();
        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object Get(Type type);
        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        bool TryGet<T>(out T extent);
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="extents">The extents.</param>
        void AddRange(IEnumerable<object> extents);
    }

    /// <summary>
    /// ExtentSet
    /// </summary>
    public class ExtentSet : IExtentSet
    {
        private static readonly MethodInfo _hasExtentMethod = typeof(ExtentSet).GetGenericMethod("HasExtent");
        private static readonly MethodInfo _setMethod = typeof(ExtentSet).GetGenericMethod("Set");
        private static readonly MethodInfo _clearMethod = typeof(ExtentSet).GetGenericMethod("Clear");
        private static readonly MethodInfo _getMethod = typeof(ExtentSet).GetGenericMethod("Get");
        private Dictionary<Type, object> _extents;

        /// <summary>
        /// Gets or sets the extents.
        /// </summary>
        /// <value>
        /// The extents.
        /// </value>
        public Dictionary<Type, object> Extents
        {
            get { return _extents; }
            set { _extents = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has extents.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has extents; otherwise, <c>false</c>.
        /// </value>
        public bool HasExtents
        {
            get { return (_extents != null && _extents.Count > 0); }
        }

        /// <summary>
        /// Determines whether this instance has extent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///   <c>true</c> if this instance has extent; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExtent<T>()
        {
            return (_extents != null && _extents.ContainsKey(typeof(T)));
        }
        /// <summary>
        /// Determines whether the specified type has extent.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type has extent; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExtent(Type type) { return (bool)_hasExtentMethod.MakeGenericMethod(type).Invoke(this, null); }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        public void Set<T>(T value)
        {
            if (_extents == null)
                _extents = new Dictionary<Type, object>();
            _extents[typeof(T)] = value;
        }
        /// <summary>
        /// Sets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        public void SetMany<T>(IEnumerable<T> value)
        {
            if (_extents == null)
                _extents = new Dictionary<Type, object>();
            _extents[typeof(IEnumerable<T>)] = value;
        }
        /// <summary>
        /// Sets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public void Set(Type type, object value) { _setMethod.MakeGenericMethod(type).Invoke(this, new[] { value }); }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Clear<T>()
        {
            if (_extents != null)
                _extents.Remove(typeof(T));
        }
        /// <summary>
        /// Clears the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Clear(Type type) { _clearMethod.MakeGenericMethod(type).Invoke(this, null); }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            object value;
            return (_extents == null || !_extents.TryGetValue(typeof(T), out value) ? default(T) : (T)value);
        }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMany<T>()
        {
            object value;
            return (_extents == null || !_extents.TryGetValue(typeof(IEnumerable<T>), out value) ? default(IEnumerable<T>) : (IEnumerable<T>)value);
        }
        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object Get(Type type) { return _hasExtentMethod.MakeGenericMethod(type).Invoke(this, null); }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        public bool TryGet<T>(out T extent)
        {
            object value;
            if (_extents == null || !_extents.TryGetValue(typeof(T), out value))
            {
                extent = default(T);
                return false;
            }
            extent = (T)value;
            return true;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="extents">The extents.</param>
        public void AddRange(IEnumerable<object> extents)
        {
            if (extents == null)
                throw new ArgumentNullException("extents");
            foreach (var extent in extents)
                Set(extent.GetType(), extent);
        }
    }
}
