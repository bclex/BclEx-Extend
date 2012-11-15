#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TimeTrigger : Trigger, ITriggerDelay
    {
        public TimeTrigger()
            : base(TaskTriggerType.Time) { }
        internal TimeTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.RunOnce) { }
        internal TimeTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public TimeTrigger(DateTime startBoundary)
            : base(TaskTriggerType.Time)
        {
            base.StartBoundary = startBoundary;
        }

        protected override string V2GetTriggerString()
        {
            return string.Format(Local.TriggerTime1, base.StartBoundary);
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
                    return Task.StringToTimeSpan(((ITimeTrigger)base._v2Trigger).RandomDelay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("RandomDelay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["RandomDelay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ITimeTrigger)base._v2Trigger).RandomDelay = Task.TimeSpanToString(value);
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

