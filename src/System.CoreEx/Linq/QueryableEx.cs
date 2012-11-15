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
using System.Collections.Generic;
namespace System.Linq
{
    /// <summary>
    /// QueryableExtensions
    /// </summary>
    public static partial class QueryableEx
    {
        /// <summary>
        /// Wheres the in.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source1">The source1.</param>
        /// <param name="source2">The source2.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIn<TSource, TKey>(this IQueryable<TSource> source1, IEnumerable<TKey> source2, Expression<Func<TSource, TKey>> keySelector)
        {
            if (source1 == null)
                throw new ArgumentNullException("source1");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            if (source2 == null)
                throw new ArgumentNullException("source2");
            Expression where = null;
            foreach (TKey value in source2)
            {
                var equal = Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey)));
                where = (where != null ? Expression.OrElse(where, equal) : equal);
            }
            return source1.Where<TSource>(Expression.Lambda<Func<TSource, bool>>(where, keySelector.Parameters));
        }
    }
}
