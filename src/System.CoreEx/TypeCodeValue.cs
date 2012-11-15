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
    /// Provides an object wrapper class to allow association of tagging information to other objects types that
    /// do not provide a generic tag property for this purpose. Used to associate a TypeCode instance to a class.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public struct TypeCodeValue<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCodeValue&lt;TValue&gt;"/> struct.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        public TypeCodeValue(TypeCode typeCode)
        {
            TypeCode = typeCode;
            Value = default(TValue);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCodeValue&lt;TValue&gt;"/> struct.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        /// <param name="value">The value.</param>
        public TypeCodeValue(TypeCode typeCode, TValue value)
        {
            TypeCode = typeCode;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public TypeCode TypeCode;
        /// <summary>
        /// 
        /// </summary>
        public TValue Value;
    }
}
