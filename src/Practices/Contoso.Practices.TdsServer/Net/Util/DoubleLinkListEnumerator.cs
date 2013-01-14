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
using System.Collections;

namespace System.Net.Util
{
    internal class DoubleLinkListEnumerator : IEnumerator
    {
        private DoubleLink _current;
        private DoubleLinkList _list;

        internal DoubleLinkListEnumerator(DoubleLinkList list)
        {
            _list = list;
            _current = list;
        }

        internal DoubleLink GetDoubleLink() { return _current; }

        public bool MoveNext()
        {
            if (_current.Next == _list)
            {
                _current = null;
                return false;
            }
            _current = _current.Next;
            return true;
        }

        public void Reset() { _current = _list; }

        public object Current
        {
            get
            {
                if (_current == null || _current == _list)
                    throw new InvalidOperationException();
                return _current.Item;
            }
        }
    }
}
