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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
namespace System.ComponentModel
{
    /// <summary>
    /// TypeDescriptorEvaluator
    /// </summary>
    //+ copied from DataSourceHelper:DataBinder
    public class TypeDescriptorEvaluator
    {
        private static readonly char[] s_expressionPartSeparator = new char[] { '.' };
        private static readonly char[] s_indexExpressionEndChars = new char[] { ']', ')' };
        private static readonly char[] s_indexExpressionStartChars = new char[] { '[', '(' };

        /// <summary>
        /// Gets the resolved data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        /// <returns></returns>
        public static IEnumerable GetResolvedDataSource(object dataSource, string dataMember)
        {
            if (dataSource != null)
            {
                var source = (dataSource as IListSource);
                if (source != null)
                {
                    var list = source.GetList();
                    if (!source.ContainsListCollection)
                        return list;
                    if ((list != null) && (list is ITypedList))
                    {
                        var itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                        if ((itemProperties == null) || (itemProperties.Count == 0))
                            throw new Exception("ListSource_Without_DataMembers");
                        var descriptor = (string.IsNullOrEmpty(dataMember) ? itemProperties[0] : itemProperties.Find(dataMember, true));
                        if (descriptor != null)
                        {
                            object component = list[0];
                            object obj3 = descriptor.GetValue(component);
                            if ((obj3 != null) && (obj3 is IEnumerable))
                                return (IEnumerable)obj3;
                        }
                        throw new Exception(string.Format("ListSource_Missing_DataMemberA[{0}]", dataMember));
                    }
                }
                if (dataSource is IEnumerable)
                    return (IEnumerable)dataSource;
            }
            return null;
        }

        /// <summary>
        /// Evals the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static object Eval(object container, string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if ((expression = expression.Trim()).Length == 0)
                throw new ArgumentNullException("expression");
            if (container == null)
                return null;
            string[] expressionParts = expression.Split(s_expressionPartSeparator);
            return Eval(container, expressionParts);
        }
        /// <summary>
        /// Evals the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="expressionParts">The expression parts.</param>
        /// <returns></returns>
        private static object Eval(object container, string[] expressionParts)
        {
            object expressionValue = container;
            for (int expressionPartIndex = 0; (expressionPartIndex < expressionParts.Length) && (expressionValue != null); expressionPartIndex++)
            {
                string expressionPart = expressionParts[expressionPartIndex];
                expressionValue = (expressionPart.IndexOfAny(s_indexExpressionStartChars) < 0 ? GetPropertyValue(expressionValue, expressionPart) : GetIndexedPropertyValue(expressionValue, expressionPart));
            }
            return expressionValue;
        }
        /// <summary>
        /// Evals the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string Eval(object container, string expression, string format)
        {
            object expressionValue = Eval(container, expression);
            if ((expressionValue == null) || (expressionValue == DBNull.Value))
                return string.Empty;
            return (string.IsNullOrEmpty(format) ? expressionValue.ToString() : string.Format(format, expressionValue));
        }

        //public static object GetDataItem(object container)
        //{
        //    bool flag;
        //    return GetDataItem(container, out flag);
        //}
        //public static object GetDataItem(object container, out bool foundDataItem)
        //{
        //    if (container == null)
        //    {
        //        foundDataItem = false;
        //        return null;
        //    }
        //    var container2 = (container as IDataItemContainer);
        //    if (container2 != null)
        //    {
        //        foundDataItem = true;
        //        return container2.DataItem;
        //    }
        //    string name = "DataItem";
        //    var property = container.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //    if (property == null)
        //    {
        //        foundDataItem = false;
        //        return null;
        //    }
        //    foundDataItem = true;
        //    return property.GetValue(container, null);
        //}

        /// <summary>
        /// Gets the indexed property value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="propertyName">The expr.</param>
        /// <returns></returns>
        public static object GetIndexedPropertyValue(object container, string propertyName)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            object obj2 = null;
            bool flag = false;
            int length = propertyName.IndexOfAny(s_indexExpressionStartChars);
            int num2 = propertyName.IndexOfAny(s_indexExpressionEndChars, length + 1);
            if (((length < 0) || (num2 < 0)) || (num2 == (length + 1)))
                throw new ArgumentException(string.Format("DataBinder_Invalid_Indexed_ExprA[{0}]", propertyName));
            string actualPropertyName = null;
            object obj3 = null;
            string s = propertyName.Substring(length + 1, (num2 - length) - 1).Trim();
            if (length != 0)
                actualPropertyName = propertyName.Substring(0, length);
            if (s.Length != 0)
            {
                if (((s[0] == '"') && (s[s.Length - 1] == '"')) || ((s[0] == '\'') && (s[s.Length - 1] == '\'')))
                    obj3 = s.Substring(1, s.Length - 2);
                else if (char.IsDigit(s[0]))
                {
                    int num3;
                    flag = int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3);
                    if (flag)
                        obj3 = num3;
                    else
                        obj3 = s;
                }
                else
                    obj3 = s;
            }
            if (obj3 == null)
                throw new ArgumentException(string.Format("DataBinder_Invalid_Indexed_ExprA[{0}]", propertyName));
            object propertyValue = (!string.IsNullOrEmpty(actualPropertyName) ? GetPropertyValue(container, actualPropertyName) : container);
            if (propertyValue == null)
                return obj2;
            var array = (propertyValue as System.Array);
            if ((array != null) && (flag))
                return array.GetValue((int)obj3);
            if ((propertyValue is IList) && (flag))
                return ((IList)propertyValue)[(int)obj3];
            var info = propertyValue.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { obj3.GetType() }, null);
            if (info == null)
                throw new ArgumentException(string.Format("DataBinder_No_Indexed_AccessorA[{0}]", propertyValue.GetType().FullName));
            return info.GetValue(propertyValue, new object[] { obj3 });
        }
        /// <summary>
        /// Gets the indexed property value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="propertyName">Name of the prop.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string GetIndexedPropertyValue(object container, string propertyName, string format)
        {
            object propertyValue = GetIndexedPropertyValue(container, propertyName);
            if ((propertyValue == null) || (propertyValue == DBNull.Value))
                return string.Empty;
            return (string.IsNullOrEmpty(format) ? propertyValue.ToString() : string.Format(format, propertyValue));
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="propertyName">Name of the prop.</param>
        /// <returns></returns>
        public static object GetPropertyValue(object container, string propertyName)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            var descriptor = TypeDescriptor.GetProperties(container).Find(propertyName, true);
            if (descriptor == null)
                throw new Exception(string.Format("DataBinder_Prop_Not_FoundAB[{0},{1}]", container.GetType().FullName, propertyName));
            return descriptor.GetValue(container);
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="propertyName">Name of the prop.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string GetPropertyValue(object container, string propertyName, string format)
        {
            object propertyValue = GetPropertyValue(container, propertyName);
            if ((propertyValue == null) || (propertyValue == DBNull.Value))
                return string.Empty;
            return (string.IsNullOrEmpty(format) ? propertyValue.ToString() : string.Format(format, propertyValue));
        }

        private static bool IsNull(object value)
        {
            return (!((value != null) && (!Convert.IsDBNull(value))));
        }
    }
}