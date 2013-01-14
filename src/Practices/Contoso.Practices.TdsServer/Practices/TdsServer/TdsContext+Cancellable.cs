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
using System.Net.Sockets;
using System.Net.Util;
using System.Threading;
using System.Runtime;
using System.Net;
using System;
using System.Diagnostics;

namespace Contoso.Practices.TdsServer
{
    public partial class TdsContext
    {
        //private DateTime _timeoutStartTime;
        //private bool _timeoutSet;
        //private TimeSpan _timeout;
        private int _timeoutState;
        private DoubleLink _timeoutLink;
        private Thread _thread;
        //
        private bool _threadAbortOnTimeout;
        private long _timeoutStartTimeUtcTicks;
        private long _timeoutTicks;
        private CancellationTokenHelper _timeoutCancellationTokenHelper;

        internal void InvokeCancellableCallback(WaitCallback callback, object state)
        {
            if (IsInCancellablePeriod)
                callback(state);
            else
            {
                try
                {
                    BeginCancellablePeriod();
                    try { callback(state); }
                    finally { EndCancellablePeriod(); }
                    WaitForExceptionIfCancelled();
                }
                catch (ThreadAbortException ex)
                {
                    if (ex.ExceptionState != null && ex.ExceptionState is TdsListenerHost.CancelModuleException && ((TdsListenerHost.CancelModuleException)ex.ExceptionState).Timeout)
                    {
                        Thread.ResetAbort();
                        throw new TdsException(0xbb9, "Request_timed_out");
                    }
                }
            }
        }

        public bool ThreadAbortOnTimeout
        {
            get { return Volatile.Read(ref _threadAbortOnTimeout); } //:kludge
            set { Volatile.Write(ref _threadAbortOnTimeout, value); } //:kludge
        }

        //internal Thread MustTimeout(DateTime utcNow)
        //{
        //    if (_timeoutState == 1 && TimeSpan.Compare(utcNow.Subtract(_timeoutStartTime), Timeout) >= 0)
        //    {
        //        try
        //        {
        //            if (TdsListenerHost.IsDebuggingEnabled || Debugger.IsAttached)
        //                return null;
        //        }
        //        catch { return null; }
        //        if (Interlocked.CompareExchange(ref _timeoutState, -1, 1) == 1)
        //            return _thread;
        //    }
        //    return null;
        //}

        internal Thread MustTimeout(DateTime utcNow) //:kludge
        {
            if ((_utcTimestamp + Timeout) < utcNow)
                LazyInitializer.EnsureInitialized(ref _timeoutCancellationTokenHelper, () => new CancellationTokenHelper(true)).Cancel();
            if (Volatile.Read(ref _timeoutState) == 1 && ThreadAbortOnTimeout)
            {
                var num = Volatile.Read(ref _timeoutStartTimeUtcTicks) + Timeout.Ticks;
                if (num < utcNow.Ticks)
                {
                    try
                    {
                        if (TdsListenerHost.IsDebuggingEnabled || Debugger.IsAttached)
                            return null;
                    }
                    catch { return null; }
                    if (Interlocked.CompareExchange(ref _timeoutState, -1, 1) == 1)
                        return _thread;
                }
            }
            return null;
        }

        internal bool IsInCancellablePeriod
        {
            get { return Volatile.Read(ref _timeoutState) == 1; } //:kludge
            //get { return (_timeoutState == 1); }
        }

        //internal void EnsureTimeout()
        //{
        //    if (!_timeoutSet)
        //    {
        //        var totalSeconds = (int)TdsListenerHost.ExecutionTimeout.TotalSeconds;
        //        _timeout = new TimeSpan(0, 0, totalSeconds);
        //        _timeoutSet = true;
        //    }
        //}
        internal long EnsureTimeout()
        {
            var ticks = Volatile.Read(ref _timeoutTicks);
            if (ticks == -1L)
            {
                ticks = TdsListenerHost.ExecutionTimeout.Ticks;
                var num2 = Interlocked.CompareExchange(ref _timeoutTicks, ticks, -1L);
                if (num2 != -1L)
                    ticks = num2;
            }
            return ticks;
        }

        internal DoubleLink TimeoutLink
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _timeoutLink; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _timeoutLink = value; }
        }

        internal TimeSpan Timeout
        {
            get { return TimeSpan.FromTicks(EnsureTimeout()); } //: kludge
            //get
            //{
            //    EnsureTimeout();
            //    return _timeout;
            //}
            set { Interlocked.Exchange(ref _timeoutTicks, value.Ticks); } //: kludge
            //set
            //{
            //    _timeout = value;
            //    _timeoutSet = true;
            //}
        }

        internal Thread CurrentThread
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _thread; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { _thread = value; }
        }
        internal void SetStartTime() //: kludge
        {
            Interlocked.Exchange(ref _timeoutStartTimeUtcTicks, DateTime.UtcNow.Ticks);
            //_timeoutStartTime = DateTime.UtcNow;
        }

        internal void BeginCancellablePeriod() //: kludge
        {
            if (Volatile.Read(ref _timeoutStartTimeUtcTicks) == -1L)
                SetStartTime();
            Volatile.Write(ref _timeoutState, 1);
            //if (_timeoutStartTime == DateTime.MinValue)
            //    SetStartTime();
            //_timeoutState = 1;
        }

        internal void EndCancellablePeriod() { Interlocked.CompareExchange(ref _timeoutState, 0, 1); }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        internal void WaitForExceptionIfCancelled() //: kludge
        {
            while (Volatile.Read(ref _timeoutState) == -1)
                Thread.Sleep(100);
            //while (_timeoutState == -1)
            //    Thread.Sleep(100);
        }
    }
}
