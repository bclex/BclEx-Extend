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
namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        private class WhereEnumerableIterator<TSource> : Iterator<TSource>
        {
            private IEnumerator<TSource> _enumerator;
            private Func<TSource, bool> _predicate;
            private IEnumerable<TSource> _source;

            public WhereEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public override Iterator<TSource> Clone()
            {
                return new WhereEnumerableIterator<TSource>(_source, _predicate);
            }

            public override void Dispose()
            {
                if (_enumerator != null)
                    _enumerator.Dispose();
                _enumerator = null;
                base.Dispose();
            }

            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        _state = 2;
                        break;
                    case 2:
                        break;
                    default:
                        return false;
                }
                while (_enumerator.MoveNext())
                {
                    TSource current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        _current = current;
                        return true;
                    }
                }
                Dispose();
                return false;
            }

            public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
            {
                return new WhereSelectEnumerableIterator<TSource, TResult>(_source, _predicate, selector);
            }

            public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
            {
                return new WhereEnumerableIterator<TSource>(_source, CombinePredicates<TSource>(_predicate, predicate));
            }
        }
    }
}
#endif