#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, DefaultMember("TargetServer"), TypeLibType((short)2), Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD"), ClassInterface((short)0), SuppressUnmanagedCodeSecurity]
    internal class TaskSchedulerClass : TaskScheduler, ITaskService
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        public virtual extern void Connect([In, Optional, MarshalAs(UnmanagedType.Struct)] object serverName, [In, Optional, MarshalAs(UnmanagedType.Struct)] object user, [In, Optional, MarshalAs(UnmanagedType.Struct)] object domain, [In, Optional, MarshalAs(UnmanagedType.Struct)] object password);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        public virtual extern ITaskFolder GetFolder([In, MarshalAs(UnmanagedType.BStr)] string Path);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        public virtual extern IRunningTaskCollection GetRunningTasks(int flags);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        public virtual extern ITaskDefinition NewTask([In] uint flags);

        [DispId(5)]
        public virtual bool Connected { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; }

        [DispId(7)]
        public virtual string ConnectedDomain { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; }

        [DispId(6)]
        public virtual string ConnectedUser { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; }

        [DispId(8)]
        public virtual uint HighestVersion { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; }

        [DispId(0)]
        public virtual string TargetServer { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
    }
}

