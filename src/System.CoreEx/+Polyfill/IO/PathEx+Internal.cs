#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System.IO
{
    /// <summary>
    /// PathEx
    /// </summary>
    partial class PathEx
    {
        internal static readonly char[] TrimEndChars = new char[] { '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', '\x00a0' };

        internal static bool IsDirectorySeparator(char c)
        {
            if (c != Path.DirectorySeparatorChar)
                return (c == Path.AltDirectorySeparatorChar);
            return true;
        }

        internal static void CheckSearchPattern(string searchPattern)
        {
            int num;
            while ((num = searchPattern.IndexOf("..", StringComparison.Ordinal)) != -1)
            {
                if ((num + 2) == searchPattern.Length)
                    throw new ArgumentException(SR.GetResourceString("Arg_InvalidSearchPattern"));
                if (searchPattern[num + 2] == Path.DirectorySeparatorChar || searchPattern[num + 2] == Path.AltDirectorySeparatorChar)
                    throw new ArgumentException(SR.GetResourceString("Arg_InvalidSearchPattern"));
                searchPattern = searchPattern.Substring(num + 2);
            }
        }
    }
}
#endif