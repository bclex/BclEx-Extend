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
using System.Text;
using System.Security.Cryptography;
using System;
namespace Contoso.Primitives.Codecs
{
    /// <summary>
    /// Encodes and decodes the generation of digest values using an instance of <see cref="System.Security.Cryptography.SHA1Managed"/>.
    /// </summary>
    public class SHA1DigestCodec : ICodec<string, string>
    {
        private static readonly SHA1Managed _digest = new SHA1Managed();

        /// <summary>
        /// Encodes the specified text using the underlying
        /// <see cref="System.Security.Cryptography.SHA1Managed"/> instance.
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <returns>Returns decoded value.</returns>
        public static string Encode(string text)
        {
            return ConvertEx.ToBase16String(_digest.ComputeHash(Encoding.Unicode.GetBytes(text)));
        }

        #region ICodec

        /// <summary>
        /// Abstract member whose implementation by the derived class decodes a string into another string.
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// Returns results of decoding <paramref name="text"/> object instance.
        /// </returns>
        string ICodec<string, string>.Decode(string text, object tag) { throw new NotSupportedException(); }

        /// <summary>
        /// Abstract member whose implementation by the derived class encodes a string into another string.
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// Returns results of decoding <paramref name="text"/> object instance.
        /// </returns>
        string ICodec<string, string>.Encode(string text, object tag) { return Encode(text); }

        #endregion
    }
}