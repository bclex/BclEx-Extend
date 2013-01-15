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
using System.Net;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;

namespace Contoso.Practices.TdsServer
{
    /// <summary>
    /// TdsRuntime
    /// </summary>
    public partial class TdsRuntime
    {
        private static TdsRuntime _theRuntime;
        
        static TdsRuntime()
        {
            _theRuntime = new TdsRuntime();
            _theRuntime.Init();
        }

        #region Initialization & Dispose
        private Exception _initializationError;
        private bool _shutdownInProgress;
        private volatile bool _disposingTdsRuntime;

        private void Init()
        {
            try
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                    throw new PlatformNotSupportedException(SR.GetString("RequiresNT"));
                _timeoutManager = new RequestTimeoutManager();
            }
            catch (Exception exception) { InitializationException = exception; }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
        public static void Close()
        {
            if (_theRuntime.InitiateShutdownOnce())
            {
                //SetShutdownReason(ApplicationShutdownReason.HttpRuntimeClose, "HttpRuntime.Close is called");
                _theRuntime.Dispose();
            }
        }

        private bool InitiateShutdownOnce()
        {
            if (_shutdownInProgress)
                return false;
            lock (this)
            {
                if (_shutdownInProgress)
                    return false;
                _shutdownInProgress = true;
            }
            return true;
        }

        private void Dispose()
        {
            var totalSeconds = 90;
            try
            {
                WaitForRequestsToFinish(totalSeconds * 0x3e8);
                if (_requestQueue != null)
                    _requestQueue.Drain();
            }
            finally { _disposingTdsRuntime = true; }
            WaitForRequestsToFinish((totalSeconds * 0x3e8) / 6);
            _timeoutManager.Stop();
        }

        internal static Exception InitializationException
        {
            get { return _theRuntime._initializationError; }
            set { _theRuntime._initializationError = value; }
        }

        #endregion

        #region ProcessRequest

        private RequestTimeoutManager _timeoutManager;
        internal int _activeRequests = 0;
        internal RequestQueue _requestQueue;

        private void InitRequestQueue()
        {
            _requestQueue = new RequestQueue(this, 5, 5, 3, new TimeSpan(0, 0, 30));
        }

        protected virtual TdsListenerContext CreateWorkerRequest(TdsListenerContext wr)
        {
            return ((TdsListenerContext)Activator.CreateInstance(_workerRequestType, wr));
        }

        internal void ProcessRequestNoDemand(TdsListenerContext wr)
        {
            var queue = _requestQueue;
            if (queue != null)
                wr = queue.GetRequestToExecute(wr);
            if (wr != null)
                ProcessRequestNow(wr);
        }

        internal virtual void ProcessRequestNow(TdsListenerContext wr) { }

        internal virtual void RejectRequestNow(TdsListenerContext wr, bool silent) { }

        private void WaitForRequestsToFinish(int waitTimeoutMs)
        {
            var time = DateTime.UtcNow.AddMilliseconds((double)waitTimeoutMs);
            do
            {
                if (_activeRequests == 0)
                {
                    if (_requestQueue == null)
                        break;
                    if (_requestQueue.IsEmpty)
                        return;
                }
                Thread.Sleep(250);
            }
            while (Debugger.IsAttached || DateTime.UtcNow <= time);
        }

        #endregion
    }
}
