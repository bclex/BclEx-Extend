namespace System.DirectoryServices
{
    /// <summary>
    /// LdapWildCardQuery
    /// </summary>
    public class LdapWildCardQuery : LdapQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapWildCardQuery"/> class.
        /// </summary>
        public LdapWildCardQuery() 
            : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapWildCardQuery"/> class.
        /// </summary>
        /// <param name="attribue">The attribue.</param>
        /// <param name="value">The value.</param>
        public LdapWildCardQuery(string attribue, object value)
            : this(attribue, "=", value) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapWildCardQuery"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public LdapWildCardQuery(string attribute, params LdapQuery[] value) { Add(attribute, value); }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapWildCardQuery"/> class.
        /// </summary>
        /// <param name="attribue">The attribue.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="value">The value.</param>
        public LdapWildCardQuery(string attribue, string operation, object value) { Add(attribue, (value is LdapQuery ? value : operation + LdapPath.EncodeFilter(value.ToString(), false))); }
    }
}