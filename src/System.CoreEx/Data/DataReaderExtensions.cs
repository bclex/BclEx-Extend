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
namespace System.Data
{
    /// <summary>
    /// DataReaderExtensions
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Ases the enumerable.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static IEnumerable<IDataReader> AsEnumerable(this IDataReader r)
        {
            if (r == null)
                throw new ArgumentNullException("source");
            while (r.Read())
                yield return r;
        }

        ////? Still needed - YeildRows can do same
        //public static IEnumerable<Object[]> AsEnumerableValues(this IDataReader r)
        //{
        //    if (r == null)
        //        throw new ArgumentNullException("source");
        //    while (r.Read())
        //    {
        //        var values = new Object[r.FieldCount];
        //        r.GetValues(values);
        //        yield return values;
        //    }
        //}

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public static T Field<T>(this IDataReader r, int columnIndex)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            return DelegateFactory<T>.Field(r, columnIndex);
        }

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Field<T>(this IDataReader r, int columnIndex, T defaultValue)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            return (!r.IsDBNull(columnIndex) ? DelegateFactory<T>.Field(r, columnIndex) : defaultValue);
        }

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static T Field<T>(this IDataReader r, string columnName)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            return DelegateFactory<T>.Field(r, r.GetOrdinal(columnName));
        }

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Field<T>(this IDataReader r, string columnName, T defaultValue)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            int columnIndex = r.GetOrdinal(columnName);
            return (!r.IsDBNull(columnIndex) ? DelegateFactory<T>.Field(r, columnIndex) : defaultValue);
        }

        private static class DelegateFactory<T>
        {
            internal static readonly Func<IDataReader, int, T> Field = Create(typeof(T));

            static DelegateFactory() { }

            private static Func<IDataReader, int, T> Create(Type type)
            {
                // reference types
                if (!type.IsValueType)
                {
                    if (type == typeof(string))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetString", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(IDataReader))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetData", BindingFlags.NonPublic | BindingFlags.Static));
                }
                // nullable types
                if (((type.IsGenericType) && (!type.IsGenericTypeDefinition)) && (typeof(Nullable<>) == type.GetGenericTypeDefinition()))
                {
                    if (type == typeof(bool?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableBoolean", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(byte?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableByte", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(char?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableChar", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(DateTime?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableDateTime", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(Decimal))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableDecimal", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(double?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableDouble", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(float?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableFloat", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(Guid?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableGuid", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(short?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableInt16", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(int?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableInt32", BindingFlags.NonPublic | BindingFlags.Static));
                    if (type == typeof(long?))
                        return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetNullableInt64", BindingFlags.NonPublic | BindingFlags.Static));
                }
                // value types
                if (type == typeof(bool))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetBoolean", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(byte))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetByte", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(char))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetChar", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(DateTime))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetDateTime", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(Decimal))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetDecimal", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(double))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetDouble", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(float))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetFloat", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(Guid))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetGuid", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(short))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetInt16", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(int))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetInt32", BindingFlags.NonPublic | BindingFlags.Static));
                if (type == typeof(long))
                    return (Func<IDataReader, int, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, int, T>), typeof(DelegateFactory<T>).GetMethod("GetInt64", BindingFlags.NonPublic | BindingFlags.Static));
                throw new InvalidOperationException();
            }

            #region Reference Types
            private static string GetString(IDataReader r, int columnIndex)
            {
                return (!r.IsDBNull(columnIndex) ? r.GetString(columnIndex) : null);
            }
            private static IDataReader GetData(IDataReader r, int columnIndex)
            {
                return (!r.IsDBNull(columnIndex) ? r.GetData(columnIndex) : null);
            }
            #endregion

            #region Nullable Types
            private static bool? GetNullableBoolean(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new bool?(r.GetBoolean(columnIndex)));
            }
            private static byte? GetNullableByte(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new byte?(r.GetByte(columnIndex)));
            }
            private static char? GetNullableChar(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new char?(r.GetChar(columnIndex)));
            }
            private static DateTime? GetNullableDateTime(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new DateTime?(r.GetDateTime(columnIndex)));
            }
            private static decimal? GetNullableDecimal(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new decimal?(r.GetDecimal(columnIndex)));
            }
            private static double? GetNullableDouble(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new double?(r.GetDouble(columnIndex)));
            }
            private static float? GetNullableFloat(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new float?(r.GetFloat(columnIndex)));
            }
            private static Guid? GetNullableGuid(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new Guid?(r.GetGuid(columnIndex)));
            }
            private static short? GetNullableInt16(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new short?(r.GetInt16(columnIndex)));
            }
            private static int? GetNullableInt32(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new int?(r.GetInt32(columnIndex)));
            }
            private static long? GetNullableInt64(IDataReader r, int columnIndex)
            {
                return (r.IsDBNull(columnIndex) ? null : new long?(r.GetInt64(columnIndex)));
            }
            #endregion

            #region Value Types
            private static bool GetBoolean(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetBoolean(columnIndex);
            }
            private static byte GetByte(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetByte(columnIndex);
            }
            private static char GetChar(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetChar(columnIndex);
            }
            private static DateTime GetDateTime(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetDateTime(columnIndex);
            }
            private static decimal GetDecimal(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetDecimal(columnIndex);
            }
            private static double GetDouble(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetDouble(columnIndex);
            }
            private static float GetFloat(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetFloat(columnIndex);
            }
            private static Guid GetGuid(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetGuid(columnIndex);
            }
            private static short GetInt16(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetInt16(columnIndex);
            }
            private static int GetInt32(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetInt32(columnIndex);
            }
            private static long GetInt64(IDataReader r, int columnIndex)
            {
                if (r.IsDBNull(columnIndex))
                    throw new InvalidCastException(string.Format("DataSetLinq_NonNullableCast({0})", typeof(T).ToString()));
                return r.GetInt64(columnIndex);
            }
            #endregion
        }
    }
}