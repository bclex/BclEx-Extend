#region Foreign-License
// x
#endregion
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("248919AE-E345-4A6D-8AEB-E0D3165C904E"), TypeLibType((short)0x10c0), SuppressUnmanagedCodeSecurity]
    internal interface IPrincipal2
    {
        [DispId(7)]
        TaskProcessTokenSidType ProcessTokenSidType { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] set; }
        [DispId(8)]
        long RequiredPrivilegeCount { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)]
        string GetRequiredPrivilege(long index);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)]
        void AddRequiredPrivilege([In, MarshalAs(UnmanagedType.BStr)] string privilege);
    }
}

