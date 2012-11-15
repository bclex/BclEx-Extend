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
    /// MhtmlBuilder class
    /// </summary>
    public class MhtmlBuilder
    {
        private static readonly string s_mimeBoundary = GenerateMimeBoundary();
        private TextWriter _w;

        /// <summary>
        /// Initializes a new instance of the <see cref="MhtmlBuilder"/> class.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="subject">The subject.</param>
        public MhtmlBuilder(TextWriter w, string subject)
        {
            if (w == null)
                throw new ArgumentNullException("outputWriter");
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException("subject");
            _w = w;
            //
            DateTime now = DateTime.Now;
            _w.WriteLine(string.Format(@"From: ""BCLConfig MHTML Builder""
Subject: {0}
Date: {1}
MIME-Version: 1.0
Content-Type: multipart/related;
	type=""text/html"";
	boundary=""{2}""

This is left multi-part message in MIME format.
", subject, now.ToString("ddd, d MMM yyyy HH:mm:ss ") + now.ToString("zzz").Replace(":", ""), s_mimeBoundary));
        }

        /// <summary>
        /// Adds the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">The content encoding.</param>
        public void Add(string uri, string contentType, string contentEncoding)
        {
            // fetch resource from Internet
            // if uri is relative, append base uri
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="text">The text.</param>
        public void Add(string uri, string contentType, string contentEncoding, string text)
        {
            Add(MhtmlPartBase.Create(uri, contentType, contentEncoding), text);
        }
        /// <summary>
        /// Adds the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="stream">The stream.</param>
        public void Add(string uri, string contentType, string contentEncoding, Stream stream)
        {
            Add(MhtmlPartBase.Create(uri, contentType, contentEncoding), stream);
        }
        /// <summary>
        /// Adds the specified MHTML part.
        /// </summary>
        /// <param name="mhtmlPart">The MHTML part.</param>
        /// <param name="text">The text.</param>
        public virtual void Add(MhtmlPartBase mhtmlPart, string text)
        {
            if (mhtmlPart == null)
                throw new ArgumentNullException("mhtmlPart");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            AppendBoundary();
            mhtmlPart.Encode(_w, text);
        }
        /// <summary>
        /// Adds the specified MHTML part.
        /// </summary>
        /// <param name="mhtmlPart">The MHTML part.</param>
        /// <param name="stream">The stream.</param>
        public virtual void Add(MhtmlPartBase mhtmlPart, Stream stream)
        {
            if (mhtmlPart == null)
                throw new ArgumentNullException("mhtmlPart");
            if (stream == null)
                throw new ArgumentNullException("stream");
            AppendBoundary();
            mhtmlPart.Encode(_w, stream);
        }

        /// <summary>
        /// Appends the boundary.
        /// </summary>
        private void AppendBoundary()
        {
            AppendBoundary(string.Empty);
        }
        /// <summary>
        /// Appends the boundary.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        private void AppendBoundary(string suffix)
        {
            _w.WriteLine("--" + s_mimeBoundary + suffix);
        }

        /// <summary>
        /// Gets or sets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        public string BaseUri { get; set; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            AppendBoundary("--");
            _w.Flush();
        }

        /// <summary>
        /// Generates the MIME boundary.
        /// </summary>
        /// <returns></returns>
        private static string GenerateMimeBoundary()
        {
            return "----=_Boundary_" + Guid.NewGuid().ToString("N");
        }
    }
}
