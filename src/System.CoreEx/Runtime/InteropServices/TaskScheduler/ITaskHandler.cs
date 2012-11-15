#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [ComImport, Guid("839D7762-5121-4009-9234-4F0D19394F04"), SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITaskHandler
    {
        void Start([In, MarshalAs(UnmanagedType.IUnknown)] object pHandlerServices, [In, MarshalAs(UnmanagedType.BStr)] string Data);
        void Stop([MarshalAs(UnmanagedType.Error)] out int pRetCode);
        void Pause();
        void Resume();
    }
}

