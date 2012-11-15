using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using System.Security.Principal;

namespace System.DirectoryServices
{
    // http://msdn.microsoft.com/en-us/library/bb267453.aspx
    /// <summary>
    /// LdapContext
    /// </summary>
    public partial class LdapContext
    {
        private DirectoryContext _context;
        private string _bindingPrefix;
        private Hashtable _cache = new Hashtable();
        private UnsafeNativeMethods.IAdsPathname _pathname;
        private static readonly bool _serverBindSupported;
        private static readonly MethodInfo _getServerNameInfo;
        private static readonly PropertyInfo _passwordInfo;

        static LdapContext()
        {
            var t = typeof(DirectoryContext);
            _getServerNameInfo = t.GetMethod("GetServerName", BindingFlags.NonPublic | BindingFlags.Instance);
            _passwordInfo = t.GetProperty("Password", BindingFlags.NonPublic | BindingFlags.Instance);
            var serverBindSupportedInfo = t.GetProperty("ServerBindSupported", BindingFlags.NonPublic | BindingFlags.Static);
            if (_getServerNameInfo == null || _passwordInfo == null || serverBindSupportedInfo == null)
                throw new Exception("Unable to get all required DirectoryContext methodInfos.");
            _serverBindSupported = (bool)serverBindSupportedInfo.GetValue(null, null);
            //
            DefaultAuthType = AuthenticationTypes.Sealing | AuthenticationTypes.Signing | AuthenticationTypes.Secure;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        public LdapContext()
            : this(new DirectoryContext(DirectoryContextType.Domain)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        public LdapContext(LdapContextType contextType)
            : this(new DirectoryContext((DirectoryContextType)contextType)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="name">The name.</param>
        public LdapContext(LdapContextType contextType, string name)
            : this(new DirectoryContext((DirectoryContextType)contextType, name)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public LdapContext(LdapContextType contextType, string username, string password)
            : this(new DirectoryContext((DirectoryContextType)contextType, username, password)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public LdapContext(LdapContextType contextType, string name, string username, string password)
            : this(new DirectoryContext((DirectoryContextType)contextType, name, username, password)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LdapContext(DirectoryContext context)
        {
            _context = context;
            _pathname = (UnsafeNativeMethods.IAdsPathname)new UnsafeNativeMethods.Pathname();
            _pathname.EscapedMode = 2;
            Containers = new Dictionary<string, string>();
            // user settings
            LdapUserNameAttribute = "uid";
            LdapUserDNAttribute = "dn";
            LdapUserFilter = "(&(objectClass=InetOrgPerson))";
            LdapUserSearchScope = SearchScope.OneLevel;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LdapContext"/> class.
        /// </summary>
        /// <param name="ldapConnectionString">The LDAP connection string.</param>
        public LdapContext(string ldapConnectionString)
            : this((DirectoryContext)null) { } // server={0};user={0};password={2};bindings=1|2|3|4;partition={4};

        private bool UseServerBind
        {
            get { return (_context.ContextType != DirectoryContextType.DirectoryServer ? _context.ContextType == DirectoryContextType.ConfigurationSet : true); }
        }

        private static AuthenticationTypes DefaultAuthType { get; set; }

        /// <summary>
        /// Gets the directory context.
        /// </summary>
        public DirectoryContext DirectoryContext
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public string ServerName
        {
            get { return (string)_getServerNameInfo.Invoke(_context, null); }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get { return _context.UserName; }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password
        {
            get { return (string)_passwordInfo.GetValue(_context, null); }
        }
    }
}