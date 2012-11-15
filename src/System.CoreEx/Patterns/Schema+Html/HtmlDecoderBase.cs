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
using System.Text;
using System.Text.Html;
namespace System.Patterns.Schema
{
    /// <summary>
    /// HtmlDecoderBase
    /// </summary>
    public abstract class HtmlDecoderBase
    {
        /// <summary>
        /// Decodes the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="text">The text.</param>
        public abstract void Decode(StringBuilder b, HtmlFormatterBase formatter, string text);
        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        protected internal bool IsBound { get; internal set; }
        internal uint Flag { get; set; }
        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        protected internal HtmlFormatterBase Formatter { get; set; }
    }
}
