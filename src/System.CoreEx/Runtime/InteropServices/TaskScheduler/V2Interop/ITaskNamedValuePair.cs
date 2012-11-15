#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("39038068-2B46-4AFD-8662-7BB6F868D221"), DefaultMember("Name"), TypeLibType((short)0x10c0), SuppressUnmanagedCodeSecurity]
    internal interface ITaskNamedValuePair
    {
        [DispId(0)]
        string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] set; }
        [DispId(1)]
        string Value { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
    }
}

