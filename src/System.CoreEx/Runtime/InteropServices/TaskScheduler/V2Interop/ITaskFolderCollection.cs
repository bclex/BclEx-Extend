#region Foreign-License
// x
#endregion
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("79184A66-8664-423F-97F1-637356A5D812"), SuppressUnmanagedCodeSecurity, TypeLibType((short)0x10c0)]
    internal interface ITaskFolderCollection : IEnumerable
    {
        [DispId(0x60020000)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020000)] get; }
        [DispId(0)]
        ITaskFolder this[object index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4)]
        IEnumerator GetEnumerator();
    }
}

