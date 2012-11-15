#region Foreign-License
// x
#endregion
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("84594461-0053-4342-A8FD-088FABF11F32"), SuppressUnmanagedCodeSecurity, TypeLibType((short)0x10c0)]
    internal interface IIdleSettings
    {
        [DispId(1)]
        string IdleDuration { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        [DispId(2)]
        string WaitTimeout { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        [DispId(3)]
        bool StopOnIdleEnd { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
        [DispId(4)]
        bool RestartOnIdle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] set; }
    }
}

