//#region License
///*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//using System.Threading;

//namespace System.Net.Util
//{
//    internal sealed class CancellationTokenHelper : IDisposable
//    {
//        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
//        private int _state;
//        private const int STATE_CANCELED = 2;
//        private const int STATE_CANCELING = 1;
//        private const int STATE_CREATED = 0;
//        private const int STATE_DISPOSED = 4;
//        private const int STATE_DISPOSING = 3;
//        internal static readonly CancellationTokenHelper StaticDisposed = GetStaticDisposedHelper();

//        public CancellationTokenHelper(bool canceled)
//        {
//            if (canceled)
//                _cts.Cancel();
//            _state = (canceled ? 2 : 0);
//        }

//        public void Cancel()
//        {
//            WaitCallback callBack = null;
//            if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
//            {
//                if (callBack == null)
//                    callBack = delegate(object _)
//                    {
//                        try { _cts.Cancel(); }
//                        catch { }
//                        finally
//                        {
//                            if (Interlocked.CompareExchange(ref _state, 2, 1) == 3)
//                            {
//                                _cts.Dispose();
//                                Interlocked.Exchange(ref _state, 4);
//                            }
//                        }
//                    };
//                ThreadPool.UnsafeQueueUserWorkItem(callBack, null);
//            }
//        }

//        public void Dispose()
//        {
//            switch (Interlocked.Exchange(ref _state, 3))
//            {
//                case 0:
//                case 2: _cts.Dispose(); Interlocked.Exchange(ref _state, 4); return;
//                case 1:
//                case 3: break;
//                case 4: Interlocked.Exchange(ref _state, 4); break;
//                default: return;
//            }
//        }

//        private static CancellationTokenHelper GetStaticDisposedHelper()
//        {
//            var helper = new CancellationTokenHelper(false);
//            helper.Dispose();
//            return helper;
//        }

//        internal CancellationToken Token
//        {
//            get { return _cts.Token; }
//        }
//    }
//}
