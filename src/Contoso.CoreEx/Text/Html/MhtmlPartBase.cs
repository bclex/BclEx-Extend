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
using System.IO;
using System;
namespace Contoso.Text.Html
{
    /// <summary>
    /// MhtmlPartBase class
    /// </summary>
    public abstract class MhtmlPartBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MhtmlPartBase"/> class.
        /// </summary>
        protected MhtmlPartBase() { }

        /// <summary>
        /// Creates the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <returns></returns>
        public static MhtmlPartBase Create(string uri, string contentType, string contentEncoding)
        {
            var mhtmlPart = (contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ? (MhtmlPartBase)new TextMhtmlPart() : (MhtmlPartBase)new BinaryMhtmlPart());
            mhtmlPart.ContentEncoding = contentEncoding;
            mhtmlPart.ContentType = contentType;
            mhtmlPart.Uri = uri;
            return mhtmlPart;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content encoding.
        /// </summary>
        /// <value>The content encoding.</value>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Encodes the specified output writer.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="text">The text.</param>
        public abstract void Encode(TextWriter w, string text);
        /// <summary>
        /// Encodes the specified output writer.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="stream">The stream.</param>
        public abstract void Encode(TextWriter w, Stream stream);

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; set; }
    }
}
