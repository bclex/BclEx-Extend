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
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
namespace System.Linq
{
    /// <summary>
    /// QueryableExtensions
    /// </summary>
    public static partial class QueryableExtensions
    {
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IQueryable<TSource> source, int pageIndex, out IPagedMetadata meta) { return ToPagedArray<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex }, out meta); }
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize, out IPagedMetadata meta) { return ToPagedArray<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = pageSize }, out meta); }
        /// <summary>
        /// Toes the paged array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        public static TSource[] ToPagedArray<TSource>(this IQueryable<TSource> source, LinqPagedCriteria criteria, out IPagedMetadata meta)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            meta = LinqPagedMetadataProviders.Current.GetMetadata<TSource>(source, criteria);
            int pageSize = criteria.PageSize;
            int index = meta.Index;
            if (meta.TotalItems > 0)
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
        public static IPagedList<TSource> ToPagedList<TSource>(this IQueryable<TSource> source, int pageIndex) { return ToPagedList<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex }); }
        /// <summary>
        /// Toes the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public static IPagedList<TSource> ToPagedList<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize) { return ToPagedList<TSource>(source, new LinqPagedCriteria { PageIndex = pageIndex, PageSize = pageSize }); }
        /// <summary>
        /// Toes the paged list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public static IPagedList<TSource> ToPagedList<TSource>(this IQueryable<TSource> source, LinqPagedCriteria criteria)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            var meta = LinqPagedMetadataProviders.Current.GetMetadata<TSource>(source, criteria);
            int pageSize = criteria.PageSize;
            int index = meta.Index;
            if (meta.TotalItems > 0)
                return new PagedList<TSource>((index == 0 ? source.Take(pageSize) : source.Skip(index * pageSize).Take(pageSize)), meta);
            return new PagedList<TSource>(meta);
        }
    }
}
