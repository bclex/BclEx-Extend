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
#if xEXPERIMENTAL
using System.Collections.Generic;
using System.Collections;
using System.Threading;
namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        private abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
        {
            internal TSource _current;
            internal int _state;
            private int _threadId;

            public Iterator()
            {
                _threadId = Thread.CurrentThread.ManagedThreadId;
            }

            public abstract Iterator<TSource> Clone();

            public virtual void Dispose()
            {
                _current = default(TSource);
                _state = -1;
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                if ((_threadId == Thread.CurrentThread.ManagedThreadId) && (_state == 0))
                {
                    _state = 1;
                    return this;
                }
                var iterator = Clone();
                iterator._state = 1;
                return iterator;
            }

            public abstract bool MoveNext();

            public abstract IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector);

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            public abstract IEnumerable<TSource> Where(Func<TSource, bool> predicate);

            public TSource Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
#endif