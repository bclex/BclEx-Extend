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
using System.Diagnostics;
using System.Collections.Generic;
namespace System.Reflection
{
    /// <summary>
    /// IDynamicProxyMethodEmitter
    /// </summary>
    public interface IDynamicProxyMethodEmitter
    {
        /// <summary>
        /// Creates the proxied method.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="field">The field.</param>
        /// <param name="method">The method.</param>
        void CreateProxiedMethod(TypeBuilder b, FieldInfo field, MethodInfo method);
    }

    /// <summary>
    /// DynamicProxyMethodEmitter
    /// </summary>
    public class DynamicProxyMethodEmitter : IDynamicProxyMethodEmitter
    {
        static DynamicProxyMethodEmitter()
        {
            s_stindMap["Bool&"] = OpCodes.Stind_I1;
            s_stindMap["Int8&"] = OpCodes.Stind_I1;
            s_stindMap["Uint8&"] = OpCodes.Stind_I1;
            s_stindMap["Int16&"] = OpCodes.Stind_I2;
            s_stindMap["Uint16&"] = OpCodes.Stind_I2;
            s_stindMap["Uint32&"] = OpCodes.Stind_I4;
            s_stindMap["Int32&"] = OpCodes.Stind_I4;
            s_stindMap["IntPtr"] = OpCodes.Stind_I4;
            s_stindMap["Uint64&"] = OpCodes.Stind_I8;
            s_stindMap["Int64&"] = OpCodes.Stind_I8;
            s_stindMap["Float32&"] = OpCodes.Stind_R4;
            s_stindMap["Float64&"] = OpCodes.Stind_R8;
        }

        #region Method

        private static readonly MethodInfo s_getGenericMethodFromHandleMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }, null);
        private static readonly MethodInfo s_getMethodFromHandleMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
        private static readonly MethodInfo s_getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");
        private static readonly ConstructorInfo s_notImplementedConstructor = typeof(NotImplementedException).GetConstructor(new Type[0]);
        private static readonly Dictionary<string, OpCode> s_stindMap = new Dictionary<string, OpCode>();
        //
        private static readonly MethodInfo s_handlerMethod = typeof(IMethodInterceptor).GetMethod("Intercept");
        private static readonly ConstructorInfo s_invocationInfoConstructor = typeof(MethodInvocationInfo).GetConstructor(new[] { typeof(object), typeof(MethodInfo), typeof(StackTrace), typeof(Type[]), typeof(object[]) });
        private static readonly PropertyInfo s_interceptorProperty = typeof(IDynamicProxy).GetProperty("Interceptor");
        private static readonly MethodInfo s_getInterceptorMethod = s_interceptorProperty.GetGetMethod();

        /// <summary>
        /// Creates the proxied method.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="field">The field.</param>
        /// <param name="method">The method.</param>
        public void CreateProxiedMethod(TypeBuilder b, FieldInfo field, MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>();
            foreach (var parameter in parameters)
                parameterTypes.Add(parameter.ParameterType);
            const MethodAttributes attributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
            var methodBuilder = b.DefineMethod(method.Name, attributes, CallingConventions.HasThis, method.ReturnType, parameterTypes.ToArray());
            var genericArguments = method.GetGenericArguments();
            if ((genericArguments != null) && (genericArguments.Length > 0))
            {
                var genericParameters = new List<string>();
                for (int index = 0; index < genericArguments.Length; index++)
                    genericParameters.Add(string.Format("T{0}", index));
                methodBuilder.DefineGenericParameters(genericParameters.ToArray());
            }
            var w = methodBuilder.GetILGenerator();
            EmitMethodBody(w, method, field);
        }

        /// <summary>
        /// Emits the method body.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="method">The method.</param>
        /// <param name="field">The field.</param>
        public void EmitMethodBody(ILGenerator w, MethodInfo method, FieldInfo field)
        {
            var parameters = method.GetParameters();
            w.DeclareLocal(typeof(object[]));
            w.DeclareLocal(typeof(MethodInvocationInfo));
            w.DeclareLocal(typeof(Type[]));
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Callvirt, s_getInterceptorMethod);
            var label = w.DefineLabel();
            w.Emit(OpCodes.Dup);
            w.Emit(OpCodes.Ldnull);
            w.Emit(OpCodes.Bne_Un, label);
            w.Emit(OpCodes.Newobj, s_notImplementedConstructor);
            w.Emit(OpCodes.Throw);
            w.MarkLabel(label);
            w.Emit(OpCodes.Ldarg_0);
            var declaringType = method.DeclaringType;
            w.Emit(OpCodes.Ldtoken, method);
            if (declaringType.IsGenericType)
            {
                w.Emit(OpCodes.Ldtoken, declaringType);
                w.Emit(OpCodes.Call, s_getGenericMethodFromHandleMethod);
            }
            else
                w.Emit(OpCodes.Call, s_getMethodFromHandleMethod);
            w.Emit(OpCodes.Castclass, typeof(MethodInfo));
            PushStackTrace(w);
            PushGenericArguments(w, method);
            PushArguments(w, parameters);
            w.Emit(OpCodes.Newobj, s_invocationInfoConstructor);
            w.Emit(OpCodes.Stloc_1);
            w.Emit(OpCodes.Ldloc_1);
            w.Emit(OpCodes.Callvirt, s_handlerMethod);
            SaveRefArguments(w, parameters);
            PackageReturnType(w, method);
            w.Emit(OpCodes.Ret);
        }

        private static OpCode GetStindInstruction(Type parameterType)
        {
            if ((parameterType.IsClass) && (!parameterType.Name.EndsWith("&")))
                return OpCodes.Stind_Ref;
            string name = parameterType.Name;
            if ((!s_stindMap.ContainsKey(name)) && (parameterType.IsByRef))
                return OpCodes.Stind_Ref;
            return s_stindMap[name];
        }

        private static void PackageReturnType(ILGenerator w, MethodInfo method)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof(void))
                w.Emit(OpCodes.Pop);
            else
                w.Emit(OpCodes.Unbox_Any, returnType);
        }

        private static void PushStackTrace(ILGenerator w)
        {
            w.Emit(OpCodes.Ldnull);
        }

        #endregion

        #region Arguments

        private static void PushArguments(ILGenerator w, ParameterInfo[] parameters)
        {
            int count = (parameters == null ? 0 : parameters.Length);
            w.Emit(OpCodes.Ldc_I4, count);
            w.Emit(OpCodes.Newarr, typeof(object));
            w.Emit(OpCodes.Stloc_S, 0);
            if (count == 0)
                w.Emit(OpCodes.Ldloc_S, 0);
            else
            {
                int index = 0;
                foreach (var parameter in parameters)
                {
                    var parameterType = parameter.ParameterType;
                    w.Emit(OpCodes.Ldloc_S, 0);
                    w.Emit(OpCodes.Ldc_I4, index);
                    if (parameter.IsOut)
                    {
                        w.Emit(OpCodes.Ldnull);
                        w.Emit(OpCodes.Stelem_Ref);
                    }
                    else
                    {
                        w.Emit(OpCodes.Ldarg, index + 1);
                        bool isGenericParameter = parameterType.IsGenericParameter;
                        if ((parameterType.IsValueType) || (isGenericParameter))
                            w.Emit(OpCodes.Box, parameterType);
                        w.Emit(OpCodes.Stelem_Ref);
                    }
                    index++;
                }
                w.Emit(OpCodes.Ldloc_S, 0);
            }
        }

        private static void SaveRefArguments(ILGenerator w, ParameterInfo[] parameters)
        {
            var method = typeof(MethodInvocationInfo).GetMethod("get_Arguments");
            w.Emit(OpCodes.Ldloc_1);
            w.Emit(OpCodes.Call, method);
            w.Emit(OpCodes.Stloc_0);
            foreach (var parameter in parameters)
            {
                string name = parameter.ParameterType.Name;
                if ((parameter.ParameterType.IsByRef) && (name.EndsWith("&")))
                {
                    w.Emit(OpCodes.Ldarg, (int)(parameter.Position + 1));
                    w.Emit(OpCodes.Ldloc_0);
                    w.Emit(OpCodes.Ldc_I4, parameter.Position);
                    w.Emit(OpCodes.Ldelem_Ref);
                    var cls = Type.GetType(name.Replace("&", ""));
                    w.Emit(OpCodes.Unbox_Any, cls);
                    var stindInstruction = GetStindInstruction(parameter.ParameterType);
                    w.Emit(stindInstruction);
                }
            }
        }

        private static void PushGenericArguments(ILGenerator w, MethodInfo method)
        {
            var genericArguments = method.GetGenericArguments();
            int count = (genericArguments == null ? 0 : genericArguments.Length);
            w.Emit(OpCodes.Ldc_I4, count);
            w.Emit(OpCodes.Newarr, typeof(Type));
            if (count != 0)
                for (int index = 0; index < count; index++)
                {
                    var cls = genericArguments[index];
                    w.Emit(OpCodes.Dup);
                    w.Emit(OpCodes.Ldc_I4, index);
                    w.Emit(OpCodes.Ldtoken, cls);
                    w.Emit(OpCodes.Call, s_getTypeFromHandleMethod);
                    w.Emit(OpCodes.Stelem_Ref);
                }
        }

        #endregion
    }
}
