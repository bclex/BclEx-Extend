using System.Collections.Generic;

namespace System.DirectoryServices
{
    public partial class LdapContext
    {
        /// <summary>
        /// Gets or sets the containers.
        /// </summary>
        /// <value>
        /// The containers.
        /// </value>
        public Dictionary<string, string> Containers { get; protected set; }

        //public bool TryGetContainerDN(string name, out string dn)
        //{
        //    // check name, get ref to DE
        //    dn = "";
        //    return true;
        //}
        //public bool TryGetContainer(string name, out DirectoryEntry de)
        //{
        //    // check name, get ref to DE
        //    var refToDE = "";
        //    return TryGetEntry(refToDE, out de);
        //}
    }
}