#region Foreign-License
// .Net40 Surface
#endregion
using System.Runtime;
using System.Threading;

namespace System.Net.Mime
{
    internal class MultiAsyncResult : LazyAsyncResult
    {
        private object _context;
        private int _outstanding;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal MultiAsyncResult(object context, AsyncCallback callback, object state)
            : base(context, state, callback)
        {
            _context = context;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal void CompleteSequence()
        {
            Decrement();
        }

        private void Decrement()
        {
            if (Interlocked.Decrement(ref _outstanding) == -1)
                base.InvokeCallback(base.Result);
        }

        internal static object End(IAsyncResult result)
        {
            var result2 = (MultiAsyncResult)result;
            result2.InternalWaitForCompletion();
            return result2.Result;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal void Enter()
        {
            Increment();
        }

        private void Increment()
        {
            Interlocked.Increment(ref _outstanding);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal void Leave()
        {
            Decrement();
        }

        internal void Leave(object result)
        {
            base.Result = result;
            Decrement();
        }

        internal object Context
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _context; }
        }
    }
}

