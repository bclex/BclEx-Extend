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
using System.Collections.Generic;
namespace System.Text
{
    /// <summary>
    /// StringSearcher
    /// </summary>
    public class StringSearcher
    {
        private List<StringSpan> _excludedSpans;
        private string _s;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSearcher"/> class.
        /// </summary>
        /// <param name="s">The text.</param>
        /// <param name="openExcludeToken">The open exclude token.</param>
        /// <param name="closeExcludeToken">The close exclude token.</param>
        public StringSearcher(string s, string openExcludeToken, string closeExcludeToken)
            : this(s, false, new string[] { openExcludeToken, closeExcludeToken }) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSearcher"/> class.
        /// </summary>
        /// <param name="s">The text.</param>
        /// <param name="isNested">if set to <c>true</c> [is nested].</param>
        /// <param name="excludedTokens">The exclude tokens.</param>
        public StringSearcher(string s, bool isNested, string[] excludedTokens)
        {
            _s = s;
            _excludedSpans = (!isNested ? GetFlatSpans(0, excludedTokens) : GetNestedSpans(0, excludedTokens));
        }

        /// <summary>
        /// Adds the exclude text span.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        public void AddExcludeTextSpan(int startIndex, int length)
        {
            if ((startIndex < 0) || (startIndex >= _s.Length))
                throw new ArgumentOutOfRangeException("startIndex", startIndex, string.Format(Local.IndexOutOfRangeAB, 0, _s.Length));
            if ((length <= 0) || ((startIndex + length) > _s.Length))
                throw new ArgumentOutOfRangeException("length", length, string.Format(Local.IndexOutOfRangeAB, 1, _s.Length - startIndex));
            _excludedSpans.Add(new StringSpan(startIndex, startIndex + length - 1));
        }

        /// <summary>
        /// Creates the flat text span list.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        private List<StringSpan> GetFlatSpans(int startIndex, string[] tokens)
        {
            if ((tokens == null) || (tokens.Length == 0))
                throw new ArgumentNullException("tokens");
            if (tokens.Length != 2)
                throw new ArgumentException("tokens", Local.InvalidArrayLength);
            string openToken = tokens[0];
            int openTokenLength = openToken.Length;
            string closeToken = tokens[1];
            int closeTokenLength = closeToken.Length;
            //
            var stringSpans = new List<StringSpan>();
            StringSpan span = null;
            int nextStartIndex = startIndex;
            int nextEndIndex = -1;
            do
            {
                // locate openning token
                int startSpanIndex = _s.IndexOf(openToken, nextStartIndex, StringComparison.OrdinalIgnoreCase);
                if (startSpanIndex == -1)
                    break;
                // starting position to search for closing token
                if ((nextEndIndex == -1) || (nextEndIndex < startSpanIndex))
                    nextEndIndex = startSpanIndex;
                // locate closing token
                int endSpanIndex = _s.IndexOf(closeToken, nextEndIndex, StringComparison.OrdinalIgnoreCase);
                if (endSpanIndex == -1)
                    break;
                if (span == null)
                    // first span
                    span = new StringSpan(startSpanIndex, endSpanIndex + closeTokenLength - 1);
                else if (span.Merge(startSpanIndex, endSpanIndex + closeTokenLength - 1))
                {
                    // overlapping with current span - do nothing
                }
                else
                {
                    // not-overlapping with current span
                    stringSpans.MergeAdd(span);
                    span = new StringSpan(startSpanIndex, endSpanIndex + closeTokenLength - 1);
                }
                nextStartIndex = startSpanIndex + openTokenLength;
                nextEndIndex = endSpanIndex + closeTokenLength;
            } while (true);
            // append last span, if one exists
            if (span != null)
                stringSpans.MergeAdd(span);
            return stringSpans;
        }

        /// <summary>
        /// Creates the nested text span list.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        private List<StringSpan> GetNestedSpans(int startIndex, string[] tokens)
        {
            if ((tokens == null) || (tokens.Length == 0))
                throw new ArgumentNullException("tokens");
            if ((tokens.Length % 2) != 0)
                throw new ArgumentException("tokens", Local.InvalidArrayLength);
            var stringSpans = new List<StringSpan>();
            // for each span token pair
            for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex += 2)
            {
                string openToken = tokens[tokenIndex];
                int openTokenLength = openToken.Length;
                string closeToken = tokens[tokenIndex + 1];
                int closeTokenLength = closeToken.Length;
                //
                StringSpan span = null;
                int nextStartIndex = startIndex;
                int nextEndIndex = -1;
                do
                {
                    // locate openning token
                    int startSpanIndex = _s.IndexOf(openToken, nextStartIndex, StringComparison.OrdinalIgnoreCase);
                    if (startSpanIndex == -1)
                        break;
                    // starting position to search for closing token
                    // starting position to search for closing token
                    if ((nextEndIndex == -1) || (nextEndIndex < startSpanIndex))
                        nextEndIndex = startSpanIndex;
                    // locate closing token
                    int endSpanIndex = _s.IndexOf(closeToken, nextEndIndex, StringComparison.OrdinalIgnoreCase);
                    if (endSpanIndex == -1)
                        break;
                    if (span == null)
                        // first span
                        span = new StringSpan(startSpanIndex, endSpanIndex + closeTokenLength - 1);
                    else if (span.Merge(startSpanIndex, endSpanIndex + closeTokenLength - 1))
                    {
                        // overlapping with current span - do nothing
                    }
                    else
                    {
                        // not-overlapping with current span
                        stringSpans.MergeAdd(span);
                        span = new StringSpan(startSpanIndex, endSpanIndex + closeTokenLength - 1);
                    }
                    nextStartIndex = startSpanIndex + openTokenLength;
                    nextEndIndex = endSpanIndex + closeTokenLength;
                } while (true);
                // append last span, if one exists
                if (span != null)
                    stringSpans.MergeAdd(span);
            }
            return stringSpans;
        }

        /// <summary>
        /// Finds the text span.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="openToken">The open token.</param>
        /// <param name="closeToken">The close token.</param>
        /// <param name="startSpanIndex">Start index of the text span.</param>
        /// <param name="endSpanIndex">End index of the text span.</param>
        /// <returns></returns>
        public bool FindStringSpan(int startIndex, string openToken, string closeToken, out int startSpanIndex, out int endSpanIndex)
        {
            startSpanIndex = -1;
            endSpanIndex = -1;
            int openTokenIndex;
            int openTokenLength = openToken.Length;
            int closeTokenIndex;
            int closeTokenLength = closeToken.Length;
            // find matching open/close tokens
            if ((startIndex < _s.Length) && ((openTokenIndex = IndexOf(openToken, startIndex)) > -1) && ((closeTokenIndex = IndexOf(closeToken, openTokenIndex)) > -1))
            {
                // handle multiple consecutive closing tokens are
                int moveBy = 0;
                while ((closeTokenIndex < _s.Length - closeTokenLength) && (_s.Substring(closeTokenIndex + 1, closeTokenLength) == closeToken))
                {
                    moveBy++;
                    closeTokenIndex++;
                }
                if (moveBy > 0)
                    closeTokenIndex -= (moveBy - (moveBy % closeTokenLength));
                // find matching open token to last
                openTokenIndex = LastIndexOf(openToken, closeTokenIndex, closeTokenIndex - openTokenIndex + 1);
                // compute start/end index
                startSpanIndex = openTokenIndex;
                endSpanIndex = closeTokenIndex + closeTokenLength - 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public int IndexOf(string value, int startIndex) { return IndexOf(value, startIndex, value.Length - startIndex + 1); }
        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int IndexOf(string value, int startIndex, int count)
        {
            int endScanIndex = (count > -1 ? startIndex + count : _s.Length);
            int index = startIndex;
            do
            {
                index = _s.IndexOf(value, index, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                    return -1;
                // determine whether matched token is in an exclude zone
				var boundingStringSpan = _excludedSpans.GetBoundingStringSpan(index);
                if (boundingStringSpan == null)
                    return index;
                index = boundingStringSpan.EndIndex;
                // increment by one character just in case searched value is subset
                index++;
            } while (index < endScanIndex);
            return -1;
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public int LastIndexOf(string value, int startIndex) { return LastIndexOf(value, startIndex, startIndex + 1); }
        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int LastIndexOf(string value, int startIndex, int count)
        {
            int valueLength = value.Length;
            int nextStartIndex = startIndex;
            int index = -1;
            do
            {
                index = _s.LastIndexOf(value, nextStartIndex, count, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                    return -1;
                // determine whether matched token is in an exclude zone
                var boundingStringSpan = _excludedSpans.GetBoundingStringSpan(index);
                if (boundingStringSpan == null)
                    return index;
                index = boundingStringSpan.StartIndex;
                count -= (nextStartIndex - index + 1);
                // decrement by one character just in case searched value is subset
                nextStartIndex = index - 1;
            } while (index > -1);
            return -1;
        }

        //#region DEBUG
        //public void Dump()
        //{
        //    string text = _text.Replace("\x0d", "?").Replace("\x0a", "?");
        //    var mask = new StringBuilder(new string(' ', text.Length), text.Length);
        //    foreach (TextSpan textSpan in _excludeTextSpanList)
        //        for (int textIndex = textSpan.StartIndex; textIndex <= textSpan.EndIndex; textIndex++)
        //            mask[textIndex] = '*';
        //    Console.WriteLine(text);
        //    Console.WriteLine(mask);
        //    Console.WriteLine("===");
        //}
        //#endregion
    }
}