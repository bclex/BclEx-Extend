#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class RegistrationTrigger : Trigger, ITriggerDelay
    {
        public RegistrationTrigger()
            : base(TaskTriggerType.Registration) { }
        internal RegistrationTrigger(ITrigger iTrigger)
            : base(iTrigger) { }

        protected override string V2GetTriggerString()
        {
            return Local.TriggerRegistration1;
        }

        [DefaultValue(0)]
        public TimeSpan Delay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((IRegistrationTrigger)base._v2Trigger).Delay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("Delay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["Delay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IRegistrationTrigger)base._v2Trigger).Delay = Task.TimeSpanToString(value);
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

