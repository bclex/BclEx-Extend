#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, TypeLibType((short)0x10c0), DefaultMember("InstanceGuid"), Guid("653758FB-7B9A-4F1E-A471-BEEB8E9B834E"), SuppressUnmanagedCodeSecurity]
    internal interface IRunningTask
    {
        [DispId(1)]
        string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        string InstanceGuid { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [DispId(2)]
        string Path { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
        [DispId(3)]
        TaskState State { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; }
        [DispId(4)]
        string CurrentAction { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void Stop();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        void Refresh();
        [DispId(7)]
        uint EnginePID { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; }
    }
}

