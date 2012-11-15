#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class RunningTask : Task
    {
        private IRunningTask _v2RunningTask;

        internal RunningTask(TaskService svc, ITask iTask)
            : base(svc, iTask) { }
        internal RunningTask(TaskService svc, IRegisteredTask iTask, IRunningTask iRunningTask)
            : base(svc, iTask)
        {
            _v2RunningTask = iRunningTask;
        }

        public void Dispose()
        {
            base.Dispose();
            if (this._v2RunningTask != null)
                Marshal.ReleaseComObject(_v2RunningTask);
        }

        public void Refresh()
        {
            if (_v2RunningTask != null)
                _v2RunningTask.Refresh();
        }

        public string CurrentAction
        {
            get
            {
                if (_v2RunningTask != null)
                    return _v2RunningTask.CurrentAction;
                return (string)base._v1Task.GetApplicationName();
            }
        }

        public uint EnginePID
        {
            get
            {
                if (_v2RunningTask == null)
                    throw new NotV1SupportedException();
                return _v2RunningTask.EnginePID;
            }
        }

        public Guid InstanceGuid
        {
            get
            {
                if (_v2RunningTask != null)
                    return new Guid(_v2RunningTask.InstanceGuid);
                return Guid.Empty;
            }
        }
    }
}

