#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class WeeklyTrigger : Trigger, ITriggerDelay
    {
        internal WeeklyTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, V1Interop.TaskTriggerType.RunWeekly) { }
        internal WeeklyTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public WeeklyTrigger(DaysOfTheWeek daysOfWeek = (DaysOfTheWeek)1, short weeksInterval = 1)
            : base(TaskTriggerType.Weekly)
        {
            DaysOfWeek = daysOfWeek;
            WeeksInterval = weeksInterval;
        }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
            {
                DaysOfWeek = ((WeeklyTrigger)sourceTrigger).DaysOfWeek;
                WeeksInterval = ((WeeklyTrigger)sourceTrigger).WeeksInterval;
            }
        }

        protected override string V2GetTriggerString()
        {
            var str = (DaysOfWeek == (DaysOfTheWeek.Friday | DaysOfTheWeek.Thursday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Sunday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Monday) ? Local.DOWAllDays : Trigger.BuildEnumString("DOW", DaysOfWeek));
            return string.Format(WeeksInterval == 1 ? Local.TriggerWeekly1Week : Local.TriggerWeeklyMultWeeks, base.StartBoundary, str, WeeksInterval);
        }

        [DefaultValue(0)]
        public DaysOfTheWeek DaysOfWeek
        {
            get
            {
                if (base._v2Trigger != null)
                    return (DaysOfTheWeek)((IWeeklyTrigger)base._v2Trigger).DaysOfWeek;
                return _v1TriggerData.Data.weekly.DaysOfTheWeek;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IWeeklyTrigger)base._v2Trigger).DaysOfWeek = (short)value;
                else
                {
                    _v1TriggerData.Data.weekly.DaysOfTheWeek = value;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["DaysOfWeek"] = (short)value;
                }
            }
        }

        TimeSpan ITriggerDelay.Delay
        {
            get { return RandomDelay; }
            set { RandomDelay = value; }
        }

        [DefaultValue(0)]
        public TimeSpan RandomDelay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((IWeeklyTrigger)base._v2Trigger).RandomDelay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RandomDelay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["RandomDelay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IWeeklyTrigger)base._v2Trigger).RandomDelay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RandomDelay"] = value;
                }
            }
        }

        [DefaultValue(0)]
        public short WeeksInterval
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((IWeeklyTrigger)base._v2Trigger).WeeksInterval;
                return (short)_v1TriggerData.Data.weekly.WeeksInterval;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IWeeklyTrigger)base._v2Trigger).WeeksInterval = value;
                else
                {
                    _v1TriggerData.Data.weekly.WeeksInterval = (ushort)value;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["WeeksInterval"] = value;
                }
            }
        }
    }
}

