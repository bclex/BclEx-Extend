#region Foreign-License
// x
#endregion
using System.Security;
using System.Runtime.CompilerServices;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), SuppressUnmanagedCodeSecurity, Guid("BAE54997-48B1-4CBE-9965-D6BE263EBEA4")]
    internal interface IAction
    {
        [DispId(1)]
        string Id { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        [DispId(2)]
        TaskActionType Type { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
    }
}

