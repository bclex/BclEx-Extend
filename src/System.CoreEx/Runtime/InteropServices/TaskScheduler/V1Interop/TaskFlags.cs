#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [Flags]
    internal enum TaskFlags
    {
        DeleteWhenDone = 2,
        Disabled = 4,
        DontStartIfOnBatteries = 0x40,
        Hidden = 0x200,
        Interactive = 1,
        KillIfGoingOnBatteries = 0x80,
        KillOnIdleEnd = 0x20,
        RestartOnIdleResume = 0x800,
        RunIfConnectedToInternet = 0x400,
        RunOnlyIfDocked = 0x100,
        RunOnlyIfLoggedOn = 0x2000,
        StartOnlyIfIdle = 0x10,
        SystemRequired = 0x1000
    }
}

