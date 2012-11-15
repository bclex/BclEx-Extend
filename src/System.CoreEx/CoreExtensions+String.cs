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
using System.Text.RegularExpressions;
namespace System
{
    static partial class CoreExtensions
    {
        /// <summary>
        /// Replaces the ex.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static string ReplaceEx(this string text, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
                return text.Replace(oldValue, newValue);
            var textLength = text.Length;
            oldValue = oldValue.ToUpperInvariant();
            var oldValueLength = oldValue.Length;
            var newValueLength = newValue.Length;
            //
            var startIndex = 0;
            var index = 0;
            var upperString = text.ToUpperInvariant();
            //
            var sizeIncrease = Math.Max(0, (textLength / oldValueLength) * (newValueLength - oldValueLength));
            var buffer = new char[textLength + sizeIncrease];
            var bufferIndex = 0;
            int copyCount;
            while ((index = upperString.IndexOf(oldValue, startIndex, comparisonType)) != -1)
            {
                copyCount = index - startIndex; text.CopyTo(startIndex, buffer, bufferIndex, copyCount); bufferIndex += copyCount;
                newValue.CopyTo(0, buffer, bufferIndex, newValueLength); bufferIndex += newValueLength;
                //for (int textIndex = startIndex; textIndex < index; textIndex++)
                //    buffer[bufferIndex++] = text[textIndex];
                //for (int textIndex = 0; textIndex < newValueLength; textIndex++)
                //    buffer[bufferIndex++] = newValue[textIndex];
                startIndex = index + oldValueLength;
            }
            if (startIndex == 0)
                return text;
            copyCount = textLength - startIndex; text.CopyTo(startIndex, buffer, bufferIndex, copyCount); bufferIndex += copyCount;
            //for (int textIndex = startIndex; textIndex < textLength; textIndex++)
            //    buffer[bufferIndex++] = text[textIndex];
            return new string(buffer, 0, bufferIndex);
        }

        /// <summary>
        /// Enumeration representing the types of truncation support by a class.
        /// </summary>
        public enum TextTruncateType
        {
            /// <summary>
            /// Normal
            /// </summary>
            Normal,
            /// <summary>
            /// LastWhitespace
            /// </summary>
            LastWhitespace,
            /// <summary>
            /// FromAtomSite
            /// </summary>
            FromAtomSite,
        }

        //public static void Guard(this string str, string paramName)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        throw new ArgumentNullException(paramName);
        //}
        //public static void Guard(this string str, string paramName, string message)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        throw new ArgumentNullException(paramName, message);
        //}

        /// <summary>
        /// Spaces the on pascal case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string SpaceOnPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return Regex.Replace(text, "([A-Z])", " $1").Trim();
        }

        /// <summary>
        /// Determines whether value specified is contained in the array provided by performing a case-insensitive
        /// culture-invariant string comparision.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is value in array invariant] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool ExistsIgnoreCase(this string[] array, string value)
        {
            if (value != null)
            {
                foreach (string c in array)
                    if (c != null && string.Equals(c, value, StringComparison.OrdinalIgnoreCase))
                        return true;
                return false;
            }
            foreach (string c in array)
                if (c == null)
                    return true;
            return false;
        }

        /// <summary>
        /// Camelizes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Camelize(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return (text.Length > 2 ? char.ToLowerInvariant(text[0]) + text.Substring(1) : text);
        }

        /// <summary>
        /// Counts the of.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="countOfText">The count of text.</param>
        /// <returns></returns>
        public static int CountOf(this string text, string countOfText)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (countOfText == null)
                throw new ArgumentNullException("countOfText");
            var countOfTextLength = countOfText.Length;
            var countOfTextRemain = (text.Length - text.Replace(countOfText, string.Empty).Length);
            return (countOfTextLength == 1 ? countOfTextRemain : (countOfTextLength == 2 ? countOfTextRemain >> 1 : countOfTextRemain / countOfTextLength));
        }

        /// <summary>
        /// Determine whether a string ends with the provided value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool EndsWithSlim(this string text, string value)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (value == null)
                throw new ArgumentNullException("value");
            var valueLength = value.Length;
            return (text.Length < valueLength ? false : (text.Substring(text.Length - valueLength) == value));
        }

        /// <summary>
        /// Determine whether a string ends with the provided value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c> a case-insensitive compare is performed.</param>
        /// <returns></returns>
        public static bool EndsWithSlim(this string text, string value, bool isIgnoreCase)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (value == null)
                throw new ArgumentNullException("value");
            var valueLength = value.Length;
            return (text.Length < valueLength ? false : (string.Compare(text.Substring(text.Length - valueLength), value, isIgnoreCase) == 0));
        }

        /// <summary>
        /// Ensures the ends with.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns></returns>
        public static string EnsureEndsWith(this string text, string suffix)
        {
            return (!string.IsNullOrEmpty(text) && !text.EndsWithSlim(suffix) ? text + suffix : text);
        }

        /// <summary>
        /// Returns the left most portion of the string provided.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string Left(this string text, int maxLength)
        {
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength");
            return (string.IsNullOrEmpty(text) ? string.Empty : (text.Length >= maxLength ? text.Substring(0, maxLength) : text));
        }

        /// <summary>
        /// Returns string based on an invariant, case-insensitive string comparison. Returns nullIfText is text is empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string NullIf(this string text)
        {
            return (!string.IsNullOrEmpty(text) ? text : null);
        }
        /// <summary>
        /// Returns string based on an invariant, case-insensitive string comparison. Returns nullIfText is text is empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="nullIfText">The null if text.</param>
        /// <returns></returns>
        public static string NullIf(this string text, string nullIfText)
        {
            return (text != null && !string.Equals(text, nullIfText, StringComparison.OrdinalIgnoreCase) ? text : null);
        }

        /// <summary>
        /// Nulls if.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static string NullIf(this string text, Predicate<string> condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");
            return (text != null && !condition(text) ? text : null);
        }

        /// <summary>
        /// Returns a section of the provided text starting at the startIndex.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>Returns string result.</returns>
        public static string Mid(this string text, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            return (text == null || startIndex >= text.Length ? string.Empty : text.Substring(startIndex));
        }
        /// <summary>
        /// Returns a section of the provided text starting at the startIndex and extending up to the maxlength value provided.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns>
        /// Returns string result.
        /// </returns>
        public static string Mid(this string text, int startIndex, int maxLength)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength");
            if (text == null || startIndex >= text.Length || (startIndex + maxLength) <= 0)
                return string.Empty;
            return (text.Length >= (startIndex + maxLength) ? text.Substring(startIndex, maxLength) : text.Substring(startIndex));
        }

        /// <summary>
        /// Parses the the string value representing a prefix based on the defined bindingSet provided.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bindingSet">The binding set.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public static string ParseBoundedPrefix(this string text, string bindingSet, out string prefix)
        {
            if (bindingSet == null || bindingSet.Length != 2)
                throw new ArgumentNullException("bindingSet");
            if (text.Length > 0 && text[0] == bindingSet[0])
            {
                var key = text.IndexOf(bindingSet[1]);
                if (key > -1)
                {
                    prefix = text.Substring(1, key - 1);
                    return text.Substring(key + 1);
                }
            }
            prefix = string.Empty;
            return (text ?? string.Empty);
        }

        /// <summary>
        /// Parses the bounded postfix.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bindingSet">The binding set.</param>
        /// <param name="postfix">The postfix.</param>
        /// <returns></returns>
        public static string ParseBoundedPostfix(this string text, string bindingSet, out string postfix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the right-most portion of the provided text, up to the maxLength provided.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string Right(this string text, int maxLength)
        {
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength");
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return (text.Length >= maxLength ? text.Substring(text.Length - maxLength) : text);
        }

        /// <summary>
        /// Singles the split.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="split">The split.</param>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool SingleSplit(this string text, string split, out string a, out string b)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (split == null || split.Length == 0)
                throw new ArgumentNullException("split");
            var splitIndex = text.IndexOf(split);
            if (splitIndex > -1)
            {
                a = text.Substring(0, splitIndex);
                b = text.Substring(splitIndex + split.Length);
                return true;
            }
            a = text;
            b = string.Empty;
            return false;
        }

        /// <summary>
        /// Determine whether a string starts with the provided value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool StartsWithSlim(this string text, string value)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (value == null)
                throw new ArgumentNullException("value");
            var valueLength = value.Length;
            return (text.Length < valueLength ? false : text.Substring(0, valueLength) == value);
        }

        /// <summary>
        /// Determine whether a string starts with the provided value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="isIgnoreCase">if set to <c>true</c> a case-insensitive compare is performed.</param>
        /// <returns></returns>
        public static bool StartsWithSlim(this string text, string value, bool isIgnoreCase)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (value == null)
                throw new ArgumentNullException("value");
            var valueLength = value.Length;
            return (text.Length < valueLength ? false : string.Compare(text.Substring(0, valueLength), value, isIgnoreCase) == 0);
        }

        /// <summary>
        /// Truncates the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string Truncate(this string text, int maxLength) { return Truncate(text, maxLength, TextTruncateType.Normal); }
        /// <summary>
        /// Truncates the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="textTruncateType">Type of the text truncate.</param>
        /// <returns></returns>
        public static string Truncate(this string text, int maxLength, TextTruncateType textTruncateType) { return Truncate(text, maxLength, textTruncateType, "..."); }
        /// <summary>
        /// Truncates the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="textTruncateType">Type of the text truncate.</param>
        /// <param name="trailerText">The trailer text.</param>
        /// <returns></returns>
        public static string Truncate(this string text, int maxLength, TextTruncateType textTruncateType, string trailerText)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty; //? throw new ArgumentNullException("text");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength");
            if (string.IsNullOrEmpty(trailerText))
                throw new ArgumentNullException("trailerText");
            var trailerTextLength = trailerText.Length;
            switch (textTruncateType)
            {
                case TextTruncateType.Normal:
                    return (text.Length > maxLength ? (maxLength > trailerTextLength ? text.Substring(0, maxLength - trailerTextLength) + trailerText : text.Substring(0, maxLength)) : text);
                case TextTruncateType.LastWhitespace:
                    if (text.Length > maxLength)
                    {
                        if (maxLength > trailerTextLength)
                        {
                            var truncatedText = text.Substring(0, maxLength - trailerTextLength);
                            var whiteIndex = truncatedText.LastIndexOf(' ');
                            string truncatedTextUntilWhite;
                            return ((whiteIndex > 0) && ((truncatedTextUntilWhite = truncatedText.Substring(0, whiteIndex).Trim()).Length > 0) ? truncatedTextUntilWhite : truncatedText) + trailerText;
                        }
                        return text.Substring(0, maxLength);
                    }
                    return text;
                case TextTruncateType.FromAtomSite:
                    // Add room for a word not breaking and the trailer
                    var b = new StringBuilder(maxLength + 20);
                    var words = text.Split(new char[] { ' ' });
                    var index = 0;
                    while (b.Length + words[index].Length + trailerTextLength < maxLength - trailerTextLength && index < words.GetUpperBound(0))
                    {
                        b.Append(words[index]);
                        b.Append(" ");
                        index++;
                    }
                    // We exited the loop before reaching the end of the array - which would normally be the case.
                    if (index < words.GetUpperBound(0))
                    {
                        // Remove the ending space before attaching the trailer.
                        b.Remove(b.Length - 1, 1);
                        b.Append(trailerText);
                    }
                    else
                        b.Append(words[index]);
                    return b.ToString();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
