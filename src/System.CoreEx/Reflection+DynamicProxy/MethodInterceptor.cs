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
using System.Diagnostics;
namespace System.Reflection
{
    /// <summary>
    /// IMethodInterceptor
    /// </summary>
    public interface IMethodInterceptor
    {
        /// <summary>
        /// Intercepts the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object Intercept(MethodInvocationInfo info);
    }

    /// <summary>
    /// MethodInterceptor
    /// </summary>
    public abstract class MethodInterceptor : IMethodInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInterceptor"/> class.
        /// </summary>
        protected MethodInterceptor() { }

        /// <summary>
        /// Intercepts the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="genericTypeArguments">The generic type arguments.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public object Intercept(object proxy, MethodInfo targetMethod, StackTrace stackTrace, Type[] genericTypeArguments, object[] arguments) { return Intercept(new MethodInvocationInfo(proxy, targetMethod, stackTrace, genericTypeArguments, arguments)); }
        /// <summary>
        /// Intercepts the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public abstract object Intercept(MethodInvocationInfo info);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.MethodInterceptor"/> to <see cref="System.Reflection.MethodInterceptorHandler"/>.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator MethodInterceptorHandler(MethodInterceptor interceptor)
        {
            return interceptor.Intercept;
        }
    }
}
