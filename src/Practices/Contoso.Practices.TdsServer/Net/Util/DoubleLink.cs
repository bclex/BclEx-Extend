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

namespace System.Net.Util
{
    internal class DoubleLink
    {
        internal DoubleLink _next;
        internal DoubleLink _prev;
        internal object Item;
        
        internal DoubleLink() { _next = _prev = this; }
        internal DoubleLink(object item) : this() { Item = item; }

        internal void InsertAfter(DoubleLink after)
        {
            _prev = after;
            _next = after._next;
            after._next = this;
            _next._prev = this;
        }

        internal void InsertBefore(DoubleLink before)
        {
            _prev = before._prev;
            _next = before;
            before._prev = this;
            _prev._next = this;
        }

        internal void Remove()
        {
            _prev._next = _next;
            _next._prev = _prev;
            _next = _prev = this;
        }

        internal DoubleLink Next
        {
            get { return _next; }
        }
    }
}
