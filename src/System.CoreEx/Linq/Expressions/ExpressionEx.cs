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
namespace System.Linq.Expressions
{
    /// <summary>
    /// ExpressionEx
    /// </summary>
#if COREINTERNAL
    internal
#else
    public
#endif
 static class ExpressionEx
    {
#if !NO_EXPRESSIONS
        /// <summary>
        /// Covariants the cast.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <typeparam name="TDerived">The type of the derived.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Action<TBase> CovariantCast<TBase, TDerived>(Expression<Action<TDerived>> source)
            where TDerived : TBase
        {
            if (typeof(TDerived) == typeof(TBase))
                return (Action<TBase>)((Delegate)source.Compile());
            var obj = Expression.Parameter(typeof(TBase), "obj");
            var expression = Expression.Lambda<Action<TBase>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(obj, typeof(TDerived))),
                obj);
            return expression.Compile();
        }
#endif

        /// <summary>
        /// Covariants the cast.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <typeparam name="TDerived">The type of the derived.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static Action<TBase> CovariantCast<TBase, TDerived>(MethodInfo method)
            where TDerived : TBase
        {
#if NO_EXPRESSIONS
            return (x => method.Invoke(x, new object[] { }));
#else
            var obj = Expression.Parameter(typeof(TBase), "obj");
            var expression = Expression.Lambda<Action<TBase>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(obj, typeof(TDerived)),
                        method),
                    typeof(TBase)),
                obj);
            return expression.Compile();
#endif
        }

        /// <summary>
        /// Covariants the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <typeparam name="TDerived">The type of the derived.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Func<T, TBase> CovariantAccessor<T, TBase, TDerived>(Func<T, TDerived> source)
            where TDerived : TBase { return CovariantAccessor<T, TBase, TDerived>(source.Method); }
        /// <summary>
        /// Covariants the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <typeparam name="TDerived">The type of the derived.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static Func<T, TBase> CovariantAccessor<T, TBase, TDerived>(MethodInfo method)
            where TDerived : TBase
        {
#if NO_EXPRESSIONS
            return (x => method.Invoke(x, new object[] { }));
#else
            var obj = Expression.Parameter(typeof(TDerived), "obj");
            var expression = Expression.Lambda<Func<T, TBase>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method),
                    typeof(TDerived)),
                obj);
            return expression.Compile();
#endif
        }
    }
}