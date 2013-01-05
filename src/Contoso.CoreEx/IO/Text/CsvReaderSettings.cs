#region Foreign-License
//	LumenWorks.Framework.IO.CSV.CsvReader
//	Copyright (c) 2005 Sébastien Lorion
//
//	MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Modified: Sky Morey <moreys@digitalev.com>
//
#endregion
namespace Contoso.IO.Text
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to CSV data.  
    /// </summary>
    public class CsvReaderSettings
    {
        /// <summary>
        /// Defines the default delimiter character separating each field.
        /// </summary>
        public const char DefaultDelimiter = ',';
        /// <summary>
        /// Defines the default quote character wrapping every field.
        /// </summary>
        public const char DefaultQuote = '"';
        /// <summary>
        /// Defines the default escape character letting insert quotation characters inside a quoted field.
        /// </summary>
        public const char DefaultEscape = '"';
        /// <summary>
        /// Defines the default comment character indicating that a line is commented out.
        /// </summary>
        public const char DefaultComment = '#';

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderSettings"/> class.
        /// </summary>
        public CsvReaderSettings()
        {
            Delimiter = DefaultDelimiter;
            Quote = DefaultQuote;
            Escape = DefaultEscape;
            Comment = DefaultComment;
            HasHeaders = false;
            ValueTrimmings = CsvValueTrimmings.UnquotedOnly;
            SupportsMultiline = true;
            SkipEmptyLines = true;
            MissingFieldAction = MissingCsvFieldAction.ReplaceByNull;
            DefaultParseErrorAction = CsvParseErrorAction.RaiseEvent;
            DefaultHeaderName = "Column";
        }

        /// <summary>
        /// Gets the comment character indicating that a line is commented out.
        /// </summary>
        /// <value>The comment character indicating that a line is commented out.</value>
        public char Comment { get; set; }

        /// <summary>
        /// Gets the escape character letting insert quotation characters inside a quoted field.
        /// </summary>
        /// <value>The escape character letting insert quotation characters inside a quoted field.</value>
        public char Escape { get; set; }

        /// <summary>
        /// Gets the delimiter character separating each field.
        /// </summary>
        /// <value>The delimiter character separating each field.</value>
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets the quotation character wrapping every field.
        /// </summary>
        /// <value>The quotation character wrapping every field.</value>
        public char Quote { get; set; }

        /// <summary>
        /// Indicates if field names are located on the first non commented line.
        /// </summary>
        /// <value><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</value>
        public bool HasHeaders { get; set; }

        /// <summary>
        /// Indicates if spaces at the start and end of a field are trimmed.
        /// </summary>
        /// <value><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>.</value>
        public CsvValueTrimmings ValueTrimmings { get; set; }

        /// <summary>
        /// Gets or sets the default action to take when a parsing error has occured.
        /// </summary>
        /// <value>The default action to take when a parsing error has occured.</value>
        public CsvParseErrorAction DefaultParseErrorAction { get; set; }

        /// <summary>
        /// Gets or sets the action to take when a field is missing.
        /// </summary>
        /// <value>The action to take when a field is missing.</value>
        public MissingCsvFieldAction MissingFieldAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the reader supports multiline fields.
        /// </summary>
        /// <value>A value indicating if the reader supports multiline field.</value>
        public bool SupportsMultiline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the reader will skip empty lines.
        /// </summary>
        /// <value>A value indicating if the reader will skip empty lines.</value>
        public bool SkipEmptyLines { get; set; }

        /// <summary>
        /// Gets or sets the default header name when it is an empty string or only whitespaces.
        /// The header index will be appended to the specified name.
        /// </summary>
        /// <value>The default header name when it is an empty string or only whitespaces.</value>
        public string DefaultHeaderName { get; set; }
    }
}