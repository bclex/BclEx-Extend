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
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// RequiredNullIfAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredNullIfAttribute : ValidationAttribute
    {
        private static readonly Comparer s_comparer = Comparer.Default;

        private object _nullValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredNullIfAttribute"/> class.
        /// </summary>
        /// <param name="nullValue">The null value.</param>
        public RequiredNullIfAttribute(object nullValue)
            : base(() => DataAnnotationsResources.RequiredAttribute_ValidationError)
        {
            _nullValue = nullValue;
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            if ((value == null) || (s_comparer.Compare(_nullValue, value) == 0))
                return false;
            string text = (value as string);
            return (text != null ? (text.Trim().Length != 0) : true);
        }
    }
}
