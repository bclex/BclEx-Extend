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
namespace System
{
    static partial class CoreExtensions
    {
        /// <summary>
        /// Hooks the value factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazy">The lazy.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns></returns>
        public static Lazy<T> HookValueFactory<T>(this Lazy<T> lazy, Func<Func<T>, T> valueFactory) { LazyHelper<T>.HookValueFactory(lazy, valueFactory); return lazy; }

        private class LazyHelper<T>
        {
            private static readonly object _lock = new object();
            private static readonly FieldInfo _valueFactoryField = typeof(Lazy<T>).GetField("m_valueFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            public static void HookValueFactory(Lazy<T> lazy, Func<Func<T>, T> valueFactory)
            {
                lock (_lock)
                {
                    var hook = (Func<T>)_valueFactoryField.GetValue(lazy);
                    Func<T> newHook = () => valueFactory(hook);
                    _valueFactoryField.SetValue(lazy, newHook);
                }
            }
        }
    }
}
