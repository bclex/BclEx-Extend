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
    /// MethodInterceptorHandler
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <param name="targetMethod">The target method.</param>
    /// <param name="stackTrace">The stack trace.</param>
    /// <param name="genericTypeArguments">The generic type arguments.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns></returns>
    public delegate object MethodInterceptorHandler(object proxy, MethodInfo targetMethod, StackTrace stackTrace, Type[] genericTypeArguments, object[] arguments);
}
