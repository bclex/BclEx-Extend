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
namespace System
{
    /// <summary>
    /// Range
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Range<T>
    {
        private bool _hasEndValue;
        private T _endValue;

        /// <summary>
        /// Gets or sets the begin value.
        /// </summary>
        /// <value>The begin value.</value>
        public T BeginValue { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has end value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has end value; otherwise, <c>false</c>.
        /// </value>
        public bool HasEndValue
        {
            get { return _hasEndValue; }
            set
            {
                _hasEndValue = value;
                if (!_hasEndValue)
                    EndValue = default(T);
            }
        }
        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        /// <value>The end value.</value>
        public T EndValue
        {
            get { return _endValue; }
            set
            {
                _endValue = value;
                _hasEndValue = true;
            }
        }
    }
}