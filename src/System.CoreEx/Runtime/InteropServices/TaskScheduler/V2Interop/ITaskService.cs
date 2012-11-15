#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), DefaultMember("TargetServer"), Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85"), SuppressUnmanagedCodeSecurity]
    internal interface ITaskService
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        ITaskFolder GetFolder([In, MarshalAs(UnmanagedType.BStr)] string Path);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        IRunningTaskCollection GetRunningTasks(int flags);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        ITaskDefinition NewTask([In] uint flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void Connect([In, Optional, MarshalAs(UnmanagedType.Struct)] object serverName, [In, Optional, MarshalAs(UnmanagedType.Struct)] object user, [In, Optional, MarshalAs(UnmanagedType.Struct)] object domain, [In, Optional, MarshalAs(UnmanagedType.Struct)] object password);
        [DispId(5)]
        bool Connected { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; }
        [DispId(0)]
        string TargetServer { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [DispId(6)]
        string ConnectedUser { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; }
        [DispId(7)]
        string ConnectedDomain { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; }
        [DispId(8)]
        uint HighestVersion { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; }
    }
}

