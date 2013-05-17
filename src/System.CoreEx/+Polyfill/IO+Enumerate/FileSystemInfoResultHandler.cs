#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
namespace System.IO
{
    /// <summary>
    /// FileSystemInfoResultHandler
    /// </summary>
    internal class FileSystemInfoResultHandler : SearchResultHandler<FileSystemInfo>
    {
        [SecurityCritical]
        internal override FileSystemInfo CreateObject(SearchResult result)
        {
            FileSystemEnumerableHelpers.IsFile(result.FindData);
            if (FileSystemEnumerableHelpers.IsDir(result.FindData))
            {
                var str = result.FullPath;
                var info = new DirectoryInfo(str);
                info.InitializeFrom(result.FindData);
                return info;
            }
            var fullPath = result.FullPath;
            var info2 = new FileInfo(fullPath);
            info2.InitializeFrom(result.FindData);
            return info2;
        }

        [SecurityCritical]
        internal override bool IsResultIncluded(SearchResult result)
        {
            var flag = FileSystemEnumerableHelpers.IsFile(result.FindData);
            if (!FileSystemEnumerableHelpers.IsDir(result.FindData))
                return flag;
            return true;
        }
    }
}
#endif