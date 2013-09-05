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
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Contoso.Primitives.Codecs;
namespace Contoso.IO.Text
{
    /// <summary>
    /// CsvEmitter
    /// </summary>
    public class CsvEmitter
    {
        /// <summary>
        /// FieldAttrib
        /// </summary>
        public class FieldAttrib
        {
            /// <summary>
            /// DoNotEncode
            /// </summary>
            public bool? DoNotEncode;
            /// <summary>
            /// AsExcelFunction
            /// </summary>
            public bool? AsExcelFunction;
        }

        /// <summary>
        /// Emits the specified w.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="w">The w.</param>
        /// <param name="set">The set.</param>
        public void Emit<TItem>(TextWriter w, IEnumerable<TItem> set) { Emit<TItem>(CsvEmitContext.DefaultContext, w, set); }
        /// <summary>
        /// Emits the specified context.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="ctx">The context.</param>
        /// <param name="w">The w.</param>
        /// <param name="set">The set.</param>
        public void Emit<TItem>(CsvEmitContext ctx, TextWriter w, IEnumerable<TItem> set)
        {
            if (ctx == null)
                throw new ArgumentNullException("context");
            if (w == null)
                throw new ArgumentNullException("w");
            if (set == null)
                throw new ArgumentNullException("set");
            var itemProperties = GetItemProperties<TItem>();
            var shouldEncodeValues = ((ctx.EmitOptions & CsvEmitOptions.EncodeValues) != 0);
            // header
            var fields = (ctx.Fields.Count > 0 ? ctx.Fields : null);
            var b = new StringBuilder();
            if ((ctx.EmitOptions & CsvEmitOptions.HasHeaderRow) != 0)
            {
                foreach (var itemProperty in itemProperties)
                {
                    // decode value
                    var name = itemProperty.Name;
                    CsvEmitField field;
                    if (fields != null && fields.TryGetValue(name, out field) && field != null)
                        if (field.Ignore)
                            continue;
                        else if (field.DisplayName != null)
                            name = field.DisplayName;
                    b.Append(TryEncode(shouldEncodeValues, name) + ",");
                }
                if (b.Length > 0)
                    b.Length--;
                w.Write(b.ToString() + Environment.NewLine);
            }
            // rows
            try
            {
                foreach (var group in set.Cast<object>().GroupAt(ctx.FlushAt))
                {
                    var newGroup = (ctx.BeforeFlush == null ? group : ctx.BeforeFlush(group));
                    if (newGroup == null)
                        return;
                    foreach (var item in newGroup)
                    {
                        b.Length = 0;
                        foreach (var itemProperty in itemProperties)
                        {
                            string valueAsText;
                            var value = itemProperty.GetValue(item, null);
                            // decode value
                            CsvEmitField field;
                            if (fields != null && fields.TryGetValue(itemProperty.Name, out field) && field != null)
                            {
                                if (field.Ignore)
                                    continue;
                                IConvertFormatter convertFormatter;
                                var fieldFormatter = field.CustomFieldFormatter;
                                if (fieldFormatter != null)
                                {
                                    // formatter
                                    valueAsText = fieldFormatter(field, item, value);
                                    if (!string.IsNullOrEmpty(valueAsText))
                                    {
                                        var fieldAttrib = field.Args.Get<FieldAttrib>();
                                        if (fieldAttrib != null)
                                        {
                                            if (fieldAttrib.DoNotEncode == true)
                                            {
                                                b.Append(valueAsText + ",");
                                                continue;
                                            }
                                            if (fieldAttrib.AsExcelFunction == true)
                                                valueAsText = "=" + valueAsText;
                                        }
                                    }
                                }
                                else if ((convertFormatter = field.ConvertFormatter) != null)
                                    // datatype
                                    valueAsText = convertFormatter.Format(value, field.DefaultValue, field.Args);
                                else
                                {
                                    // default formatter
                                    valueAsText = (value != null ? value.ToString() : string.Empty);
                                    if (valueAsText.Length == 0)
                                        valueAsText = field.DefaultValue;
                                }
                            }
                            else
                                valueAsText = (value != null ? value.ToString() : string.Empty);
                            // append value
                            b.Append(TryEncode(shouldEncodeValues, valueAsText) + ",");
                        }
                        b.Length--;
                        w.Write(b.ToString() + Environment.NewLine);
                    }
                    w.Flush();
                }
            }
            finally { w.Flush(); }
        }

        private static PropertyInfo[] GetItemProperties<T>()
        {
            return typeof(T).GetProperties().ToArray();
        }

        private string TryEncode(bool shouldEncodeValues, string value)
        {
            return (shouldEncodeValues ? CsvCodec.Encode(value) : value);
        }
    }
}