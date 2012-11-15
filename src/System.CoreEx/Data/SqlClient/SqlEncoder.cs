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
using System.Text;
using System.Globalization;
#if SQLCLR
using System.Data.SqlTypes;
#endif
namespace System.Data.SqlClient
{
    /// <summary>
    /// SqlEncoder
    /// </summary>
	public static class SqlEncoder
    {
#if SQLCLR
        /// <summary>
        /// Converts the type of from SQL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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
#endif

        /// <summary>
        /// Encodes the partial text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		public static string EncodePartialText(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var b = new StringBuilder(value.Length);
			InternalEscapeText(InternalEscapeTextKind.Text, b, value);
			return b.ToString();
		}

        /// <summary>
        /// Encodes the text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		public static string EncodeText(string value)
		{
			if (value == null)
				return "Null";
			else if (value.Length == 0)
				return "N''";
			else
			{
				var b = new StringBuilder("N'", value.Length + 3);
				InternalEscapeText(InternalEscapeTextKind.Text, b, value);
				b.Append("'");
				return b.ToString();
			}
		}

		/// <summary>
		/// Encodes the execute partial text.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string EncodeExecutePartialText(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var b = new StringBuilder(value.Length);
			InternalEscapeText(InternalEscapeTextKind.ExecuteText, b, value);
			return b.ToString();
		}

		/// <summary>
		/// Encodes the execute text.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string EncodeExecuteText(string value)
		{
			if (value == null)
				return "Null";
			else if (value.Length == 0)
				return "N''''";
			else
			{
				var b = new StringBuilder("N''", value.Length + 5);
				InternalEscapeText(InternalEscapeTextKind.ExecuteText, b, value);
				b.Append("''");
				return b.ToString();
			}
		}

		/// <summary>
		/// Encodes the execute partial text.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string EncodeObjectPartialText(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var b = new StringBuilder(value.Length);
			InternalEscapeText(InternalEscapeTextKind.Object, b, value);
			return b.ToString();
		}

		/// <summary>
		/// Encodes the execute text.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string EncodeObjectText(string value)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentNullException();
			var b = new StringBuilder("[", value.Length + 2);
			InternalEscapeText(InternalEscapeTextKind.Object, b, value);
			b.Append("]");
			return b.ToString();
		}

		#region InternalEscapeText
		private enum InternalEscapeTextKind
		{
			Text,
			ExecuteText,
			Object,
		}

		private static void InternalEscapeText(InternalEscapeTextKind kind, StringBuilder b, string value)
		{
			switch (kind)
			{
				case InternalEscapeTextKind.Text:
					foreach (char c in value)
					{
						int code = (int)c;
						switch (c)
						{
							case '\'':
								b.Append("''");
								break;
							default:
								if ((code >= 32) && (code < 128))
									b.Append(c);
								else
									b.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "\\u{0:X4}", code);
								break;
						}
					}
					break;
				case InternalEscapeTextKind.ExecuteText:
					foreach (char c in value)
					{
						int code = (int)c;
						switch (c)
						{
							case '\'':
								b.Append("''");
								break;
							default:
								if ((code >= 32) && (code < 128))
									b.Append(c);
								else
									b.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "\\u{0:X4}", code);
								break;
						}
					}
					break;
				case InternalEscapeTextKind.Object:
					foreach (char c in value)
					{
						int code = (int)c;
						switch (c)
						{
							case '[':
								b.Append("");
								break;
							case ']':
								b.Append("");
								break;
							default:
								b.Append(c);
								break;
						}
					}
					break;
				default:
					throw new InvalidOperationException();
			}
		}
		#endregion
	}
}
