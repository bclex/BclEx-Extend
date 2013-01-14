#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net
{
    internal class LazyAsyncResult : IAsyncResult
    {
        private const int _forceAsyncCount = 50;
        private const int _highBit = -2147483648;
        private System.AsyncCallback _asyncCallback;
        private object _asyncObject;
        private object _asyncState;
        private bool _endCalled;
        private int _errorCode;
        private object _event;
        private int _initCompleted;
        private object _result;
        private bool _userEvent;
        [ThreadStatic]
        private static ThreadContext _threadContext;

        internal LazyAsyncResult(object myObject, object myState, System.AsyncCallback myCallBack)
        {
            _asyncObject = myObject;
            _asyncState = myState;
            _asyncCallback = myCallBack;
            _result = DBNull.Value;
        }

        internal LazyAsyncResult(object myObject, object myState, System.AsyncCallback myCallBack, object result)
        {
            _asyncObject = myObject;
            _asyncState = myState;
            _asyncCallback = myCallBack;
            _result = result;
            _initCompleted = 1;
            if (_asyncCallback != null)
                _asyncCallback(this);
        }

        protected virtual void Cleanup() { }

        protected virtual void Complete(IntPtr userToken)
        {
            var flag = false;
            var currentThreadContext = CurrentThreadContext;
            try
            {
                currentThreadContext._nestedIOCount++;
                if (_asyncCallback != null)
                {
                    if (currentThreadContext._nestedIOCount >= 50)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerThreadComplete));
                        flag = true;
                    }
                    else
                        _asyncCallback(this);
                }
            }
            finally
            {
                currentThreadContext._nestedIOCount--;
                if (!flag)
                    Cleanup();
            }
        }

        [Conditional("DEBUG")]
        protected void DebugProtectState(bool protect) { }

        internal void InternalCleanup()
        {
            if ((_initCompleted & 0x7fffffff) == 0 && (Interlocked.Increment(ref _initCompleted) & 0x7fffffff) == 1)
            {
                _result = null;
                Cleanup();
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal object InternalWaitForCompletion() { return WaitForCompletion(true); }

        internal void InvokeCallback() { ProtectedInvokeCallback(null, IntPtr.Zero); }
        internal void InvokeCallback(object result) { ProtectedInvokeCallback(result, IntPtr.Zero); }

        private bool LazilyCreateEvent(out ManualResetEvent waitHandle)
        {
            bool flag;
            waitHandle = new ManualResetEvent(false);
            try
            {
                if (Interlocked.CompareExchange(ref _event, waitHandle, null) == null)
                {
                    if (InternalPeekCompleted)
                        waitHandle.Set();
                    return true;
                }
                waitHandle.Close();
                waitHandle = (ManualResetEvent)_event;
                flag = false;
            }
            catch
            {
                _event = null;
                if (waitHandle != null)
                    waitHandle.Close();
                throw;
            }
            return flag;
        }

        protected void ProtectedInvokeCallback(object result, IntPtr userToken)
        {
            if (result == DBNull.Value)
                throw new ArgumentNullException("result");
            if ((_initCompleted & 0x7fffffff) == 0 && (Interlocked.Increment(ref _initCompleted) & 0x7fffffff) == 1)
            {
                if (_result == DBNull.Value)
                    _result = result;
                var event2 = (ManualResetEvent)_event;
                if (event2 != null)
                    try { event2.Set(); }
                    catch (ObjectDisposedException) { }
                Complete(userToken);
            }
        }

        private object WaitForCompletion(bool snap)
        {
            ManualResetEvent waitHandle = null;
            var flag = false;
            if (!(snap ? IsCompleted : InternalPeekCompleted))
            {
                waitHandle = (ManualResetEvent)_event;
                if (waitHandle == null)
                    flag = LazilyCreateEvent(out waitHandle);
            }
            if (waitHandle == null)
                goto SpinWait_;
            try
            {
                try { waitHandle.WaitOne(-1, false); }
                catch (ObjectDisposedException) { }
                goto SpinWait_;
            }
            finally
            {
                if (flag && !_userEvent)
                {
                    var event3 = (ManualResetEvent)_event;
                    _event = null;
                    if (!_userEvent)
                        event3.Close();
                }
            }
        SpinWaitAgain_:
            Thread.SpinWait(1);
        SpinWait_:
            if (_result == DBNull.Value)
                goto SpinWaitAgain_;
            return _result;
        }

        private void WorkerThreadComplete(object state)
        {
            try { _asyncCallback(this); }
            finally { Cleanup(); }
        }

        protected AsyncCallback AsyncCallback
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _asyncCallback; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _asyncCallback = value; }
        }

        internal object AsyncObject
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _asyncObject; }
        }

        public object AsyncState
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _asyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                _userEvent = true;
                if (_initCompleted == 0)
                    Interlocked.CompareExchange(ref _initCompleted, int.MinValue, 0);
                var waitHandle = (ManualResetEvent)_event;
                while (waitHandle == null)
                    LazilyCreateEvent(out waitHandle);
                return waitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                var intCompleted = _initCompleted;
                if (intCompleted == 0)
                    intCompleted = Interlocked.CompareExchange(ref _initCompleted, int.MinValue, 0);
                return (intCompleted > 0);
            }
        }

        private static ThreadContext CurrentThreadContext
        {
            get
            {
                var context = _threadContext;
                if (context == null)
                {
                    context = new ThreadContext();
                    _threadContext = context;
                }
                return context;
            }
        }

        internal bool EndCalled
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _endCalled; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _endCalled = value; }
        }

        internal int ErrorCode
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _errorCode; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _errorCode = value; }
        }

        internal bool InternalPeekCompleted
        {
            get { return ((_initCompleted & 0x7fffffff) != 0); }
        }

        public bool IsCompleted
        {
            get
            {
                var intCompleted = _initCompleted;
                if (intCompleted == 0)
                    intCompleted = Interlocked.CompareExchange(ref _initCompleted, int.MinValue, 0);
                return ((intCompleted & 0x7fffffff) != 0);
            }
        }

        internal object Result
        {
            get { return (_result != DBNull.Value ? _result : null); }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _result = value; }
        }

        private class ThreadContext
        {
            internal int _nestedIOCount;
        }
    }
}

