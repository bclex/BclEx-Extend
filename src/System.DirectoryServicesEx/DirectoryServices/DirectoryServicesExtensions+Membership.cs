using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace System.DirectoryServices
{
    public static partial class DirectoryServicesExtensions
    {
        /// <summary>
        /// Gets the member of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="container">The container.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMemberOf<T>(this DirectoryEntry source, string container, Func<string, T> selector) { return GetMemberOf<T>(source, container, null, selector); }
        /// <summary>
        /// Gets the member of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="container">The container.</param>
        /// <param name="suffix">The suffix.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMemberOf<T>(this DirectoryEntry source, string container, string suffix, Func<string, T> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentNullException("container");
            suffix = (suffix ?? string.Empty);
            if (selector == null)
                throw new ArgumentNullException("selector");
            var memberOf = source.Properties[LdapProperty.memberOf].Value;
            var filter = suffix + "," + container;
            //
            IEnumerable<string> groupDNs;
            if (memberOf is object[])
                groupDNs = ((object[])memberOf).Cast<string>();
            else if (memberOf is string)
                groupDNs = new[] { (string)memberOf };
            else
                yield break;
            //
            foreach (var groupDN in groupDNs)
                if (groupDN.EndsWith(filter) && groupDN.LastIndexOf(',', groupDN.Length - filter.Length - 1) == -1)
                    yield return selector(groupDN.Substring(3, groupDN.Length - filter.Length - 3) + suffix);
        }

        /// <summary>
        /// Determines whether [is member of] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="groupDN">The group DN.</param>
        /// <returns>
        ///   <c>true</c> if [is member of] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMemberOf(this DirectoryEntry source, string groupDN)
        {
            var memberOf = source.Properties[LdapProperty.memberOf].Value;
            if (memberOf is object[])
                return ((object[])memberOf).Cast<string>().Any(c => c == groupDN);
            else if (memberOf is string)
                return ((string)memberOf == groupDN);
            return false;
        }

        /// <summary>
        /// Merges the group membership.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whatIf">if set to <c>true</c> [what if].</param>
        /// <param name="dn">The dn.</param>
        /// <param name="members">The members.</param>
        /// <param name="removeMembers">if set to <c>true</c> [remove members].</param>
        /// <param name="inserts">The inserts.</param>
        /// <param name="deletes">The deletes.</param>
        /// <returns></returns>
        public static string MergeGroupMembership(this Func<string, DirectoryEntry> source, bool whatIf, string dn, IEnumerable<string> members, bool removeMembers, out int inserts, out int deletes)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            if (members == null)
                throw new ArgumentNullException("members");
            Trace.TraceInformation("Merge: " + dn);
            var b = new StringBuilder();
            //
            deletes = 0;
            inserts = 0;
            using (var rootDE = source("/"))
            {
                var de = source(dn);
                try
                {
                    var deMembers = de.QueryDNsByInvoke("members", null);
                    var deRemoves = deMembers.Except(members, StringComparer.OrdinalIgnoreCase).ToList();
                    deletes = deRemoves.Count;
                    if (removeMembers && deletes > 0)
                        foreach (var g in deRemoves.GroupAt(100))
                        {
                            if (whatIf)
                                foreach (var v in g)
                                    b.AppendLine("MD: " + v);
                            else
                                b.Append(source.MergeGroupMembership_Delete(ref de, dn, g));
                        }
                    //
                    var deInserts = members.Except(deMembers, StringComparer.OrdinalIgnoreCase).ToList();
                    inserts = deInserts.Count;
                    if (inserts > 0)
                        foreach (var g in deInserts.GroupAt(50))
                        {
                            if (whatIf)
                                foreach (var v in g)
                                    b.AppendLine("MI: " + v);
                            else
                                b.Append(source.MergeGroupMembership_Insert(rootDE, ref de, dn, g));
                        }
                }
                catch { if (de != null) de.Dispose(); }
                return (b.Length > 0 ? b.ToString() : null);
            }
        }

        private static string MergeGroupMembership_Delete(this Func<string, DirectoryEntry> source, ref DirectoryEntry de, string dn, IEnumerable<string> g)
        {
            try
            {
                foreach (var v in g)
                {
                    Trace.TraceInformation("MD: " + v);
                    de.Properties["member"].Remove(v);
                }
                de.CommitChanges();
            }
            catch
            {
                { if (de != null) de.Dispose(); de = source(dn); }
                // regroup by 10
                var b = new StringBuilder();
                foreach (var g2 in g.GroupAt(10))
                    try
                    {
                        foreach (var v in g2)
                            de.Properties["member"].Remove(v);
                        de.CommitChanges();
                    }
                    catch
                    {
                        { if (de != null) de.Dispose(); de = source(dn); }
                        // regroup by 1
                        foreach (var v in g2)
                            try { de.Properties["member"].Remove(v); de.CommitChanges(); }
                            catch
                            {
                                { if (de != null) de.Dispose(); de = source(dn); }
                                Trace.TraceInformation("xMD: " + v); b.AppendLine("xMD: " + v);
                            }
                    }
                return b.ToString();
            }
            return null;
        }

        private static string MergeGroupMembership_Insert(this Func<string, DirectoryEntry> source, DirectoryEntry rootDE, ref DirectoryEntry de, string dn, IEnumerable<string> g)
        {
            try
            {
                foreach (var v in g)
                {
                    Trace.TraceInformation("MI: " + v); Trace.Flush();
                    de.Properties["member"].Add(v);
                }
                de.CommitChanges();
            }
            catch
            {
                { if (de != null) de.Dispose(); de = source(dn); }
                // lookup items
                g = rootDE.QueryByIdentities<string>("distinguishedName", null, g, x => x.GetDN());
                try
                {
                    foreach (var v in g)
                    {
                        Trace.TraceInformation("MI: " + v);
                        de.Properties["member"].Add(v);
                    }
                    de.CommitChanges();
                }
                catch
                {
                    { if (de != null) de.Dispose(); de = source(dn); }
                    // regroup by 10
                    var b = new StringBuilder();
                    foreach (var g2 in g.GroupAt(10))
                        try
                        {
                            foreach (var v in g2)
                                de.Properties["member"].Add(v);
                            de.CommitChanges();
                        }
                        catch
                        {
                            if (de != null) de.Dispose(); de = source(dn);
                            // regroup by 1
                            foreach (var v in g2)
                                try { de.Properties["member"].Add(v); de.CommitChanges(); }
                                catch
                                {
                                    { if (de != null) de.Dispose(); de = source(dn); }
                                    Trace.TraceInformation("xMI: " + v); b.AppendLine("xMI: " + v);
                                }
                        }
                    return b.ToString();
                }
            }
            return null;
        }
    }
}
