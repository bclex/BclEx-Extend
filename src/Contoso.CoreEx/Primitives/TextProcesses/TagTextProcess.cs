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
#if xEXPERIMENTAL
using System.Text;
namespace System.Primitives.TextProcesses
{
    /// <summary>
    /// Provides processing implementation for strings encoded using the format "[[key{parameter}parameter2]]".
    /// </summary>
    public class TagTextProcess : TextProcessBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagTextProcess"/> class.
        /// </summary>
        public TagTextProcess() { }

        public override string Process(string[] texts, Nattrib attrib)
        {
            if (texts == null)
                throw new ArgumentNullException("text");
            if (texts.Length != 1)
                throw new ArgumentOutOfRangeException("text");
            string singleText = texts[0];
            int index = 0;
            while ((singleText.IndexOf("[[") > -1) && (index++ < 5))
                singleText = ProcessInternal(singleText, attrib);
            return singleText;
        }

        private string ProcessInternal(string text, Nattrib attrib)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            int openTagIndex;
            int closeTagIndex;
            int startIndex = 0;
            var b = new StringBuilder();
            var textSearcher = new StringSearcher(text, "<![CDATA[", "]]>");
            // 1. expand tag
            while (textSearcher.FindTextSpan(startIndex, "[[", "]]", out openTagIndex, out closeTagIndex))
            {
                b.Append(text.Substring(startIndex, openTagIndex - startIndex));
                startIndex = openTagIndex + 2;
                //
                string tagKey;
                string[] args;
                //
                int openArgIndex;
                int closeArgIndex;
                if (((openArgIndex = textSearcher.IndexOf("{", startIndex, closeTagIndex - startIndex - 1)) > -1) && ((closeArgIndex = textSearcher.IndexOf("}", openArgIndex, closeTagIndex - openArgIndex - 1)) > -1))
                {
                    // has arguments
                    tagKey = text.Substring(startIndex, openArgIndex - startIndex).Trim();
                    string arg = text.Substring(openArgIndex + 1, closeArgIndex - openArgIndex - 1);
                    string arg2 = text.Substring(closeArgIndex + 1, closeTagIndex - closeArgIndex - 2);
                    args = new[] { arg, arg2 };
                }
                else
                {
                    // tag only - no arguments
                    tagKey = text.Substring(startIndex, closeTagIndex - startIndex - 1).Trim();
                    args = null;
                }
                //
                TextProcessBase tag;
                if ((tagKey.Length > 0) && ((tag = TextProcessBase.Get(tagKey)) != null))
                    b.Append(tag.Process(args));
                else
                    b.Append(text.Substring(openTagIndex, closeTagIndex - openTagIndex + 2));
                startIndex = closeTagIndex + 1;
            }
            b.Append(text.Substring(startIndex));
            return b.ToString();
        }
    }
}
#endif