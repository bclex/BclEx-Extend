#if SQLCLR
using System.IO;
using System.Reflection;
using System.Data.SqlTypes;
using System.Xml;
namespace System.Data.SqlClient
{
    /// <summary>
    /// SqlParseEx
    /// </summary>
    public static class SqlParseEx
    {
        internal static readonly Type SqlInt32Type = typeof(SqlInt32);
        internal static readonly Type SqlDateTimeType = typeof(SqlDateTime);
        internal static readonly Type SqlStringType = typeof(SqlString);
        internal static readonly Type SqlBooleanType = typeof(SqlBoolean);
        internal static readonly Type SqlXmlType = typeof(SqlXml);
        private static readonly Type BinaryReaderType = typeof(BinaryReader);
        private static readonly Type BinaryWriterType = typeof(BinaryWriter);

        #region SqlTypeT
        /// <summary>
        /// SqlTypeT class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class SqlTypeT<T>
        {
            private static readonly Type s_type = typeof(T);
            public static readonly Func<BinaryReader, T> BinaryRead = SqlTypeT<T>.CreateBinaryRead(s_type);
            public static readonly Action<BinaryWriter, T> BinaryWrite = SqlTypeT<T>.CreateBinaryWrite(s_type);
            public static readonly Func<XmlReader, string, T> XmlRead = SqlTypeT<T>.CreateXmlRead(s_type);
            public static readonly Action<XmlWriter, string, T> XmlWrite = SqlTypeT<T>.CreateXmlWrite(s_type);

            /// <summary>
            /// Initializes the <see cref="SqlTypeT&lt;T&gt;"/> class.
            /// </summary>
            static SqlTypeT() { }

            #region BinaryRead Implementation
            /// <summary>
            /// Creates the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            private static Func<BinaryReader, T> CreateBinaryRead(Type type)
            {
                if (type == SqlInt32Type)
                    return (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), typeof(SqlTypeT<T>).GetMethod("BinaryRead_SqlInt32", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryReaderType }, null));
                else if (type == SqlDateTimeType)
                    return (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), typeof(SqlTypeT<T>).GetMethod("BinaryRead_SqlDateTime", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryReaderType }, null));
                else if (type == SqlStringType)
                    return (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), typeof(SqlTypeT<T>).GetMethod("BinaryRead_SqlString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryReaderType }, null));
                else if (type == SqlBooleanType)
                    return (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), typeof(SqlTypeT<T>).GetMethod("BinaryRead_SqlBoolean", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryReaderType }, null));
                else if (type == SqlXmlType)
                    return (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), typeof(SqlTypeT<T>).GetMethod("BinaryRead_SqlXml", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryReaderType }, null));
                throw new InvalidOperationException(string.Format("\nUndefined Type [{0}]", type.ToString()));
            }

            /// <summary>
            /// Read_s the SQL int32.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlInt32 BinaryRead_SqlInt32(BinaryReader r)
            {
                return (!r.ReadBoolean() ? new SqlInt32(r.ReadInt32()) : SqlInt32.Null);
            }

            /// <summary>
            /// Read_s the SQL int32.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlDateTime BinaryRead_SqlDateTime(BinaryReader r)
            {
                return (!r.ReadBoolean() ? new SqlDateTime(r.ReadInt32(), r.ReadInt32()) : SqlDateTime.Null);
            }

            /// <summary>
            /// Read_s the SQL string.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlString BinaryRead_SqlString(BinaryReader r)
            {
                return (!r.ReadBoolean() ? new SqlString(r.ReadString()) : SqlString.Null);
            }

            /// <summary>
            /// Read_s the SQL boolean.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlBoolean BinaryRead_SqlBoolean(BinaryReader r)
            {
                return (!r.ReadBoolean() ? new SqlBoolean(r.ReadBoolean()) : SqlBoolean.Null);
            }

            /// <summary>
            /// Read_s the SQL XML.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlXml BinaryRead_SqlXml(BinaryReader r)
            {
                if (!r.ReadBoolean())
                {
                    var s = new MemoryStream();
                    var w = new StreamWriter(s);
                    w.Write(r.ReadString());
                    w.Flush();
                    return new SqlXml(s);
                }
                return SqlXml.Null;
            }
            #endregion

            #region BinaryWrite Implementation
            /// <summary>
            /// Creates the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            private static Action<BinaryWriter, T> CreateBinaryWrite(Type type)
            {
                if (type == SqlInt32Type)
                    return (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), typeof(SqlTypeT<T>).GetMethod("BinaryWrite_SqlInt32", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryWriterType, SqlInt32Type }, null));
                else if (type == SqlDateTimeType)
                    return (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), typeof(SqlTypeT<T>).GetMethod("BinaryWrite_SqlDateTime", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryWriterType, SqlDateTimeType }, null));
                else if (type == SqlStringType)
                    return (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), typeof(SqlTypeT<T>).GetMethod("BinaryWrite_SqlString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryWriterType, SqlStringType }, null));
                else if (type == SqlBooleanType)
                    return (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), typeof(SqlTypeT<T>).GetMethod("BinaryWrite_SqlBoolean", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryWriterType, SqlBooleanType }, null));
                else if (type == SqlXmlType)
                    return (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), typeof(SqlTypeT<T>).GetMethod("BinaryWrite_SqlXml", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { BinaryWriterType, SqlXmlType }, null));
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Write_s the SQL int32.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void BinaryWrite_SqlInt32(BinaryWriter w, SqlInt32 value)
            {
                bool isNull = value.IsNull;
                w.Write(isNull);
                if (!isNull)
                    w.Write(value.Value);
            }

            /// <summary>
            /// Write_s the SQL int32.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void BinaryWrite_SqlDateTime(BinaryWriter w, SqlDateTime value)
            {
                bool isNull = value.IsNull;
                w.Write(isNull);
                if (!isNull)
                {
                    w.Write(value.DayTicks);
                    w.Write(value.TimeTicks);
                }
            }

            /// <summary>
            /// Write_s the SQL string.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void BinaryWrite_SqlString(BinaryWriter w, SqlString value)
            {
                bool isNull = value.IsNull;
                w.Write(isNull);
                if (!isNull)
                    w.Write(value.Value);
            }

            /// <summary>
            /// Write_s the SQL boolean.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void BinaryWrite_SqlBoolean(BinaryWriter w, SqlBoolean value)
            {
                bool isNull = value.IsNull;
                w.Write(isNull);
                if (!isNull)
                    w.Write(value.Value);
            }

            /// <summary>
            /// Write_s the SQL XML.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void BinaryWrite_SqlXml(BinaryWriter w, SqlXml value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                bool isNull = value.IsNull;
                w.Write(isNull);
                if (!isNull)
                    w.Write(value.Value);
            }
            #endregion

            #region XmlRead Implementation
            /// <summary>
            /// Creates the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            private static Func<XmlReader, string, T> CreateXmlRead(Type type)
            {
                if (type == SqlInt32Type)
                    return (Func<XmlReader, string, T>)Delegate.CreateDelegate(typeof(Func<XmlReader, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlRead_SqlInt32", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlReaderType, CoreExInternal.StringType }, null));
                else if (type == SqlDateTimeType)
                    return (Func<XmlReader, string, T>)Delegate.CreateDelegate(typeof(Func<XmlReader, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlRead_SqlDateTime", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlReaderType, CoreExInternal.StringType }, null));
                else if (type == SqlStringType)
                    return (Func<XmlReader, string, T>)Delegate.CreateDelegate(typeof(Func<XmlReader, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlRead_SqlString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlReaderType, CoreExInternal.StringType }, null));
                else if (type == SqlBooleanType)
                    return (Func<XmlReader, string, T>)Delegate.CreateDelegate(typeof(Func<XmlReader, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlRead_SqlBoolean", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlReaderType, CoreExInternal.StringType }, null));
                else if (type == SqlXmlType)
                    return (Func<XmlReader, string, T>)Delegate.CreateDelegate(typeof(Func<XmlReader, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlRead_SqlXml", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlReaderType, CoreExInternal.StringType }, null));
                throw new InvalidOperationException(string.Format("\nUndefined Type [{0}]", type.ToString()));
            }

            /// <summary>
            /// Read_s the SQL int32.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlInt32 XmlRead_SqlInt32(XmlReader r, string id)
            {
                string value = r.GetAttribute(id);
                return (value != null ? new SqlInt32(value.Parse<int>()) : SqlInt32.Null);
            }

            /// <summary>
            /// Read_s the SQL int32.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <param name="id">The id.</param>
            /// <returns></returns>
            private static SqlDateTime XmlRead_SqlDateTime(XmlReader r, string id)
            {
                string value = r.GetAttribute(id);
                return (value != null ? new SqlDateTime(value.Parse<DateTime>()) : SqlDateTime.Null);
            }

            /// <summary>
            /// Read_s the SQL string.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlString XmlRead_SqlString(XmlReader r, string id)
            {
                string value = r.GetAttribute(id);
                return (value != null ? new SqlString(value) : SqlString.Null);
            }

            /// <summary>
            /// Read_s the SQL boolean.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlBoolean XmlRead_SqlBoolean(XmlReader r, string id)
            {
                string value = r.GetAttribute(id);
                return (value != null ? new SqlBoolean(value.Parse<bool>()) : SqlBoolean.Null);
            }

            /// <summary>
            /// Read_s the SQL XML.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <returns></returns>
            private static SqlXml XmlRead_SqlXml(XmlReader r, string id)
            {
                string value;
                switch (id)
                {
                    case "":
                        return (!r.IsEmptyElement ? new SqlXml(r.ReadSubtree()) : SqlXml.Null);
                    case "#":
                        value = r.ReadString();
                        break;
                    default:
                        value = r.GetAttribute(id);
                        break;
                }
                if (!string.IsNullOrEmpty(value))
                {
                    var s = new MemoryStream();
                    var w = new StreamWriter(s);
                    w.Write(value);
                    w.Flush();
                    return new SqlXml(s);
                }
                return SqlXml.Null;
            }
            #endregion

            #region XmlWrite Implementation
            /// <summary>
            /// Creates the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            private static Action<XmlWriter, string, T> CreateXmlWrite(Type type)
            {
                if (type == SqlInt32Type)
                    return (Action<XmlWriter, string, T>)Delegate.CreateDelegate(typeof(Action<XmlWriter, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlWrite_SqlInt32", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlWriterType, CoreExInternal.StringType, SqlInt32Type }, null));
                else if (type == SqlDateTimeType)
                    return (Action<XmlWriter, string, T>)Delegate.CreateDelegate(typeof(Action<XmlWriter, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlWrite_SqlDateTime", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlWriterType, CoreExInternal.StringType, SqlDateTimeType }, null));
                else if (type == SqlStringType)
                    return (Action<XmlWriter, string, T>)Delegate.CreateDelegate(typeof(Action<XmlWriter, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlWrite_SqlString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlWriterType, CoreExInternal.StringType, SqlStringType }, null));
                else if (type == SqlBooleanType)
                    return (Action<XmlWriter, string, T>)Delegate.CreateDelegate(typeof(Action<XmlWriter, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlWrite_SqlBoolean", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlWriterType, CoreExInternal.StringType, SqlBooleanType }, null));
                else if (type == SqlXmlType)
                    return (Action<XmlWriter, string, T>)Delegate.CreateDelegate(typeof(Action<XmlWriter, string, T>), typeof(SqlTypeT<T>).GetMethod("XmlWrite_SqlXml", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExInternal.XmlWriterType, CoreExInternal.StringType, SqlXmlType }, null));
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Write_s the SQL int32.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void XmlWrite_SqlInt32(XmlWriter w, string id, SqlInt32 value)
            {
                bool isNull = value.IsNull;
                if (!isNull)
                    w.WriteAttributeString(id, value.Value.ToString());
            }

            /// <summary>
            /// Write_s the SQL int32.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void XmlWrite_SqlDateTime(XmlWriter w, string id, SqlDateTime value)
            {
                bool isNull = value.IsNull;
                if (!isNull)
                    w.WriteAttributeString(id, value.Value.ToString());
            }

            /// <summary>
            /// Write_s the SQL string.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void XmlWrite_SqlString(XmlWriter w, string id, SqlString value)
            {
                bool isNull = value.IsNull;
                if (!isNull)
                    w.WriteAttributeString(id, value.Value.ToString());
            }

            /// <summary>
            /// Write_s the SQL boolean.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void XmlWrite_SqlBoolean(XmlWriter w, string id, SqlBoolean value)
            {
                bool isNull = value.IsNull;
                if (!isNull)
                    w.WriteAttributeString(id, value.Value.ToString());
            }

            /// <summary>
            /// Write_s the SQL XML.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="value">The value.</param>
            private static void XmlWrite_SqlXml(XmlWriter w, string id, SqlXml value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                bool isNull = value.IsNull;
                if (!isNull)
                {
                    string valueAsText;
                    switch (id)
                    {
                        case "":
                            w.WriteNode(value.CreateReader(), false);
                            return;
                        case "#":
                            valueAsText = value.Value;
                            if (!string.IsNullOrEmpty(valueAsText))
                                w.WriteString(valueAsText);
                            break;
                        default:
                            valueAsText = value.Value;
                            if (!string.IsNullOrEmpty(valueAsText))
                                w.WriteAttributeString(id, valueAsText);
                            break;
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
#endif