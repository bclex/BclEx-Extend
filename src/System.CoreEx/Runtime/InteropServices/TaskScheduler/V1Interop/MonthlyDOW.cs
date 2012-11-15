#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonthlyDOW
    {
        public ushort WhichWeek;
        public DaysOfTheWeek DaysOfTheWeek;
        public MonthsOfTheYear Months;
        public WhichWeek V2WhichWeek
        {
            get { return (WhichWeek)((short)(((int)1) << (((short)WhichWeek) - 1))); }
            set
            {
                var index = Array.IndexOf<short>(new short[] { 1, 2, 4, 8, 0x10 }, (short)value);
                if (index < 0)
                    throw new NotV1SupportedException("Only a single week can be set with Task Scheduler 1.0.");
                WhichWeek = (ushort)(index + 1);
            }
        }
    }
}

