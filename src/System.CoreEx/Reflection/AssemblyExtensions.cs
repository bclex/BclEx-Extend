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
using System.Collections.Generic;
namespace System.Reflection
{
    /// <summary>
    /// AssemblyExtensions
    /// </summary>
#if COREINTERNAL
    internal
#else
    public
#endif
 static class AssemblyExtensions
    {
        /// <summary>
        /// Ases the types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static IEnumerable<Type> AsTypes(this Assembly assembly, Predicate<Type> predicate)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            foreach (var type in assembly.GetTypes())
                if (predicate == null || predicate(type))
                    yield return type;
        }

        /// <summary>
        /// Ases the concrete types.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static IEnumerable<Type> AsConcreteTypes<TBasedOn>(this Assembly assembly) { return AsConcreteTypes(assembly, typeof(TBasedOn), null); }
        /// <summary>
        /// Ases the concrete types.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static IEnumerable<Type> AsConcreteTypes<TBasedOn>(this Assembly assembly, Predicate<Type> predicate) { return AsConcreteTypes(assembly, typeof(TBasedOn), null); }
        /// <summary>
        /// Ases the concrete types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <returns></returns>
        public static IEnumerable<Type> AsConcreteTypes(this Assembly assembly, Type basedOnType) { return AsConcreteTypes(assembly, basedOnType, null); }
        /// <summary>
        /// Ases the concrete types.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static IEnumerable<Type> AsConcreteTypes(this Assembly assembly, Type basedOnType, Predicate<Type> predicate)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            foreach (var t in assembly.GetTypes())
                if (basedOnType.IsAssignableFrom(t) && !t.Equals(basedOnType) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)))
                    yield return t;
        }


        //public Type[] GetDerivedTypes(Type type, bool concretable)
        //{
        //    var expression = _assemblies.SelectMany(a => a.GetTypes())
        //        .Where(t => (t != type) && (type.IsAssignableFrom(t)));
        //    if (concretable)
        //        expression = expression.Where(t => (!t.IsInterface) && (!t.IsAbstract));
        //    return expression.ToArray();
        //}

        //public Type[] GetInterfaceImplementations(Type type, bool concretable)
        //{
        //    var expression = _assemblies.SelectMany(a => a.GetTypes())
        //        .Where(t => t.GetInterfaces().Contains(type));
        //    if (concretable)
        //        expression = expression.Where(t => (!t.IsInterface) && (!t.IsAbstract));
        //    return expression.Distinct()
        //        .ToArray();
        //}

        //public Type[] GetGenericInterfaceImplementations(Type type, bool concretable)
        //{
        //    var expression = _assemblies.SelectMany(a => a.GetTypes())
        //        .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == type)));
        //    if (concretable)
        //        expression = expression.Where(t => (!t.IsInterface) && (!t.IsAbstract));
        //    return expression.Distinct()
        //        .ToArray();
        //}
    }
}