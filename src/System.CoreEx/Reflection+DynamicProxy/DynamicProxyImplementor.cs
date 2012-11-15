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
    internal class DynamicProxyImplementor
    {
        private FieldBuilder _fieldBuilder;

        public void ImplementProxy(TypeBuilder b)
        {
            var dynamicProxyType = typeof(IDynamicProxy);
            b.AddInterfaceImplementation(dynamicProxyType);
            _fieldBuilder = b.DefineField("__interceptor", typeof(IMethodInterceptor), FieldAttributes.Private);
            const MethodAttributes attributes = MethodAttributes.SpecialName | MethodAttributes.VtableLayoutMask | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
            var getMethodBuilder = b.DefineMethod("get_Interceptor", attributes, CallingConventions.HasThis, typeof(IMethodInterceptor), new Type[0]);
            getMethodBuilder.SetImplementationFlags(MethodImplAttributes.IL);
            var w = getMethodBuilder.GetILGenerator();
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Ldfld, _fieldBuilder);
            w.Emit(OpCodes.Ret);
            var setMethodBuilder = b.DefineMethod("set_Interceptor", attributes, CallingConventions.HasThis, typeof(void), new Type[] { typeof(IMethodInterceptor) });
            setMethodBuilder.SetImplementationFlags(MethodImplAttributes.IL);
            w = setMethodBuilder.GetILGenerator();
            w.Emit(OpCodes.Ldarg_0);
            w.Emit(OpCodes.Ldarg_1);
            w.Emit(OpCodes.Stfld, _fieldBuilder);
            w.Emit(OpCodes.Ret);
            b.DefineMethodOverride(setMethodBuilder, dynamicProxyType.GetMethod("set_Interceptor"));
            b.DefineMethodOverride(getMethodBuilder, dynamicProxyType.GetMethod("get_Interceptor"));
        }

        public FieldBuilder InterceptorField
        {
            get { return _fieldBuilder; }
        }
    }
}
