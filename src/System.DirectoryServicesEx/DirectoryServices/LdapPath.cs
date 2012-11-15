using System.Security.Principal;

namespace System.DirectoryServices
{
    /// <summary>
    /// LdapPath
    /// </summary>
    public static class LdapPath
    {
        private static char[] _splitChars = new[] { '/' };

        /// <summary>
        /// Transforms the CN from DN.
        /// </summary>
        /// <param name="sourceDN">The source DN.</param>
        /// <param name="targetDN">The target DN.</param>
        /// <returns></returns>
        public static string TransformCNFromDN(string sourceDN, string targetDN)
        {
            return "CN=" + GetNameFromDN(sourceDN) + "," + targetDN;
        }

        /// <summary>
        /// Gets the container from DN.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static string GetContainerFromDN(string dn)
        {
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            var startIndex = dn.IndexOf(',') + 1;
            return dn.Substring(startIndex);
        }

        //public static string GetNameFromDN(string dn)
        //{
        //    if (string.IsNullOrEmpty(dn))
        //        throw new ArgumentNullException("dn");
        //    var startIndex = dn.IndexOf('=') + 1;
        //    return dn.Substring(startIndex, dn.IndexOf(',') - startIndex);
        //}

        private static string[] dnDelimiters = new[] { ",O=", ",OU=", ",CN=", ",DC=" };
        /// <summary>
        /// Gets the name from DN.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static string GetNameFromDN(string dn)
        {
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            var startIndex = dn.IndexOf('=') + 1;
            var sIX = dn.Length;
            foreach (var s in dnDelimiters)
            {
                var x = dn.ToUpper().IndexOf(s);
                if (x >= 0 && x < sIX)
                    sIX = x;
            }
            if (sIX == dn.Length) sIX = dn.IndexOf(',');
            return dn.Substring(startIndex, sIX - startIndex);
        }

        /// <summary>
        /// Gets the DN from A ds path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetDNFromADsPath(string path)
        {
            var split = path.Split(_splitChars);
            return split[split.Length - 1];
        }

        internal static string GetPartitionFromDN(string defaultDN)
        {
            if (string.IsNullOrEmpty(defaultDN))
                throw new ArgumentNullException("defaultDN");
            var organizationIndex = defaultDN.IndexOf("O=", StringComparison.OrdinalIgnoreCase);
            if (organizationIndex > -1)
                return defaultDN.Substring(organizationIndex);
            var dcIndex = defaultDN.IndexOf("DC=", StringComparison.OrdinalIgnoreCase);
            if (dcIndex > -1)
                return defaultDN.Substring(dcIndex);
            throw new ArgumentOutOfRangeException("defaultDN", string.Format("Invalid DN '{0}'", defaultDN));
        }

        /// <summary>
        /// Encodes the filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeFilter<T>(T value)
        {
            var valueAsSid = (value as SecurityIdentifier);
            if (valueAsSid != null)
                return EncodeFilter(valueAsSid);
            var valueAsBytes = (value as byte[]);
            if (valueAsBytes != null)
                return EncodeFilter(valueAsBytes);
            var valueAsString = (value as string);
            if (valueAsString != null)
                return EncodeFilter(valueAsString);
            return EncodeFilter(value.ToString());
        }
        /// <summary>
        /// Encodes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeFilter(string value) { return EncodeFilter(value, true); }
        /// <summary>
        /// Encodes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encodeWildcard">if set to <c>true</c> [encode wildcard].</param>
        /// <returns></returns>
        public static string EncodeFilter(string value, bool encodeWildcard)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            value = value.Replace(@"\", @"\5c").Replace("/", @"\2f").Replace("(", @"\28").Replace(")", @"\29").Replace("\0", @"\00");
            return (encodeWildcard ? value.Replace("*", @"\2a") : value);
        }
        /// <summary>
        /// Encodes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeFilter(byte[] value)
        {
            if (value == null)
                return string.Empty;
            return @"\" + BitConverter.ToString(value).Replace("-", @"\");
        }
        /// <summary>
        /// Encodes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeFilter(SecurityIdentifier value)
        {
            if (value == null)
                return string.Empty;
            var valueAsBytes = new byte[value.BinaryLength];
            value.GetBinaryForm(valueAsBytes, 0);
            return EncodeFilter(valueAsBytes);
        }

        /// <summary>
        /// Encodes the RDN.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeRdn(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("!", @"\21").Replace("*", @"\2A").Replace("/", @"\2F").Replace(":", @"\3A").Replace("?", @"\3F");
        }
    }
}
