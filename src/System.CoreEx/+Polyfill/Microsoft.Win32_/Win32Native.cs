#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
namespace Microsoft.Win32
{
    /// <summary>
    /// Win32Native
    /// </summary>
    internal static partial class Win32Native
    {
        /// <summary>
        /// WIN32_FIND_DATA
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), BestFitMapping(false)]
        internal class WIN32_FIND_DATA
        {
            internal int dwFileAttributes;
            internal uint ftCreationTime_dwLowDateTime;
            internal uint ftCreationTime_dwHighDateTime;
            internal uint ftLastAccessTime_dwLowDateTime;
            internal uint ftLastAccessTime_dwHighDateTime;
            internal uint ftLastWriteTime_dwLowDateTime;
            internal uint ftLastWriteTime_dwHighDateTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternateFileName;
        }

        [DllImport("kernel32.dll")]
        internal static extern int SetErrorMode(int newMode);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll")]
        internal static extern bool FindClose(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFindHandle FindFirstFile(string fileName, [In, Out] WIN32_FIND_DATA data);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool FindNextFile(SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);

        /// <summary>
        /// WIN32_FILE_ATTRIBUTE_DATA
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        internal struct WIN32_FILE_ATTRIBUTE_DATA
        {
            private static readonly Type _baseType = Type.GetType("Microsoft.Win32.Win32Native+WIN32_FILE_ATTRIBUTE_DATA");

            internal int fileAttributes;
            internal uint ftCreationTimeLow;
            internal uint ftCreationTimeHigh;
            internal uint ftLastAccessTimeLow;
            internal uint ftLastAccessTimeHigh;
            internal uint ftLastWriteTimeLow;
            internal uint ftLastWriteTimeHigh;
            internal int fileSizeHigh;
            internal int fileSizeLow;

            [SecurityCritical]
            internal void PopulateFrom(Win32Native.WIN32_FIND_DATA findData)
            {
                this.fileAttributes = findData.dwFileAttributes;
                this.ftCreationTimeLow = findData.ftCreationTime_dwLowDateTime;
                this.ftCreationTimeHigh = findData.ftCreationTime_dwHighDateTime;
                this.ftLastAccessTimeLow = findData.ftLastAccessTime_dwLowDateTime;
                this.ftLastAccessTimeHigh = findData.ftLastAccessTime_dwHighDateTime;
                this.ftLastWriteTimeLow = findData.ftLastWriteTime_dwLowDateTime;
                this.ftLastWriteTimeHigh = findData.ftLastWriteTime_dwHighDateTime;
                this.fileSizeHigh = findData.nFileSizeHigh;
                this.fileSizeLow = findData.nFileSizeLow;
            }

            internal object ToBase()
            {
                var handle = GCHandle.Alloc(this, GCHandleType.Pinned);
                var r = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), _baseType);
                handle.Free();
                return r;
            }
        }
    }
}
#endif