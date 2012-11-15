using System.Text;

namespace System.DirectoryServices
{
    /// <summary>
    /// LdapQuery
    /// </summary>
    public class LdapQuery
    {
        private StringBuilder _builder = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="LdapQuery"/> class.
        /// </summary>
        public LdapQuery() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapQuery"/> class.
        /// </summary>
        /// <param name="attribue">The attribue.</param>
        /// <param name="value">The value.</param>
        public LdapQuery(string attribue, object value)
            : this(attribue, "=", value) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapQuery"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public LdapQuery(string attribute, params LdapQuery[] value) { Add(attribute, value); }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapQuery"/> class.
        /// </summary>
        /// <param name="attribue">The attribue.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="value">The value.</param>
        public LdapQuery(string attribue, string operation, object value) { Add(attribue, (value is LdapQuery ? value : operation + LdapPath.EncodeFilter(value.ToString(), false))); }

        /// <summary>
        /// Adds the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public virtual void Add(string attribute, params object[] value)
        {
            var b = new StringBuilder();
            foreach (var q in value)
                b.Append(q);
            _builder.AppendFormat("({0}{1})", attribute, b);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() { return _builder.ToString(); }
    }
}