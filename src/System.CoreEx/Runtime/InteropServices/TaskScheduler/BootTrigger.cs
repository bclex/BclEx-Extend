#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class BootTrigger : Trigger, ITriggerDelay
    {
        public BootTrigger()
            : base(TaskTriggerType.Boot) { }
        internal BootTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.OnSystemStart) { }
        internal BootTrigger(ITrigger iTrigger)
            : base(iTrigger) { }

        protected override string V2GetTriggerString() { return Local.TriggerBoot1; }

        [DefaultValue(0)]
        public TimeSpan Delay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((IBootTrigger)base._v2Trigger).Delay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("Delay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["Delay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IBootTrigger)base._v2Trigger).Delay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["Delay"] = value;
                }
            }
        }
    }
}

