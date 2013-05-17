#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Reflection;
using System.Security;
using Microsoft.Win32;
namespace System.IO
{
    /// <summary>
    /// KludgeExtensions
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static partial class KludgeExtensions
    {
        private static readonly FieldInfo _dataInfo = typeof(FileSystemInfo).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo _dataInitialisedInfo = typeof(FileSystemInfo).GetField("_dataInitialised", BindingFlags.NonPublic | BindingFlags.Instance);

        [SecurityCritical]
        internal static void InitializeFrom(this FileSystemInfo source, Win32Native.WIN32_FIND_DATA findData)
        {
            var data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            data.PopulateFrom(findData);
            _dataInfo.SetValue(source, data.ToBase());
            _dataInitialisedInfo.SetValue(source, 0);
        }
    }
}
#endif