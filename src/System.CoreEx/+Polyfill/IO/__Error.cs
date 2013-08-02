#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Collections.Generic;
using System.Reflection;
namespace System.IO
{
    /// <summary>
    /// __Error
    /// </summary>
    internal class __Error
    {
        private static readonly MethodInfo _winIOErrorInfo = Type.GetType("System.IO.__Error").GetMethod("WinIOError", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int), typeof(string) }, null);

        internal static void WinIOError(int errorCode, string maybeFullPath) { _winIOErrorInfo.Invoke(null, new object[] { errorCode, maybeFullPath }); }
    }
}
#endif