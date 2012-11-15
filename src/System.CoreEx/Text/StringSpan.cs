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
	/// StringSpan
    /// </summary>
    public class StringSpan
    {
        private int _startIndex;
        private int _endIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSpan"/> class.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public StringSpan(int startIndex, int endIndex)
        {
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        /// <summary>
        /// Gets or sets the end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get { return _endIndex; }
            set { _endIndex = value; }
        }

        /// <summary>
        /// Determines whether the specified text span is contain.
        /// </summary>
        /// <param name="span">The text span.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text span is contain; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContain(StringSpan span) { return IsContain(span.StartIndex, span.EndIndex); }
        /// <summary>
        /// Determines whether the specified start index is contain.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>
        /// 	<c>true</c> if the specified start index is contain; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContain(int startIndex, int endIndex)
        {
            return ((startIndex >= _startIndex) && (endIndex <= _endIndex));
        }

        /// <summary>
        /// Determines whether the specified text span is overlap.
        /// </summary>
        /// <param name="span">The text span.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text span is overlap; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOverlap(StringSpan span) { return IsOverlap(span.StartIndex, span.EndIndex); }
        /// <summary>
        /// Determines whether the specified start index is overlap.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>
        /// 	<c>true</c> if the specified start index is overlap; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOverlap(int startIndex, int endIndex)
        {
            return ((startIndex <= _endIndex) && (endIndex >= _startIndex));
        }

        /// <summary>
        /// Merges the specified text span.
        /// </summary>
        /// <param name="span">The text span.</param>
        /// <returns></returns>
        public bool Merge(StringSpan span) { return Merge(span.StartIndex, span.EndIndex); }
        /// <summary>
        /// Merges the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        public bool Merge(int startIndex, int endIndex)
        {
            bool isMerged = false;
            if ((startIndex < _startIndex) && ((endIndex + 1) >= _startIndex))
            {
                isMerged = true;
                _startIndex = startIndex;
            }
            if ((endIndex > _endIndex) && ((startIndex - 1) <= _endIndex))
            {
                isMerged = true;
                _endIndex = endIndex;
            }
            return isMerged;
        }

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return (_endIndex - _startIndex + 1); }
		}
	}
}
        
