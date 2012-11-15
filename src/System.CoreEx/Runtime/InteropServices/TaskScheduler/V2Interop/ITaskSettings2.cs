#region Foreign-License
// x
#endregion
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), Guid("2C05C3F0-6EED-4c05-A15F-ED7D7A98A369"), SuppressUnmanagedCodeSecurity]
    internal interface ITaskSettings2
    {
        [DispId(30)]
        bool DisallowStartOnRemoteAppSession { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(30)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(30)] set; }
        [DispId(0x1f)]
        bool UseUnifiedSchedulingEngine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1f)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x1f)] set; }
    }
}

