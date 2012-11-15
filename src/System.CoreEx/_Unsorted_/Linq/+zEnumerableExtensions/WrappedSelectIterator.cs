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
using System.Diagnostics;
namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        private sealed class WrappedSelectIterator<TSource, TResult> : IEnumerable<TResult>, IEnumerable, IEnumerator<TResult>, IEnumerator, IDisposable
        {
            private int _state;
            private TResult _current;
            private Func<TSource, int, TResult> _originalSelector;
            private IEnumerable<TSource> _originalSource;
            private IEnumerator<TSource> _wrapa;
            private int _threadId;
            private TSource _a;
            private int _b;
            private Func<TSource, int, TResult> _selector;
            private IEnumerable<TSource> _source;

            [DebuggerHidden]
            public WrappedSelectIterator(int state)
            {
                _state = state;
                _threadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void Finally()
            {
                _state = -1;
                if (_wrapa != null)
                    _wrapa.Dispose();
            }

            public bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (_state)
                    {
                        case 0:
                            _state = -1;
                            _b = -1;
                            _wrapa = _source.GetEnumerator();
                            _state = 1;
                            goto Label_0094;
                        case 2:
                            _state = 1;
                            goto Label_0094;
                        default:
                            goto Label_00A7;
                    }
                Label_0046:
                    _a = _wrapa.Current;
                    _b++;
                    _current = _selector(_a, _b);
                    _state = 2;
                    return true;
                Label_0094:
                    if (_wrapa.MoveNext())
                        goto Label_0046;
                    Finally();
                Label_00A7:
                    flag = false;
                }
                finally { ((IDisposable)this).Dispose(); }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
            {
                WrappedSelectIterator<TSource, TResult> d;
                if ((_threadId == Thread.CurrentThread.ManagedThreadId) && (_state == -2))
                {
                    _state = 0;
                    d = (WrappedSelectIterator<TSource, TResult>)this;
                }
                else
                    d = new WrappedSelectIterator<TSource, TResult>(0);
                d._source = _originalSource;
                d._selector = _originalSelector;
                return d;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<TResult>)this).GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (_state)
                {
                    case 1:
                    case 2:
                        try { }
                        finally { Finally(); }
                        return;
                }
            }

            TResult IEnumerator<TResult>.Current
            {
                [DebuggerHidden]
                get { return _current; }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get { return _current; }
            }
        }
    }
}
#endif
