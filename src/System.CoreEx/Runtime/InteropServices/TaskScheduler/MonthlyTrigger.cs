#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class MonthlyTrigger : Trigger, ITriggerDelay
    {
        internal MonthlyTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.RunMonthly) { }
        internal MonthlyTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public MonthlyTrigger(int dayOfMonth = 1, MonthsOfTheYear monthsOfYear = (MonthsOfTheYear)0xfff)
            : base(TaskTriggerType.Monthly)
        {
            DaysOfMonth = new int[] { dayOfMonth };
            MonthsOfYear = monthsOfYear;
        }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
            {
                DaysOfMonth = ((MonthlyTrigger)sourceTrigger).DaysOfMonth;
                MonthsOfYear = ((MonthlyTrigger)sourceTrigger).MonthsOfYear;
                try { RunOnLastDayOfMonth = ((MonthlyTrigger)sourceTrigger).RunOnLastDayOfMonth; }
                catch { }
            }
        }

        internal static int IndicesToMask(int[] indices)
        {
            var num = 0;
            foreach (var num2 in indices)
            {
                if (num2 < 1 || num2 > 0x1f)
                    throw new ArgumentException("Days must be in the range 1..31");
                num |= ((int)1) << (num2 - 1);
            }
            return num;
        }

        internal static int[] MaskToIndices(int mask)
        {
            var num = 0;
            for (var i = 0; (mask >> i) > 0; i++)
                num += 1 & (mask >> i);
            var numArray = new int[num];
            num = 0;
            for (var j = 0; (mask >> j) > 0; j++)
                if ((1 & (mask >> j)) == 1)
                    numArray[num++] = j + 1;
            return numArray;
        }

        protected override string V2GetTriggerString()
        {
            var str = string.Join(", ", Array.ConvertAll<int, string>(DaysOfMonth, i => i.ToString()));
            var str2 = (MonthsOfYear == (MonthsOfTheYear.September | MonthsOfTheYear.August | MonthsOfTheYear.July | MonthsOfTheYear.October | MonthsOfTheYear.December | MonthsOfTheYear.November | MonthsOfTheYear.February | MonthsOfTheYear.January | MonthsOfTheYear.March | MonthsOfTheYear.June | MonthsOfTheYear.May | MonthsOfTheYear.April) ? Local.MOYAllMonths : Trigger.BuildEnumString("MOY", MonthsOfYear));
            return string.Format(Local.TriggerMonthly1, base.StartBoundary, str, str2);
        }

        public int[] DaysOfMonth
        {
            get
            {
                if (base._v2Trigger != null)
                    return MaskToIndices(((IMonthlyTrigger)base._v2Trigger).DaysOfMonth);
                return MaskToIndices((int)_v1TriggerData.Data.monthlyDate.Days);
            }
            set
            {
                var num = IndicesToMask(value);
                if (base._v2Trigger != null)
                    ((IMonthlyTrigger)base._v2Trigger).DaysOfMonth = num;
                else
                {
                    _v1TriggerData.Data.monthlyDate.Days = (uint)num;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["DaysOfMonth"] = num;
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
                    return (MonthsOfTheYear)((IMonthlyTrigger)base._v2Trigger).MonthsOfYear;
                return _v1TriggerData.Data.monthlyDOW.Months;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyTrigger)base._v2Trigger).MonthsOfYear = (short)value;
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
                    return Task.StringToTimeSpan(((IMonthlyTrigger)base._v2Trigger).RandomDelay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RandomDelay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["RandomDelay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyTrigger)base._v2Trigger).RandomDelay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RandomDelay"] = value;
                }
            }
        }

        [DefaultValue(false)]
        public bool RunOnLastDayOfMonth
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((IMonthlyTrigger)base._v2Trigger).RunOnLastDayOfMonth;
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RunOnLastDayOfMonth"))
                    return false;
                return (bool)base.unboundValues["RunOnLastDayOfMonth"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IMonthlyTrigger)base._v2Trigger).RunOnLastDayOfMonth = value;
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RunOnLastDayOfMonth"] = value;
                }
            }
        }
    }
}

