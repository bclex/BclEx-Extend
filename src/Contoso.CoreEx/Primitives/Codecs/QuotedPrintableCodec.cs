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
using System.Diagnostics;
using System;
namespace Contoso.Primitives.Codecs
{
    /// <summary>
    /// QuotedPrintableCodec
    /// </summary>
    public class QuotedPrintableCodec : ICodec<string>
    {
        /// <summary>
        /// EncodeNullValue
        /// </summary>
        public static readonly string EncodeNullValue = string.Empty;
        /// <summary>
        /// DecodeNullValue
        /// </summary>
        public static readonly string DecodeNullValue = string.Empty;

        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Decode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return DecodeNullValue;
            // normalize line-break
            text = text.Replace("\x0d\x0a", "\x0a").Replace('\x0d', '\x0a').Replace("=\x0a", string.Empty);
            //
            var b = new StringBuilder();
            int startIndex = 0;
            int equalIndex;
            int textLength = text.Length;
            while ((equalIndex = text.IndexOf('=', startIndex)) > -1)
            {
                b.Append(text.Substring(startIndex, equalIndex - startIndex));
                Debug.Assert(((equalIndex + 2) < textLength), "Incorrect length quote-printable string");
                if ((equalIndex + 2) < textLength)
                {
                    // decode hex character
                    b.Append((char)Convert.ToInt16(text.Substring(equalIndex + 1, 2), 16));
                    startIndex = equalIndex + 3;
                }
                else
                    break;
            }
            if (startIndex < textLength)
                b.Append(text.Substring(startIndex, textLength - startIndex - 1));
            // convert line-break to system line-break sequence
            return b.ToString().Replace("\x0a", Environment.NewLine);
        }

        /// <summary>
        /// Encodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return EncodeNullValue;
            int lineLength = 0;
            var b = new StringBuilder();
            for (int index = 0; index < text.Length; index++)
            {
                if (lineLength >= 73)
                {
                    // soft line-break
                    b.AppendLine("=");
                    lineLength = 0;
                }
                //
                char character = text[index];
                int characterCode = Convert.ToInt16(character);
                if (characterCode <= 255)
                {
                    if (((characterCode >= 32) && (characterCode < 127) && (characterCode != 61)) || (characterCode == 9))
                    {
                        b.Append(character);
                        lineLength++;
                    }
                    else if ((characterCode == 13) && ((index + 1) < text.Length) && (text[index + 1] == '\x0a'))
                    {
                        index++;
                        // fix line ending with tab or space
                        FixTrailingSpace(b);
                        b.AppendLine();
                        lineLength = 0;
                    }
                    else
                    {
                        b.Append("=" + ((characterCode < 15 ? "0" : string.Empty) + Convert.ToString(characterCode, 16)).ToUpper());
                        lineLength += 3;
                    }
                    //} else {
                    //   // double-byte character
                    //   if (lineLength >= 70) {
                    //      //+ soft line-break
                    //      b.AppendLine("=");
                    //      lineLength = 0;
                    //   }
                    //   //b.Append("=" + ((byteValue < 15 ? "0" : string.Empty) + Convert.ToString(byteValue, 16)).ToUpper());
                    //   //lineLength += 3;
                }
            }
            FixTrailingSpace(b);
            return b.ToString();
        }

        /// <summary>
        /// Fixes the trailing space.
        /// </summary>
        /// <param name="b">The buffer.</param>
        private static void FixTrailingSpace(StringBuilder b)
        {
            int lastCharacterCode = Convert.ToInt16(b[b.Length - 1]);
            if ((lastCharacterCode == 9) || (lastCharacterCode == 32))
            {
                b.Length--;
                b.Append("=" + ((lastCharacterCode < 15 ? "0" : string.Empty) + Convert.ToString(lastCharacterCode, 16)).ToUpper());
            }
        }

        #region ICodec

        /// <summary>
        /// Abstract member whose implementation by the derived class decodes a string into another string.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="text">String to decode.</param>
        /// <returns>
        /// Returns results of decoding <paramref name="text"/> object instance.
        /// </returns>
        string ICodec<string>.Decode(object tag, string text) { return Decode(text); }

        /// <summary>
        /// Abstract member whose implementation by the derived class encodes a string into another string.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="text">String to decode.</param>
        /// <returns>
        /// Returns results of decoding <paramref name="text"/> object instance.
        /// </returns>
        string ICodec<string>.Encode(object tag, string text) { return Encode(text); }

        #endregion
    }
}