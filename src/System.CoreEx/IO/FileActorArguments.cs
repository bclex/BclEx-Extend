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
namespace System.IO
{
    /// <summary>
    /// FileActorArguments
    /// </summary>
    public class FileActorArguments
    {
        private string _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileActorArguments"/> class.
        /// </summary>
        public FileActorArguments()
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
        }

        /// <summary>
        /// Gets or sets the poll interval.
        /// </summary>
        /// <value>
        /// The poll interval.
        /// </value>
        public TimeSpan PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter
        {
            get { return (string.IsNullOrEmpty(_filter) ? "*.*" : _filter); }
            set { _filter = value; }
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include subdirectories].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [include subdirectories]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// Gets or sets the notify filter.
        /// </summary>
        /// <value>
        /// The notify filter.
        /// </value>
        public NotifyFilters NotifyFilter { get; set; }
    }
}
