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
using System.Collections;
using System.Threading;
using System.Net;

namespace Contoso.Practices.TdsServer
{
    /// <summary>
    /// RequestQueue
    /// </summary>
    internal class RequestQueue
    {
        private TdsRuntime _runtime;
        private TimeSpan _clientConnectedTime;
        private int _count;
        private bool _draining;
        private Queue _externQueue;
        private Queue _localQueue;
        private int _minExternFreeThreads;
        private int _minLocalFreeThreads;
        private int _queueLimit;
        private Timer _timer;
        private readonly TimeSpan _timerPeriod;
        private WaitCallback _workItemCallback;
        private int _workItemCount;
        private const int _workItemLimit = 2;

        internal RequestQueue(TdsRuntime runtime, int minExternFreeThreads, int minLocalFreeThreads, int queueLimit, TimeSpan clientConnectedTime)
        {
            _runtime = runtime;
            _localQueue = new Queue();
            _externQueue = new Queue();
            _timerPeriod = new TimeSpan(0, 0, 10);
            _minExternFreeThreads = minExternFreeThreads;
            _minLocalFreeThreads = minLocalFreeThreads;
            _queueLimit = queueLimit;
            _clientConnectedTime = clientConnectedTime;
            _workItemCallback = new WaitCallback(WorkItemCallback);
            _timer = new Timer(new TimerCallback(TimerCompletionCallback), null, _timerPeriod, _timerPeriod);
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
        }

        private bool CheckClientConnected(TdsListenerContext wr)
        {
            return ((DateTime.UtcNow - wr.GetStartTime()) > _clientConnectedTime ? wr.IsClientConnected() : true);
        }

        private TdsListenerContext DequeueRequest(bool localOnly)
        {
            TdsListenerContext wr = null;
            while (_count > 0)
            {
                lock (this)
                {
                    if (_localQueue.Count > 0)
                    {
                        wr = (TdsListenerContext)_localQueue.Dequeue();
                        _count--;
                    }
                    else if (!localOnly && _externQueue.Count > 0)
                    {
                        wr = (TdsListenerContext)_externQueue.Dequeue();
                        _count--;
                    }
                }
                if (wr == null)
                    return wr;
                if (CheckClientConnected(wr))
                    return wr;
                _runtime.RejectRequestNow(wr, true);
                wr = null;
            }
            return wr;
        }

        internal void Drain()
        {
            _draining = true;
            if (_timer != null)
            {
                _timer.Dispose(); _timer = null;
            }
            while (_workItemCount > 0)
                Thread.Sleep(100);
            if (_count == 0)
                return;
            while (true)
            {
                var wr = DequeueRequest(false);
                if (wr == null)
                    return;
                _runtime.RejectRequestNow(wr, false);
            }
        }

        internal TdsListenerContext GetRequestToExecute(TdsListenerContext wr)
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            var threads = (completionPortThreads > workerThreads ? workerThreads : completionPortThreads);
            if (threads < _minExternFreeThreads || _count != 0)
            {
                var isLocal = IsLocal(wr);
                if (isLocal && threads >= _minLocalFreeThreads && _count == 0)
                    return wr;
                if (_count >= _queueLimit)
                {
                    _runtime.RejectRequestNow(wr, false);
                    return null;
                }
                QueueRequest(wr, isLocal);
                if (threads >= _minExternFreeThreads)
                {
                    wr = DequeueRequest(false);
                    return wr;
                }
                if (threads >= _minLocalFreeThreads)
                {
                    wr = DequeueRequest(true);
                    return wr;
                }
                wr = null;
                ScheduleMoreWorkIfNeeded();
            }
            return wr;
        }

        private static bool IsLocal(TdsListenerContext wr)
        {
            var remoteAddress = wr.GetRemoteAddress();
            switch (remoteAddress)
            {
                case "127.0.0.1":
                case "::1":
                    return true;
            }
            if (string.IsNullOrEmpty(remoteAddress))
                return false;
            return (remoteAddress == wr.GetLocalAddress());
        }

        private void QueueRequest(TdsListenerContext wr, bool isLocal)
        {
            lock (this)
            {
                if (isLocal)
                    _localQueue.Enqueue(wr);
                else
                    _externQueue.Enqueue(wr);
                _count++;
            }
        }

        internal void ScheduleMoreWorkIfNeeded()
        {
            if (!_draining && _count != 0 && _workItemCount < 2)
            {
                int workerThreads;
                int completionPortThreads;
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                if (workerThreads >= _minLocalFreeThreads)
                {
                    Interlocked.Increment(ref _workItemCount);
                    ThreadPool.QueueUserWorkItem(_workItemCallback);
                }
            }
        }

        private void TimerCompletionCallback(object state)
        {
            ScheduleMoreWorkIfNeeded();
        }

        private void WorkItemCallback(object state)
        {
            Interlocked.Decrement(ref _workItemCount);
            if (!_draining && _count != 0)
            {
                int workerThreads;
                int completionPortThreads;
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                if (workerThreads >= _minLocalFreeThreads)
                {
                    var wr = DequeueRequest(workerThreads < _minExternFreeThreads);
                    if (wr != null)
                    {
                        ScheduleMoreWorkIfNeeded();
                        _runtime.ProcessRequestNow(wr);
                    }
                }
            }
        }

        internal bool IsEmpty
        {
            get { return (_count == 0); }
        }
    }
}
