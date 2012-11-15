using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.DirectoryServices
{
    /// <summary>
    /// DirectoryServicesExtensions
    /// </summary>
    public static partial class DirectoryServicesExtensions
    {
        /// <summary>
        /// QueryInfo
        /// </summary>
        public class QueryInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="QueryInfo"/> class.
            /// </summary>
            public QueryInfo()
            {
                BatchSize = 1000;
                QueryFilter = "{0}";
            }
            /// <summary>
            /// Gets or sets the size of the batch.
            /// </summary>
            /// <value>
            /// The size of the batch.
            /// </value>
            public int BatchSize { get; set; }
            /// <summary>
            /// Gets or sets the query filter.
            /// </summary>
            /// <value>
            /// The query filter.
            /// </value>
            public string QueryFilter { get; set; }
            /// <summary>
            /// Gets or sets the limiter.
            /// </summary>
            /// <value>
            /// The limiter.
            /// </value>
            public Action<DirectorySearcher> Limiter { get; set; }
        }

        private static PropertyCollection GuardProperty(PropertyCollection properties, string propertyName)
        {
            try
            {
                if (properties[propertyName].Count == 0)
                {
                    if (properties[LdapProperty.distinguishedName].Count != 0)
                        throw new ActiveDirectoryOperationException(string.Format(Local.PropertyNotFoundAB, propertyName, properties[LdapProperty.distinguishedName].Value));
                    throw new ActiveDirectoryOperationException(string.Format(Local.PropertyNotFoundA, propertyName));
                }
            }
            catch (COMException e) { throw e.GetException(null); }
            catch (Exception e) { throw e; }
            return properties;
        }

        private static ResultPropertyCollection GuardProperty(ResultPropertyCollection properties, string propertyName)
        {
            try
            {
                var values = properties[propertyName];
                if (values == null || values.Count < 1)
                    throw new ActiveDirectoryOperationException(string.Format(Local.PropertyNotFoundA, propertyName));
            }
            catch (COMException e) { throw e.GetException(null); }
            catch (Exception e) { throw e; }
            return properties;
        }

        internal static byte[] ToByteArray(this SecurityIdentifier source)
        {
            if (source == null)
                return null;
            var bytes = new byte[source.BinaryLength];
            source.GetBinaryForm(bytes, 0);
            return bytes;
        }

        #region Sid/Guid/Token

        /// <summary>
        /// Gets the sid.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SecurityIdentifier GetSid(this DirectoryEntry source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source.Properties, LdapProperty.objectSid);
            var objectSid = (byte[])properties[LdapProperty.objectSid].Value;
            return (objectSid != null ? new SecurityIdentifier(objectSid, 0) : null);
        }

        /// <summary>
        /// Gets the sid.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SecurityIdentifier GetSid(this SearchResult source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source.Properties, LdapProperty.objectSid);
            var values = properties[LdapProperty.objectSid];
            return (values != null && values.Count > 0 && values[0] is byte[] ? new SecurityIdentifier((byte[])values[0], 0) : null);
        }

        /// <summary>
        /// Gets the sid.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SecurityIdentifier GetSid(this PropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source, LdapProperty.objectSid);
            var objectSid = (byte[])properties[LdapProperty.objectSid].Value;
            return (objectSid != null ? new SecurityIdentifier(objectSid, 0) : null);
        }

        /// <summary>
        /// Gets the sid.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SecurityIdentifier GetSid(this ResultPropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source, LdapProperty.objectSid);
            var values = properties[LdapProperty.objectSid];
            return (values != null && values.Count > 0 && values[0] is byte[] ? new SecurityIdentifier((byte[])values[0], 0) : null);
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Guid GetGuid(this DirectoryEntry source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source.Properties, LdapProperty.objectGUID);
            var objectGuid = (byte[])properties[LdapProperty.objectGUID].Value;
            return (objectGuid != null ? new Guid(objectGuid) : Guid.Empty);
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Guid GetGuid(this SearchResult source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source.Properties, LdapProperty.objectGUID);
            var values = properties[LdapProperty.objectGUID];
            return (values != null && values.Count > 0 && values[0] is byte[] ? new Guid((byte[])values[0]) : Guid.Empty);
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Guid GetGuid(this PropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source, LdapProperty.objectGUID);
            var objectGuid = (byte[])properties[LdapProperty.objectGUID].Value;
            return (objectGuid != null ? new Guid(objectGuid) : Guid.Empty);
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Guid GetGuid(this ResultPropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var properties = GuardProperty(source, LdapProperty.objectGUID);
            var values = properties[LdapProperty.objectGUID];
            return (values != null && values.Count > 0 && values[0] is byte[] ? new Guid((byte[])values[0]) : Guid.Empty);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetToken(this DirectoryEntry source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var sid = source.GetSid();
            return (sid != null ? Convert.ToBase64String(sid.ToByteArray()) : null);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetToken(this SearchResult source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var sid = source.GetSid();
            return (sid != null ? Convert.ToBase64String(sid.ToByteArray()) : null);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetToken(this PropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var sid = source.GetSid();
            return (sid != null ? Convert.ToBase64String(sid.ToByteArray()) : null);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string GetToken(this ResultPropertyCollection source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var sid = source.GetSid();
            return (sid != null ? Convert.ToBase64String(sid.ToByteArray()) : null);
        }

        #endregion

        /// <summary>
        /// Gets the single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetSingleValue<T>(this DirectoryEntry source, string propertyName) { if (source == null) throw new ArgumentNullException("source"); return source.Properties.GetSingleValue<T>(propertyName); }
        /// <summary>
        /// Gets the single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetSingleValue<T>(this PropertyCollection source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            GuardProperty(source, propertyName);
            var values = source[propertyName];
            return (values != null && values.Count > 0 && values[0] is T ? (T)values[0] : default(T));
        }
        /// <summary>
        /// Gets the single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetSingleValue<T>(this SearchResult source, string propertyName) { if (source == null) throw new ArgumentNullException("source"); return source.Properties.GetSingleValue<T>(propertyName); }
        /// <summary>
        /// Gets the single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetSingleValue<T>(this ResultPropertyCollection source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            GuardProperty(source, propertyName);
            var values = source[propertyName];
            return (values != null && values.Count > 0 && values[0] is T ? (T)values[0] : default(T));
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this DirectoryEntry source, string propertyName) { if (source == null) throw new ArgumentNullException("source"); return source.Properties.GetValues<T>(propertyName); }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this PropertyCollection source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            GuardProperty(source, propertyName);
            var values = source[propertyName];
            return (values != null && values.Count > 0 ? values.Cast<T>() : Enumerable.Empty<T>());
        }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this SearchResult source, string propertyName) { if (source == null) throw new ArgumentNullException("source"); return source.Properties.GetValues<T>(propertyName); }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this ResultPropertyCollection source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            GuardProperty(source, propertyName);
            var values = source[propertyName];
            return (values != null && values.Count > 0 ? values.Cast<T>() : Enumerable.Empty<T>());
        }

        /// <summary>
        /// Queries the first or default.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <returns></returns>
        public static ResultPropertyCollection QueryFirstOrDefault(this DirectoryEntry source, string filter, params string[] propertiesToLoad) { return QueryFirstOrDefault(source, filter, SearchScope.Subtree, propertiesToLoad); }
        /// <summary>
        /// Queries the first or default.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <returns></returns>
        public static ResultPropertyCollection QueryFirstOrDefault(this DirectoryEntry source, string filter, SearchScope scope, params string[] propertiesToLoad)
        {
            try
            {
                using (var searcher = new DirectorySearcher(source, filter, propertiesToLoad, scope))
                {
                    var result = searcher.FindOne();
                    return (result != null ? result.Properties : null);
                }
            }
            //catch (DirectoryServicesCOMException ex) { }
            catch (COMException e) { throw e.GetException(null).PrepareForRethrow(); }
            catch (Exception e) { throw e.PrepareForRethrow(); }
        }

        /// <summary>
        /// Queries the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <returns></returns>
        public static SearchResultCollection QueryMany(this DirectoryEntry source, string filter, params string[] propertiesToLoad) { return QueryMany(source, filter, SearchScope.Subtree, propertiesToLoad); }
        /// <summary>
        /// Queries the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <returns></returns>
        public static SearchResultCollection QueryMany(this DirectoryEntry source, string filter, SearchScope scope, params string[] propertiesToLoad)
        {
            try
            {
                using (var searcher = new DirectorySearcher(source, filter, propertiesToLoad, scope))
                {
                    searcher.PageSize = 1000;
                    return searcher.FindAll();
                }
            }
            //catch (DirectoryServicesCOMException ex) { }
            catch (COMException e) { throw e.GetException(null).PrepareForRethrow(); }
            catch (Exception e) { throw e.PrepareForRethrow(); }
        }

        /// <summary>
        /// Queries the by identities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="identityProperty">The identity property.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <param name="identities">The identities.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryByIdentities<T>(this DirectoryEntry source, string identityProperty, string[] propertiesToLoad, IEnumerable<string> identities, Func<SearchResult, T> selector) { return source.QueryByIdentities<string, T>(identityProperty, propertiesToLoad, identities, selector, null); }
        /// <summary>
        /// Queries the by identities.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="identityProperty">The identity property.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        /// <param name="identities">The identities.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="queryInfo">The query info.</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryByIdentities<TSource, T>(this DirectoryEntry source, string identityProperty, string[] propertiesToLoad, IEnumerable<TSource> identities, Func<SearchResult, T> selector, QueryInfo queryInfo)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(identityProperty))
                throw new ArgumentNullException("identityProperty");
            if (propertiesToLoad == null || propertiesToLoad.Length == 0)
                propertiesToLoad = new string[] { };
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (queryInfo == null)
                queryInfo = new QueryInfo();
            //
            try
            {
                var list = new List<T>();
                foreach (var batch in identities.GroupAt(queryInfo.BatchSize))
                {
                    var filterPart = "(" + LdapPath.EncodeFilter(identityProperty, true) + "=" + string.Join(")(" + LdapPath.EncodeFilter(identityProperty, true) + "=", batch.Select(LdapPath.EncodeFilter).ToArray()) + ")";
                    var searcher = new DirectorySearcher(source, string.Format(queryInfo.QueryFilter, "(|" + filterPart + ")"), propertiesToLoad);
                    searcher.PageSize = 1000;
                    if (queryInfo.Limiter != null)
                        queryInfo.Limiter(searcher);
                    list.AddRange(
                        searcher.FindAll()
                        .Cast<SearchResult>()
                        .Select(selector)
                        .OfType<T>());
                }
                return list;
            }
            catch (COMException e) { throw e.GetException(null).PrepareForRethrow(); }
            catch (Exception e) { throw e.PrepareForRethrow(); }
        }

        /// <summary>
        /// Queries the D ns by invoke.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static IEnumerable<string> QueryDNsByInvoke(this DirectoryEntry source, string methodName, params object[] args)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var items = (IEnumerable)source.Invoke(methodName, args);
            if (items == null)
                return null;
            var itemAsString = (items as string);
            if (itemAsString != null)
                return new[] { itemAsString };
            var itemAsAds = (items as UnsafeNativeMethods.IAds);
            if (itemAsAds != null)
                return new[] { LdapPath.GetDNFromADsPath(itemAsAds.ADsPath) };
            return items.Cast<object>()
                .Select(item =>
                {
                    itemAsString = (item as string);
                    if (itemAsString != null)
                        return itemAsString;
                    itemAsAds = (item as UnsafeNativeMethods.IAds);
                    if (itemAsAds != null)
                        return LdapPath.GetDNFromADsPath(itemAsAds.ADsPath);
                    return null;
                })
                .Where(x => x != null)
                .ToList();
        }

        /// <summary>
        /// Queries the D ns by invoke get.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IEnumerable<string> QueryDNsByInvokeGet(this DirectoryEntry source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var members = (IEnumerable)source.InvokeGet(propertyName);
            if (members == null)
                return null;
            var itemAsString = (members as string);
            if (itemAsString != null)
                return new[] { itemAsString };
            var itemAsAds = (members as UnsafeNativeMethods.IAds);
            if (itemAsAds != null)
                return new[] { LdapPath.GetDNFromADsPath(itemAsAds.ADsPath) };
            return members.Cast<object>()
                .Select(item =>
                {
                    itemAsString = (item as string);
                    if (itemAsString != null)
                        return itemAsString;
                    itemAsAds = (item as UnsafeNativeMethods.IAds);
                    if (itemAsAds != null)
                        return LdapPath.GetDNFromADsPath(itemAsAds.ADsPath);
                    return null;
                })
                .Where(x => x != null)
                .ToList();
        }

        /// <summary>
        /// Queries the direct reports.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dn">The dn.</param>
        /// <param name="set">The set.</param>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        public static IEnumerable<string> QueryDirectReports(this Func<string, DirectoryEntry> source, string dn, HashSet<string> set, int depth)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            if (set == null)
                throw new ArgumentNullException("set");
            if (set.Contains(dn))
                throw new InvalidOperationException("Recursive directReports : " + dn);
            set.Add(dn);
            using (var de = source(dn))
            {
                var childDNs = de.QueryDNsByInvokeGet("directReports");
                if (childDNs == null)
                    yield break;
                foreach (var childDN in childDNs)
                {
                    yield return childDN;
                    // recurse
                    var childs = source.QueryDirectReports(childDN, set, depth + 1);
                    if (childs != null)
                        foreach (var child in childs)
                            yield return child;
                }
            }
        }
    }
}
