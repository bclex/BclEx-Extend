#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TaskTrigger
    {
        public ushort TriggerSize;
        public ushort Reserved1;
        public ushort BeginYear;
        public ushort BeginMonth;
        public ushort BeginDay;
        public ushort EndYear;
        public ushort EndMonth;
        public ushort EndDay;
        public ushort StartHour;
        public ushort StartMinute;
        public uint MinutesDuration;
        public uint MinutesInterval;
        public TaskTriggerFlags Flags;
        public TaskTriggerType Type;
        public TriggerTypeData Data;
        public ushort Reserved2;
        public ushort RandomMinutesInterval;
        public DateTime BeginDate
        {
            get { return new DateTime(BeginYear, BeginMonth, BeginDay, StartHour, StartMinute, 0); }
            set
            {
                if (value != DateTime.MinValue)
                {
                    BeginYear = (ushort)value.Year;
                    BeginMonth = (ushort)value.Month;
                    BeginDay = (ushort)value.Day;
                    StartHour = (ushort)value.Hour;
                    StartMinute = (ushort)value.Minute;
                }
                else
                    BeginYear = BeginMonth = BeginDay = StartHour = (ushort)(StartMinute = 0);
            }
        }
        public DateTime EndDate
        {
            get { return (EndYear != 0 ? new DateTime(this.EndYear, this.EndMonth, this.EndDay) : DateTime.MaxValue); }
            set
            {
                if (value != DateTime.MaxValue)
                {
                    EndYear = (ushort)value.Year;
                    EndMonth = (ushort)value.Month;
                    EndDay = (ushort)value.Day;
                    Flags |= TaskTriggerFlags.HasEndDate;
                }
                else
                {
                    EndYear = EndMonth = (ushort)(EndDay = 0);
                    Flags &= ~TaskTriggerFlags.HasEndDate;
                }
            }
        }

        public override string ToString() { return string.Format("Trigger Type: {6};\n> Start: {0}; End: {1};\n> DurMin: {4}; DurItv: {5};\n>", new [] { BeginDate, (EndYear == 0 ? "null" : EndDate.ToString()), Data, Flags, MinutesDuration, MinutesInterval, Type }); }
    }
}

