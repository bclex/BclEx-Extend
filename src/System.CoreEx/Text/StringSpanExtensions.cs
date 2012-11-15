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
	/// StringSpanExtensions
	/// </summary>
	public static class StringSpanExtensions
	{
        /// <summary>
        /// Merges the add.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="span">The text span.</param>
		public static void MergeAdd(this List<StringSpan> list, StringSpan span)
		{
			// flatten
			for (int textSpanIndex = 0; textSpanIndex < list.Count; textSpanIndex++)
			{
				var textSpan2 = list[textSpanIndex];
				// if new text span node is to the left of the current node
				if ((span.EndIndex - 1) < textSpan2.StartIndex)
				{
					// insert new node at current index
					list.Insert(textSpanIndex, span);
					return;
				}
				// if new text span node overlaps with current node, merge the text span
				if (textSpan2.Merge(span))
				{
					// merge subsequent nodes if they overlap
					textSpanIndex++;
					while ((textSpanIndex < list.Count) && (textSpan2.Merge(list[textSpanIndex])))
						// remove merged node
						list.RemoveAt(textSpanIndex);
					return;
				}
			}
			list.Add(span);
		}

        /// <summary>
        /// Determines whether the specified index contains index.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the specified index contains index; otherwise, <c>false</c>.
        /// </returns>
		public static bool ContainsIndex(this List<StringSpan> list, int index) { return (GetBoundingStringSpan(list, index) != null); }

        /// <summary>
        /// Gets the bounding text span.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public static StringSpan GetBoundingStringSpan(this List<StringSpan> list, int index)
		{
			foreach (var textSpan in list)
				if (index < textSpan.StartIndex)
					return null;
				else if (index <= textSpan.EndIndex)
					return textSpan;
			return null;
		}
	}
}

