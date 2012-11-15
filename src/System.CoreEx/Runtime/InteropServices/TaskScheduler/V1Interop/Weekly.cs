#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Weekly
    {
        public ushort WeeksInterval;
        public DaysOfTheWeek DaysOfTheWeek;
    }
}

