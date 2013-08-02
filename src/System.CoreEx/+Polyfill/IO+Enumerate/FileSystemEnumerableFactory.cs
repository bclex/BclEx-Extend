#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Collections.Generic;
namespace System.IO
{
    /// <summary>
    /// FileSystemEnumerableFactory
    /// </summary>
    internal static class FileSystemEnumerableFactory
    {
        internal static IEnumerable<DirectoryInfo> CreateDirectoryInfoIterator(string path, string originalUserPath, string searchPattern, SearchOption searchOption)
        {
            return new FileSystemEnumerableIterator<DirectoryInfo>(path, originalUserPath, searchPattern, searchOption, new DirectoryInfoResultHandler(), true);
        }

        internal static IEnumerable<FileInfo> CreateFileInfoIterator(string path, string originalUserPath, string searchPattern, SearchOption searchOption)
        {
            return new FileSystemEnumerableIterator<FileInfo>(path, originalUserPath, searchPattern, searchOption, new FileInfoResultHandler(), true);
        }

        internal static IEnumerable<string> CreateFileNameIterator(string path, string originalUserPath, string searchPattern, bool includeFiles, bool includeDirs, SearchOption searchOption, bool checkHost)
        {
            return new FileSystemEnumerableIterator<string>(path, originalUserPath, searchPattern, searchOption, new StringResultHandler(includeFiles, includeDirs), checkHost);
        }

        internal static IEnumerable<FileSystemInfo> CreateFileSystemInfoIterator(string path, string originalUserPath, string searchPattern, SearchOption searchOption)
        {
            return new FileSystemEnumerableIterator<FileSystemInfo>(path, originalUserPath, searchPattern, searchOption, new FileSystemInfoResultHandler(), true);
        }
    }
}
#endif