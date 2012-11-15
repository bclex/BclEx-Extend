#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.ComponentModel;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class SessionStateChangeTrigger : Trigger, ITriggerDelay, ITriggerUserId
    {
        public SessionStateChangeTrigger()
            : base(TaskTriggerType.SessionStateChange) { }
        public SessionStateChangeTrigger(TaskSessionStateChangeType stateChange)
            : this()
        {
            StateChange = stateChange;
        }
        internal SessionStateChangeTrigger(ITrigger iTrigger)
            : base(iTrigger) { }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
                StateChange = ((SessionStateChangeTrigger)sourceTrigger).StateChange;
        }

        protected override string V2GetTriggerString()
        {
            var format = Local.ResourceManager.GetString("TriggerSession" + StateChange.ToString());
            var str2 = (string.IsNullOrEmpty(this.UserId) ? Local.TriggerAnyUser : UserId);
            if (StateChange != TaskSessionStateChangeType.SessionLock && StateChange != TaskSessionStateChangeType.SessionUnlock)
                str2 = string.Format(Local.TriggerSessionUserSession, str2);
            return string.Format(format, str2);
        }

        [DefaultValue(0)]
        public TimeSpan Delay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((ISessionStateChangeTrigger)base._v2Trigger).Delay);
                if (!base.unboundValues.ContainsKey("Delay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["Delay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ISessionStateChangeTrigger)base._v2Trigger).Delay = Task.TimeSpanToString(value);
                else
                    base.unboundValues["Delay"] = value;
            }
        }

        [DefaultValue(0)]
        public TaskSessionStateChangeType StateChange
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((ISessionStateChangeTrigger)base._v2Trigger).StateChange;
                if (!base.unboundValues.ContainsKey("StateChange"))
                    return TaskSessionStateChangeType.ConsoleConnect;
                return (TaskSessionStateChangeType)base.unboundValues["StateChange"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ISessionStateChangeTrigger)base._v2Trigger).StateChange = value;
                else
                    base.unboundValues["StateChange"] = value;
            }
        }

        [DefaultValue((string)null)]
        public string UserId
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((ISessionStateChangeTrigger)base._v2Trigger).UserId;
                if (!base.unboundValues.ContainsKey("UserId"))
                    return null;
                return (string)base.unboundValues["UserId"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((ISessionStateChangeTrigger)base._v2Trigger).UserId = value;
                else
                    base.unboundValues["UserId"] = value;
            }
        }
    }
}

