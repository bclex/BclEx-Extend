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
using System.Security.Permissions;
namespace System.Configuration
{
    /// <summary>
    /// AssemblyNameConverter
    /// </summary>
    public sealed class AssemblyNameConverter : ConfigurationConverterBase
    {
        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            var assembly = GetAssembly((string)data, false);
            if (assembly == null)
                throw new ArgumentException(string.Format("Type_cannot_be_resolved {0}", (string)data));
            return assembly;
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            if (!(value is Type))
                ValidateType(value, typeof(Assembly));
            return (value != null ? ((Assembly)value).FullName : null);
        }

        private void ValidateType(object value, Type expected)
        {
            if (value != null && value.GetType() != expected)
                throw new ArgumentException(string.Format("Converter_unsupported_value_type {0}", expected.Name));
        }

        private static Assembly GetAssembly(string assemblyString, bool throwOnError)
        {
            var fileAsUri = (assemblyString.StartsWith("file://", StringComparison.OrdinalIgnoreCase) ? new Uri(assemblyString) : null);
            if (fileAsUri == null || !fileAsUri.IsFile)
                return Assembly.Load(assemblyString);
            return Assembly.LoadFile(fileAsUri.LocalPath);
        }
    }
}