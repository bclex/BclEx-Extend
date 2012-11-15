#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct SystemTime
    {
        public ushort Year;
        public ushort Month;
        public ushort DayOfWeek;
        public ushort Day;
        public ushort Hour;
        public ushort Minute;
        public ushort Second;
        public ushort Milliseconds;

        public SystemTime(DateTime dt)
        {
            dt = dt.ToLocalTime();
            Year = Convert.ToUInt16(dt.Year);
            Month = Convert.ToUInt16(dt.Month);
            DayOfWeek = Convert.ToUInt16(dt.DayOfWeek);
            Day = Convert.ToUInt16(dt.Day);
            Hour = Convert.ToUInt16(dt.Hour);
            Minute = Convert.ToUInt16(dt.Minute);
            Second = Convert.ToUInt16(dt.Second);
            Milliseconds = Convert.ToUInt16(dt.Millisecond);
        }

        public static implicit operator DateTime(SystemTime st) { return (st.Year == 0 ? DateTime.MinValue : new DateTime(st.Year, st.Month, st.Day, st.Hour, st.Minute, st.Second, st.Milliseconds, DateTimeKind.Local)); }
    }
}

