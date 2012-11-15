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
namespace System
{
    /// <summary>
    /// Provides an advanced façade pattern that facilitates a large range of text-oriented text checking, parsing, and calculation
    /// functions into a single wrapper class.
    /// </summary>
#if COREINTERNAL
    internal
#else
    public
#endif
 static partial class StringEx
    {
        /// <summary>
        /// String value 'Null'
        /// </summary>
        public const string Null = "Null";

        /// <summary>
        /// Returns string based on values provided. If <paramref name="a"/> and 'b' are non-null, then returns 'a+x+b'. Otherwise returns 'a', unless empty, then
        /// returns 'b'.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="x">Second value.</param>
        /// <param name="b">Third value.</param>
        /// <returns>String result.</returns>
        public static string Axb(string a, string x, string b)
        {
            var isAValid = ParserEx_Validate_String(a);
            var isBValid = ParserEx_Validate_String(b);
            if (isAValid && isBValid) return a + x + b;
            else if (isAValid) return a;
            else if (isBValid) return b;
            return string.Empty;
        }
        /// <summary>
        /// Returns string based on values provided. If c3 is non-empty, then appends c2 and c3 to the StringBuilder provided.
        /// Otherwise, appends c3.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <param name="b">The text builder.</param>
        /// <returns></returns>
        public static StringBuilder Axb(StringBuilder a, string x, string b)
        {
            if (a == null)
                throw new ArgumentNullException("left");
            var isBValid = ParserEx_Validate_String(b);
            if (isBValid)
                a.Append(a.Length > 0 ? x + b : b);
            return a;
        }

        /// <summary>
        /// Returns a+y if a is a valid string, otherwise returns string.Empty
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static string Ay(string a, string y)
        {
            return (ParserEx_Validate_String(a) ? a + y : string.Empty);
        }

        /// <summary>
        /// If all values are non-empty, returns concatenated string, otherwise returns empty string.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="y">The y.</param>
        /// <param name="b">The b.</param>
        /// <returns>String result.</returns>
        public static string Ayb(string a, string y, string b)
        {
            return (ParserEx_Validate_String(a) && ParserEx_Validate_String(b) ? a + y + b : string.Empty);
        }

        /// <summary>
        /// Coalesces the specified parameter array.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>First non-null or empty string.</returns>
        public static string Coalesce(params string[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            foreach (string value in values)
                if (!string.IsNullOrEmpty(value))
                    return value;
            return string.Empty;
        }

        //public static string Space(int length)
        //{
        //    if (length < 0)
        //        throw new ArgumentOutOfRangeException("length");
        //    return new string(' ', length);
        //}

        /// <summary>
        /// Returns x+a if a is a valid string, otherwise returns string.Empty
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="a">A.</param>
        /// <returns></returns>
        public static string Xa(string x, string a)
        {
            return (ParserEx_Validate_String(a) ? x + a : string.Empty);
        }

        /// <summary>
        /// Returns x+a+y if a is a valid string, otherwise returns string.Empty
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="a">A.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static string Xay(string x, string a, string y)
        {
            return (ParserEx_Validate_String(a) ? x + a + y : string.Empty);
        }

        /// <summary>
        /// ParserEx.Validate
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static bool ParserEx_Validate_String(string text)
        {
            return (text != null && text.Trim().Length > 0);
        }
    }
}
