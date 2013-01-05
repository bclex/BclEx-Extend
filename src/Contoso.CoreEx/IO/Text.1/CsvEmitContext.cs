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
namespace Contoso.IO.Text
{
    /// <summary>
    /// CsvEmitContext
    /// </summary>
    public class CsvEmitContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly CsvEmitContext DefaultContext = new CsvEmitContext { };

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvEmitContext"/> class.
        /// </summary>
        public CsvEmitContext()
        {
            EmitOptions = CsvEmitOptions.HasHeaderRow | CsvEmitOptions.EncodeValues;
            FilterMode = CsvEmitFilterMode.ExceptionsInFields;
            Fields = new CsvEmitFieldCollection();
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Boolean"/> with the specified option.
        /// </summary>
        public bool this[CsvEmitOptions option]
        {
            get { return ((EmitOptions & option) == option); }
            set { EmitOptions = (value ? EmitOptions | option : EmitOptions & ~option); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has header row.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has header row; otherwise, <c>false</c>.
        /// </value>
        public bool HasHeaderRow
        {
            get { return this[CsvEmitOptions.HasHeaderRow]; }
            set { this[CsvEmitOptions.HasHeaderRow] = value; }
        }

        /// <summary>
        /// Gets or sets the filter mode.
        /// </summary>
        /// <value>
        /// The filter mode.
        /// </value>
        public CsvEmitFilterMode FilterMode { get; set; }
        /// <summary>
        /// Gets the fields.
        /// </summary>
        public CsvEmitFieldCollection Fields { get; private set; }
        /// <summary>
        /// Gets or sets the emit options.
        /// </summary>
        /// <value>
        /// The emit options.
        /// </value>
        public CsvEmitOptions EmitOptions { get; set; }
    }
}