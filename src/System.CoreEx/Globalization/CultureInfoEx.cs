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
using System.Text;
#if EXPERIMENTAL
using System.Patterns.Schema;
#endif
namespace System.Globalization
{
    /// <summary>
    /// CultureInfoEx
    /// </summary>
    public class CultureInfoEx : CultureInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoEx"/> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        public CultureInfoEx(int culture)
            : base(culture) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoEx"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CultureInfoEx(string name)
            : base(name) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoEx"/> class.
        /// </summary>
        /// <param name="culture">A predefined <see cref="T:System.Globalization.CultureInfo"/> identifier, <see cref="P:System.Globalization.CultureInfo.LCID"/> property of an existing <see cref="T:System.Globalization.CultureInfo"/> object, or Windows-only culture identifier.</param>
        /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false).</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="culture"/> is less than zero.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="culture"/> is not a valid culture identifier.
        /// -or-
        /// In .NET Compact Framework applications, <paramref name="culture"/> is not supported by the operating system of the device.
        ///   </exception>
        public CultureInfoEx(int culture, bool useUserOverride)
            : base(culture, useUserOverride) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoEx"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="useUserOverride">if set to <c>true</c> [use user override].</param>
        public CultureInfoEx(string name, bool useUserOverride)
            : base(name, useUserOverride) { }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the display name ex.
        /// </summary>
        /// <value>
        /// The display name ex.
        /// </value>
        public string DisplayNameEx { get; set; }
        /// <summary>
        /// Gets or sets the name of the localized.
        /// </summary>
        /// <value>
        /// The name of the localized.
        /// </value>
        public string LocalizedName { get; set; }
        /// <summary>
        /// Gets or sets the default for location code.
        /// </summary>
        /// <value>
        /// The default for location code.
        /// </value>
        public string DefaultForLocationCode { get; set; }

        /// <summary>
        /// Tries the get culture info.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns></returns>
        public static bool TryGetCultureInfo(string name, out CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            try { cultureInfo = new CultureInfo(name); return true; }
            catch (Exception) { cultureInfo = null; return false; }
        }

#if EXPERIMENTAL
        public static bool TryGetCultureInfoEx(string name, CultureSchemaBase schema, out CultureInfoEx cultureInfo)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (schema != null)
            {
                cultureInfo = schema.Cultures.Where(c => string.CompareOrdinal(c.Name, name) == 0).FirstOrDefault();
                if (cultureInfo != null)
                    return true;
            }
            try { cultureInfo = new CultureInfoEx(name); return true; }
            catch (Exception) { cultureInfo = null; return false; }
        }
#endif
    }
}