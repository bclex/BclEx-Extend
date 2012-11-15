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
namespace Contoso.Primitives.TextPacks
{
    /// <summary>
    /// Abstract base class that defines the core feature-set for encoding and decoding string data
    /// into and out of specific string representations defined by the implementation logic
    /// provided by the subclass.
    /// </summary>
    public interface ITextPack
    {
        /// <summary>
        /// Decodes the specified pack.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <param name="namespaceID">The namespace ID.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDictionary<string, string> Decode(string pack, string namespaceID, Predicate<string> predicate);
        /// <summary>
        /// Encodes the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="namespaceID">The namespace ID.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        string Encode(IDictionary<string, string> values, string namespaceID, Predicate<string> predicate);
    }
}