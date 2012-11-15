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
using System.Reflection;
using System.Threading;
namespace System
{
    /// <summary>
    /// ExceptionEx
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static class ExceptionExtensions
    {
        private static readonly MethodInfo _prepForRemotingMethod = typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _internalPreserveStackTraceMethod = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Determines whether the specified exception is critical.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///   <c>true</c> if the specified exception is critical; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCritical(this Exception exception)
        {
#if CLR4
            return (exception is AccessViolationException || exception is NullReferenceException || exception is StackOverflowException || exception is OutOfMemoryException || exception is ThreadAbortException);
#else
            return (exception is AccessViolationException || exception is NullReferenceException || exception is StackOverflowException || exception is OutOfMemoryException || exception is ExecutionEngineException || exception is ThreadAbortException);
#endif
        }

        /// <summary>
        /// Prepares for rethrow.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static Exception PrepareForRethrow(this Exception exception) { return PrepareForRethrow(exception, false); }
        /// <summary>
        /// Prepares for rethrow.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="remoting">if set to <c>true</c> [remoting].</param>
        /// <returns></returns>
        public static Exception PrepareForRethrow(this Exception exception, bool remoting)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (remoting)
                _prepForRemotingMethod.Invoke(exception, null);
            else
                _internalPreserveStackTraceMethod.Invoke(exception, null);
            return exception;
        }
    }
}
