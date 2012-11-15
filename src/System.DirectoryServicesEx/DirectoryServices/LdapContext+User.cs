using System.Security.Principal;

namespace System.DirectoryServices
{
    public partial class LdapContext
    {
        /// <summary>
        /// Gets or sets the LDAP user name attribute.
        /// </summary>
        /// <value>
        /// The LDAP user name attribute.
        /// </value>
        public string LdapUserNameAttribute { get; set; }
        /// <summary>
        /// Gets or sets the LDAP user DN attribute.
        /// </summary>
        /// <value>
        /// The LDAP user DN attribute.
        /// </value>
        public string LdapUserDNAttribute { get; set; }
        /// <summary>
        /// Gets or sets the LDAP user filter.
        /// </summary>
        /// <value>
        /// The LDAP user filter.
        /// </value>
        public string LdapUserFilter { get; set; }
        /// <summary>
        /// Gets or sets the LDAP user search scope.
        /// </summary>
        /// <value>
        /// The LDAP user search scope.
        /// </value>
        public SearchScope LdapUserSearchScope { get; set; }

        internal string GetUserAttributeBySearchProperty(string searchContainer, string searchValue, out string userToken)
        {
            using (var searchRoot = GetEntry(searchContainer))
            {
                string filter;
                if (!string.IsNullOrEmpty(searchValue))
                    filter = string.Format("(&({0})({1}={2}))", LdapUserFilter, LdapPath.EncodeFilter(LdapUserNameAttribute), LdapPath.EncodeFilter(searchValue));
                else
                    filter = string.Format("(&({0})(!({1}=*)))", LdapUserFilter, LdapPath.EncodeFilter(LdapUserNameAttribute));
                var propertyCollection = searchRoot.QueryFirstOrDefault(filter, LdapUserSearchScope, LdapUserDNAttribute, LdapProperty.objectSid);
                if (propertyCollection != null)
                {
                    userToken = propertyCollection.GetToken();
                    return propertyCollection.GetSingleValue<string>(LdapUserDNAttribute);
                }
            }
            userToken = null;
            return null;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static string GetToken(WindowsIdentity user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var sid = user.User;
            return (sid != null ? Convert.ToBase64String(sid.ToByteArray()) : null);
        }
    }
}