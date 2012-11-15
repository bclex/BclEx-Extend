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
    public static partial class EnumerableEx
    {
        /// <summary>
        /// Tries the get tightest match.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="tightestValue">The tightest value.</param>
        /// <returns></returns>
        public static bool TryGetTightestMatch<TSource>(this IEnumerable<TSource> source, string value, char scope, out TSource tightestValue)
            where TSource : IValue<string>
        {
            if (value == null)
                throw new ArgumentNullException("value");
            // ensure url is proper by starting with a slash
            if ((value.Length == 0) || (value[0] != scope))
                throw new ArgumentException(string.Format(Local.InvalidIdA, value), "text");
            // then remove all leading slashes
            int firstNonSlashIndex = 0;
            for (; (firstNonSlashIndex < value.Length) && (value[firstNonSlashIndex] == scope); firstNonSlashIndex++) ;
            if ((firstNonSlashIndex > 0) && (firstNonSlashIndex < value.Length))
                value = value.Substring(firstNonSlashIndex);
            //string matchText = value + scope;
            // locate tightest virtual path match
            tightestValue = default(TSource);
            int tightestValueLength = 0;
            foreach (TSource item in source)
            {
                string itemValue = item.Value;
                int itemValueLength;
                if ((value.StartsWith(itemValue + scope)) && ((itemValueLength = itemValue.Length) > tightestValueLength))
                {
                    tightestValue = item;
                    tightestValueLength = itemValueLength;
                }
            }
            return (tightestValueLength > 0);
        }

        /// <summary>
        /// Tries the get tightest match.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="values">The values.</param>
        /// <param name="tightestValue">The tightest value.</param>
        /// <returns></returns>
        public static bool TryGetTightestMatch<T, TSource>(this IEnumerable<TSource> source, T[] values, out TSource tightestValue)
            where TSource : IValue<T[]>
        {
            if (values == null)
                throw new ArgumentNullException("values");
            // locate tightest virtual path match
            tightestValue = default(TSource);
            //T[] tightestValues = null;
            int tightestValuesLength = 0;
            foreach (TSource item in source)
            {
                T[] itemValues = item.Value;
                int itemValuesLength;
                if ((itemValues.Match(values, false)) && ((itemValuesLength = itemValues.Length) > tightestValuesLength))
                {
                    tightestValue = item;
                    //tightestValues = itemValues;
                    tightestValuesLength = itemValuesLength;
                }
            }
            return (tightestValuesLength > 0);
        }
    }
}
