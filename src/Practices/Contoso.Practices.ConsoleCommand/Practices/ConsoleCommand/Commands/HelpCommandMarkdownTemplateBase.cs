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
using System.Reflection;
using System.Text;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    /// <summary>
    /// HelpCommandMarkdownTemplateBase
    /// </summary>
    public class HelpCommandMarkdownTemplateBase
    {
        private StringBuilder _generatingEnvironment = new StringBuilder();

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(object value)
        {
            string valueAsString;
            if (value == null)
                throw new ArgumentNullException("value");
            var method = value.GetType().GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
            if (method == null)
                valueAsString = value.ToString();
            else
                valueAsString = (string)method.Invoke(value, new object[] { CultureInfo.InvariantCulture });
            WriteLiteral(valueAsString);
        }

        /// <summary>
        /// Writes the literal.
        /// </summary>
        /// <param name="textToAppend">The text to append.</param>
        public void WriteLiteral(string textToAppend)
        {
            if (!string.IsNullOrEmpty(textToAppend))
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        /// Gets or sets the generation environment.
        /// </summary>
        /// <value>
        /// The generation environment.
        /// </value>
        protected StringBuilder GenerationEnvironment
        {
            get { return _generatingEnvironment; }
            set { _generatingEnvironment = value; }
        }
    }
}

