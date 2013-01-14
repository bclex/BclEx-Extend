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

namespace System.Net
{
    // HttpWorkerRequest
    public partial class TdsListenerContext
    {
        private DateTime _startTime;
        private Socket _clientSocket;
        //private NetworkStream _dataStream;

        internal TdsListenerContext(Socket acceptedSocket)
        {
            _startTime = DateTime.UtcNow;
            if (Logging.On)
                Logging.Enter(Logging.Sockets, this, "TdsListenerContext", acceptedSocket);
            _clientSocket = acceptedSocket;
            if (Logging.On)
                Logging.Exit(Logging.Sockets, this, "TdsListenerContext", (string)null);
        }

        internal virtual DateTime GetStartTime()
        {
            return _startTime;
        }

        internal virtual void ResetStartTime()
        {
            _startTime = DateTime.UtcNow;
        }

        public virtual bool IsClientConnected()
        {
            return _clientSocket.IsBound;
        }

        public virtual string GetRemoteAddress()
        {
            return _clientSocket.RemoteEndPoint.ToString();
        }

        public virtual string GetLocalAddress()
        {
            return _clientSocket.LocalEndPoint.ToString();
        }
    }
}
