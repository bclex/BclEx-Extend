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
 interface IIndexer<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <value>The value associated with the specified key.</value>
        TValue this[TKey key] { get; set; }
    }
}
