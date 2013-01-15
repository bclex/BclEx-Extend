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
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System;
using System.Net;
using Contoso.Practices.TdsServer;

namespace Contoso.Practices.TdsServer
{
    // HttpRuntime/HttpApplication
    /// <summary>
    /// TdsListenerHost
    /// </summary>
    public partial class TdsListenerHost : IDisposable
    {
        private TdsListener _listener;
        private Thread _connectionManagerThread;
        private bool _disposed;
        private long _runState = (long)State.Closed;
        private Type _workerRequestType;
        //private static RequestTimeoutManager _timeoutManager;
        //private static int _activeHosts;

        /// <summary>
        /// State
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Closed
            /// </summary>
            Closed,
            /// <summary>
            /// Closing
            /// </summary>
            Closing,
            /// <summary>
            /// Opening
            /// </summary>
            Opening,
            /// <summary>
            /// Opened
            /// </summary>
            Opened
        }

        static TdsListenerHost()
        {
            ExecutionTimeout = new TimeSpan(0, 3, 0);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TdsListenerHost"/> class.
        /// </summary>
        /// <param name="workerRequestType">Type of the worker request.</param>
        /// <param name="localEP">The local EP.</param>
        public TdsListenerHost(Type workerRequestType, IPEndPoint localEP)
        {
            if (workerRequestType != null && !typeof(TdsListenerContext).IsAssignableFrom(workerRequestType))
                throw new ArgumentOutOfRangeException("workerRequestType", "must be of type TdsWorkerRequest");
            _listener = new TdsListener(localEP);
            _workerRequestType = workerRequestType;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TdsListenerHost"/> class.
        /// </summary>
        /// <param name="workerRequestType">Type of the worker request.</param>
        /// <param name="localaddr">The localaddr.</param>
        /// <param name="port">The port.</param>
        public TdsListenerHost(Type workerRequestType, IPAddress localaddr, int port)
        {
            if (workerRequestType != null && !typeof(TdsListenerContext).IsAssignableFrom(workerRequestType))
                throw new ArgumentOutOfRangeException("workerRequestType", "must be of type TdsWorkerRequest");
            _listener = new TdsListener(localaddr, port);
            _workerRequestType = workerRequestType;
        }

        /// <summary>
        /// Gets or sets the execution timeout.
        /// </summary>
        /// <value>
        /// The execution timeout.
        /// </value>
        public static TimeSpan ExecutionTimeout { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is debugging enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is debugging enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebuggingEnabled { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (RunState != State.Closed)
                    Close();
                if (_connectionManagerThread != null)
                {
                    _connectionManagerThread.Abort();
                    _connectionManagerThread = null;
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Gets the listener.
        /// </summary>
        public TdsListener Listener
        {
            get { return _listener; }
        }

        /// <summary>
        /// Gets the state of the run.
        /// </summary>
        /// <value>
        /// The state of the run.
        /// </value>
        public State RunState
        {
            get { return (State)Interlocked.Read(ref _runState); }
        }

        internal static RequestTimeoutManager RequestTimeoutManager
        {
            get { return _timeoutManager; }
        }

        #region Threading

        public virtual void Open()
        {
            if (_listener.IsListening)
                throw new InvalidOperationException("Already opened.");
            Interlocked.Increment(ref _activeHosts);
            try { _listener.Start(); }
            catch (SocketException ex)
            {
                if (!ex.Message.Contains("Access is denied"))
                    throw;
                _listener = null;
                return;
            }
            _timeoutManager = new RequestTimeoutManager();
            // create thread for listening and block till its in opened state
            _connectionManagerThread = new Thread(new ThreadStart(ConnectionManager)) { Name = "Connection Manager: " + Guid.NewGuid() };
            _connectionManagerThread.Start();
            var waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond * 10;
            while (RunState != State.Opened)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime)
                    throw new TimeoutException("Unable to start the request handling process.");
            }
        }

        public virtual void Close()
        {
            if (!_listener.IsListening)
                throw new InvalidOperationException("Already closed.");
            Interlocked.Exchange(ref _runState, (long)State.Closing);
            _listener.Stop();
            //
            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond * 10;
            while (RunState != State.Closed)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime)
                    throw new TimeoutException("Unable to stop the web server process.");
            }
            _timeoutManager.Stop();
            Interlocked.Decrement(ref _activeHosts);
            _connectionManagerThread.Abort(); _connectionManagerThread = null;
        }

        private void ConnectionManager()
        {
            Interlocked.Exchange(ref _runState, (long)State.Opening);
            try
            {
                Interlocked.Exchange(ref _runState, (long)State.Opened);
                try
                {
                    InitRequestQueue();
                    Console.WriteLine("Tds:Opened");
                    while (RunState == State.Opened)
                    {
                        var context = _listener.AcceptTdsContext();
                        //Console.WriteLine("Web:Request");
                        var wr = CreateWorkerRequest(context);
                        ProcessRequestNoDemand(wr);
                    }
                }
                // This will occur when the listener gets shut down. Just swallow it and move on.
                catch (HttpListenerException) { }
            }
            finally { Interlocked.Exchange(ref _runState, (long)State.Closed); }
            //
            TdsRuntime.Close();
            //var totalSeconds = 90;
            //WaitForRequestsToFinish(totalSeconds * 0x3e8);
            //if (_requestQueue != null)
            //    _requestQueue.Drain();
            //WaitForRequestsToFinish((totalSeconds * 0x3e8) / 6);
        }

        #endregion

        //#region ProcessRequest

        //internal RequestQueue _requestQueue;

        //private void InitRequestQueue()
        //{
        //    _requestQueue = new RequestQueue(this, 5, 5, 3, new TimeSpan(0, 0, 30));
        //}

        //protected virtual TdsListenerContext CreateWorkerRequest(TdsListenerContext wr)
        //{
        //    return ((TdsListenerContext)Activator.CreateInstance(_workerRequestType, wr));
        //}

        //internal void ProcessRequestNoDemand(TdsListenerContext wr)
        //{
        //    var queue = _requestQueue;
        //    if (queue != null)
        //        wr = queue.GetRequestToExecute(wr);
        //    if (wr != null)
        //        ProcessRequestNow(wr);
        //}

        //internal virtual void ProcessRequestNow(TdsListenerContext wr) { }

        //internal virtual void RejectRequestNow(TdsListenerContext wr, bool silent) { }

        //#endregion
    }
}
