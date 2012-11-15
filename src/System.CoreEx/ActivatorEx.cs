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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
namespace System
{
    /// <summary>
    /// ActivatorEx
    /// </summary>
    public static class ActivatorEx
    {
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static object CreateInstance(string type, params object[] args)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");
            return Activator.CreateInstance(Type.GetType(type, true), args);
        }

        /// <summary>
        /// Creates the instance with composite arguments.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="prefixArgs">The prefix args.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static object CreateInstanceWithCompositeArguments(Type type, object[] prefixArgs, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (prefixArgs == null)
                throw new ArgumentNullException("prefixArgs");
            int prefixArgsLength;
            if (args != null && args.Length > 0 && (prefixArgsLength = prefixArgs.Length) > 0)
            {
                var compositeArgs = new object[args.Length + prefixArgsLength];
                prefixArgs.CopyTo(compositeArgs, 0);
                args.CopyTo(compositeArgs, prefixArgsLength);
                return Activator.CreateInstance(type, compositeArgs);
            }
            return Activator.CreateInstance(type, prefixArgs);
        }

        /// <summary>
        /// Creates the instance by reader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="r">The r.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static object CreateInstanceByReader(Type type, XmlReader r, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (r == null)
                return Activator.CreateInstance(type, args);
            var serializer = new XmlSerializer(type, (string)null);
            return serializer.Deserialize(r, (string)null);
        }

        /// <summary>
        /// Creates the instance indirect.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static object CreateInstanceIndirect(Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return type.InvokeMember("Create" + type.Name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, args);
        }

        #region Fast-Create

        private static Dictionary<Type, Func<object>> _factoryCache = new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Fasts the create instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FastCreateInstance<T>() { return TypeFactory<T>.Create(); }
        /// <summary>
        /// Fasts the create instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object FastCreateInstance(Type type)
        {
            Func<object> f;
            if (!_factoryCache.TryGetValue(type, out f))
                lock (_factoryCache)
                    if (!_factoryCache.TryGetValue(type, out f))
                        _factoryCache[type] = f = Expression.Lambda<Func<object>>(Expression.New(type), new ParameterExpression[0]).Compile();
            return f();
        }

        private static class TypeFactory<T>
        {
            public static readonly Func<T> Create;

            static TypeFactory()
            {
                Create = Expression.Lambda<Func<T>>(Expression.New(typeof(T)), new ParameterExpression[0]).Compile();
            }
        }

        #endregion
    }
}
