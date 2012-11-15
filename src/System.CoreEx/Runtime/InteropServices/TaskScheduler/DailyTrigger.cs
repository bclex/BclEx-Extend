#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class DailyTrigger : Trigger, ITriggerDelay
    {
        internal DailyTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.RunDaily) { }
        internal DailyTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public DailyTrigger(short daysInterval = 1)
            : base(TaskTriggerType.Daily)
        {
            DaysInterval = daysInterval;
        }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
                DaysInterval = ((DailyTrigger)sourceTrigger).DaysInterval;
        }

        protected override string V2GetTriggerString()
        {
            if (DaysInterval == 1)
                return string.Format(Local.TriggerDaily1, base.StartBoundary);
            return string.Format(Local.TriggerDaily2, base.StartBoundary, DaysInterval);
        }

        [DefaultValue(1)]
        public short DaysInterval
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((IDailyTrigger)base._v2Trigger).DaysInterval;
                return (short)_v1TriggerData.Data.daily.DaysInterval;
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IDailyTrigger)base._v2Trigger).DaysInterval = value;
                else
                {
                    _v1TriggerData.Data.daily.DaysInterval = (ushort)value;
                    if (base._v1Trigger != null)
                        base.SetV1TriggerData();
                    else
                        base.unboundValues["DaysInterval"] = value;
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
                    return Task.StringToTimeSpan(((IDailyTrigger)base._v2Trigger).RandomDelay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RandomDelay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["RandomDelay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IDailyTrigger)base._v2Trigger).RandomDelay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["RandomDelay"] = value;
                }
            }
        }
    }
}

