#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [ComImport, Guid("EAEC7A8F-27A0-4DDC-8675-14726A01A38A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
    public interface ITaskHandlerStatus
    {
        void UpdateStatus([In] short percentComplete, [In, MarshalAs(UnmanagedType.BStr)] string statusMessage);
        void TaskCompleted([In, MarshalAs(UnmanagedType.Error)] int taskErrCode);
    }
}

