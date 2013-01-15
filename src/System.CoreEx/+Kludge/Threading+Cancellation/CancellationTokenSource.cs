#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Runtime;
using System.Collections.Generic;
namespace System.Threading
{
    /// <summary>
    /// CancellationTokenSource
    /// </summary>
    [ComVisible(false), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class CancellationTokenSource : IDisposable
    {
        private static readonly CancellationTokenSource _staticSource_NotCancelable = new CancellationTokenSource(false);
        private static readonly CancellationTokenSource _staticSource_Set = new CancellationTokenSource(true);
        private const int CANNOT_BE_CANCELED = 0;
        private bool _disposed;
        private volatile CancellationCallbackInfo _executingCallback;
        private volatile ManualResetEvent _kernelEvent;
        private CancellationTokenRegistration[] _linkingRegistrations;
        private volatile SparselyPopulatedArray<CancellationCallbackInfo>[] _registeredCallbacksLists;
        private volatile int _state;
        private volatile int _threadIDExecutingCallbacks;
        private volatile Timer _timer;
        private const int NOT_CANCELED = 1;
        private const int NOTIFYING = 2;
        private const int NOTIFYINGCOMPLETE = 3;
        private static readonly Action<object> _LinkedTokenCancelDelegate = new Action<object>(CancellationTokenSource.LinkedTokenCancelDelegate);
        private static readonly int _nLists = (PlatformHelper.ProcessorCount > 0x18 ? 0x18 : PlatformHelper.ProcessorCount);
        private static readonly TimerCallback _timerCallback = new TimerCallback(CancellationTokenSource.TimerCallbackLogic);

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationTokenSource"/> class.
        /// </summary>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public CancellationTokenSource()
        {
            _threadIDExecutingCallbacks = -1;
            _state = 1;
        }
        private CancellationTokenSource(bool set)
        {
            _threadIDExecutingCallbacks = -1;
            _state = (set ? 3 : 0);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationTokenSource"/> class.
        /// </summary>
        /// <param name="millisecondsDelay">The milliseconds delay.</param>
        public CancellationTokenSource(int millisecondsDelay)
        {
            _threadIDExecutingCallbacks = -1;
            if (millisecondsDelay < -1)
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            InitializeWithTimer(millisecondsDelay);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationTokenSource"/> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public CancellationTokenSource(TimeSpan delay)
        {
            _threadIDExecutingCallbacks = -1;
            var totalMilliseconds = (long)delay.TotalMilliseconds;
            if (totalMilliseconds < -1L || totalMilliseconds > 0x7fffffffL)
                throw new ArgumentOutOfRangeException("delay");
            InitializeWithTimer((int)totalMilliseconds);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_timer != null)
                    _timer.Dispose();
                var linkingRegistrations = _linkingRegistrations;
                if (linkingRegistrations != null)
                {
                    _linkingRegistrations = null;
                    for (int i = 0; i < linkingRegistrations.Length; i++)
                        linkingRegistrations[i].Dispose();
                }
                _registeredCallbacksLists = null;
                if (_kernelEvent != null)
                {
                    _kernelEvent.Close();
                    _kernelEvent = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Cancel()
        {
            Cancel(false);
        }

        /// <summary>
        /// Cancels the specified throw on first exception.
        /// </summary>
        /// <param name="throwOnFirstException">if set to <c>true</c> [throw on first exception].</param>
        public void Cancel(bool throwOnFirstException)
        {
            ThrowIfDisposed();
            NotifyCancellation(throwOnFirstException);
        }

        /// <summary>
        /// Cancels the after.
        /// </summary>
        /// <param name="millisecondsDelay">The milliseconds delay.</param>
        public void CancelAfter(int millisecondsDelay)
        {
            ThrowIfDisposed();
            if (millisecondsDelay < -1)
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            if (!IsCancellationRequested)
            {
                if (_timer == null)
                {
                    var timer = new Timer(_timerCallback, this, -1, -1);
                    if (Interlocked.CompareExchange<Timer>(ref _timer, timer, null) != null)
                        timer.Dispose();
                }
                try { _timer.Change(millisecondsDelay, -1); }
                catch (ObjectDisposedException) { }
            }
        }

        /// <summary>
        /// Cancels the after.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public void CancelAfter(TimeSpan delay)
        {
            var totalMilliseconds = (long)delay.TotalMilliseconds;
            if (totalMilliseconds < -1L || totalMilliseconds > 0x7fffffffL)
                throw new ArgumentOutOfRangeException("delay");
            CancelAfter((int)totalMilliseconds);
        }

        private void CancellationCallbackCoreWork(CancellationCallbackCoreWorkArguments args)
        {
            var info = args._currArrayFragment.SafeAtomicRemove(args._currArrayIndex, _executingCallback);
            if (info == _executingCallback)
            {
                if (info.TargetExecutionContext != null)
                    info.CancellationTokenSource.ThreadIDExecutingCallbacks = Thread.CurrentThread.ManagedThreadId;
                info.ExecuteCallback();
            }
        }

        private void CancellationCallbackCoreWork_OnSyncContext(object obj)
        {
            CancellationCallbackCoreWork((CancellationCallbackCoreWorkArguments)obj);
        }

        /// <summary>
        /// Creates the linked token source.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        public static CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException("tokens");
            if (tokens.Length == 0)
                throw new ArgumentException(EnvironmentEx.GetResourceString("CancellationToken_CreateLinkedToken_TokensIsEmpty"));
            var state = new CancellationTokenSource
            {
                _linkingRegistrations = new CancellationTokenRegistration[tokens.Length]
            };
            for (int i = 0; i < tokens.Length; i++)
                if (tokens[i].CanBeCanceled)
                    state._linkingRegistrations[i] = tokens[i].InternalRegisterWithoutEC(_LinkedTokenCancelDelegate, state);
            return state;
        }

        /// <summary>
        /// Creates the linked token source.
        /// </summary>
        /// <param name="token1">The token1.</param>
        /// <param name="token2">The token2.</param>
        /// <returns></returns>
        public static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token1, CancellationToken token2)
        {
            var state = new CancellationTokenSource();
            var canBeCanceled = token2.CanBeCanceled;
            if (token1.CanBeCanceled)
            {
                state._linkingRegistrations = new CancellationTokenRegistration[canBeCanceled ? 2 : 1];
                state._linkingRegistrations[0] = token1.InternalRegisterWithoutEC(_LinkedTokenCancelDelegate, state);
            }
            if (canBeCanceled)
            {
                var index = 1;
                if (state._linkingRegistrations == null)
                {
                    state._linkingRegistrations = new CancellationTokenRegistration[1];
                    index = 0;
                }
                state._linkingRegistrations[index] = token2.InternalRegisterWithoutEC(_LinkedTokenCancelDelegate, state);
            }
            return state;
        }

        private void ExecuteCallbackHandlers(bool throwOnFirstException)
        {
            List<Exception> innerExceptions = null;
            var registeredCallbacksLists = _registeredCallbacksLists;
            if (registeredCallbacksLists == null)
                Interlocked.Exchange(ref _state, 3);
            else
            {
                try
                {
                    for (int i = 0; i < registeredCallbacksLists.Length; i++)
                    {
                        var array = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref registeredCallbacksLists[i]);
                        if (array != null)
                            for (SparselyPopulatedArrayFragment<CancellationCallbackInfo> fragment = array.Tail; fragment != null; fragment = fragment.Prev)
                                for (int j = fragment.Length - 1; j >= 0; j--)
                                {
                                    _executingCallback = fragment[j];
                                    if (_executingCallback != null)
                                    {
                                        var state = new CancellationCallbackCoreWorkArguments(fragment, j);
                                        try
                                        {
                                            if (_executingCallback.TargetSyncContext != null)
                                            {
                                                _executingCallback.TargetSyncContext.Send(new SendOrPostCallback(CancellationCallbackCoreWork_OnSyncContext), state);
                                                ThreadIDExecutingCallbacks = Thread.CurrentThread.ManagedThreadId;
                                            }
                                            else
                                                CancellationCallbackCoreWork(state);
                                        }
                                        catch (Exception exception)
                                        {
                                            if (throwOnFirstException)
                                                throw;
                                            if (innerExceptions == null)
                                                innerExceptions = new List<Exception>();
                                            innerExceptions.Add(exception);
                                        }
                                    }
                                }
                    }
                }
                finally
                {
                    _state = 3;
                    _executingCallback = null;
                    Thread.MemoryBarrier();
                }
                if (innerExceptions != null)
                    throw new AggregateException(innerExceptions);
            }
        }

        private void InitializeWithTimer(int millisecondsDelay)
        {
            _state = 1;
            _timer = new Timer(_timerCallback, this, millisecondsDelay, -1);
        }

        internal static CancellationTokenSource InternalGetStaticSource(bool set)
        {
            if (!set)
                return _staticSource_NotCancelable;
            return _staticSource_Set;
        }

        internal CancellationTokenRegistration InternalRegister(Action<object> callback, object stateForCallback, SynchronizationContext targetSyncContext, ExecutionContext executionContext)
        {
            ThrowIfDisposed();
            if (!IsCancellationRequested)
            {
                var index = Thread.CurrentThread.ManagedThreadId % _nLists;
                var element = new CancellationCallbackInfo(callback, stateForCallback, targetSyncContext, executionContext, this);
                var registeredCallbacksLists = _registeredCallbacksLists;
                if (registeredCallbacksLists == null)
                {
                    var arrayArray2 = new SparselyPopulatedArray<CancellationCallbackInfo>[_nLists];
                    registeredCallbacksLists = Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>[]>(ref _registeredCallbacksLists, arrayArray2, null);
                    if (registeredCallbacksLists == null)
                        registeredCallbacksLists = arrayArray2;
                }
                var array = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref registeredCallbacksLists[index]);
                if (array == null)
                {
                    var array2 = new SparselyPopulatedArray<CancellationCallbackInfo>(4);
                    Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>>(ref registeredCallbacksLists[index], array2, null);
                    array = registeredCallbacksLists[index];
                }
                var registrationInfo = array.Add(element);
                var registration = new CancellationTokenRegistration(element, registrationInfo);
                if (!IsCancellationRequested)
                    return registration;
                if (!registration.TryDeregister())
                    return registration;
            }
            callback(stateForCallback);
            return new CancellationTokenRegistration();
        }

        private static void LinkedTokenCancelDelegate(object source)
        {
            (source as CancellationTokenSource).Cancel();
        }

        private void NotifyCancellation(bool throwOnFirstException)
        {
            if (!IsCancellationRequested && Interlocked.CompareExchange(ref _state, 2, 1) == 1)
            {
                var timer = _timer;
                if (timer != null)
                    timer.Dispose();
                ThreadIDExecutingCallbacks = Thread.CurrentThread.ManagedThreadId;
                if (_kernelEvent != null)
                    _kernelEvent.Set();
                ExecuteCallbackHandlers(throwOnFirstException);
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        internal void ThrowIfDisposed()
        {
            if (_disposed)
                ThrowObjectDisposedException();
        }

        private static void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(null, EnvironmentEx.GetResourceString("CancellationTokenSource_Disposed"));
        }

        private static void TimerCallbackLogic(object obj)
        {
            var source = (CancellationTokenSource)obj;
            if (!source.IsDisposed)
                try { source.Cancel(); }
                catch (ObjectDisposedException)
                {
                    if (!source.IsDisposed)
                        throw;
                }
        }

        internal void WaitForCallbackToComplete(CancellationCallbackInfo callbackInfo)
        {
            var wait = new SpinWait();
            while (ExecutingCallback == callbackInfo)
                wait.SpinOnce();
        }

        internal bool CanBeCanceled
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get { return (_state != 0); }
        }

        internal CancellationCallbackInfo ExecutingCallback
        {
            get { return _executingCallback; }
        }

        internal bool IsCancellationCompleted
        {
            get { return (_state == 3); }
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
            get { return (_state >= 2); }
        }

        internal bool IsDisposed
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _disposed; }
        }

        internal int ThreadIDExecutingCallbacks
        {
            get { return _threadIDExecutingCallbacks; }
            set { _threadIDExecutingCallbacks = value; }
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        public CancellationToken Token
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get
            {
                ThrowIfDisposed();
                return new CancellationToken(this);
            }
        }

        internal WaitHandle WaitHandle
        {
            get
            {
                ThrowIfDisposed();
                if (_kernelEvent == null)
                {
                    var event2 = new ManualResetEvent(false);
                    if (Interlocked.CompareExchange<ManualResetEvent>(ref _kernelEvent, event2, null) != null)
                        ((IDisposable)event2).Dispose();
                    if (IsCancellationRequested)
                        _kernelEvent.Set();
                }
                return _kernelEvent;
            }
        }
    }
}
#endif