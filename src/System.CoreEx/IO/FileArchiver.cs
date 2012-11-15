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
using System.Collections;
namespace System.IO
{
    /// <summary>
    /// FileArchiver
    /// </summary>
    public class FileArchiver
    {
        /// <summary>
        /// ArchiveEveryMode
        /// </summary>
        public enum ArchiveEveryMode
        {
            /// <summary>
            /// 
            /// </summary>
            None,
            /// <summary>
            /// 
            /// </summary>
            Year,
            /// <summary>
            /// 
            /// </summary>
            Month,
            /// <summary>
            /// 
            /// </summary>
            Day,
            /// <summary>
            /// 
            /// </summary>
            Hour,
            /// <summary>
            /// 
            /// </summary>
            Minute
        }

        /// <summary>
        /// ArchiveNumberingMode
        /// </summary>
        public enum ArchiveNumberingMode
        {
            /// <summary>
            /// 
            /// </summary>
            Sequence,
            /// <summary>
            /// 
            /// </summary>
            Rolling
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchiver"/> class.
        /// </summary>
        public FileArchiver()
        {
            MaxArchiveFiles = 9;
            ArchiveAboveSize = -1L;
            ArchiveNumbering = ArchiveNumberingMode.Rolling;
        }

        /// <summary>
        /// Gets or sets the max archive files.
        /// </summary>
        /// <value>The max archive files.</value>
        public int MaxArchiveFiles { get; set; }

        /// <summary>
        /// Gets or sets the archive numbering.
        /// </summary>
        /// <value>The archive numbering.</value>
        public ArchiveNumberingMode ArchiveNumbering { get; set; }

        /// <summary>
        /// Gets or sets the size of the archive above.
        /// </summary>
        /// <value>The size of the archive above.</value>
        public long ArchiveAboveSize { get; set; }

        /// <summary>
        /// Gets or sets the archive every.
        /// </summary>
        /// <value>The archive every.</value>
        public ArchiveEveryMode ArchiveEvery { get; set; }

        /// <summary>
        /// Gets the file info.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns></returns>
        private bool GetFileInfo(string fileName, out DateTime lastWriteTime, out long fileLength)
        {
            var info = new FileInfo(fileName);
            if (info.Exists)
            {
                fileLength = info.Length;
                lastWriteTime = info.LastWriteTime;
                return true;
            }
            fileLength = -1L;
            lastWriteTime = DateTime.MinValue;
            return false;
        }

        /// <summary>
        /// Shoulds the auto archive.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="upcomingWriteTime">The upcoming write time.</param>
        /// <param name="upcomingWriteSize">Size of the upcoming write.</param>
        /// <returns></returns>
        public bool ShouldAutoArchive(string fileName, DateTime upcomingWriteTime, int upcomingWriteSize)
        {
            DateTime time;
            long num;
            string str;
            if ((ArchiveAboveSize == -1L) && (ArchiveEvery == ArchiveEveryMode.None))
                return false;
            if (!GetFileInfo(fileName, out time, out num))
                return false;
            if ((ArchiveAboveSize != -1L) && ((num + upcomingWriteSize) > ArchiveAboveSize))
                return true;
            switch (ArchiveEvery)
            {
                case ArchiveEveryMode.Year:
                    str = "yyyy";
                    break;
                case ArchiveEveryMode.Month:
                    str = "yyyyMM";
                    break;
                case ArchiveEveryMode.Hour:
                    str = "yyyyMMddHH";
                    break;
                case ArchiveEveryMode.Minute:
                    str = "yyyyMMddHHmm";
                    break;
                case ArchiveEveryMode.None:
                    return false;
                default:
                    str = "yyyyMMdd";
                    break;
            }
            string str2 = time.ToString(str);
            string str3 = upcomingWriteTime.ToString(str);
            if (str2 != str3)
                return true;
            return false;
        }

        /// <summary>
        /// Res the scope path.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ReScopePath(string directory, string path)
        {
            return Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(path), directory), Path.GetFileName(path)));
        }

        /// <summary>
        /// Does the auto archive.
        /// </summary>
        /// <param name="archiveDirectory">The archive directory.</param>
        /// <param name="fileName">Name of the file.</param>
        public void DoAutoArchive(string archiveDirectory, string fileName)
        {
            var info = new FileInfo(fileName);
            if (info.Exists)
            {
                string formattedMessage = Path.ChangeExtension(info.FullName, ".{#}" + Path.GetExtension(fileName));
                if ((archiveDirectory != null) && (archiveDirectory.Length > 0))
                    formattedMessage = ReScopePath(archiveDirectory, formattedMessage);
                switch (ArchiveNumbering)
                {
                    case ArchiveNumberingMode.Sequence:
                        SequentialArchive(info.FullName, formattedMessage);
                        return;
                    case ArchiveNumberingMode.Rolling:
                        RecursiveRollingRename(info.FullName, formattedMessage, 0);
                        return;
                }
            }
        }
        private void RecursiveRollingRename(string fileName, string pattern, int archiveNumber)
        {
            if ((MaxArchiveFiles != -1) && (archiveNumber >= MaxArchiveFiles))
                File.Delete(fileName);
            else if (File.Exists(fileName))
            {
                string str = ReplaceNumber(pattern, archiveNumber);
                if (File.Exists(fileName))
                    RecursiveRollingRename(str, pattern, archiveNumber + 1);
                try { File.Move(fileName, str); }
                catch (DirectoryNotFoundException) { Directory.CreateDirectory(Path.GetDirectoryName(str)); File.Move(fileName, str); }
            }
        }

        private void SequentialArchive(string fileName, string pattern)
        {
            string str = Path.GetFileName(pattern);
            int index = str.IndexOf("{#");
            int startIndex = str.IndexOf("#}") + 2;
            int num3 = str.Length - startIndex;
            string searchPattern = str.Substring(0, index) + "*" + str.Substring(startIndex);
            string directoryName = Path.GetDirectoryName(Path.GetFullPath(pattern));
            int num4 = -1;
            int num5 = -1;
            var hashtable = new Hashtable();
            try
            {
                foreach (string str4 in Directory.GetFiles(directoryName, searchPattern))
                {
                    int num6;
                    string str5 = Path.GetFileName(str4);
                    string str6 = str5.Substring(index, (str5.Length - num3) - index);
                    try { num6 = Convert.ToInt32(str6); }
                    catch (FormatException) { continue; }
                    num4 = Math.Max(num4, num6);
                    num5 = (num5 != -1 ? Math.Min(num5, num6) : num6);
                    hashtable[num6] = str4;
                }
                num4++;
            }
            catch (DirectoryNotFoundException) { Directory.CreateDirectory(directoryName); num4 = 0; }
            if ((MaxArchiveFiles != -1) && (num5 != -1))
            {
                int num7 = (num4 - MaxArchiveFiles) + 1;
                for (int i = num5; i < num7; i++)
                {
                    string path = (string)hashtable[i];
                    if (path != null)
                        File.Delete(path);
                }
            }
            string destFileName = ReplaceNumber(pattern, num4);
            File.Move(fileName, destFileName);
        }

        /// <summary>
        /// Replaces the number.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string ReplaceNumber(string pattern, int value)
        {
            int index = pattern.IndexOf("{#");
            int startIndex = pattern.IndexOf("#}") + 2;
            int totalWidth = (startIndex - index) - 2;
            return (pattern.Substring(0, index) + Convert.ToString(value, 10).PadLeft(totalWidth, '0') + pattern.Substring(startIndex));
        }
    }
}
