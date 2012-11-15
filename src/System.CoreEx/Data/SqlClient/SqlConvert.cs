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
using System.Data.SqlTypes;
namespace System.Data.SqlClient
{
	public static class SqlConvert
	{
        public static object ConvertFromSqlType(object value)
        {
            if ((value == null) || (value is DBNull))
                return null;
            else if (value is SqlBoolean)
                return ((SqlBoolean)value).Value;
            else if (value is SqlChars)
                return ((SqlChars)value).Value;
            else if (value is SqlDateTime)
                return ((SqlDateTime)value).Value;
            else if (value is SqlDecimal)
                return ((SqlDecimal)value).Value;
            else if (value is SqlDouble)
                return ((SqlDouble)value).Value;
            else if (value is SqlInt32)
                return ((SqlInt32)value).Value;
            else if (value is SqlMoney)
                return ((SqlMoney)value).Value;
            else if (value is SqlSingle)
                return ((SqlSingle)value).Value;
            else if (value is SqlString)
                return ((SqlString)value).Value;
            throw new InvalidOperationException("type was: " + value.GetType().ToString());
        }	
	}
}
#endif