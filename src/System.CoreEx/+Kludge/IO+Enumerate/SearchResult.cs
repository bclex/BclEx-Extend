#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Security;
using Microsoft.Win32;
namespace System.IO
{
    /// <summary>
    /// SearchResult
    /// </summary>
    internal sealed class SearchResult
    {
        [SecurityCritical]
        private Win32Native.WIN32_FIND_DATA findData;
        private string fullPath;
        private string userPath;

        [SecurityCritical]
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal SearchResult(string fullPath, string userPath, Win32Native.WIN32_FIND_DATA findData)
        {
            this.fullPath = fullPath;
            this.userPath = userPath;
            this.findData = findData;
        }

        internal Win32Native.WIN32_FIND_DATA FindData
        {
            [SecurityCritical]
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return findData; }
        }

        internal string FullPath
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return fullPath; }
        }

        internal string UserPath
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return userPath; }
        }
    }
}
#endif