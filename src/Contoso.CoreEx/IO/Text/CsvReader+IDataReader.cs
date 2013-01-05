#region Foreign-License
//	LumenWorks.Framework.IO.CSV.CsvReader
//	Copyright (c) 2005 Sébastien Lorion
//
//	MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Modified: Sky Morey <moreys@digitalev.com>
//
#endregion
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Debug = System.Diagnostics.Debug;
namespace Contoso.IO.Text
{
    public partial class CsvReader
    {
        /// <summary>
        /// Validates the state of the data reader.
        /// </summary>
        /// <param name="validations">The validations to accomplish.</param>
        /// <exception cref="InvalidOperationException">
        ///	No current record.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///	This operation is invalid when the reader is closed.
        /// </exception>
        private void ValidateDataReader(DataReaderValidations validations)
        {
            if ((validations & DataReaderValidations.IsInitialized) != 0 && !_initialized)
                throw new InvalidOperationException(Local.NoCurrentRecord);
            if ((validations & DataReaderValidations.IsNotClosed) != 0 && _isDisposed)
                throw new InvalidOperationException(Local.ReaderClosed);
        }

        /// <summary>
        /// Copy the value of the specified field to an array.
        /// </summary>
        /// <param name="field">The index of the field.</param>
        /// <param name="fieldOffset">The offset in the field value.</param>
        /// <param name="destinationArray">The destination array where the field value will be copied.</param>
        /// <param name="destinationOffset">The destination array offset.</param>
        /// <param name="length">The number of characters to copy from the field value.</param>
        /// <returns></returns>
        private long CopyFieldToArray(int field, long fieldOffset, Array destinationArray, int destinationOffset, int length)
        {
            EnsureInitialize();
            if (field < 0 || field >= _fieldCount)
                throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, Local.FieldIndexOutOfRange, field));
            if (fieldOffset < 0 || fieldOffset >= int.MaxValue)
                throw new ArgumentOutOfRangeException("fieldOffset");
            // Array.Copy(...) will do the remaining argument checks
            if (length == 0)
                return 0;
            var value = (this[field] ?? string.Empty);
            Debug.Assert(fieldOffset < int.MaxValue);
            Debug.Assert(destinationArray.GetType() == typeof(char[]) || destinationArray.GetType() == typeof(byte[]));
            if (destinationArray.GetType() == typeof(char[]))
                Array.Copy(value.ToCharArray((int)fieldOffset, length), 0, destinationArray, destinationOffset, length);
            else
            {
                var chars = value.ToCharArray((int)fieldOffset, length);
                var source = new byte[chars.Length];
                for (int i = 0; i < chars.Length; i++)
                    source[i] = Convert.ToByte(chars[i]);
                Array.Copy(source, 0, destinationArray, destinationOffset, length);
            }
            return length;
        }

        int IDataReader.RecordsAffected
        {
            // For SELECT statements, -1 must be returned.
            get { return -1; }
        }

        bool IDataReader.IsClosed
        {
            get { return _endOfStream; }
        }

        bool IDataReader.NextResult()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);
            return false;
        }

        void IDataReader.Close() { Dispose(); }

        bool IDataReader.Read()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);
            return Read();
        }

        int IDataReader.Depth
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsNotClosed);
                return 0;
            }
        }

        DataTable IDataReader.GetSchemaTable()
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);
            var schema = new DataTable("SchemaTable")
            {
                Locale = CultureInfo.InvariantCulture,
                MinimumCapacity = _fieldCount,
            };
            var columns = schema.Columns;
            columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.BaseColumnName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableColumn.BaseTableName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableColumn.ColumnName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int)).ReadOnly = true;
            columns.Add(SchemaTableColumn.ColumnSize, typeof(int)).ReadOnly = true;
            columns.Add(SchemaTableColumn.DataType, typeof(object)).ReadOnly = true;
            columns.Add(SchemaTableColumn.IsAliased, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.IsExpression, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.IsKey, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.IsLong, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.IsUnique, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableColumn.NumericPrecision, typeof(short)).ReadOnly = true;
            columns.Add(SchemaTableColumn.NumericScale, typeof(short)).ReadOnly = true;
            columns.Add(SchemaTableColumn.ProviderType, typeof(int)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)).ReadOnly = true;
            columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)).ReadOnly = true;
            string[] columnNames;
            if (_settings.HasHeaders)
                columnNames = _fieldHeaders;
            else
            {
                columnNames = new string[_fieldCount];
                for (var i = 0; i < _fieldCount; i++)
                    columnNames[i] = "Column" + i.ToString(CultureInfo.InvariantCulture);
            }
            // null marks columns that will change for each row
            var schemaRow = new object[]
            { 
			    true,					// 00- AllowDBNull
			    null,					// 01- BaseColumnName
			    string.Empty,			// 02- BaseSchemaName
			    string.Empty,			// 03- BaseTableName
			    null,					// 04- ColumnName
			    null,					// 05- ColumnOrdinal
			    int.MaxValue,			// 06- ColumnSize
			    typeof(string),			// 07- DataType
			    false,					// 08- IsAliased
			    false,					// 09- IsExpression
			    false,					// 10- IsKey
			    false,					// 11- IsLong
			    false,					// 12- IsUnique
			    DBNull.Value,			// 13- NumericPrecision
			    DBNull.Value,			// 14- NumericScale
			    (int) DbType.String,	// 15- ProviderType
			    string.Empty,			// 16- BaseCatalogName
			    string.Empty,			// 17- BaseServerName
			    false,					// 18- IsAutoIncrement
			    false,					// 19- IsHidden
			    true,					// 20- IsReadOnly
			    false					// 21- IsRowVersion
            };
            for (var i = 0; i < columnNames.Length; i++)
            {
                schemaRow[1] = columnNames[i]; // Base column name
                schemaRow[4] = columnNames[i]; // Column name
                schemaRow[5] = i; // Column ordinal
                schema.Rows.Add(schemaRow);
            }
            return schema;
        }

        #region IDataRecord

        int IDataRecord.GetInt32(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Int32.Parse(this[i] ?? string.Empty, CultureInfo.CurrentCulture);
        }

        object IDataRecord.this[string name]
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[name];
            }
        }

        object IDataRecord.this[int i]
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[i];
            }
        }

        object IDataRecord.GetValue(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return (((IDataRecord)this).IsDBNull(i) ? (object)DBNull.Value : this[i]);
        }

        bool IDataRecord.IsDBNull(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return (string.IsNullOrEmpty(this[i]));
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return CopyFieldToArray(i, fieldOffset, buffer, bufferoffset, length);
        }

        byte IDataRecord.GetByte(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Byte.Parse(this[i], CultureInfo.CurrentCulture);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            if (i < 0 || i >= _fieldCount)
                throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, Local.FieldIndexOutOfRange, i));
            return typeof(string);
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Decimal.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetValues(object[] values)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            var record = (IDataRecord)this;
            for (var i = 0; i < _fieldCount; i++)
                values[i] = record.GetValue(i);
            return _fieldCount;
        }

        string IDataRecord.GetName(int i)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);
            if (i < 0 || i >= _fieldCount)
                throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, Local.FieldIndexOutOfRange, i));
            return (_settings.HasHeaders ? _fieldHeaders[i] : "Column" + i.ToString(CultureInfo.InvariantCulture));
        }

        long IDataRecord.GetInt64(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Int64.Parse(this[i], CultureInfo.CurrentCulture);
        }

        double IDataRecord.GetDouble(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Double.Parse(this[i], CultureInfo.CurrentCulture);
        }

        bool IDataRecord.GetBoolean(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            string value = this[i];
            int result;
            return (Int32.TryParse(value, out result) ? result != 0 : Boolean.Parse(value));
        }

        Guid IDataRecord.GetGuid(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return new Guid(this[i]);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return DateTime.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);
            int index;
            if (!_fieldHeaderIndexes.TryGetValue(name, out index))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Local.FieldHeaderNotFound, name), "name");
            return index;
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return typeof(string).FullName;
        }

        float IDataRecord.GetFloat(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Single.Parse(this[i], CultureInfo.CurrentCulture);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return (i == 0 ? this : null);
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return CopyFieldToArray(i, fieldoffset, buffer, bufferoffset, length);
        }

        string IDataRecord.GetString(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return this[i];
        }

        char IDataRecord.GetChar(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Char.Parse(this[i]);
        }

        short IDataRecord.GetInt16(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Int16.Parse(this[i], CultureInfo.CurrentCulture);
        }

        #endregion
    }
}