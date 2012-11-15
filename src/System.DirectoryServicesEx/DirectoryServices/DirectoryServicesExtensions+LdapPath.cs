
namespace System.DirectoryServices
{
    public static partial class DirectoryServicesExtensions
    {
        private static char[] _splitChars = new[] { '/' };

        /// <summary>
        /// Gets the DN.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetDN(this DirectoryEntry source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var split = source.Path.Split(_splitChars);
            return split[split.Length - 1];
        }
        /// <summary>
        /// Gets the DN.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetDN(this SearchResult source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var split = source.Path.Split(_splitChars);
            return split[split.Length - 1];
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetName(this DirectoryEntry source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var split = source.Path.Split(_splitChars);
            return LdapPath.GetNameFromDN(split[split.Length - 1]);
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetName(this SearchResult source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var split = source.Path.Split(_splitChars);
            return LdapPath.GetNameFromDN(split[split.Length - 1]);
        }

        /// <summary>
        /// Gets the new A ds path.
        /// </summary>
        /// <param name="de">The de.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static string GetNewADsPath(this DirectoryEntry de, string dn)
        {
            var split = de.Path.Split(_splitChars);
            split[split.Length - 1] = dn;
            return string.Join("/", split);
        }
    }
}
