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
using System.Runtime.InteropServices;
using System.Xml;
namespace System
{
    /// <summary>
    /// CoreExtensions
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static partial class CoreExtensions
    {
        internal const int LOGON32_LOGON_INTERACTIVE = 2;
        internal const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll")]
        internal static extern int LogonUserA(String lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool RevertToSelf();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern bool CloseHandle(IntPtr handle);

        internal static readonly Type BoolType = typeof(bool);
        internal static readonly Type NBoolType = typeof(bool?);
        internal static readonly Type DateTimeType = typeof(DateTime);
        internal static readonly Type NDateTimeType = typeof(DateTime?);
        internal static readonly Type DecimalType = typeof(decimal);
        internal static readonly Type NDecimalType = typeof(decimal?);
        internal static readonly Type Int32Type = typeof(int);
        internal static readonly Type NInt32Type = typeof(int?);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly Type XmlReaderType = typeof(XmlReader);
        internal static readonly Type XmlWriterType = typeof(XmlWriter);

        /// <summary>
        /// Raises the specified ev.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public static void Raise(this EventHandler ev, object sender = null, EventArgs e = default(EventArgs))
        {
            if (ev != null)
                ev(sender, e);
        }

        /// <summary>
        /// Raises the specified ev.
        /// </summary>
        /// <typeparam name="Args">The type of the RGS.</typeparam>
        /// <param name="ev">The ev.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public static void Raise<Args>(this EventHandler<Args> ev, object sender = null, Args e = default(Args))
            where Args : EventArgs
        {
            if (ev != null)
                ev(sender, e);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp">The sp.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static T GetService<T>(this IServiceProvider sp, Type serviceType)
            where T : class
        {
            return (T)sp.GetService(serviceType);
        }
    }
}

