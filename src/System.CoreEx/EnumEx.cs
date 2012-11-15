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
using System.Linq;
namespace System
{
    /// <summary>
    /// EnumEx
    /// </summary>
    public struct EnumEx
    {
        private static readonly Type _enumNameAttributeType = typeof(EnumNameAttribute);

        /// <summary>
        /// Parses the specified s.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static TEnum Parse<TEnum>(string s) { return (TEnum)Enum.Parse(typeof(TEnum), s); }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="s">The s.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParse<TEnum>(string s, out TEnum result)
        {
            try { result = (TEnum)Enum.Parse(typeof(TEnum), s); return true; }
            catch { result = default(TEnum); return false; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public static string ToString(Type enumType, object value)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (value == null)
                throw new ArgumentNullException("value");
            return value.ToString();
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public static string ToString<TEnum>(TEnum value)
            where TEnum : struct { return value.ToString(); }

        /// <summary>
        /// Tries the name of the parse.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="s">The s.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseName<TEnum>(string s, out TEnum result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToName<TEnum>(TEnum value)
            where TEnum : struct { return ToName(typeof(TEnum), value, null); }
        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public static string ToName<TEnum>(TEnum value, string kind)
            where TEnum : struct { return ToName(typeof(TEnum), value, kind); }
        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToName<TEnum>(string value)
            where TEnum : struct { return ToName(typeof(TEnum), Parse<TEnum>(value), null); }
        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public static string ToName<TEnum>(string value, string kind)
            where TEnum : struct { return ToName(typeof(TEnum), Parse<TEnum>(value), kind); }
        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToName(Type enumType, object value) { return ToName(enumType, value, null); }
        /// <summary>
        /// Toes the name.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="value">The value.</param>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public static string ToName(Type enumType, object value, string kind)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (value == null)
                throw new ArgumentNullException("value");
            //
            var fieldInfo = enumType.GetField(value.ToString());
            EnumNameAttribute[] attributes; //Q: use build in DescriptionAttribute?
            if (fieldInfo == null || (attributes = (EnumNameAttribute[])fieldInfo.GetCustomAttributes(_enumNameAttributeType, true)).Length == 0)
                return value.ToString();
            // return (attributes.Length > 0 ? attributes[0].Description : value.ToString())
            var attribute = attributes.SingleOrDefault(x => x.Kind == kind);
            return (attribute != null ? attribute.Name : value.ToString());
        }
    }
}
