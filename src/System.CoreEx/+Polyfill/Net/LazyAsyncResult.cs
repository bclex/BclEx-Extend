#region Foreign-License
// .Net40 Polyfill
#endregion
using System.Diagnostics;
using System.Runtime;
using System.Threading;

namespace System.Net
{
    internal class LazyAsyncResult : IAsyncResult
    {
        private const int c_ForceAsyncCount = 50;
        private const int c_HighBit = -2147483648;
        private System.AsyncCallback m_AsyncCallback;
        private object m_AsyncObject;
        private object m_AsyncState;
        private bool m_EndCalled;
        private int m_ErrorCode;
        private object m_Event;
        private int m_IntCompleted;
        private object m_Result;
        private bool m_UserEvent;
        [ThreadStatic]
        private static ThreadContext t_ThreadContext;

        private class ThreadContext
        {
            internal int m_NestedIOCount;
        }

        internal LazyAsyncResult(object myObject, object myState, System.AsyncCallback myCallBack)
        {
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = DBNull.Value;
        }

        internal LazyAsyncResult(object myObject, object myState, System.AsyncCallback myCallBack, object result)
        {
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = result;
            m_IntCompleted = 1;
            if (m_AsyncCallback != null)
                m_AsyncCallback(this);
        }

        protected virtual void Cleanup() { }

        protected virtual void Complete(IntPtr userToken)
        {
            var flag = false;
            var currentThreadContext = CurrentThreadContext;
            try
            {
                currentThreadContext.m_NestedIOCount++;
                if (m_AsyncCallback != null)
                {
                    if (currentThreadContext.m_NestedIOCount >= 50)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerThreadComplete));
                        flag = true;
                    }
                    else
                        m_AsyncCallback(this);
                }
            }
            finally
            {
                currentThreadContext.m_NestedIOCount--;
                if (!flag)
                    Cleanup();
            }
        }

        [Conditional("DEBUG")]
        protected void DebugProtectState(bool protect) { }

        internal void InternalCleanup()
        {
            if ((m_IntCompleted & 0x7fffffff) == 0 && (Interlocked.Increment(ref m_IntCompleted) & 0x7fffffff) == 1)
            {
                m_Result = null;
                Cleanup();
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal object InternalWaitForCompletion()
        {
            return WaitForCompletion(true);
        }

        internal void InvokeCallback()
        {
            ProtectedInvokeCallback(null, IntPtr.Zero);
        }

        internal void InvokeCallback(object result)
        {
            ProtectedInvokeCallback(result, IntPtr.Zero);
        }

        private bool LazilyCreateEvent(out ManualResetEvent waitHandle)
        {
            bool flag;
            waitHandle = new ManualResetEvent(false);
            try
            {
                if (Interlocked.CompareExchange(ref m_Event, waitHandle, null) == null)
                {
                    if (InternalPeekCompleted)
                        waitHandle.Set();
                    return true;
                }
                waitHandle.Close();
                waitHandle = (ManualResetEvent)m_Event;
                flag = false;
            }
            catch
            {
                m_Event = null;
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
            if ((m_IntCompleted & 0x7fffffff) == 0 && (Interlocked.Increment(ref m_IntCompleted) & 0x7fffffff) == 1)
            {
                if (m_Result == DBNull.Value)
                    m_Result = result;
                var event2 = (ManualResetEvent)m_Event;
                if (event2 != null)
                    try { event2.Set(); }
                    catch (ObjectDisposedException) { }
                Complete(userToken);
            }
        }

        private object WaitForCompletion(bool snap)
        {
            var waitHandle = (ManualResetEvent)null;
            var flag = false;
            if (!(snap ? IsCompleted : InternalPeekCompleted))
            {
                waitHandle = (ManualResetEvent)m_Event;
                if (waitHandle == null)
                    flag = LazilyCreateEvent(out waitHandle);
            }
            if (waitHandle == null)
                goto Label_0077;
            try
            {
                try { waitHandle.WaitOne(-1, false); }
                catch (ObjectDisposedException) { }
                goto Label_0077;
            }
            finally
            {
                if (flag && !m_UserEvent)
                {
                    ManualResetEvent event3 = (ManualResetEvent)m_Event;
                    m_Event = null;
                    if (!m_UserEvent)
                        event3.Close();
                }
            }
        Label_0071:
            Thread.SpinWait(1);
        Label_0077:
            if (m_Result == DBNull.Value)
                goto Label_0071;
            return m_Result;
        }

        private void WorkerThreadComplete(object state)
        {
            try { m_AsyncCallback(this); }
            finally { Cleanup(); }
        }

        protected System.AsyncCallback AsyncCallback
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_AsyncCallback; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { m_AsyncCallback = value; }
        }

        internal object AsyncObject
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_AsyncObject; }
        }

        public object AsyncState
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_AsyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                m_UserEvent = true;
                if (m_IntCompleted == 0)
                    Interlocked.CompareExchange(ref m_IntCompleted, -2147483648, 0);
                var waitHandle = (ManualResetEvent)m_Event;
                while (waitHandle == null)
                    LazilyCreateEvent(out waitHandle);
                return waitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                var intCompleted = m_IntCompleted;
                if (intCompleted == 0)
                    intCompleted = Interlocked.CompareExchange(ref m_IntCompleted, -2147483648, 0);
                return (intCompleted > 0);
            }
        }

        private static ThreadContext CurrentThreadContext
        {
            get
            {
                var context = t_ThreadContext;
                if (context == null)
                {
                    context = new ThreadContext();
                    t_ThreadContext = context;
                }
                return context;
            }
        }

        internal bool EndCalled
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_EndCalled; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { m_EndCalled = value; }
        }

        internal int ErrorCode
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_ErrorCode; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { m_ErrorCode = value; }
        }

        internal bool InternalPeekCompleted
        {
            get { return ((m_IntCompleted & 0x7fffffff) != 0); }
        }

        public bool IsCompleted
        {
            get
            {
                var intCompleted = m_IntCompleted;
                if (intCompleted == 0)
                    intCompleted = Interlocked.CompareExchange(ref m_IntCompleted, -2147483648, 0);
                return ((intCompleted & 0x7fffffff) != 0);
            }
        }

        internal object Result
        {
            get
            {
                if (m_Result != DBNull.Value)
                    return m_Result;
                return null;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { m_Result = value; }
        }
    }
}

