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
using System.Net.Util;
using System.Threading;

namespace Contoso.Practices.TdsServer
{
    /// <summary>
    /// RequestTimeoutManager
    /// </summary>
    internal class RequestTimeoutManager
    {
        private int _currentList;
        private int _inProgressLock;
        private DoubleLinkList[] _lists = new DoubleLinkList[13];
        private int _requestCount = 0;
        private Timer _timer;
        private readonly TimeSpan _timerPeriod = new TimeSpan(0, 0, 15);

        internal RequestTimeoutManager()
        {
            for (int index = 0; index < _lists.Length; index++)
                _lists[index] = new DoubleLinkList();
            _currentList = 0;
            _inProgressLock = 0;
            _timer = new Timer(new TimerCallback(TimerCompletionCallback), null, _timerPeriod, _timerPeriod);
        }

        internal void Add(TdsContext context)
        {
            if (context.TimeoutLink != null)
                ((RequestTimeoutEntry)context.TimeoutLink).IncrementCount();
            else
            {
                var entry = new RequestTimeoutEntry(context);
                var index = _currentList++;
                if (index >= _lists.Length)
                {
                    index = 0;
                    _currentList = 0;
                }
                entry.AddToList(_lists[index]);
                Interlocked.Increment(ref _requestCount);
                context.TimeoutLink = entry;
            }
        }

        private void CancelTimedOutRequests(DateTime now)
        {
            if (Interlocked.CompareExchange(ref _inProgressLock, 1, 0) == 0)
            {
                var list = new ArrayList(_requestCount);
                for (int i = 0; i < _lists.Length; i++)
                {
                    lock (_lists[i])
                    {
                        var enumerator = _lists[i].GetEnumerator();
                        while (enumerator.MoveNext())
                            list.Add(enumerator.GetDoubleLink());
                        enumerator = null;
                    }
                }
                var count = list.Count;
                for (int j = 0; j < count; j++)
                    ((RequestTimeoutEntry)list[j]).TimeoutIfNeeded(now);
                Interlocked.Exchange(ref _inProgressLock, 0);
            }
        }

        internal void Remove(TdsContext context)
        {
            var timeoutLink = (RequestTimeoutEntry)context.TimeoutLink;
            if (timeoutLink != null)
            {
                if (timeoutLink.DecrementCount() != 0)
                    return;
                timeoutLink.RemoveFromList();
                Interlocked.Decrement(ref _requestCount);
            }
            context.TimeoutLink = null;
        }

        internal void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            while (_inProgressLock != 0)
                Thread.Sleep(100);
            if (_requestCount > 0)
                CancelTimedOutRequests(DateTime.UtcNow.AddYears(1));
        }

        private void TimerCompletionCallback(object state)
        {
            if (_requestCount > 0)
                CancelTimedOutRequests(DateTime.UtcNow);
        }

        /// <summary>
        /// RequestTimeoutEntry
        /// </summary>
        private class RequestTimeoutEntry : DoubleLink
        {
            private TdsContext _context;
            private int _count;
            private DoubleLinkList _list;

            internal RequestTimeoutEntry(TdsContext context)
            {
                _context = context;
                _count = 1;
            }

            internal void AddToList(DoubleLinkList list)
            {
                lock (list)
                {
                    list.InsertTail(this);
                    _list = list;
                }
            }

            internal int DecrementCount() { return Interlocked.Decrement(ref _count); }

            internal void IncrementCount() { Interlocked.Increment(ref _count); }

            internal void RemoveFromList()
            {
                if (_list != null)
                {
                    lock (_list)
                    {
                        base.Remove();
                        _list = null;
                    }
                }
            }

            internal void TimeoutIfNeeded(DateTime now)
            {
                var thread = _context.MustTimeout(now);
                if (thread != null)
                {
                    RemoveFromList();
                    thread.Abort(new TdsListenerHost.CancelModuleException(true));
                }
            }
        }
    }
}
