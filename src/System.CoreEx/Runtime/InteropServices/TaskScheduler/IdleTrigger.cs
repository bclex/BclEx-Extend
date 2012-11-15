#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class IdleTrigger : Trigger
    {
        public IdleTrigger()
            : base(TaskTriggerType.Idle) { }
        internal IdleTrigger(ITaskTrigger iTrigger)
            : base(iTrigger, TaskTriggerType.OnIdle) { }
        internal IdleTrigger(ITrigger iTrigger)
            : base(iTrigger) { }

        protected override string V2GetTriggerString()
        {
            return Local.TriggerIdle1;
        }
    }
}

