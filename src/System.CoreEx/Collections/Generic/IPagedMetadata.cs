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
namespace System.Collections.Generic
{
    /// <summary>
    /// IPagedMetadata
    /// </summary>
    public interface IPagedMetadata
    {
        /// <summary>
        /// Gets the pages.
        /// </summary>
        int Pages { get; }
        /// <summary>
        /// Gets the total items.
        /// </summary>
        int TotalItems { get; }
        /// <summary>
        /// Gets the items.
        /// </summary>
        int Items { get; }
        /// <summary>
        /// Gets the index.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Gets a value indicating whether this instance has previous page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        bool HasPreviousPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance has next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        bool HasNextPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        bool IsFirstPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        bool IsLastPage { get; }
    }
}