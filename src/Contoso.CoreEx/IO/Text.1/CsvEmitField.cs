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
namespace Contoso.IO.Text
{
    /// <summary>
    /// CsvEmitField
    /// </summary>
    public class CsvEmitField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvEmitField"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CsvEmitField(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is ignore.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore; otherwise, <c>false</c>.
        /// </value>
        public bool Ignore { get; set; }
        /// <summary>
        /// Gets or sets the custom field formatter.
        /// </summary>
        /// <value>
        /// The custom field formatter.
        /// </value>
        public CsvEmitFieldFormatter CustomFieldFormatter { get; set; }
        /// <summary>
        /// Gets or sets the convert formatter.
        /// </summary>
        /// <value>
        /// The convert formatter.
        /// </value>
        public IConvertFormatter ConvertFormatter { get; set; }
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public string DefaultValue { get; set; }
        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        /// <value>
        /// The args.
        /// </value>
        public Nparams Args { get; set; }
    }
}