#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler
{
    [Flags]
    public enum DaysOfTheWeek : short
    {
        AllDays = 0x7f,
        Friday = 0x20,
        Monday = 2,
        Saturday = 0x40,
        Sunday = 1,
        Thursday = 0x10,
        Tuesday = 4,
        Wednesday = 8
    }
}

