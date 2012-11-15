#region Foreign-License
// x
#endregion
using System.Text.RegularExpressions;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public class Wildcard : Regex
    {
        public Wildcard(string pattern, RegexOptions options = (RegexOptions)0x29)
            : base(WildcardToRegex(pattern), options) { }

        public static string WildcardToRegex(string pattern)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace("^" + Regex.Escape(pattern) + "$", @"(?<!\\)\\\*", ".*"), @"\\\\\\\*", @"\*"), @"(?<!\\)\\\?", "."), @"\\\\\\\?", @"\?"), @"\\\\\\\\", @"\\");
        }
    }
}

