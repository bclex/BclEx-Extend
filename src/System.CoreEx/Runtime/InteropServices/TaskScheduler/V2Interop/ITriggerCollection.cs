#region Foreign-License
// x
#endregion
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, SuppressUnmanagedCodeSecurity, TypeLibType((short)0x10c0), Guid("85DF5081-1B24-4F32-878A-D9D14DF4CB77")]
    internal interface ITriggerCollection : IEnumerable
    {
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        ITrigger this[int index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4)]
        IEnumerator GetEnumerator();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        ITrigger Create([In] TaskTriggerType Type);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void Remove([In, MarshalAs(UnmanagedType.Struct)] object index);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void Clear();
    }
}

