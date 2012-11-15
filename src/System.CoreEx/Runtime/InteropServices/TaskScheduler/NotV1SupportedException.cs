#region Foreign-License
// x
#endregion
using System.Diagnostics;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [DebuggerStepThrough]
    public class NotV1SupportedException : TSNotSupportedException
    {
        internal NotV1SupportedException() { }
        internal NotV1SupportedException(string message)
            : base(message) { }

        internal override string LibName
        {
            get { return "Task Scheduler 1.0"; }
        }
    }
}

