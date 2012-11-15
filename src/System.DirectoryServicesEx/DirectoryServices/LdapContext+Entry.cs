using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Text;

namespace System.DirectoryServices
{
    // System.DirectoryServices.ActiveDirectory.DirectoryEntryManager
    public partial class LdapContext
    {
        private const string DefaultNamingContext = "defaultNamingContext";
        private const string SchemaNamingContext = "schemaNamingContext";
        private const string ConfigurationNamingContext = "configurationNamingContext";
        private static string RootDomainNamingContext = "rootDomainNamingContext";

        private static DirectoryEntry Bind(string ldapPath, string username, string password, bool useServerBind, Func<AuthenticationTypes, AuthenticationTypes> authenticationTypes)
        {
            var defaultAuthType = DefaultAuthType;
            if (_serverBindSupported && useServerBind)
                defaultAuthType |= AuthenticationTypes.ServerBind;
            return new DirectoryEntry(ldapPath, username, password, (authenticationTypes == null ? defaultAuthType : authenticationTypes(defaultAuthType)));
        }

        private string ExpandWellKnownDN(WellKnownDN dn)
        {
            switch (dn)
            {
                case WellKnownDN.RootDSE: return "RootDSE";
                case WellKnownDN.DefaultNamingContext: var entry = GetCachedEntry("RootDSE"); return entry.Properties.GetSingleValue<string>(DefaultNamingContext);
                case WellKnownDN.SchemaNamingContext: var entry2 = GetCachedEntry("RootDSE"); return entry2.Properties.GetSingleValue<string>(SchemaNamingContext);
                case WellKnownDN.ConfigurationNamingContext: var entry3 = GetCachedEntry("RootDSE"); return entry3.Properties.GetSingleValue<string>(ConfigurationNamingContext);
                case WellKnownDN.PartitionsContainer: return ("CN=Partitions," + ExpandWellKnownDN(WellKnownDN.ConfigurationNamingContext));
                case WellKnownDN.SitesContainer: return ("CN=Sites," + ExpandWellKnownDN(WellKnownDN.ConfigurationNamingContext));
                case WellKnownDN.SystemContainer: return ("CN=System," + ExpandWellKnownDN(WellKnownDN.DefaultNamingContext));
                case WellKnownDN.RidManager: return ("CN=RID Manager$," + ExpandWellKnownDN(WellKnownDN.SystemContainer));
                case WellKnownDN.Infrastructure: return ("CN=Infrastructure," + ExpandWellKnownDN(WellKnownDN.DefaultNamingContext));
                case WellKnownDN.RootDomainNamingContext: var entry4 = GetCachedEntry("RootDSE"); return entry4.Properties.GetSingleValue<string>(RootDomainNamingContext);
                default: throw new ArgumentOutOfRangeException("dn", dn, "Invalid");
            }
        }

        private static string ExpandWellKnownDN(LdapContext context, WellKnownDN dn)
        {
            switch (dn)
            {
                case WellKnownDN.RootDSE: return "RootDSE";
                case WellKnownDN.DefaultNamingContext: using (var entry = GetEntry(context, "RootDSE")) return entry.Properties.GetSingleValue<string>(DefaultNamingContext);
                case WellKnownDN.SchemaNamingContext: using (var entry2 = GetEntry(context, "RootDSE")) return entry2.Properties.GetSingleValue<string>(SchemaNamingContext);
                case WellKnownDN.ConfigurationNamingContext: using (var entry3 = GetEntry(context, "RootDSE")) return entry3.Properties.GetSingleValue<string>(ConfigurationNamingContext);
                case WellKnownDN.PartitionsContainer: return ("CN=Partitions," + ExpandWellKnownDN(context, WellKnownDN.ConfigurationNamingContext));
                case WellKnownDN.SitesContainer: return ("CN=Sites," + ExpandWellKnownDN(context, WellKnownDN.ConfigurationNamingContext));
                case WellKnownDN.SystemContainer: return ("CN=System," + ExpandWellKnownDN(context, WellKnownDN.DefaultNamingContext));
                case WellKnownDN.RidManager: return ("CN=RID Manager$," + ExpandWellKnownDN(context, WellKnownDN.SystemContainer));
                case WellKnownDN.Infrastructure: return ("CN=Infrastructure," + ExpandWellKnownDN(context, WellKnownDN.DefaultNamingContext));
                case WellKnownDN.RootDomainNamingContext: using (var entry4 = GetEntry(context, "RootDSE")) return entry4.Properties.GetSingleValue<string>(RootDomainNamingContext);
                default: throw new ArgumentOutOfRangeException("dn", dn, "Invalid");
            }
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, WellKnownDN dn) { return GetEntry(context, null, ExpandWellKnownDN(context, dn)); }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationTypes">The authentication types.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, Func<AuthenticationTypes, AuthenticationTypes> authenticationTypes, WellKnownDN dn) { return GetEntry(context, ExpandWellKnownDN(context, dn)); }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, string dn) { return GetEntry(context, null, dn); }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationTypes">The authentication types.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, Func<AuthenticationTypes, AuthenticationTypes> authenticationTypes, string dn)
        {
            var pathname = (UnsafeNativeMethods.IAdsPathname)new UnsafeNativeMethods.Pathname();
            pathname.EscapedMode = 2;
            var bindingPrefix = "LDAP://" + context.ServerName + "/";
            pathname.Set(dn, 4);
            return Bind(bindingPrefix + pathname.Retrieve(7), context.UserName, context.Password, context.UseServerBind, authenticationTypes);
        }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dn">The dn.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, string dn, string userName, string password) { return GetEntry(context, null, dn, userName, password); }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationTypes">The authentication types.</param>
        /// <param name="dn">The dn.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static DirectoryEntry GetEntry(LdapContext context, Func<AuthenticationTypes, AuthenticationTypes> authenticationTypes, string dn, string userName, string password)
        {
            var pathname = (UnsafeNativeMethods.IAdsPathname)new UnsafeNativeMethods.Pathname();
            pathname.EscapedMode = 2;
            var bindingPrefix = "LDAP://" + context.ServerName + "/";
            pathname.Set(dn, 4);
            return Bind(bindingPrefix + pathname.Retrieve(7), userName, password, context.UseServerBind, authenticationTypes);
        }
        internal static DirectoryEntry GetEntryInternal(LdapContext context, Func<AuthenticationTypes, AuthenticationTypes> authenticationTypes, string path) { return Bind(path, context.UserName, context.Password, context.UseServerBind, authenticationTypes); }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public DirectoryEntry GetEntry(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("id");
            if (_bindingPrefix == null)
                _bindingPrefix = "LDAP://" + ServerName + "/";
            var path = "<GUID=" + id.ToString() + ">";
            return Bind(_bindingPrefix + path, UserName, Password, UseServerBind, null);
        }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public DirectoryEntry GetEntry(string dn)
        {
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            if (_bindingPrefix == null)
                _bindingPrefix = "LDAP://" + ServerName + "/";
            _pathname.Set(dn, 4);
            return Bind(_bindingPrefix + _pathname.Retrieve(7), UserName, Password, UseServerBind, null);
        }
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public DirectoryEntry GetEntry(SecurityIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (_bindingPrefix == null)
                _bindingPrefix = "LDAP://" + ServerName + "/";
            var path = "<SID=" + id.Value + ">";
            return Bind(_bindingPrefix + path, UserName, Password, UseServerBind, null);
        }

        /// <summary>
        /// Gets the entry by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public DirectoryEntry GetEntryByToken(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            var bytes = Convert.FromBase64String(token);
            return (bytes != null ? GetEntry(new SecurityIdentifier(bytes, 0)) : null);
        }

        /// <summary>
        /// Tries the get entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public bool TryGetEntry(Guid id, out DirectoryEntry entry)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("id");
            try { entry = GetEntry(id); }
            catch (Exception) { entry = null; return false; }
            return true;
        }
        /// <summary>
        /// Tries the get entry.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public bool TryGetEntry(string dn, out DirectoryEntry entry)
        {
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            try { entry = GetEntry(dn); }
            catch (Exception) { entry = null; return false; }
            return true;
        }
        /// <summary>
        /// Tries the get entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public bool TryGetEntry(SecurityIdentifier id, out DirectoryEntry entry)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            try { entry = GetEntry(id); }
            catch (Exception) { entry = null; return false; }
            return true;
        }

        #region Cache

        /// <summary>
        /// Gets the cached entries.
        /// </summary>
        /// <returns></returns>
        public ICollection GetCachedEntries() { return _cache.Values; }

        /// <summary>
        /// Gets the cached entry.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public DirectoryEntry GetCachedEntry(WellKnownDN dn) { return GetCachedEntry(ExpandWellKnownDN(dn)); }
        /// <summary>
        /// Gets the cached entry.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public DirectoryEntry GetCachedEntry(string dn)
        {
            object key = dn;
            if (!string.Equals(dn, "rootdse", StringComparison.OrdinalIgnoreCase) && !string.Equals(dn, "schema", StringComparison.OrdinalIgnoreCase))
                key = new DistinguishedName(dn);
            if (!_cache.ContainsKey(key))
            {
                var entry = GetEntry(dn);
                _cache.Add(key, entry);
            }
            return (DirectoryEntry)this._cache[key];
        }

        /// <summary>
        /// Removes the cached entry.
        /// </summary>
        /// <param name="dn">The dn.</param>
        public void RemoveCachedEntry(string dn)
        {
            object key = dn;
            if (!string.Equals(dn, "rootdse", StringComparison.OrdinalIgnoreCase) && !string.Equals(dn, "schema", StringComparison.OrdinalIgnoreCase))
                key = new DistinguishedName(dn);
            if (_cache.ContainsKey(key))
            {
                var entry = (DirectoryEntry)_cache[key];
                if (entry != null)
                {
                    _cache.Remove(key);
                    entry.Dispose();
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the primary group.
        /// </summary>
        /// <param name="aEntry">A entry.</param>
        /// <param name="aDomainEntry">A domain entry.</param>
        /// <returns></returns>
        public string GetPrimaryGroup(DirectoryEntry aEntry, DirectoryEntry aDomainEntry)
        {
            var primaryGroupID = (int)aEntry.Properties[LdapProperty.primaryGroupID].Value;
            var objectSid = (byte[])aEntry.Properties[LdapProperty.objectSid].Value;
            var escapedGroupSid = new StringBuilder();
            // copy over everything but the last four bytes(sub-authority) doing so gives us the RID of the domain
            for (uint i = 0; i < objectSid.Length - 4; i++)
                escapedGroupSid.AppendFormat("\\{0:x2}", objectSid[i]);
            // Add the primaryGroupID to the escape string to build the SID of the primaryGroup
            for (uint i = 0; i < 4; i++)
            {
                escapedGroupSid.AppendFormat("\\{0:x2}", (primaryGroupID & 0xFF));
                primaryGroupID >>= 8;
            }
            // search the directory for a group with this SID
            var searcher = new DirectorySearcher();
            if (aDomainEntry != null)
                searcher.SearchRoot = aDomainEntry;
            searcher.Filter = "(&(objectCategory=Group)(objectSID=" + escapedGroupSid.ToString() + "))";
            searcher.PropertiesToLoad.Add(LdapProperty.distinguishedName);
            return searcher.FindOne().Properties[LdapProperty.distinguishedName][0].ToString();
        }
    }
}