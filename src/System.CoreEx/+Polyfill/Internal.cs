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
#if CLRSQL
namespace System
{
    /// <summary>
    /// Action
    /// </summary>
    public delegate void Action();
    /// <summary>
    /// Action
    /// </summary>
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
    /// <summary>
    /// Action
    /// </summary>
    public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Action
    /// </summary>
    public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Func
    /// </summary>
    public delegate TResult Func<TResult>();
    /// <summary>
    /// Func
    /// </summary>
    public delegate TResult Func<T, TResult>(T arg);
    /// <summary>
    /// Func
    /// </summary>
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    /// <summary>
    /// Func
    /// </summary>
    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    /// <summary>
    /// Func
    /// </summary>
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// ExtensionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ExtensionAttribute : Attribute { }
}
#endif
