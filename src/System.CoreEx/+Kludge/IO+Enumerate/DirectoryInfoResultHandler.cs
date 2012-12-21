#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Security;
using System.Security.Permissions;
namespace System.IO
{
    /// <summary>
    /// DirectoryInfoResultHandler
    /// </summary>
    internal class DirectoryInfoResultHandler : SearchResultHandler<DirectoryInfo>
    {
        [SecurityCritical]
        internal override DirectoryInfo CreateObject(SearchResult result)
        {
            var fullPath = result.FullPath;
            var info = new DirectoryInfo(fullPath);
            info.InitializeFrom(result.FindData);
            return info;
        }

        [SecurityCritical]
        internal override bool IsResultIncluded(SearchResult result) { return FileSystemEnumerableHelpers.IsDir(result.FindData); }
    }
}
#endif