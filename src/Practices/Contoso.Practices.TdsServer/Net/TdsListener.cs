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
using System.Security.Permissions;
using System.Runtime;
#if CLR4
using System.Threading.Tasks;
#endif

namespace System.Net
{
    /// <summary>
    /// TdsListener
    /// </summary>
    public partial class TdsListener
    {
        private bool _active;
        private bool _exclusiveAddressUse;
        private Socket _serverSocket;
        private IPEndPoint _serverSocketEP;

        /// <summary>
        /// Initializes a new instance of the <see cref="TdsListener"/> class.
        /// </summary>
        /// <param name="localEP">The local EP.</param>
        public TdsListener(IPEndPoint localEP)
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "TdsListener", localEP);
            if (localEP == null)
                throw new ArgumentNullException("localEP");
            _serverSocketEP = localEP;
            _serverSocket = new Socket(_serverSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "TdsListener", (string)null);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TdsListener"/> class.
        /// </summary>
        /// <param name="localaddr">The localaddr.</param>
        /// <param name="port">The port.</param>
        public TdsListener(IPAddress localaddr, int port)
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "TdsListener", localaddr);
            if (localaddr == null)
                throw new ArgumentNullException("localaddr");
            if (!ValidationHelper.ValidateTcpPort(port))
                throw new ArgumentOutOfRangeException("port");
            _serverSocketEP = new IPEndPoint(localaddr, port);
            _serverSocket = new Socket(_serverSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "TdsListener", (string)null);
        }

        /// <summary>
        /// Accepts the TDS context.
        /// </summary>
        /// <returns></returns>
        public TdsListenerContext AcceptTdsContext()
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "AcceptTdsContext", (string)null);
            if (!_active)
                throw new InvalidOperationException(SR.GetString(SR.net_stopped));
            var retObject = new TdsListenerContext(_serverSocket.Accept());
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "AcceptTdsContext", retObject);
            return retObject;
        }

        /// <summary>
        /// Begins the accept TDS context.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public IAsyncResult BeginAcceptTdsContext(AsyncCallback callback, object state)
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "BeginAcceptTdsContext", (string)null);
            if (!_active)
                throw new InvalidOperationException(SR.GetString(SR.net_stopped));
            var result = _serverSocket.BeginAccept(callback, state);
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "BeginAcceptTdsContext", (string)null);
            return result;
        }

        /// <summary>
        /// Ends the accept TDS context.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        /// <returns></returns>
        public TdsListenerContext EndAcceptTdsContext(IAsyncResult asyncResult)
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "EndAcceptTdsClient", (string)null);
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            var result = (asyncResult as LazyAsyncResult);
            var socket = (result == null ? null : result.AsyncObject as Socket);
            if (socket == null)
                throw new ArgumentException(SR.GetString(SR.net_io_invalidasyncresult), "asyncResult");
            var retObject = socket.EndAccept(asyncResult);
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "EndAcceptTdsClient", retObject);
            return new TdsListenerContext(retObject);
        }

        /// <summary>
        /// Pendings this instance.
        /// </summary>
        /// <returns></returns>
        public bool Pending()
        {
            if (!_active)
                throw new InvalidOperationException(SR.GetString("net_stopped"));
            return _serverSocket.Poll(0, SelectMode.SelectRead);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Start()
        {
            Start(0x7fffffff);
        }

        /// <summary>
        /// Starts the specified backlog.
        /// </summary>
        /// <param name="backlog">The backlog.</param>
        public void Start(int backlog)
        {
            if (backlog > 0x7fffffff || backlog < 0)
                throw new ArgumentOutOfRangeException("backlog");
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "Start", (string)null);
            if (_serverSocket == null)
                throw new InvalidOperationException(SR.GetString(SR.net_InvalidSocketHandle));
            if (_active)
            {
                if (Logging.On)
                    Logging.Exit(Logging.Sockets, this, "Start", (string)null);
            }
            else
            {
                _serverSocket.Bind(_serverSocketEP);
                try { _serverSocket.Listen(backlog); }
                catch (SocketException)
                {
                    Stop();
                    throw;
                }
                _active = true;
                if (Logging.On)
                    Logging.Exit(Logging.Sockets, this, "Start", (string)null);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "Stop", (string)null);
            if (_serverSocket != null)
            {
                _serverSocket.Close();
                _serverSocket = null;
            }
            _active = false;
            _serverSocket = new Socket(_serverSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (_exclusiveAddressUse)
                _serverSocket.ExclusiveAddressUse = true;
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "Stop", (string)null);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is listening.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is listening; otherwise, <c>false</c>.
        /// </value>
        public bool IsListening
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _active; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclusive address use].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive address use]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveAddressUse
        {
            get { return _serverSocket.ExclusiveAddressUse; }
            set
            {
                if (_active)
                    throw new InvalidOperationException(SR.GetString(SR.net_tdslistener_mustbestopped));
                _serverSocket.ExclusiveAddressUse = value;
                _exclusiveAddressUse = value;
            }
        }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        public EndPoint LocalEndpoint
        {
            get
            {
                if (!_active)
                    return _serverSocketEP;
                return _serverSocket.LocalEndPoint;
            }
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        public Socket Server
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _serverSocket; }
        }

#if CLR4
        /// <summary>
        /// Accepts the TDS context async.
        /// </summary>
        /// <returns></returns>
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public Task<TdsListenerContext> AcceptTdsContextAsync()
        {
            return Task<TdsListenerContext>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(BeginAcceptTdsContext), new Func<IAsyncResult, TdsListenerContext>(EndAcceptTdsContext), null);
        }

        /// <summary>
        /// Allows the nat traversal.
        /// </summary>
        /// <param name="allowed">if set to <c>true</c> [allowed].</param>
        public void AllowNatTraversal(bool allowed)
        {
            if (_active)
                throw new InvalidOperationException(SR.GetString(SR.net_tdslistener_mustbestopped));
            if (allowed)
                _serverSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            else
                _serverSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
        }

        /// <summary>
        /// Creates the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public static TdsListener Create(int port)
        {
            if (Logging.On)
                Logging.Enter(Logging.Sockets, "TdsListener.Create", "Port: " + port);
            if (!ValidationHelper.ValidateTcpPort(port))
                throw new ArgumentOutOfRangeException("port");
            var listener = new TdsListener(IPAddress.IPv6Any, port)
            {
                //Server = { DualMode = true }
            };
            if (Logging.On)
                Logging.Exit(Logging.Sockets, "TdsListener.Create", "Port: " + port);
            return listener;
        }
#endif
    }
}
