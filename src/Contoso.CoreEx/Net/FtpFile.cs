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
namespace Contoso.Net
{
    /// <summary>
    /// FtpFile
    /// </summary>
    public class FtpFile
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        public int FileSize { get; set; }

        internal static FtpFile Parse(string line)
        {
            var file = new FtpFile();
            if (string.IsNullOrEmpty(line))
                return file;
            line = line.Trim();
            var dateStart = line.IndexOf(' ');
            var dateString = line.Substring(dateStart + 1, 18);
            DateTime lastModified;
            if (DateTime.TryParse(dateString, out lastModified))
                file.LastModified = lastModified;
            int fileSize;
            if (int.TryParse(line.Substring(0, dateStart), out fileSize))
                file.FileSize = fileSize;
            file.FileName = line.Substring(dateStart + 20);
            return file;
        }
    }
}
