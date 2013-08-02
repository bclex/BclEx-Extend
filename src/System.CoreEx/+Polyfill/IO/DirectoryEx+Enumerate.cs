#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Collections.Generic;
namespace System.IO
{
    partial class DirectoryEx
    {
        /// <summary>
        /// Enumerates the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return InternalEnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            return InternalEnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
                throw new ArgumentOutOfRangeException("searchOption", SR.GetResourceString("ArgumentOutOfRange_Enum"));
            return InternalEnumerateDirectories(path, searchPattern, searchOption);
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFiles(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return InternalEnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            return InternalEnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
                throw new ArgumentOutOfRangeException("searchOption", SR.GetResourceString("ArgumentOutOfRange_Enum"));
            return InternalEnumerateFiles(path, searchPattern, searchOption);
        }

        /// <summary>
        /// Enumerates the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return InternalEnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            return InternalEnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
                throw new ArgumentOutOfRangeException("searchOption", SR.GetResourceString("ArgumentOutOfRange_Enum"));
            return InternalEnumerateFileSystemEntries(path, searchPattern, searchOption);
        }

        private static IEnumerable<string> EnumerateFileSystemNames(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs) { return FileSystemEnumerableFactory.CreateFileNameIterator(path, path, searchPattern, includeFiles, includeDirs, searchOption, true); }
        private static IEnumerable<string> InternalEnumerateDirectories(string path, string searchPattern, SearchOption searchOption) { return EnumerateFileSystemNames(path, searchPattern, searchOption, false, true); }
        private static IEnumerable<string> InternalEnumerateFiles(string path, string searchPattern, SearchOption searchOption) { return EnumerateFileSystemNames(path, searchPattern, searchOption, true, false); }
        private static IEnumerable<string> InternalEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) { return EnumerateFileSystemNames(path, searchPattern, searchOption, true, true); }
    }
}
#endif