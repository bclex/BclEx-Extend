#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskPrincipal
    {
        private const string _localSystemAcct = "SYSTEM";
        private TaskPrincipalPrivileges _reqPriv;
        private ITask _v1Task;
        private IPrincipal _v2Principal;
        private IPrincipal2 _v2Principal2;

        internal TaskPrincipal(ITask iTask)
        {
            _v1Task = iTask;
        }

        internal TaskPrincipal(IPrincipal iPrincipal)
        {
            _v2Principal = iPrincipal;
            try { _v2Principal2 = (IPrincipal2)_v2Principal; }
            catch { }
        }

        public void Dispose()
        {
            if (_v2Principal != null)
                Marshal.ReleaseComObject(_v2Principal);
            _v1Task = null;
        }

        public override string ToString()
        {
            if (LogonType != TaskLogonType.Group)
                return UserId;
            return GroupId;
        }

        public string DisplayName
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.DisplayName;
                return null;
            }
            set
            {
                if (_v2Principal == null)
                    throw new NotV1SupportedException();
                _v2Principal.DisplayName = value;
            }
        }

        public string GroupId
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.GroupId;
                return null;
            }
            set
            {
                if (_v2Principal == null)
                    throw new NotV1SupportedException();
                if (string.IsNullOrEmpty(value))
                    value = null;
                else
                {
                    _v2Principal.UserId = null;
                    _v2Principal.LogonType = TaskLogonType.Group;
                }
                _v2Principal.GroupId = value;
            }
        }

        public string Id
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.Id;
                return Local.TaskDefaultPrincipal;
            }
            set
            {
                if (_v2Principal == null)
                    throw new NotV1SupportedException();
                _v2Principal.Id = value;
            }
        }

        public TaskLogonType LogonType
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.LogonType;
                if (UserId == "SYSTEM")
                    return TaskLogonType.ServiceAccount;
                if ((_v1Task.GetFlags() & TaskFlags.RunOnlyIfLoggedOn) == TaskFlags.RunOnlyIfLoggedOn)
                    return TaskLogonType.InteractiveToken;
                return TaskLogonType.InteractiveTokenOrPassword;
            }
            set
            {
                if (_v2Principal != null)
                    _v2Principal.LogonType = value;
                else
                {
                    if (value == TaskLogonType.Group || value == TaskLogonType.None || value == TaskLogonType.S4U)
                        throw new NotV1SupportedException();
                    var flags = _v1Task.GetFlags();
                    if (value == TaskLogonType.InteractiveToken)
                        flags |= TaskFlags.RunOnlyIfLoggedOn;
                    else
                        flags &= ~TaskFlags.RunOnlyIfLoggedOn;
                    _v1Task.SetFlags(flags);
                }
            }
        }

        private TaskProcessTokenSidType ProcessTokenSidType
        {
            get
            {
                if (_v2Principal2 != null)
                    return _v2Principal2.ProcessTokenSidType;
                return TaskProcessTokenSidType.Default;
            }
            set
            {
                if (_v2Principal2 == null)
                    throw new NotV1SupportedException();
                _v2Principal2.ProcessTokenSidType = value;
            }
        }

        private TaskPrincipalPrivileges RequiredPrivileges
        {
            get
            {
                if (_reqPriv == null)
                    _reqPriv = new TaskPrincipalPrivileges(_v2Principal2);
                return _reqPriv;
            }
        }

        public TaskRunLevel RunLevel
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.RunLevel;
                return TaskRunLevel.Highest;
            }
            set
            {
                if (_v2Principal == null)
                    throw new NotV1SupportedException();
                _v2Principal.RunLevel = value;
            }
        }

        public string UserId
        {
            get
            {
                if (_v2Principal != null)
                    return _v2Principal.UserId;
                try
                {
                    var accountInformation = (string)_v1Task.GetAccountInformation();
                    return (string.IsNullOrEmpty(accountInformation) ? "SYSTEM" : accountInformation);
                }
                catch { return null; }
            }
            set
            {
                if (_v2Principal != null)
                {
                    if (string.IsNullOrEmpty(value))
                        value = null;
                    else
                    {
                        _v2Principal.GroupId = null;
                        if (value.Contains(@"\") && !value.Contains(@"\\"))
                            value = value.Replace(@"\", @"\\");
                    }
                    _v2Principal.UserId = value;
                }
                else
                {
                    if (value.Equals("SYSTEM", StringComparison.CurrentCultureIgnoreCase))
                        value = "";
                    _v1Task.SetAccountInformation(value, IntPtr.Zero);
                }
            }
        }
    }
}
