#region Foreign-License
// x
#endregion
using System.Diagnostics;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [DebuggerStepThrough]
    public abstract class TSNotSupportedException : Exception
    {
        private string _myMessage;

        internal TSNotSupportedException()
        {
            var trace = new StackTrace();
            var method = trace.GetFrame(2).GetMethod();
            _myMessage = string.Format("{0}.{1} is not supported on {2}", method.DeclaringType.Name, method.Name, LibName);
        }

        internal TSNotSupportedException(string message)
        {
            _myMessage = message;
        }

        internal abstract string LibName { get; }

        public override string Message
        {
            get { return _myMessage; }
        }
    }
}

