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
    /// IPagedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IList<T>, IPagedMetadata { }

    /// <summary>
    /// PagedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        private readonly IPagedMetadata _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public PagedList(IPagedMetadata metadata)
        {
            _metadata = metadata;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="metadata">The metadata.</param>
        public PagedList(IEnumerable<T> collection, IPagedMetadata metadata)
            : base(collection)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        public int Pages
        {
            get { return _metadata.Pages; }
        }

        /// <summary>
        /// Gets the total items.
        /// </summary>
        public int TotalItems
        {
            get { return _metadata.TotalItems; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public int Items
        {
            get { return _metadata.Items; }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index
        {
            get { return _metadata.Index; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has previous page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        public bool HasPreviousPage
        {
            get { return _metadata.HasPreviousPage; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextPage
        {
            get { return _metadata.HasNextPage; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstPage
        {
            get { return _metadata.IsFirstPage; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastPage
        {
            get { return _metadata.IsLastPage; }
        }
    }
}