#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
namespace System.Threading
{
    /// <summary>
    /// CancellationToken
    /// </summary>
    [StructLayout(LayoutKind.Sequential), DebuggerDisplay("IsCancellationRequested = {IsCancellationRequested}"), ComVisible(false), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public struct CancellationToken
    {
        private CancellationTokenSource _source;
        private static readonly Action<object> _actionToActionObjShunt;

        static CancellationToken()
        {
            _actionToActionObjShunt = new Action<object>(CancellationToken.ActionToActionObjShunt);
        }
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        internal CancellationToken(CancellationTokenSource source)
        {
            _source = source;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationToken"/> struct.
        /// </summary>
        /// <param name="canceled">if set to <c>true</c> [canceled].</param>
        public CancellationToken(bool canceled)
        {
            this = new CancellationToken();
            if (canceled)
                _source = CancellationTokenSource.InternalGetStaticSource(canceled);
        }
        /// <summary>
        /// Gets the none.
        /// </summary>
        public static CancellationToken None
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get { return new CancellationToken(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is cancellation requested.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancellationRequested
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get { return (_source != null && _source.IsCancellationRequested); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can be canceled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can be canceled; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeCanceled
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get { return (_source != null && _source.CanBeCanceled); }
        }

        /// <summary>
        /// Gets the wait handle.
        /// </summary>
        public WaitHandle WaitHandle
        {
            get
            {
                if (_source == null)
                    InitializeDefaultSource();
                return _source.WaitHandle;
            }
        }

        /// <summary>
        /// Registers the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public CancellationTokenRegistration Register(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            return Register(_actionToActionObjShunt, callback, false, true);
        }

        /// <summary>
        /// Registers the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="useSynchronizationContext">if set to <c>true</c> [use synchronization context].</param>
        /// <returns></returns>
        public CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            return Register(_actionToActionObjShunt, callback, useSynchronizationContext, true);
        }

        /// <summary>
        /// Registers the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public CancellationTokenRegistration Register(Action<object> callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            return Register(callback, state, false, true);
        }

        /// <summary>
        /// Registers the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <param name="useSynchronizationContext">if set to <c>true</c> [use synchronization context].</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CancellationTokenRegistration Register(Action<object> callback, object state, bool useSynchronizationContext)
        {
            return Register(callback, state, useSynchronizationContext, true);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal CancellationTokenRegistration InternalRegisterWithoutEC(Action<object> callback, object state)
        {
            return Register(callback, state, false, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecuritySafeCritical]
        private CancellationTokenRegistration Register(Action<object> callback, object state, bool useSynchronizationContext, bool useExecutionContext)
        {
            var lookForMyCaller = StackCrawlMark.LookForMyCaller;
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (!CanBeCanceled)
                return new CancellationTokenRegistration();
            SynchronizationContext targetSyncContext = null;
            ExecutionContext executionContext = null;
            if (!IsCancellationRequested)
            {
                if (useSynchronizationContext)
                    targetSyncContext = SynchronizationContext.Current;
                if (useExecutionContext)
                    executionContext = ExecutionContext.Capture(ref lookForMyCaller, ExecutionContext.CaptureOptions.OptimizeDefaultCase);
            }
            return _source.InternalRegister(callback, state, targetSyncContext, executionContext);
        }

        public bool Equals(CancellationToken other)
        {
            if (_source == null && other._source == null)
                return true;
            if (_source == null)
                return (other._source == CancellationTokenSource.InternalGetStaticSource(false));
            if (other._source == null)
                return (_source == CancellationTokenSource.InternalGetStaticSource(false));
            return (_source == other._source);
        }

        public override bool Equals(object other)
        {
            return (other is CancellationToken && Equals((CancellationToken)other));
        }

        public override int GetHashCode()
        {
            if (_source == null)
                return CancellationTokenSource.InternalGetStaticSource(false).GetHashCode();
            return _source.GetHashCode();
        }

        public static bool operator ==(CancellationToken left, CancellationToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CancellationToken left, CancellationToken right)
        {
            return !left.Equals(right);
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
                ThrowOperationCanceledException();
        }

        internal void ThrowIfSourceDisposed()
        {
            if (_source != null && _source.IsDisposed)
                ThrowObjectDisposedException();
        }

        private void ThrowOperationCanceledException()
        {
            throw new OperationCanceledException(SR.GetResourceString("OperationCanceled"), this);
        }

        private static void ActionToActionObjShunt(object obj)
        {
            var action = obj as Action;
            action();
        }

        private static void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(null, SR.GetResourceString("CancellationToken_SourceDisposed"));
        }

        private void InitializeDefaultSource()
        {
            _source = CancellationTokenSource.InternalGetStaticSource(false);
        }
    }
}
#endif