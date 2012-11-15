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
    internal static partial class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> GroupAt<TSource>(this IEnumerable<TSource> source, int size) { return GroupAt(source, size, x => x); }
        public static IEnumerable<TResult> GroupAt<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");
            int index = 0;
            var items = new TSource[size];
            foreach (var item in source)
            {
                items[index++] = item;
                if (index != size)
                    continue;
                yield return resultSelector(items);
                index = 0;
                items = new TSource[size];
            }
            if (index > 0)
                yield return resultSelector(items.Take(index));
        }
    }
}
