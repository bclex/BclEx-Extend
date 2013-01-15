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

namespace Contoso.Practices.TdsServer
{
    /// <summary>
    /// TdsContext
    /// </summary>
    public partial class TdsContext
    {
        private TdsRequest _request;
        private DateTime _utcTimestamp;
        internal readonly object ThreadContextId;
        private TdsListenerContext _wr;

        /// <summary>
        /// Initializes a new instance of the <see cref="TdsContext"/> class.
        /// </summary>
        /// <param name="wr">The wr.</param>
        public TdsContext(TdsListenerContext wr)
        {
            _timeoutStartTimeUtcTicks = -1L;
            _timeoutTicks = -1L;
            _threadAbortOnTimeout = true;
            ThreadContextId = new object();
            _wr = wr;
            Init(new TdsRequest(wr, this));

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TdsContext"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public TdsContext(TdsRequest request)
        {
            _timeoutStartTimeUtcTicks = -1L;
            _timeoutTicks = -1L;
            _threadAbortOnTimeout = true;
            ThreadContextId = new object();
            Init(request);
            request.Context = this;
        }

        private void Init(TdsRequest request)
        {
            _request = request;
            _utcTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public TdsRequest Request
        {
            get { return _request; }
        }
    }
}
