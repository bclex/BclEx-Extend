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
#if COREINTERNAL
    internal
#else
    public
#endif
 static partial class StringEx
    {
        /// <summary>
        /// 
        /// </summary>
        public static class ExtractString
        {
            /// <summary>
            /// Text extraction type characterized by numbers
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string ExtractDigit(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                var b = new StringBuilder();
                foreach (char c in text)
                    if (char.IsDigit(c))
                        b.Append(c);
                return b.ToString();
            }

            /// <summary>
            /// Text extraction type characterized by characters
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string ExtractNonDigit(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                var b = new StringBuilder();
                foreach (char c in text)
                    if (!char.IsDigit(c))
                        b.Append(c);
                return b.ToString();
            }

            /// <summary>
            /// Text extraction type characterized by alphanumeric characters (numbers or characters).
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string ExtractAlpha(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                var b = new StringBuilder();
                foreach (char c in text)
                    if (char.IsLetter(c))
                        b.Append(c);
                return b.ToString();
            }

            /// <summary>
            /// Text extraction type characterized by alphanumeric characters (numbers or characters).
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string ExtractAlphaDigit(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                var b = new StringBuilder();
                foreach (char c in text)
                    if (char.IsLetterOrDigit(c))
                        b.Append(c);
                return b.ToString();
            }

            /// <summary>
            /// Text extraction type characterized by all but the last word (defined by uppercase)
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string ExtractAllButLastWord(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                for (var textIndex = text.Length - 1; textIndex > 0; textIndex--)
                    if (char.IsUpper(text[textIndex]))
                        return text.Substring(0, textIndex);
                return string.Empty;
            }
        }
    }
}
