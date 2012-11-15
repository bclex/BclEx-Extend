#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.ComponentModel;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class LogonTrigger : Trigger, ITriggerDelay, ITriggerUserId
    {
        public LogonTrigger()
            : base(TaskTriggerType.Logon) { }
        internal LogonTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.OnLogon) { }
        internal LogonTrigger(ITrigger iTrigger)
            : base(iTrigger) { }

        protected override string V2GetTriggerString()
        {
            var str = (string.IsNullOrEmpty(UserId) ? Local.TriggerAnyUser : UserId);
            return string.Format(Local.TriggerLogon1, str);
        }

        [DefaultValue(0)]
        public TimeSpan Delay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((ILogonTrigger)base._v2Trigger).Delay);
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("Delay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["Delay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ILogonTrigger)base._v2Trigger).Delay = Task.TimeSpanToString(value);
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["Delay"] = value;
                }
            }
        }

        [DefaultValue((string)null)]
        public string UserId
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((ILogonTrigger)base._v2Trigger).UserId;
                if (base._v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!base.unboundValues.ContainsKey("UserId"))
                    return null;
                return (string)base.unboundValues["UserId"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ILogonTrigger)base._v2Trigger).UserId = value;
                else
                {
                    if (base._v1Trigger != null)
                        throw new NotV1SupportedException();
                    base.unboundValues["UserId"] = value;
                }
            }
        }
    }
}

