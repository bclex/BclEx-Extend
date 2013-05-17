#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Globalization;
namespace System
{
    /// <summary>
    /// SR
    /// </summary>
    static class SR
    {
        internal static string GetResourceString(string key)
        {
            return key;
        }

        internal static string GetResourceString(string key, params object[] values)
        {
            return string.Format(CultureInfo.CurrentCulture, key, values);
        }
    }
}
#endif