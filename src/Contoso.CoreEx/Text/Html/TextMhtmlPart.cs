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
using Contoso.Primitives.Codecs;
namespace Contoso.Text.Html
{
    /// <summary>
    /// TextMhtmlPart
    /// </summary>
    public class TextMhtmlPart : MhtmlPartBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMhtmlPart"/> class.
        /// </summary>
        public TextMhtmlPart()
            : base() { }

        /// <summary>
        /// Appends the header.
        /// </summary>
        /// <param name="w">The w.</param>
        public void AppendHeader(TextWriter w)
        {
            w.WriteLine("Content-Type: " + ContentType);
            w.WriteLine("   charset=\"" + ContentEncoding + "\"");
            w.WriteLine("Content-Transfer-Encoding: quoted-printable");
            w.WriteLine("Content-Location: " + Uri.ToString());
            w.WriteLine();
        }

        /// <summary>
        /// Encodes the specified output writer.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="text">The text.</param>
        public override void Encode(TextWriter w, string text)
        {
            AppendHeader(w);
            w.WriteLine(QuotedPrintableCodec.Encode(text));
        }
        /// <summary>
        /// Encodes the specified output writer.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="stream">The stream.</param>
        public override void Encode(TextWriter w, Stream stream)
        {
            AppendHeader(w);
            using (var r = new StreamReader(stream))
                w.WriteLine(QuotedPrintableCodec.Encode(r.ReadToEnd()));
        }
    }
}
