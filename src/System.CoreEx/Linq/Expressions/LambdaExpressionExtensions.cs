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
    /// LambdaExpressionExtensions
    /// </summary>
#if COREINTERNAL
    internal
#else
    public
#endif
 static class LambdaExpressionExtensions
    {
        /// <summary>
        /// Finds the property slim.
        /// </summary>
        /// <param name="lambdaExpression">The lambda expression.</param>
        /// <returns></returns>
        public static MemberInfo FindPropertySlim(this LambdaExpression lambdaExpression)
        {
            var operand = (Expression)lambdaExpression;
            var flag = false;
            while (!flag)
            {
                MemberExpression expression2;
                var nodeType = operand.NodeType;
                if (nodeType != ExpressionType.Convert)
                {
                    if (nodeType == ExpressionType.Lambda)
                    {
                        operand = ((LambdaExpression)operand).Body;
                        continue;
                    }
                    if (nodeType == ExpressionType.MemberAccess)
                    {
                        expression2 = (MemberExpression)operand;
                        if (expression2.Expression.NodeType != ExpressionType.Parameter && expression2.Expression.NodeType != ExpressionType.Convert)
                            throw new ArgumentException(string.Format("Expression '{0}' must resolve to top-level member.", lambdaExpression), "lambdaExpression");
                        return expression2.Member;
                    }
                    flag = true;
                    continue;
                }
                operand = ((UnaryExpression)operand).Operand;
            }
            return null;
        }
    }
}