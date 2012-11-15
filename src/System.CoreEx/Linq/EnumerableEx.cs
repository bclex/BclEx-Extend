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
using System.Collections;
namespace System.Linq
{
    /// <summary>
    /// EnumerableExtensions
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static partial class EnumerableEx
    {
        /// <summary>
        /// Determines whether [is null or empty array] [the specified values].
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty array] [the specified values]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmptyArray(Array values)
        {
            return (values == null || values.Length == 0);
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified values].
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified values]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(IEnumerable values)
        {
            return (values == null || !values.GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified values].
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified values]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<TSource>(ICollection<TSource> values)
        {
            return (values == null || values.Count == 0);
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified values].
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified values]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<TSource>(IEnumerable<TSource> values)
        {
            if (values != null)
                using (var enumerator = values.GetEnumerator())
                    return !enumerator.MoveNext();
            return true;
        }

        /// <summary>
        /// Nulls if.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static ICollection<TSource> NullIf<TSource>(this ICollection<TSource> values) { return values.NullIf<TSource>((Predicate<ICollection<TSource>>)IsNullOrEmpty<TSource>); }
        /// <summary>
        /// Nulls if.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static ICollection<TSource> NullIf<TSource>(this ICollection<TSource> values, Predicate<ICollection<TSource>> condition)
        {
            return (values != null && !condition(values) ? values : null);
        }

        /// <summary>
        /// Nulls if.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> NullIf<TSource>(this IEnumerable<TSource> values) { return values.NullIf<TSource>((Predicate<IEnumerable<TSource>>)IsNullOrEmpty<TSource>); }
        /// <summary>
        /// Nulls if.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="values">The values.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> NullIf<TSource>(this IEnumerable<TSource> values, Predicate<IEnumerable<TSource>> condition)
        {
            return (values != null && !condition(values) ? values : null);
        }

        /// <summary>
        /// Combines the selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TMiddle">The type of the middle.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="selector1">The selector1.</param>
        /// <param name="selector2">The selector2.</param>
        /// <returns></returns>
        public static Func<TSource, TResult> CombineSelectors<TSource, TMiddle, TResult>(Func<TSource, TMiddle> selector1, Func<TMiddle, TResult> selector2) { return (x => selector2(selector1(x))); }
        /// <summary>
        /// Combines the predicates.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="predicate1">The predicate1.</param>
        /// <param name="predicate2">The predicate2.</param>
        /// <returns></returns>
        public static Func<TSource, bool> CombinePredicates<TSource>(Func<TSource, bool> predicate1, Func<TSource, bool> predicate2) { return (x => (predicate1(x) ? predicate2(x) : false)); }

        //private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        //{
        //    return new WrappedSelectIterator<TSource, TResult>(-2)
        //    {
        //        OriginalSource = source,
        //        OriginalSelector = selector,
        //    };
        //}

        /// <summary>
        /// Singles the or.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="missingValue">The missing value.</param>
        /// <returns></returns>
        public static TSource SingleOr<TSource>(this IEnumerable<TSource> source, Func<TSource> missingValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (missingValue == null)
                throw new ArgumentNullException("missingValue");
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    return missingValue();
                var first = iterator.Current;
                if (iterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains more than one element");
                return first;
            }
        }

        /// <summary>
        /// Ases the enumerable yield.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> AsEnumerableYield<TSource>(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var item in source)
                yield return (TSource)item;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (TSource item in source)
                action(item);
            return source;
        }

        /// <summary>
        /// Fors the yield.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="initialize">The initialize.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="next">The next.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> ForYield<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> initialize, Predicate<TSource> predicate, Func<TSource, TSource> next)
        {
            foreach (TSource item in source)
            {
                var value = initialize(item);
                while (predicate(value))
                {
                    yield return value;
                    value = next(value);
                }
            }
        }

        /// <summary>
        /// Coalesces the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static TSource Coalesce<TSource>(this IEnumerable<TSource> source, TSource nullValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var value in source)
                if ((value != null) && (!value.Equals(nullValue)))
                    return value;
            return nullValue;
        }

        /// <summary>
        /// Coalesces the func.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static TSource CoalesceFunc<TSource>(this IEnumerable<Func<TSource>> source, TSource nullValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var func in source)
            {
                var value = func();
                if ((value != null) && (!value.Equals(nullValue)))
                    return value;
            }
            return nullValue;
        }

        /// <summary>
        /// Coalesces the func.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static TSource CoalesceFunc<TSource, TTag>(this IEnumerable<Func<TTag, TSource>> source, TTag tag, TSource nullValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var func in source)
            {
                var value = func(tag);
                if ((value != null) && (!value.Equals(nullValue)))
                    return value;
            }
            return nullValue;
        }

        /// <summary>
        /// Maxes the skip null.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static int MaxSkipNull(this IEnumerable<int> source, int nullValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            int minValue = nullValue;
            foreach (int value in source)
                if ((value != nullValue) && ((minValue == nullValue) || (value > minValue)))
                    minValue = value;
            return minValue;
        }

        /// <summary>
        /// Maxes the skip null.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static TSource MaxSkipNull<TSource>(this IEnumerable<TSource> source, TSource nullValue)
            where TSource : IComparable<TSource>
        {
            if (source == null)
                throw new ArgumentNullException("source");
            TSource minValue = nullValue;
            foreach (TSource value in source)
                if ((value != null) && (!value.Equals(nullValue)) && ((minValue.Equals(nullValue)) || (value.CompareTo(minValue) > 0)))
                    minValue = value;
            return minValue;
        }

        /// <summary>
        /// Mins the skip null.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static int MinSkipNull(this IEnumerable<int> source, int nullValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            int minValue = nullValue;
            foreach (int value in source)
                if ((value != nullValue) && ((minValue == nullValue) || (value < minValue)))
                    minValue = value;
            return minValue;
        }

        /// <summary>
        /// Mins the skip null.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns></returns>
        public static TSource MinSkipNull<TSource>(this IEnumerable<TSource> source, TSource nullValue)
            where TSource : IComparable<TSource>
        {
            if (source == null)
                throw new ArgumentNullException("source");
            TSource minValue = nullValue;
            foreach (TSource value in source)
                if ((value != null) && (!value.Equals(nullValue)) && ((minValue.Equals(nullValue)) || (value.CompareTo(minValue) < 0)))
                    minValue = value;
            return minValue;
        }

        /// <summary>
        /// Finds the while skip null.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="nullValue">The null value.</param>
        /// <param name="finder">The finder.</param>
        /// <returns></returns>
        public static TSource FindWhileSkipNull<TSource>(this IEnumerable<TSource> source, TSource nullValue, Func<TSource, TSource, bool> finder)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            TSource seedValue = nullValue;
            foreach (TSource value in source)
                if ((!value.Equals(nullValue)) && ((seedValue.Equals(nullValue)) || (finder(seedValue, value))))
                    seedValue = value;
            return seedValue;
        }
    }
}
