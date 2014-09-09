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
    /// EnumerableExtensions
    /// </summary>
    public static partial class EnumerableEx
    {
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="metadata">The paged meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IEnumerable<TSource> source, int pageIndex, out IPagedMetadata metadata) { return ToPagedArray<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = 20 }, out metadata); }
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="metadata">The paged meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IEnumerable<TSource> source, int pageIndex, int pageSize, out IPagedMetadata metadata) { return ToPagedArray<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = pageSize }, out metadata); }
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="metadata">The paged meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IEnumerable<TSource> source, LinqPagedCriteria criteria, out IPagedMetadata metadata)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            //return QueryableExtensions.ToPagedArray(source.AsQueryable(), criteria, out metadata);
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            metadata = LinqPagedMetadataProviders.Current.GetMetadata<TSource>(source, criteria);
            var pageSize = criteria.PageSize;
            var index = metadata.Index;
            if (metadata.TotalItems > 0)
                return new Buffer<TSource>(index == 0 ? source.Take(pageSize) : source.Skip(index * pageSize).Take(pageSize)).ToArray();
            return new TSource[] { };
        }

        /// <summary>
        /// Toes the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        public static IPagedList<TSource> ToPagedList<TSource>(this IEnumerable<TSource> source, int pageIndex) { return ToPagedList<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = 20 }); }
        /// <summary>
        /// Toes the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public static IPagedList<TSource> ToPagedList<TSource>(this IEnumerable<TSource> source, int pageIndex, int pageSize) { return ToPagedList<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = pageSize }); }
        /// <summary>
        /// Toes the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public static IPagedList<TSource> ToPagedList<TSource>(this IEnumerable<TSource> source, LinqPagedCriteria criteria)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            var metadata = LinqPagedMetadataProviders.Current.GetMetadata<TSource>(source, criteria);
            var pageSize = criteria.PageSize;
            var index = metadata.Index;
            if (metadata.TotalItems > 0)
                return new PagedList<TSource>((index == 0 ? source.Take(pageSize) : source.Skip(index * pageSize).Take(pageSize)), metadata);
            return new PagedList<TSource>(metadata);
        }
        /// <summary>
        /// To the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TPagedSource">The type of the paged source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">metadata;Not from PagedList</exception>
        public static IPagedList<TSource> ToPagedList<TSource, TPagedSource>(this IEnumerable<TSource> source, IEnumerable<TPagedSource> metadata)
        {
            var pagedList = (metadata as PagedList<TPagedSource>);
            if (pagedList == null)
                throw new ArgumentNullException("metadata", "Not from PagedList<TPagedSource>");
            return new PagedList<TSource>(source, pagedList._metadata);
        }
    }
}
