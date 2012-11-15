#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    internal enum TaskStatus
    {
        Disabled = 0x41302,
        NeverRun = 0x41303,
        NoMoreRuns = 0x41304,
        NoTriggers = 0x41307,
        NoTriggerTime = 0x41308,
        NotScheduled = 0x41305,
        Ready = 0x41300,
        Running = 0x41301,
        Terminated = 0x41306
    }
}

