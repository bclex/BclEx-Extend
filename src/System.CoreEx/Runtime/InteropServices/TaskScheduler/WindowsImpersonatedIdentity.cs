#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.Security.Principal;
namespace System.Runtime.InteropServices.TaskScheduler
{
    internal class WindowsImpersonatedIdentity : IDisposable, IIdentity
    {
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private WindowsIdentity _identity;
        private WindowsImpersonationContext _impersonationContext;

        public WindowsImpersonatedIdentity(string userName, string domainName, string password)
        {
            var zero = IntPtr.Zero;
            var hNewToken = IntPtr.Zero;
            try
            {
                if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(domainName) || !string.IsNullOrEmpty(password))
                {
                    if (LogonUser(userName, domainName, password, 2, 0, ref zero) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    if (DuplicateToken(zero, 2, ref hNewToken) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    this._identity = new WindowsIdentity(hNewToken);
                    this._impersonationContext = this._identity.Impersonate();
                }
                else
                    this._identity = WindowsIdentity.GetCurrent();
            }
            finally
            {
                if (zero != IntPtr.Zero)
                    CloseHandle(zero);
                if (hNewToken != IntPtr.Zero)
                    CloseHandle(hNewToken);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        public void Dispose()
        {
            if (this._impersonationContext != null)
                this._impersonationContext.Undo();
            if (this._identity != null)
                this._identity.Dispose();
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        public string AuthenticationType
        {
            get
            {
                if (this._identity != null)
                    return this._identity.AuthenticationType;
                return null;
            }
        }

        public bool IsAuthenticated
        {
            get { return (this._identity != null && this._identity.IsAuthenticated); }
        }

        public string Name
        {
            get
            {
                if (this._identity != null)
                    return this._identity.Name;
                return null;
            }
        }
    }
}

