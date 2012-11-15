#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.ComponentModel;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class MonthlyDOWTrigger : Trigger, ITriggerDelay
    {
        internal MonthlyDOWTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.RunMonthlyDOW) { }
        internal MonthlyDOWTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public MonthlyDOWTrigger(DaysOfTheWeek daysOfWeek = (DaysOfTheWeek)1, MonthsOfTheYear monthsOfYear = (MonthsOfTheYear)0xfff, WhichWeek weeksOfMonth = (WhichWeek)1)
            : base(TaskTriggerType.MonthlyDOW)
        {
            DaysOfWeek = daysOfWeek;
            MonthsOfYear = monthsOfYear;
            WeeksOfMonth = weeksOfMonth;
        }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
            {
                DaysOfWeek = ((MonthlyDOWTrigger)sourceTrigger).DaysOfWeek;
                MonthsOfYear = ((MonthlyDOWTrigger)sourceTrigger).MonthsOfYear;
                try { RunOnLastWeekOfMonth = ((MonthlyDOWTrigger)sourceTrigger).RunOnLastWeekOfMonth; }
                catch { }
                WeeksOfMonth = ((MonthlyDOWTrigger)sourceTrigger).WeeksOfMonth;
            }
        }

        protected override string V2GetTriggerString()
        {
            var str = Trigger.BuildEnumString("WW", WeeksOfMonth);
            var str2 = (DaysOfWeek == (DaysOfTheWeek.Friday | DaysOfTheWeek.Thursday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Sunday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Monday) ? Local.DOWAllDays : Trigger.BuildEnumString("DOW", DaysOfWeek));
            var str3 = (MonthsOfYear == (MonthsOfTheYear.September | MonthsOfTheYear.August | MonthsOfTheYear.July | MonthsOfTheYear.October | MonthsOfTheYear.December | MonthsOfTheYear.November | MonthsOfTheYear.February | MonthsOfTheYear.January | MonthsOfTheYear.March | MonthsOfTheYear.June | MonthsOfTheYear.May | MonthsOfTheYear.April) ? Local.MOYAllMonths : Trigger.BuildEnumString("MOY", MonthsOfYear));
            return string.Format("At {0:t} on the {1} {2:f} each {3}, starting {0:d}", new object[] { base.StartBoundary, str, str2, str3 });
        }

        [DefaultValue(0)]
        public DaysOfTheWeek DaysOfWeek
        {
            get
            {
                if (base._v2Trigger != null)
                    return (DaysOfTheWeek)((IMonthlyDOWTrigger)base._v2Trigger).DaysOfWeek;
                return _v1TriggerData.Data.monthlyDOW.DaysOfTheWeek;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyDOWTrigger)base._v2Trigger).DaysOfWeek = (short)value;
                else
                {
                    _v1TriggerData.Data.monthlyDOW.DaysOfTheWeek = value;
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
        public MonthsOfTheYear MonthsOfYear
        {
            get
            {
                if (base._v2Trigger != null)
                    return (MonthsOfTheYear)((IMonthlyDOWTrigger)base._v2Trigger).MonthsOfYear;
                return _v1TriggerData.Data.monthlyDOW.Months;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyDOWTrigger)base._v2Trigger).MonthsOfYear = (short)value;
                else
                {
                    _v1TriggerData.Data.monthlyDOW.Months = value;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["MonthsOfYear"] = (short)value;
                }
            }
        }

        [DefaultValue(0)]
        public TimeSpan RandomDelay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((IMonthlyDOWTrigger)base._v2Trigger).RandomDelay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RandomDelay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["RandomDelay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyDOWTrigger)base._v2Trigger).RandomDelay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RandomDelay"] = value;
                }
            }
        }

        [DefaultValue(false)]
        public bool RunOnLastWeekOfMonth
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((IMonthlyDOWTrigger)base._v2Trigger).RunOnLastWeekOfMonth;
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RunOnLastWeekOfMonth"))
                    return false;
                return (bool)base.unboundValues["RunOnLastWeekOfMonth"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyDOWTrigger)base._v2Trigger).RunOnLastWeekOfMonth = value;
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RunOnLastWeekOfMonth"] = value;
                }
            }
        }

        [DefaultValue(0)]
        public WhichWeek WeeksOfMonth
        {
            get
            {
                if (base._v2Trigger != null)
                {
                    var weeksOfMonth = (WhichWeek)((IMonthlyDOWTrigger)base._v2Trigger).WeeksOfMonth;
                    if (((IMonthlyDOWTrigger)base._v2Trigger).RunOnLastWeekOfMonth)
                        weeksOfMonth = (WhichWeek)((short)(weeksOfMonth | WhichWeek.LastWeek));
                    return weeksOfMonth;
                }
                if (base._v1Trigger != null)
                    return _v1TriggerData.Data.monthlyDOW.V2WhichWeek;
                if (!base.unboundValues.ContainsKey("WeeksOfMonth"))
                    return WhichWeek.FirstWeek;
                return (WhichWeek)base.unboundValues["WeeksOfMonth"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyDOWTrigger)base._v2Trigger).WeeksOfMonth = (short)value;
                else
                {
                    _v1TriggerData.Data.monthlyDOW.V2WhichWeek = value;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["WeeksOfMonth"] = (short)value;
                }
            }
        }
    }
}

