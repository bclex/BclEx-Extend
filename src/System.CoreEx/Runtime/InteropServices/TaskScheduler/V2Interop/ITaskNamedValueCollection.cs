#region Foreign-License
// x
#endregion
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), Guid("B4EF826B-63C3-46E4-A504-EF69E4F7EA4D"), SuppressUnmanagedCodeSecurity]
    internal interface ITaskNamedValueCollection : IEnumerable
    {
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        ITaskNamedValuePair this[int index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4)]
        IEnumerator GetEnumerator();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        ITaskNamedValuePair Create([In, MarshalAs(UnmanagedType.BStr)] string Name, [In, MarshalAs(UnmanagedType.BStr)] string Value);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void Remove([In] int index);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void Clear();
    }
}

