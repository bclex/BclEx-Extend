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
using System;
using System.Globalization;
using System.Collections.Generic;
namespace System.Patterns.Schema
{
    /// <summary>
    /// HtmlSchemaBase
    /// </summary>
    public abstract class HtmlSchemaBase
    {
        /// <summary>
        /// DecodeFlags
        /// </summary>
        [Flags]
        public enum DecodeFlags
        {
            /// <summary>
            /// CrLfToBr
            /// </summary>
            CrLfToBr = 0x1,
        }

        /// <summary>
        /// Gets or sets the decoders.
        /// </summary>
        /// <value>
        /// The decoders.
        /// </value>
        public abstract Dictionary<Type, HtmlDecoderBase> Decoders { get; protected set; }
        /// <summary>
        /// Decodes the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public virtual string DecodeHtml(string html, Nparams param) { return DecodeHtml(html, (param == null ? (uint)DecodeFlags.CrLfToBr : ParseDecodeFlag(param.Get<string>("format", "Full")))); }
        /// <summary>
        /// Decodes the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="defaultDecodeFlags">The default decode flags.</param>
        /// <returns></returns>
        public abstract string DecodeHtml(string html, uint defaultDecodeFlags);

        /// <summary>
        /// Parses the decode flag.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static uint ParseDecodeFlag(string id)
        {
            throw new NotImplementedException();
        }

        #region FluentConfig

        /// <summary>
        /// Adds the decoder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decoder">The decoder.</param>
        /// <returns></returns>
        public abstract HtmlSchemaBase AddDecoder<T>(T decoder)
             where T : HtmlDecoderBase;
        /// <summary>
        /// Makes the read only.
        /// </summary>
        /// <returns></returns>
        public abstract HtmlSchemaBase MakeReadOnly();

        #endregion
    }
}
