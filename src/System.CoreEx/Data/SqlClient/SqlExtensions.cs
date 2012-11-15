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
#if SQLCLR
using System.IO;
using System.Xml;
namespace System.Data.SqlClient
{
    /// <summary>
    /// SqlExtensions class
    /// </summary>
    public static class SqlExtensions
    {
        /// <summary>
        /// Reads the type of the SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static T ReadSqlType<T>(this BinaryReader r)
        {
            return SqlParseEx.SqlTypeT<T>.BinaryRead(r);
        }

        /// <summary>
        /// Writes the type of the SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="w">The w.</param>
        /// <param name="value">The value.</param>
        public static void WriteSqlType<T>(this BinaryWriter w, T value)
        {
            SqlParseEx.SqlTypeT<T>.BinaryWrite(w, value);
        }

        /// <summary>
        /// Reads the type of the SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static T ReadSqlType<T>(this XmlReader r, string id)
        {
            return SqlParseEx.SqlTypeT<T>.XmlRead(r, id);
        }

        /// <summary>
        /// Writes the type of the SQL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="w">The w.</param>
        /// <param name="value">The value.</param>
        public static void WriteSqlType<T>(this XmlWriter w, string id, T value)
        {
            SqlParseEx.SqlTypeT<T>.XmlWrite(w, id, value);
        }
    }
}
#endif