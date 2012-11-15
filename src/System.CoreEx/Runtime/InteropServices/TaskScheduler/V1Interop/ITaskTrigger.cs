#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("148BD52B-A2AB-11CE-B11F-00AA00530503")]
    internal interface ITaskTrigger
    {
        void SetTrigger([In, Out, MarshalAs(UnmanagedType.Struct)] ref TaskTrigger Trigger);
        [return: MarshalAs(UnmanagedType.Struct)]
        TaskTrigger GetTrigger();
        CoTaskMemString GetTriggerString();
    }
}

