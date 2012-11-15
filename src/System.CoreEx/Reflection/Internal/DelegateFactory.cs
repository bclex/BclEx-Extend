// from automapper
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace System.Reflection.Internal
{
    internal static class DelegateFactory
    {
        private static readonly Dictionary<Type, LateBoundCtor> _ctorCache = new Dictionary<Type, LateBoundCtor>();
        private static readonly object _ctorCacheSync = new object();

        public static LateBoundCtor CreateCtor(Type type)
        {
            LateBoundCtor ctor;
            if (!_ctorCache.TryGetValue(type, out ctor))
                lock (_ctorCacheSync)
                {
                    if (_ctorCache.TryGetValue(type, out ctor))
                        return ctor;
                    ctor = Expression.Lambda<LateBoundCtor>(Expression.Convert(Expression.New(type), typeof(object)), new ParameterExpression[0]).Compile();
                    _ctorCache.Add(type, ctor);
                }
            return ctor;
        }

        private static DynamicMethod CreateDynamicMethod(MemberInfo member, Type sourceType)
        {
            if (sourceType.IsInterface)
                return new DynamicMethod("Set" + member.Name, null, new Type[] { typeof(object), typeof(object) }, sourceType.Assembly.ManifestModule, true);
            return new DynamicMethod("Set" + member.Name, null, new Type[] { typeof(object), typeof(object) }, sourceType, true);
        }

        public static LateBoundFieldGet CreateGet(FieldInfo field)
        {
            ParameterExpression expression;
            return Expression.Lambda<LateBoundFieldGet>(Expression.Convert(Expression.Field(Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), field.DeclaringType), field), typeof(object)), new ParameterExpression[] { expression }).Compile();
        }

        public static LateBoundMethod CreateGet(MethodInfo method)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            return Expression.Lambda<LateBoundMethod>(Expression.Convert(Expression.Call(Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), method.DeclaringType), method, CreateParameterExpressions(method, expression2 = Expression.Parameter(typeof(object[]), "arguments"))), typeof(object)), new ParameterExpression[] { expression, expression2 }).Compile();
        }

        public static LateBoundPropertyGet CreateGet(PropertyInfo property)
        {
            ParameterExpression expression;
            return Expression.Lambda<LateBoundPropertyGet>(Expression.Convert(Expression.Property(Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), property.DeclaringType), property), typeof(object)), new ParameterExpression[] { expression }).Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select<ParameterInfo, UnaryExpression>(delegate(ParameterInfo parameter, int index)
            {
                return Expression.Convert(Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)), parameter.ParameterType);
            }).ToArray<UnaryExpression>();
        }

        public static LateBoundFieldSet CreateSet(FieldInfo field)
        {
            Type declaringType = field.DeclaringType;
            DynamicMethod method = CreateDynamicMethod(field, declaringType);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, declaringType);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Unbox_Any, field.FieldType);
            iLGenerator.Emit(OpCodes.Stfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            return (LateBoundFieldSet)method.CreateDelegate(typeof(LateBoundFieldSet));
        }

        public static LateBoundPropertySet CreateSet(PropertyInfo property)
        {
            Type declaringType = property.DeclaringType;
            MethodInfo setMethod = property.GetSetMethod(true);
            DynamicMethod method = CreateDynamicMethod(property, declaringType);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, declaringType);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
            iLGenerator.Emit(OpCodes.Callvirt, setMethod);
            iLGenerator.Emit(OpCodes.Ret);
            return (LateBoundPropertySet)method.CreateDelegate(typeof(LateBoundPropertySet));
        }

        private static DynamicMethod CreateValueTypeDynamicMethod(MemberInfo member, Type sourceType)
        {
            if (sourceType.IsInterface)
                return new DynamicMethod("Set" + member.Name, null, new Type[] { typeof(object).MakeByRefType(), typeof(object) }, sourceType.Assembly.ManifestModule, true);
            return new DynamicMethod("Set" + member.Name, null, new Type[] { typeof(object).MakeByRefType(), typeof(object) }, sourceType, true);
        }

        public static LateBoundValueTypePropertySet CreateValueTypeSet(PropertyInfo property)
        {
            Type declaringType = property.DeclaringType;
            MethodInfo setMethod = property.GetSetMethod(true);
            DynamicMethod method = CreateValueTypeDynamicMethod(property, declaringType);
            ILGenerator iLGenerator = method.GetILGenerator();
            method.InitLocals = true;
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldind_Ref);
            iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldloca_S, 0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Castclass, property.PropertyType);
            iLGenerator.Emit(OpCodes.Call, setMethod);
            iLGenerator.Emit(OpCodes.Ret);
            return (LateBoundValueTypePropertySet)method.CreateDelegate(typeof(LateBoundValueTypePropertySet));
        }
    }
}