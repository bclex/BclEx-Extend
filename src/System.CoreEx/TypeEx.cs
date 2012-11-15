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
using System.Collections.Generic;
#if !CLRSQL
using System.Linq;
#endif
namespace System
{
    /// <summary>
    /// TypeEx
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, string name) { return GetGenericMethod(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, name, new Type[] { }, new Type[] { }); }
        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="genericTypes">The generic types.</param>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] genericTypes, Type[] types) { return GetGenericMethod(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, name, genericTypes, types); }
        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingAttr">The binding attr.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, BindingFlags bindingAttr, string name) { return GetGenericMethod(type, bindingAttr, name, new Type[] { }, new Type[] { }); }
        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingAttr">The binding attr.</param>
        /// <param name="name">The name.</param>
        /// <param name="genericTypes">The generic types.</param>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, BindingFlags bindingAttr, string name, Type[] genericTypes, Type[] types)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (genericTypes == null)
                throw new ArgumentNullException("genericTypes");
#if !CLRSQL
            var genericMethod = type.GetMethods(bindingAttr)
                .Where(m => m.IsGenericMethod)
                .Where(m => m.ContainsGenericParameters && m.Name == name)
                .Where(m => genericTypes.Length == 0 || m.GetGenericArguments().Single().GetGenericParameterConstraints().Match(genericTypes, (x, y) => x.Equals(y), true))
                .Where(m => (types == null && !m.GetParameters().Any()) || (types != null && MatchParameters(m, genericTypes, types)))
                .SingleOrDefault();
            return genericMethod.GetGenericMethodDefinition();
        }

        private static bool MatchParameters(MethodInfo m, Type[] genericTypes, Type[] types)
        {
            return (types.Length == 0 || m.MakeGenericMethod(genericTypes).GetParameters().Select(c => c.ParameterType).Match(types, (x, y) => x.Equals(y), true));
        }
#else
            throw new NotImplementedException();
        }
#endif

        /// <summary>
        /// Gets the type of the enumerable element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetEnumerableElementType(Type type)
        {
            var enumerable = FindIEnumerable(type);
            return (enumerable == null ? type : enumerable.GetGenericArguments()[0]);
        }

        private static Type FindIEnumerable(Type type)
        {
            if (type == null || type == CoreExtensions.StringType)
                return null;
            if (type.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            if (type.IsGenericType)
                foreach (var argument in type.GetGenericArguments())
                {
                    var enumerable = typeof(IEnumerable<>).MakeGenericType(argument);
                    if (enumerable.IsAssignableFrom(type))
                        return enumerable;
                }
            var interfaces = type.GetInterfaces();
            if (interfaces.Length > 0)
                foreach (var @interface in interfaces)
                {
                    var enumerable = FindIEnumerable(@interface);
                    if (enumerable != null)
                        return enumerable;
                }
            return (type.BaseType != null && type.BaseType != CoreExtensions.ObjectType ? FindIEnumerable(type.BaseType) : null);
        }
    }
}