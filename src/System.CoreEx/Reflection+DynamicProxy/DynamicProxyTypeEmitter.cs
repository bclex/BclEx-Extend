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
using System.Runtime.Serialization;
using System.Reflection.Emit;
using System.Collections.Generic;
namespace System.Reflection
{
    /// <summary>
    /// IDynamicProxyTypeEmitter
    /// </summary>
    public interface IDynamicProxyTypeEmitter
    {
        /// <summary>
        /// Creates the type of the proxied.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="baseInterfaces">The base interfaces.</param>
        /// <returns></returns>
        Type CreateProxiedType(ModuleBuilder b, Type baseType, Type[] baseInterfaces);
    }

    internal class DynamicProxyTypeEmitter : IDynamicProxyTypeEmitter
    {
        #region Class-types

        /// <summary>
        /// 
        /// </summary>
        public class ProxyDummy { }
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class ProxyObjectReference : IObjectReference, ISerializable
        {
            private readonly Type _baseType;
            private readonly IDynamicProxy _proxy;

            protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
            {
                string typeName = info.GetString("__baseType");
                _baseType = Type.GetType(typeName, true, false);
                var baseInterfaces = new List<Type>();
                int count = info.GetInt32("__baseInterfaceCount");
                for (int index = 0; index < count; index++)
                {
                    string name = string.Format("__baseInterface{0}", index);
                    baseInterfaces.Add(Type.GetType(info.GetString(name), true, false));
                }
                var proxyType = new DynamicProxyBuilder().CreateProxiedType(_baseType, baseInterfaces.ToArray());
                _proxy = (IDynamicProxy)Activator.CreateInstance(proxyType, new object[] { info, context });
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context) { }

            public object GetRealObject(StreamingContext context)
            {
                return _proxy;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyTypeEmitter"/> class.
        /// </summary>
        public DynamicProxyTypeEmitter()
            : this(new DynamicProxyMethodEmitter()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyTypeEmitter"/> class.
        /// </summary>
        /// <param name="proxyMethodEmitter">The proxy method emitter.</param>
        public DynamicProxyTypeEmitter(IDynamicProxyMethodEmitter proxyMethodEmitter)
        {
            ProxyMethodEmitter = proxyMethodEmitter;
        }

        /// <summary>
        /// Gets the proxy method emitter.
        /// </summary>
        public IDynamicProxyMethodEmitter ProxyMethodEmitter { get; private set; }

        #region Type

        private static readonly ConstructorInfo s_baseConstructor = typeof(object).GetConstructor(new Type[0]);
        private static readonly MethodInfo s_getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");
        private static readonly MethodInfo s_addValueMethod = typeof(SerializationInfo).GetMethod("AddValue", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(object) }, null);
        private static readonly MethodInfo s_getValueMethod = typeof(SerializationInfo).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(Type) }, null);
        private static readonly MethodInfo s_setTypeMethod = typeof(SerializationInfo).GetMethod("SetType", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);

        public Type CreateProxiedType(ModuleBuilder b, Type baseType, Type[] baseInterfaces)
        {
            string typeName = string.Format("{0}Proxy", baseType.Name);
            //
            const TypeAttributes attributes = TypeAttributes.BeforeFieldInit | TypeAttributes.AutoClass | TypeAttributes.Public;
            var interfaces = new List<Type>();
            if ((baseInterfaces != null) && (baseInterfaces.Length > 0))
                interfaces.AddRange(baseInterfaces);
            var parent = baseType;
            if (baseType.IsInterface)
            {
                parent = typeof(ProxyDummy);
                interfaces.Add(baseType);
            }
            foreach (var @interface in interfaces.ToArray())
                BuildInterfacesRecurse(@interface, interfaces);
            if (!interfaces.Contains(typeof(ISerializable)))
                interfaces.Add(typeof(ISerializable));
            var typeBuilder = b.DefineType(typeName, attributes, parent, interfaces.ToArray());
            var constructorBuilder = DefineConstructor(typeBuilder);
            var implementor = new DynamicProxyImplementor();
            implementor.ImplementProxy(typeBuilder);
            var methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var proxies = new List<MethodInfo>();
            BuildMethods(interfaces, methods, proxies);
            var interceptorField = implementor.InterceptorField;
            foreach (var proxy in proxies)
                if (proxy.DeclaringType != typeof(ISerializable))
                    ProxyMethodEmitter.CreateProxiedMethod(typeBuilder, interceptorField, proxy);
            AddSerializationSupport(typeBuilder, baseType, baseInterfaces, interceptorField, constructorBuilder);
            return typeBuilder.CreateType();
        }

        private static void BuildInterfacesRecurse(Type type, ICollection<Type> interfaces)
        {
            var childInterfaces = type.GetInterfaces();
            if ((childInterfaces != null) && (childInterfaces.Length != 0))
                foreach (var childInterface in childInterfaces)
                    if (!interfaces.Contains(childInterface))
                    {
                        interfaces.Add(childInterface);
                        BuildInterfacesRecurse(childInterface, interfaces);
                    }
        }

        private static void BuildMethods(IEnumerable<Type> interfaces, IEnumerable<MethodInfo> methods, ICollection<MethodInfo> proxies)
        {
            foreach (var method in methods)
                if (((!method.IsPrivate) && (!method.IsFinal)) && ((method.IsVirtual) || (method.IsAbstract)))
                    proxies.Add(method);
            foreach (var @interface in interfaces)
                foreach (var interfaceMethod in @interface.GetMethods())
                    if (!proxies.Contains(interfaceMethod))
                        proxies.Add(interfaceMethod);
        }

        private static void AddSerializationSupport(TypeBuilder b, Type baseType, Type[] baseInterfaces, FieldInfo interceptorField, ConstructorBuilder constructorBuilder)
        {
            var customAttributeBuilder = new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(new Type[0]), new object[0]);
            b.SetCustomAttribute(customAttributeBuilder);
            DefineSerializationConstructor(b, baseInterfaces, interceptorField, constructorBuilder);
            ImplementGetObjectData(b, baseType, baseInterfaces, interceptorField);
        }

        private static ConstructorBuilder DefineConstructor(TypeBuilder b)
        {
            const MethodAttributes attributes = MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            var methodBuilder = b.DefineConstructor(attributes, CallingConventions.Standard, new Type[0]);
            var w = methodBuilder.GetILGenerator();
            methodBuilder.SetImplementationFlags(MethodImplAttributes.IL);
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Call, s_baseConstructor);
            w.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private static void DefineSerializationConstructor(TypeBuilder b, Type[] baseInterfaces, FieldInfo interceptorField, ConstructorBuilder constructorBuilder)
        {
            const MethodAttributes attributes = MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            var parameterTypes = new[] { typeof(SerializationInfo), typeof(StreamingContext) };
            var methodBuilder = b.DefineConstructor(attributes, CallingConventions.Standard, parameterTypes);
            var w = methodBuilder.GetILGenerator();
            var local = w.DeclareLocal(typeof(Type));
            methodBuilder.SetImplementationFlags(MethodImplAttributes.IL);
            w.Emit(OpCodes.Ldtoken, typeof(IMethodInterceptor));
            w.Emit(OpCodes.Call, s_getTypeFromHandleMethod);
            w.Emit(OpCodes.Stloc, local);
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Call, constructorBuilder);
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Ldstr, "__interceptor");
            w.Emit(OpCodes.Ldloc, local);
            w.Emit(OpCodes.Callvirt, s_getValueMethod);
            w.Emit(OpCodes.Castclass, typeof(IMethodInterceptor));
            w.Emit(OpCodes.Stfld, interceptorField);
            w.Emit(OpCodes.Ret);
        }

        private static void ImplementGetObjectData(TypeBuilder b, Type baseType, Type[] baseInterfaces, FieldInfo interceptorField)
        {
            const MethodAttributes attributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
            var parameterTypes = new[] { typeof(SerializationInfo), typeof(StreamingContext) };
            var w = b.DefineMethod("GetObjectData", attributes, typeof(void), parameterTypes).GetILGenerator();
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Ldtoken, typeof(ProxyObjectReference));
            w.Emit(OpCodes.Call, s_getTypeFromHandleMethod);
            w.Emit(OpCodes.Callvirt, s_setTypeMethod);
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Ldstr, "__interceptor");
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Ldfld, interceptorField);
            w.Emit(OpCodes.Callvirt, s_addValueMethod);
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Ldstr, "__baseType");
            w.Emit(OpCodes.Ldstr, baseType.AssemblyQualifiedName);
            w.Emit(OpCodes.Callvirt, s_addValueMethod);
            int length = baseInterfaces.Length;
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Ldstr, "__baseInterfaceCount");
            w.Emit(OpCodes.Ldc_I4, length);
            w.Emit(OpCodes.Box, typeof(int));
            w.Emit(OpCodes.Callvirt, s_addValueMethod);
            int index = 0;
            foreach (var baseInterface in baseInterfaces)
            {
                w.Emit(OpCodes.Ldarg_1);
                w.Emit(OpCodes.Ldstr, string.Format("__baseInterface{0}", index++));
                w.Emit(OpCodes.Ldstr, baseInterface.AssemblyQualifiedName);
                w.Emit(OpCodes.Callvirt, s_addValueMethod);
            }
            w.Emit(OpCodes.Ret);
        }

        #endregion
    }
}
