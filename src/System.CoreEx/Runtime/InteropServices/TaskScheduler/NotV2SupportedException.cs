#region Foreign-License
// x
#endregion
using System.Diagnostics;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [DebuggerStepThrough]
    public class NotV2SupportedException : TSNotSupportedException
    {
        internal NotV2SupportedException() { }
        internal NotV2SupportedException(string message)
            : base(message) { }

        internal override string LibName
        {
            get { return "Task Scheduler 2.0 (1.2)"; }
        }
    }
}

