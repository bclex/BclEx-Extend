#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Security;
namespace System.IO
{
    /// <summary>
    /// StringResultHandler
    /// </summary>
    internal class StringResultHandler : SearchResultHandler<string>
    {
        private bool _includeDirs;
        private bool _includeFiles;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal StringResultHandler(bool includeFiles, bool includeDirs)
        {
            _includeFiles = includeFiles;
            _includeDirs = includeDirs;
        }

        [SecurityCritical]
        internal override string CreateObject(SearchResult result) { return result.UserPath; }

        [SecurityCritical]
        internal override bool IsResultIncluded(SearchResult result)
        {
            var flag = _includeFiles && FileSystemEnumerableHelpers.IsFile(result.FindData);
            var flag2 = _includeDirs && FileSystemEnumerableHelpers.IsDir(result.FindData);
            if (!flag)
                return flag2;
            return true;
        }
    }
}
#endif