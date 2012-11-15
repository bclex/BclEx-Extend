using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace System.DirectoryServices
{
    internal static class ExceptionHelper
    {
        private static readonly MethodInfo _prepForRemotingMethod = typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _internalPreserveStackTraceMethod = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static Exception GetException(this COMException e, LdapContext context)
        {
            var errorCode = e.ErrorCode;
            var message = e.Message;
            switch (errorCode)
            {
                case -2147024891: return new UnauthorizedAccessException(message, e);
                case -2147023570: return new AuthenticationException(message, e);
                case -2147016657: return new InvalidOperationException(message, e);
                case -2147016651: return new InvalidOperationException(message, e);
                case -2147019886: return new ActiveDirectoryObjectExistsException(message, e);
                case -2147024888: return new OutOfMemoryException();
                case -2147016646:
                case -2147016690:
                case -2147016689: return new ActiveDirectoryServerDownException(message, e, errorCode, (context != null ? context.ServerName : null));
                default: return new ActiveDirectoryOperationException(message, e, errorCode);
            }
        }

        internal static Exception PrepareForRethrow(this Exception exception) { return PrepareForRethrow(exception, false); }
        internal static Exception PrepareForRethrow(this Exception exception, bool remoting)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (remoting)
                _prepForRemotingMethod.Invoke(exception, null);
            else
                _internalPreserveStackTraceMethod.Invoke(exception, null);
            return exception;
        }
    }
}