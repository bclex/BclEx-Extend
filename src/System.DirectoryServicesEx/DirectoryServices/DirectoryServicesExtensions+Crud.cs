using System.Diagnostics;
using System.Security.Principal;

namespace System.DirectoryServices
{
    public static partial class DirectoryServicesExtensions
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry">The entry.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetValue<T>(this DirectoryEntry entry, string propertyName, T value)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            var properties = entry.Properties;
            if (!properties.Contains(propertyName))
                return;
            var values = properties[propertyName];
            // if string and empty then clear.
            if (typeof(T) == typeof(string) && string.IsNullOrEmpty((string)(object)value))
            {
                if (values.Count > 0)
                    values.Clear();
                return;
            }
            values.Value = value;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whatIf">if set to <c>true</c> [what if].</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string CreateUser(this Func<string, DirectoryEntry> source, bool whatIf, string objectName, string name)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Trace.TraceInformation("CreateUser: " + name);
            if (whatIf)
                return "C: " + name + "\n";
            using (var de = source(null))
            using (var newDe = de.Children.Add("CN=" + name, objectName))
            {
                newDe.Properties["userPrincipalName"].Value = name;
                newDe.CommitChanges();
            }
            return null;
        }

        /// <summary>
        /// Creates the user proxy.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whatIf">if set to <c>true</c> [what if].</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="objectSid">The object sid.</param>
        /// <returns></returns>
        public static string CreateUserProxy(this Func<string, DirectoryEntry> source, bool whatIf, string objectName, string name, SecurityIdentifier objectSid)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (objectSid == null)
                throw new ArgumentNullException("objectSid");
            var objectSidAsBytes = new byte[objectSid.BinaryLength];
            objectSid.GetBinaryForm(objectSidAsBytes, 0);
            var sid = "{\\" + BitConverter.ToString(objectSidAsBytes).Replace("-", "\\") + "}";
            Trace.TraceInformation("CreateUserProxy: " + name + " " + sid);
            if (whatIf)
                return "CP: " + name + " " + sid + "\n";
            using (var de = source(null))
            using (var newDe = de.Children.Add("CN=" + name, objectName))
            {
                newDe.Properties["objectSid"].Clear();
                newDe.Properties["objectSid"].Value = objectSidAsBytes;
                newDe.Properties["userPrincipalName"].Value = name;
                newDe.CommitChanges();
            }
            return null;
        }

        /// <summary>
        /// Moves the principal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whatIf">if set to <c>true</c> [what if].</param>
        /// <param name="sourceDN">The source DN.</param>
        /// <param name="targetDN">The target DN.</param>
        /// <returns></returns>
        public static string MovePrincipal(this Func<string, DirectoryEntry> source, bool whatIf, string sourceDN, string targetDN)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(sourceDN))
                throw new ArgumentNullException("sourceDN");
            if (string.IsNullOrEmpty(targetDN))
                throw new ArgumentNullException("targetDN");
            Trace.TraceInformation("MovePrincipal: " + sourceDN + "->" + targetDN);
            if (whatIf)
                return "R: " + sourceDN + "->" + targetDN + "\n";
            using (var sourceDE = source(sourceDN))
            using (var targetDE = source(LdapPath.GetContainerFromDN(targetDN)))
                sourceDE.MoveTo(targetDE, "CN=" + LdapPath.GetNameFromDN(targetDN));
            return null;
        }

        /// <summary>
        /// Purges the principal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whatIf">if set to <c>true</c> [what if].</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public static string PurgePrincipal(this Func<string, DirectoryEntry> source, bool whatIf, string dn)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(dn))
                throw new ArgumentNullException("dn");
            Trace.TraceInformation("Delete: " + dn);
            if (whatIf)
                return "P: " + dn + "\n";
            using (var de = source(dn))
                de.DeleteTree();
            return null;
        }
    }
}
