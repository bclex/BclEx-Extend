#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [Flags]
    internal enum TaskTriggerFlags : uint
    {
        Disabled = 4,
        HasEndDate = 1,
        KillAtDurationEnd = 2
    }
}

