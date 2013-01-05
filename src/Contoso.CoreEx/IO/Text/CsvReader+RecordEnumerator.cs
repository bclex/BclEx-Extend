#region Foreign-License
//	LumenWorks.Framework.IO.CSV.CsvReader.RecordEnumerator
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
using System.Collections;
using System.Collections.Generic;
using System;
namespace Contoso.IO.Text
{
    public partial class CsvReader
    {
        /// <summary>
        /// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
        /// </summary>
        public struct RecordEnumerator : IEnumerator<string[]>, IEnumerator
        {
            private CsvReader _r;
            private string[] _current;
            private long _currentRecordIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
            /// </summary>
            /// <param name="r">The <see cref="T:CsvReader"/> to iterate over.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="r"/> is a <see langword="null"/>.
            /// </exception>
            public RecordEnumerator(CsvReader r)
            {
                if (r == null)
                    throw new ArgumentNullException("r");
                _r = r;
                _current = null;
                _currentRecordIndex = r.CurrentRecordIndex;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _r = null;
                _current = null;
            }

            /// <summary>
            /// Gets the current record.
            /// </summary>
            public string[] Current
            {
                get { return _current; }
            }

            /// <summary>
            /// Advances the enumerator to the next record of the CSV.
            /// </summary>
            /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next record, <see langword="false"/> if the enumerator has passed the end of the CSV.</returns>
            public bool MoveNext()
            {
                if (_r.CurrentRecordIndex != _currentRecordIndex)
                    throw new InvalidOperationException(Local.EnumerationVersionCheckFailed);
                if (_r.Read())
                {
                    _current = new string[_r._fieldCount];
                    _r.CopyCurrentRecordTo(_current);
                    _currentRecordIndex = _r.CurrentRecordIndex;
                    return true;
                }
                _current = null;
                _currentRecordIndex = _r.CurrentRecordIndex;
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first record in the CSV.
            /// </summary>
            public void Reset()
            {
                if (_r.CurrentRecordIndex != _currentRecordIndex)
                    throw new InvalidOperationException(Local.EnumerationVersionCheckFailed);
                _r.MoveTo(-1);
                _current = null;
                _currentRecordIndex = _r.CurrentRecordIndex;
            }

            /// <summary>
            /// Gets the current record.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_r.CurrentRecordIndex != _currentRecordIndex)
                        throw new InvalidOperationException(Local.EnumerationVersionCheckFailed);
                    return Current;
                }
            }
        }
    }
}