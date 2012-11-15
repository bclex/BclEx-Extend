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
using System.Globalization;
namespace System
{
    /// <summary>
    /// HashCodeCombiner
    /// </summary>
    public class HashCodeCombiner
    {
        private long _combinedHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCodeCombiner"/> class.
        /// </summary>
        public HashCodeCombiner()
        {
            _combinedHash = 0x1505L;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashCodeCombiner"/> class.
        /// </summary>
        /// <param name="initialCombinedHash">The initial combined hash.</param>
        public HashCodeCombiner(long initialCombinedHash)
        {
            _combinedHash = initialCombinedHash;
        }

        /// <summary>
        /// Adds the array.
        /// </summary>
        /// <param name="a">A.</param>
        public void AddArray(string[] a)
        {
            if (a != null)
            {
                int length = a.Length;
                for (int index = 0; index < length; index++)
                    AddObject(a[index]);
            }
        }

        /// <summary>
        /// Adds the case insensitive string.
        /// </summary>
        /// <param name="s">The s.</param>
        public void AddCaseInsensitiveString(string s)
        {
            if (s != null)
                AddInt(StringComparer.OrdinalIgnoreCase.GetHashCode(s));
        }

        /// <summary>
        /// Adds the date time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        public void AddDateTime(System.DateTime dt)
        {
            AddInt(dt.GetHashCode());
        }

        ///// <summary>
        ///// Adds the directory.
        ///// </summary>
        ///// <param name="directoryName">Name of the directory.</param>
        //public void AddDirectory(string directoryName)
        //{
        //    DirectoryInfo info = new DirectoryInfo(directoryName);
        //    if (info.Exists)
        //    {
        //        AddObject(directoryName);
        //        foreach (FileData data in (IEnumerable)FileEnumerator.Create(directoryName))
        //            if (data.IsDirectory)
        //                AddDirectory(data.FullName);
        //            else
        //                AddExistingFile(data.FullName);
        //        AddDateTime(info.CreationTimeUtc);
        //        AddDateTime(info.LastWriteTimeUtc);
        //    }
        //}

        ///// <summary>
        ///// Adds the existing file.
        ///// </summary>
        ///// <param name="fileName">Name of the file.</param>
        //public void AddExistingFile(string fileName)
        //{
        //    AddInt(StringUtil.GetStringHashCode(fileName));
        //    FileInfo info = new FileInfo(fileName);
        //    AddDateTime(info.CreationTimeUtc);
        //    AddDateTime(info.LastWriteTimeUtc);
        //    AddFileSize(info.Length);
        //}

        ///// <summary>
        ///// Adds the file.
        ///// </summary>
        ///// <param name="fileName">Name of the file.</param>
        //public void AddFile(string fileName)
        //{
        //    if (!FileUtil.FileExists(fileName))
        //    {
        //        if (FileUtil.DirectoryExists(fileName))
        //            AddDirectory(fileName);
        //    }
        //    else
        //        AddExistingFile(fileName);
        //}

        /// <summary>
        /// Adds the size of the file.
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        public void AddFileSize(long fileSize)
        {
            AddInt(fileSize.GetHashCode());
        }

        /// <summary>
        /// Adds the int.
        /// </summary>
        /// <param name="n">The n.</param>
        public void AddInt(int n)
        {
            _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ n;
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        public void AddObject(bool b)
        {
            AddInt(b.GetHashCode());
        }
        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="b">The b.</param>
        public void AddObject(byte b)
        {
            AddInt(b.GetHashCode());
        }
        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="n">The n.</param>
        public void AddObject(int n)
        {
            AddInt(n);
        }
        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="l">The l.</param>
        public void AddObject(long l)
        {
            AddInt(l.GetHashCode());
        }
        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="o">The o.</param>
        public void AddObject(object o)
        {
            if (o != null)
            {
                AddInt(o.GetHashCode());
            }
        }
        ///// <summary>
        ///// Adds the object.
        ///// </summary>
        ///// <param name="s">The s.</param>
        //public void AddObject(string s)
        //{
        //    if (s != null)
        //        AddInt(StringUtil.GetStringHashCode(s));
        //}

        ///// <summary>
        ///// Adds the resources directory.
        ///// </summary>
        ///// <param name="directoryName">Name of the directory.</param>
        //public void AddResourcesDirectory(string directoryName)
        //{
        //    DirectoryInfo info = new DirectoryInfo(directoryName);
        //    if (info.Exists)
        //    {
        //        AddObject(directoryName);
        //        foreach (FileData data in (IEnumerable)FileEnumerator.Create(directoryName))
        //        {
        //            if (data.IsDirectory)
        //                AddResourcesDirectory(data.FullName);
        //            else
        //            {
        //                string fullName = data.FullName;
        //                if (Util.GetCultureName(fullName) == null)
        //                    AddExistingFile(fullName);
        //            }
        //        }
        //        AddDateTime(info.CreationTimeUtc);
        //    }
        //}

        /// <summary>
        /// Combines the hash codes.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
        /// <summary>
        /// Combines the hash codes.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <param name="h3">The h3.</param>
        /// <returns></returns>
        public static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }
        /// <summary>
        /// Combines the hash codes.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <param name="h3">The h3.</param>
        /// <param name="h4">The h4.</param>
        /// <returns></returns>
        public static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
        }
        /// <summary>
        /// Combines the hash codes.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <param name="h3">The h3.</param>
        /// <param name="h4">The h4.</param>
        /// <param name="h5">The h5.</param>
        /// <returns></returns>
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        ///// <summary>
        ///// Gets the directory hash.
        ///// </summary>
        ///// <param name="virtualDir">The virtual dir.</param>
        ///// <returns></returns>
        //public static string GetDirectoryHash(VirtualPath virtualDir)
        //{
        //    HashCodeCombiner combiner = new HashCodeCombiner();
        //    combiner.AddDirectory(virtualDir.MapPathInternal());
        //    return combiner.CombinedHashString;
        //}

        /// <summary>
        /// Gets the combined hash.
        /// </summary>
        /// <value>The combined hash.</value>
        public long CombinedHash
        {
            get { return _combinedHash; }
        }

        /// <summary>
        /// Gets the combined hash32.
        /// </summary>
        /// <value>The combined hash32.</value>
        public int CombinedHash32
        {
            get { return _combinedHash.GetHashCode(); }
        }

        /// <summary>
        /// Gets the combined hash string.
        /// </summary>
        /// <value>The combined hash string.</value>
        public string CombinedHashString
        {
            get { return _combinedHash.ToString("x", CultureInfo.InvariantCulture); }
        }
    }
}
