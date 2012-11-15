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
namespace System
{
    /// <summary>
    /// Structs containing (and wrapping) a single reference will have reference semantics
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct WeakReference<T>
        where T : class
    {
        private readonly WeakReference _wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> struct.
        /// </summary>
        /// <param name="target">The target.</param>
        public WeakReference(T target) { _wrapped = new WeakReference(target); }
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> struct.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        public WeakReference(T target, bool trackResurrection) { _wrapped = new WeakReference(target, trackResurrection); }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return _wrapped.IsAlive; }
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public T Target
        {
            get
            {
                // Will throw ClassCastException if class is not expected object, if you wish to prevent this - use "wrapped.Target as T" construct thich will return "null" - valid return value in this rare situation
                // i.e. in case if wrapped WeakReference was modified in some way outside of wrapper
                return (T)_wrapped.Target;
            }
            set { _wrapped.Target = value; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
		public override bool Equals(object o) { return _wrapped.Equals(o); }
		//    if (o == this)
		//        return true;
		//    var key = (o as WeakReference<T>);
		//    if (key == null)
		//        return false;
		//    T item = Item;
		//    if (item == null)
		//        return false;
		//    return ((_hashCode == key._hashCode) && (object.Equals(item, key.Item)));

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
		public override int GetHashCode() { return _wrapped.GetHashCode(); }

        /// <summary>
        /// Gets a value indicating whether [track resurrection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [track resurrection]; otherwise, <c>false</c>.
        /// </value>
        public bool TrackResurrection
        {
            get { return _wrapped.TrackResurrection; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.WeakReference&lt;T&gt;"/> to <see typeparamref="T"/>.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(WeakReference<T> reference) { return (reference._wrapped != null ? (T)reference._wrapped.Target : null); }
    }
}