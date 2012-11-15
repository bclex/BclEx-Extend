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
namespace System.Data
{
    //TODO: Add Safe Field for refrence types ? returns default
    /// <summary>
    /// DataRowExtensions
    /// </summary>
	public static class DataRowExtensions
	{
        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="column">The column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public static T Field<T>(this DataRow r, DataColumn column, T defaultValue)
		{
			if (r == null)
				throw new ArgumentNullException("r");
			return (!r.IsNull(column) ? r.Field<T>(column) : defaultValue);
		}

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public static T Field<T>(this DataRow r, int columnIndex, T defaultValue)
		{
			if (r == null)
				throw new ArgumentNullException("r");
			return (!r.IsNull(columnIndex) ? r.Field<T>(columnIndex) : defaultValue);
		}

        /// <summary>
        /// Fields the specified r.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The r.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public static T Field<T>(this DataRow r, string columnName, T defaultValue)
		{
			if (r == null)
				throw new ArgumentNullException("r");
			return (!r.IsNull(columnName) ? r.Field<T>(columnName) : defaultValue);
		}
	}
}