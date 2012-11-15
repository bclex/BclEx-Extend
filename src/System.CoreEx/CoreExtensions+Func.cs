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
using System.Globalization;
using System.Threading;
namespace System
{
    static partial class CoreExtensions
    {
        /// <summary>
        /// Timeouts the invoke.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TimeoutInvoke<TResult>(this Func<TResult> source, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Timeouts the invoke.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TimeoutInvoke<T1, TResult>(this Func<T1, TResult> source, T1 arg1, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Timeouts the invoke.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TimeoutInvoke<T1, T2, TResult>(this Func<T1, T2, TResult> source, T1 arg1, T2 arg2, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Timeouts the invoke.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TimeoutInvoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> source, T1 arg1, T2 arg2, T3 arg3, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Timeouts the invoke.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TimeoutInvoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> source, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3, arg4); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
#if CLR4
        /// <summary>
        /// Tries the timeout.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TryTimeout<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> source, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3, arg4, arg5); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Tries the timeout.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        /// <param name="arg6">The arg6.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TryTimeout<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> source, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3, arg4, arg5, arg6); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Tries the timeout.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        /// <param name="arg6">The arg6.</param>
        /// <param name="arg7">The arg7.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TryTimeout<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> source, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3, arg4, arg5, arg6, arg7); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
        /// <summary>
        /// Tries the timeout.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        /// <param name="arg6">The arg6.</param>
        /// <param name="arg7">The arg7.</param>
        /// <param name="arg8">The arg8.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <returns></returns>
        public static TResult TryTimeout<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> source, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Func<TResult> action = () => { threadToKill = Thread.CurrentThread; return source(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); };
            var result = action.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
                return action.EndInvoke(result);
            threadToKill.Abort();
            throw new TimeoutException();
        }
#endif
    }
}
