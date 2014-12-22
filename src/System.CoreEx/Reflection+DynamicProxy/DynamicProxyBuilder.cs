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
using System.Reflection.Emit;
namespace System.Reflection
{
    /// <summary>
    /// IDynamicProxyBuilder
    /// </summary>
    public interface IDynamicProxyBuilder
    {
        /// <summary>
        /// Creates the type of the proxied.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="baseInterfaces">The base interfaces.</param>
        /// <param name="serializationSupport">if set to <c>true</c> [serialization support].</param>
        /// <returns></returns>
        Type CreateProxiedType(Type baseType, Type[] baseInterfaces, bool serializationSupport);
    }

    /// <summary>
    /// DynamicProxyBuilder
    /// </summary>
    public class DynamicProxyBuilder : IDynamicProxyBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyBuilder"/> class.
        /// </summary>
        public DynamicProxyBuilder()
            : this(new DynamicProxyTypeEmitter(new DynamicProxyMethodEmitter())) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyBuilder"/> class.
        /// </summary>
        /// <param name="proxyTypeEmitter">The proxy type emitter.</param>
        public DynamicProxyBuilder(IDynamicProxyTypeEmitter proxyTypeEmitter)
        {
            ProxyTypeEmitter = proxyTypeEmitter;
        }

        /// <summary>
        /// Gets the proxy type emitter.
        /// </summary>
        public IDynamicProxyTypeEmitter ProxyTypeEmitter { get; private set; }

        /// <summary>
        /// Creates the type of the proxied.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="baseInterfaces">The base interfaces.</param>
        /// <param name="serializationSupport"></param>
        /// <returns></returns>
        public Type CreateProxiedType(Type baseType, Type[] baseInterfaces, bool serializationSupport)
        {
            var currentDomain = AppDomain.CurrentDomain;
            var typeName = string.Format("{0}Proxy", baseType.Name);
            var assemblyName = string.Format("{0}Assembly", typeName);
            var moduleName = string.Format("{0}Module", typeName);
            var name = new AssemblyName(assemblyName);
            var b = currentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run).DefineDynamicModule(moduleName);
            return ProxyTypeEmitter.CreateProxiedType(b, baseType, baseInterfaces, serializationSupport);
        }
    }
}
