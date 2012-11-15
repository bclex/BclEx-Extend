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
using System.Linq.Expressions;
namespace System.Linq
{
    public static partial class QueryableEx
    {
        private static IEnumerable<TResult> CreateHierarchyRecurse<TSource, TResult>(IQueryable<TSource> source, TSource parentItem, string propertyNameKey, string propertyNameParentKey, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey, int depth, int maxDepth)
            where TSource : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "e");
            Expression<Func<TSource, bool>> predicate;
            if (rootKey != null)
            {
                Expression left = Expression.Convert(Expression.Property(parameter, propertyNameKey), rootKey.GetType());
                Expression right = Expression.Constant(rootKey);
                predicate = Expression.Lambda<Func<TSource, bool>>(Expression.Equal(left, right), parameter);
            }
            else
            {
                if (parentItem == null)
                    predicate = Expression.Lambda<Func<TSource, bool>>(Expression.Equal(Expression.Property(parameter, propertyNameParentKey), Expression.Constant(null)), parameter);
                else
                {
                    Expression left = Expression.Convert(Expression.Property(parameter, propertyNameParentKey), parentItem.GetType().GetProperty(propertyNameKey).PropertyType);
                    Expression right = Expression.Constant(parentItem.GetType().GetProperty(propertyNameKey).GetValue(parentItem, null));
                    predicate = Expression.Lambda<Func<TSource, bool>>(Expression.Equal(left, right), parameter);
                }
            }
            IEnumerable<TSource> childs = source.Where(predicate).ToList();
            if (childs.Count() > 0)
            {
                depth++;
                if ((depth <= maxDepth) || (maxDepth == 0))
                    foreach (var item in childs)
                        yield return selector(new HierarchyNode<TSource, TResult>
                        {
                            Entity = item,
                            Children = CreateHierarchyRecurse(source, item, propertyNameKey, propertyNameParentKey, selector, null, depth, maxDepth),
                            Depth = depth,
                            Parent = parentItem
                        });
            }
        }

        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>>(source, null, propertyNameKey, propertyNameParentKey, x => x, null, 0, 0); }
        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey, object rootKey)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>>(source, null, propertyNameKey, propertyNameParentKey, x => x, rootKey, 0, 0); }
        //public static IEnumerable<HierarchyNode<TSource>> AsHierarchy<TSource>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey, object rootKey, int maxDepth)
        //    where TSource : class { return CreateHierarchyRecurse<TSource, HierarchyNode<TSource>>(source, null, propertyNameKey, propertyNameParentKey, x => x, rootKey, 0, maxDepth); }

        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyNameKey">The property name key.</param>
        /// <param name="propertyNameParentKey">The property name parent key.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey, Func<HierarchyNode<TSource, TResult>, TResult> selector)
            where TSource : class { return CreateHierarchyRecurse(source, null, propertyNameKey, propertyNameParentKey, selector, null, 0, 0); }
        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyNameKey">The property name key.</param>
        /// <param name="propertyNameParentKey">The property name parent key.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="rootKey">The root key.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey)
            where TSource : class { return CreateHierarchyRecurse(source, null, propertyNameKey, propertyNameParentKey, selector, rootKey, 0, 0); }
        /// <summary>
        /// Selects the hierarchy.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyNameKey">The property name key.</param>
        /// <param name="propertyNameParentKey">The property name parent key.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="rootKey">The root key.</param>
        /// <param name="maxDepth">The max depth.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectHierarchy<TSource, TResult>(this IQueryable<TSource> source, string propertyNameKey, string propertyNameParentKey, Func<HierarchyNode<TSource, TResult>, TResult> selector, object rootKey, int maxDepth)
            where TSource : class { return CreateHierarchyRecurse(source, null, propertyNameKey, propertyNameParentKey, selector, rootKey, 0, maxDepth); }

    }
}
