#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Security;
using System.Security.Permissions;
namespace System.IO
{
    /// <summary>
    /// FileInfoResultHandler
    /// </summary>
    internal class FileInfoResultHandler : SearchResultHandler<FileInfo>
    {
        [SecurityCritical]
        internal override FileInfo CreateObject(SearchResult result)
        {
            var fullPath = result.FullPath;
            var info = new FileInfo(fullPath);
            info.InitializeFrom(result.FindData);
            return info;
        }

        [SecurityCritical]
        internal override bool IsResultIncluded(SearchResult result) { return FileSystemEnumerableHelpers.IsFile(result.FindData); }
    }
}
#endif