
namespace System.DirectoryServices
{
    public static partial class DirectoryServicesExtensions
    {
        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="container">The container.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="userToken">The user token.</param>
        /// <returns></returns>
        public static bool ValidateUser(this LdapContext ctx, string container, string userID, string password, out string userToken)
        {
            if (ctx == null)
                throw new ArgumentNullException("ctx");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(userID))
                throw new ArgumentNullException("userID");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "Password required for user authentication");
            var username = ctx.GetUserAttributeBySearchProperty(container, userID, out userToken);
            if (string.IsNullOrEmpty(username))
            {
                userToken = "The user name or password is incorrect.";
                return false;
            }
            using (var entry = LdapContext.GetEntry(ctx, a => a & ~AuthenticationTypes.Secure, container, username, password))
            {
                try { object nativeObject = entry.NativeObject; }
                catch (DirectoryServicesCOMException ex) { userToken = ex.Message; return false; }
                catch (Exception ex) { userToken = ex.Message; return false; }
                return true;
            }
        }
    }
}
