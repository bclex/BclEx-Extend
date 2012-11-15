#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler
{
    [Flags]
    public enum MonthsOfTheYear : short
    {
        AllMonths = 0xfff,
        April = 8,
        August = 0x80,
        December = 0x800,
        February = 2,
        January = 1,
        July = 0x40,
        June = 0x20,
        March = 4,
        May = 0x10,
        November = 0x400,
        October = 0x200,
        September = 0x100
    }
}

