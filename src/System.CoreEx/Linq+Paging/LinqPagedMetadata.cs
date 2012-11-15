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
namespace System.Linq
{
    /// <summary>
    /// LinqPagedMetadata
    /// </summary>
    public class LinqPagedMetadata<TSource> : IPagedMetadata
    {
        /// <summary>
        /// Gets the total items.
        /// </summary>
        public int TotalItems { get; private set; }
        /// <summary>
        /// Gets the pages.
        /// </summary>
        public int Pages { get; private set; }
        /// <summary>
        /// Gets the items.
        /// </summary>
        public int Items { get; private set; }
        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// Gets the criteria.
        /// </summary>
        public LinqPagedCriteria Criteria { get; private set; }
        //
        /// <summary>
        /// Gets a value indicating whether this instance has overflowed show all.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has overflowed show all; otherwise, <c>false</c>.
        /// </value>
        public bool HasOverflowedShowAll { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqPagedMetadata&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="criteria">The criteria.</param>
        public LinqPagedMetadata(IEnumerable<TSource> items, LinqPagedCriteria criteria)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            Criteria = criteria;
            Items = items.Count();
            var totalItemsAccessor = criteria.TotalItemsAccessor;
            TotalItems = (totalItemsAccessor == null ? Items : totalItemsAccessor());
            Index = criteria.PageIndex;
            Pages = (TotalItems > 0 ? (int)Math.Ceiling(TotalItems / (decimal)Criteria.PageSize) : 1);
            HasOverflowedShowAll = ((Criteria.ShowAll) && (Items < TotalItems));
            EnsureVisiblity();
        }

        //private static int ThrowAwayPages(int totalItems, int pageSize)
        //{
        //    int pages = 1;
        //    if ((pageSize > 0) && (totalItems > pageSize))
        //    {
        //        pages = totalItems / pageSize;
        //        if (totalItems % pageSize > 0)
        //            pages++;
        //    }
        //    return pages;
        //}

        /// <summary>
        /// Ensures the visiblity.
        /// </summary>
        /// <returns></returns>
        public bool EnsureVisiblity()
        {
            if (Index > Pages)
            {
                Index = Pages;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has previous page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        public bool HasPreviousPage
        {
            get { return (Criteria.PageIndex > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextPage
        {
            get { return Index < (Pages - 1); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstPage
        {
            get { return Index <= 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastPage
        {
            get { return Index >= (Pages - 1); }
        }
    }
}
