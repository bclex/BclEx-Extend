#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections.Generic;
namespace System.IO
{
    partial class DirectoryEx
    {
        internal sealed class SearchData
        {
            public string fullPath;
            public SearchOption searchOption;
            public string userPath;

            public SearchData() { }
            public SearchData(string fullPath, string userPath, SearchOption searchOption)
            {
                this.fullPath = fullPath;
                this.userPath = userPath;
                this.searchOption = searchOption;
            }
        }

        internal static string GetDemandDir(string fullPath, bool thisDirOnly)
        {
            if (thisDirOnly)
            {
                if (EndsWith(fullPath, Path.DirectorySeparatorChar) || EndsWith(fullPath, Path.AltDirectorySeparatorChar))
                    return (fullPath + '.');
                return (fullPath + Path.DirectorySeparatorChar + '.');
            }
            if (!EndsWith(fullPath, Path.DirectorySeparatorChar) && !EndsWith(fullPath, Path.AltDirectorySeparatorChar))
                return (fullPath + Path.DirectorySeparatorChar);
            return fullPath;
        }

        private static bool EndsWith(string source, char value)
        {
            var length = source.Length;
            return (length != 0 && source[length - 1] == value);
        }
    }
}
#endif