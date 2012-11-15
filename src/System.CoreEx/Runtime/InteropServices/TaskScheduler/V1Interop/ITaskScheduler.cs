#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("148BD527-A2AB-11CE-B11F-00AA00530503"), SuppressUnmanagedCodeSecurity]
    internal interface ITaskScheduler
    {
        void SetTargetComputer([In, MarshalAs(UnmanagedType.LPWStr)] string Computer);
        CoTaskMemString GetTargetComputer();
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumWorkItems Enum();
        [return: MarshalAs(UnmanagedType.Interface)]
        ITask Activate([In, MarshalAs(UnmanagedType.LPWStr)] string Name, [In] ref Guid riid);
        void Delete([In, MarshalAs(UnmanagedType.LPWStr)] string Name);
        [return: MarshalAs(UnmanagedType.Interface)]
        ITask NewWorkItem([In, MarshalAs(UnmanagedType.LPWStr)] string TaskName, [In] ref Guid rclsid, [In] ref Guid riid);
        void AddWorkItem([In, MarshalAs(UnmanagedType.LPWStr)] string TaskName, [In, MarshalAs(UnmanagedType.Interface)] ITask WorkItem);
        void IsOfType([In, MarshalAs(UnmanagedType.LPWStr)] string TaskName, [In] ref Guid riid);
    }
}

