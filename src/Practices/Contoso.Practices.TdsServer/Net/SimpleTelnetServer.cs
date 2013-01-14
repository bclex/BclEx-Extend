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
using System.Text;
using System.Threading;

namespace System.Net
{
    /// <summary>
    /// SimpleTelnetServer
    /// </summary>
    public class SimpleTelnetServer
    {
        private TcpListener _tcpListener;
        private Thread _listenThread;

        public SimpleTelnetServer()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 1433);
            _listenThread = new Thread(new ThreadStart(ListenForClients));
            _listenThread.Start();
        }

        private void ListenForClients()
        {
            _tcpListener.Start();
            while (true)
            {
                // blocks until a client has connected to the server
                var client = _tcpListener.AcceptTcpClient();
                // create a thread to handle communication with connected client
                var clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            var e = new ASCIIEncoding();
            using (var tcpClient = (TcpClient)client)
            {
                var s = tcpClient.GetStream();
                EmitWelcome(s, e);
                var b = new byte[4096];
                int bRead;
                while (true)
                {
                    bRead = 0;
                    try { bRead = s.Read(b, 0, 4096); } // blocks until a client sends a message
                    catch { break; } // a socket error has occured
                    if (bRead == 0) // the client has disconnected from the server
                        break;
                    // message has successfully been received
                    Console.Write(e.GetString(b, 0, bRead));
                }
            }
        }

        private static void EmitWelcome(NetworkStream s, ASCIIEncoding e)
        {
            var b = e.GetBytes("Hello Client!\n");
            s.Write(b, 0, b.Length); s.Flush();
        }
    }
}

