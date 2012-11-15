#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), DefaultMember("Path"), ComConversionLoss, SuppressUnmanagedCodeSecurity, Guid("9C86F320-DEE3-4DD1-B972-A303F26B061E")]
    internal interface IRegisteredTask
    {
        [DispId(1)]
        string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        string Path { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [DispId(2)]
        TaskState State { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
        [DispId(3)]
        bool Enabled { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        IRunningTask Run([In, MarshalAs(UnmanagedType.Struct)] object parameters);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        IRunningTask RunEx([In, MarshalAs(UnmanagedType.Struct)] object parameters, [In] int flags, [In] int sessionID, [In, MarshalAs(UnmanagedType.BStr)] string user);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)]
        IRunningTaskCollection GetInstances(int flags);
        [DispId(8)]
        DateTime LastRunTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; }
        [DispId(9)]
        int LastTaskResult { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] get; }
        [DispId(11)]
        int NumberOfMissedRuns { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; }
        [DispId(12)]
        DateTime NextRunTime { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] get; }
        [DispId(13)]
        ITaskDefinition Definition { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] get; }
        [DispId(14)]
        string Xml { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] get; }
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)]
        string GetSecurityDescriptor(int securityInformation);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)]
        void SetSecurityDescriptor([In, MarshalAs(UnmanagedType.BStr)] string sddl, [In] int flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x11)]
        void Stop(int flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020011), TypeLibFunc((short)0x41)]
        void GetRunTimes([In] ref SystemTime pstStart, [In] ref SystemTime pstEnd, [In, Out] ref uint pCount, [In, Out] ref IntPtr pRunTimes);
    }
}

