#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, SuppressUnmanagedCodeSecurity, Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85"), CoClass(typeof(TaskSchedulerClass))]
    internal interface TaskScheduler : ITaskService { }
}

