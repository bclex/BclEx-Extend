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
using System.Text;
using System.Diagnostics;
namespace System.Reflection
{
    /// <summary>
    /// MethodInvocationInfo
    /// </summary>
    public class MethodInvocationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationInfo"/> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="genericTypeArguments">The generic type arguments.</param>
        /// <param name="arguments">The arguments.</param>
        public MethodInvocationInfo(object proxy, MethodInfo targetMethod, StackTrace stackTrace, Type[] genericTypeArguments, object[] arguments)
        {
            Target = proxy;
            TargetMethod = targetMethod;
            GenericTypeArguments = genericTypeArguments;
            Arguments = arguments;
            StackTrace = stackTrace;
        }

        private static string GetMethodName(MethodInfo method)
        {
            var b = new StringBuilder();
            b.AppendFormat("{0}.{1}", method.DeclaringType.Name, method.Name);
            b.Append("(");
            var parameters = method.GetParameters();
            int count = (parameters != null ? parameters.Length : 0);
            int index = 0;
            foreach (var parameter in parameters)
            {
                index++;
                b.AppendFormat("{0} {1}", parameter.ParameterType.Name, parameter.Name);
                if (index < count)
                    b.Append(", ");
            }
            b.Append(")");
            return b.ToString();
        }

        /// <summary>
        /// Sets the argument.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void SetArgument(int index, object value)
        {
            Arguments[index] = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var b = new StringBuilder();
            b.AppendFormat("Calling Method: {0,30:G}\n", GetMethodName(CallingMethod));
            b.AppendFormat("Target Method: {0,30:G}\n", GetMethodName(TargetMethod));
            b.AppendLine("Arguments:");
            foreach (var parameter in TargetMethod.GetParameters())
            {
                object value = (Arguments[parameter.Position] ?? "(null)");
                b.AppendFormat("\t{0,10:G}: {1}\n", parameter.Name, value);
            }
            b.AppendLine();
            return b.ToString();
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public object[] Arguments { get; private set; }
        /// <summary>
        /// Gets the calling method.
        /// </summary>
        public MethodInfo CallingMethod
        {
            get { return (MethodInfo)StackTrace.GetFrame(0).GetMethod(); }
        }
        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        public StackTrace StackTrace { get; private set; }
        /// <summary>
        /// Gets the target.
        /// </summary>
        public object Target { get; private set; }
        /// <summary>
        /// Gets the target method.
        /// </summary>
        public MethodInfo TargetMethod { get; private set; }
        /// <summary>
        /// Gets the generic type arguments.
        /// </summary>
        public Type[] GenericTypeArguments { get; private set; }
    }
}
