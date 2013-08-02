#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Runtime;
using System.Security;
namespace System.Threading
{
    internal class CancellationCallbackInfo
    {
        internal readonly Action<object> Callback;
        internal readonly CancellationTokenSource CancellationTokenSource;
        [SecurityCritical]
        private static ContextCallback s_executionContextCallback;
        internal readonly object StateForCallback;
        internal readonly ExecutionContext TargetExecutionContext;
        internal readonly SynchronizationContext TargetSyncContext;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal CancellationCallbackInfo(Action<object> callback, object stateForCallback, SynchronizationContext targetSyncContext, ExecutionContext targetExecutionContext, CancellationTokenSource cancellationTokenSource)
        {
            Callback = callback;
            StateForCallback = stateForCallback;
            TargetSyncContext = targetSyncContext;
            TargetExecutionContext = targetExecutionContext;
            CancellationTokenSource = cancellationTokenSource;
        }

        [SecuritySafeCritical]
        internal void ExecuteCallback()
        {
            if (TargetExecutionContext != null)
            {
                var callback = s_executionContextCallback;
                if (callback == null)
                    s_executionContextCallback = callback = new ContextCallback(CancellationCallbackInfo.ExecutionContextCallback);
                ExecutionContext.Run(this.TargetExecutionContext, callback, this);
            }
            else
                ExecutionContextCallback(this);
        }

        [SecurityCritical]
        private static void ExecutionContextCallback(object obj)
        {
            var info = obj as CancellationCallbackInfo;
            info.Callback(info.StateForCallback);
        }
    }
}
#endif