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
    public static partial class EnumerableEx
    {
        private static IEnumerable<TResult> CreateHierarchyRecurse<TSource, TResult, TKey>(IEnumerable<TSource> source, TSource parentItem, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey, int depth, int maxDepth)
            where TSource : class
        {
            IEnumerable<TSource> childs;
            if (rootKey != null)
                childs = source.Where(x => keySelector(x).Equals(rootKey));
            else
                if (parentItem == null)
                    childs = source.Where(x => parentKeySelector(x).Equals(default(TKey)));
                else
                    childs = source.Where(x => parentKeySelector(x).Equals(keySelector(parentItem)));
            if (childs.Count() > 0)
            {
                depth++;
                if (depth <= maxDepth || maxDepth == 0)
                    foreach (var item in childs)
                        yield return selector(new HierarchyNode<TSource, TResult>
                        {
                            Entity = item,
                            Children = CreateHierarchyRecurse(source, item, keySelector, parentKeySelector, selector, null, depth, maxDepth),
                            Depth = depth,
                            Parent = parentItem
                        });
            }
        }

        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>, TKey>(source, default(TSource), keySelector, parentKeySelector, x => x, null, 0, 0); }
        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, object rootKey)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>, TKey>(source, default(TSource), keySelector, parentKeySelector, x => x, rootKey, 0, 0); }
        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, object rootKey, int maxDepth)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>, TKey>(source, default(TSource), keySelector, parentKeySelector, x => x, rootKey, 0, maxDepth); }

        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="parentKeySelector">The parent key selector.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, Func<HierarchyNode<TSource, TResult>, TResult> selector)
            where TSource : class { return CreateHierarchyRecurse(source, default(TSource), keySelector, parentKeySelector, selector, null, 0, 0); }
        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="parentKeySelector">The parent key selector.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="rootKey">The root key.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey)
            where TSource : class { return CreateHierarchyRecurse(source, default(TSource), keySelector, parentKeySelector, selector, rootKey, 0, 0); }
        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="parentKeySelector">The parent key selector.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="rootKey">The root key.</param>
        /// <param name="maxDepth">The max depth.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> parentKeySelector, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey, int maxDepth)
            where TSource : class { return CreateHierarchyRecurse(source, default(TSource), keySelector, parentKeySelector, selector, rootKey, 0, maxDepth); }
    }
}
