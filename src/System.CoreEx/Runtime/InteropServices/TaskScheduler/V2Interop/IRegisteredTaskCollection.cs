#region Foreign-License
// x
#endregion
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("86627EB4-42A7-41E4-A4D9-AC33A72F2D52"), TypeLibType((short)0x10c0), SuppressUnmanagedCodeSecurity]
    internal interface IRegisteredTaskCollection : IEnumerable
    {
        [DispId(0x60020000)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020000)] get; }
        [DispId(0)]
        IRegisteredTask this[object index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4)]
        IEnumerator GetEnumerator();
    }
}

