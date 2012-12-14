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
        /// <summary>
        /// Matches the specified left.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
        /// <returns></returns>
        public static bool Match<TSource>(this IEnumerable<TSource> left, IEnumerable<TSource> right, bool exactMatch)
        {
            if (left == null || right == null)
                return (left == null && right == null);
            IEnumerator<TSource> leftEnum;
            IEnumerator<TSource> rightEnum;
            for (leftEnum = left.GetEnumerator(), rightEnum = right.GetEnumerator(); leftEnum.MoveNext() && rightEnum.MoveNext(); )
            {
                var leftValue = leftEnum.Current;
                var rightValue = rightEnum.Current;
                if ((leftValue == null && rightValue != null) || (leftValue != null && rightValue == null) || !leftValue.Equals(rightValue))
                    return false;
            }
            return (!exactMatch || leftEnum.MoveNext() == rightEnum.MoveNext());
        }
        /// <summary>
        /// Matches the specified left.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
        /// <returns></returns>
        public static bool Match<TSource>(this IEnumerable<TSource> left, IEnumerable<TSource> right, Func<TSource, TSource, bool> predicate, bool exactMatch)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (left == null || right == null)
                return (left == null && right == null);
            IEnumerator<TSource> leftEnum;
            IEnumerator<TSource> rightEnum;
            for (leftEnum = left.GetEnumerator(), rightEnum = right.GetEnumerator(); leftEnum.MoveNext() && rightEnum.MoveNext(); )
            {
                var leftValue = leftEnum.Current;
                var rightValue = rightEnum.Current;
                if ((leftValue == null && rightValue != null) || (leftValue != null && rightValue == null) || !predicate(leftValue, rightValue))
                    return false;
            }
            return (!exactMatch || leftEnum.MoveNext() == rightEnum.MoveNext());
        }

        /// <summary>
        /// Matches the specified left.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
        /// <returns></returns>
        public static bool Match<TSource>(this TSource[] left, TSource[] right, bool exactMatch)
        {
            if (left == null || right == null)
                return (left == null && right == null);
            if (exactMatch && left.Length != right.Length)
                return false;
            for (int index = 0; index < left.Length; index++)
            {
                var leftValue = left[index];
                var rightValue = right[index];
                if ((leftValue == null && rightValue != null) || (leftValue != null && rightValue == null) || !leftValue.Equals(rightValue))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Matches the specified left.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
        /// <returns></returns>
        public static bool Match<TSource>(this TSource[] left, TSource[] right, Func<TSource, TSource, bool> predicate, bool exactMatch)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (left == null || right == null)
                return (left == null && right == null);
            if (exactMatch && left.Length != right.Length)
                return false;
            for (int index = 0; index < left.Length; index++)
            {
                var leftValue = left[index];
                var rightValue = right[index];
                if ((leftValue == null && rightValue != null) || (leftValue != null && rightValue == null) || !predicate(leftValue, rightValue))
                    return false;
            }
            return true;
        }
    }
}
