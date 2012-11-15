#region Foreign-License
// x
#endregion
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Text.RegularExpressions;
namespace System.Runtime.InteropServices.TaskScheduler
{
    [Description("Provides access to the Task Scheduler service.")]
    public sealed class TaskService : Component, IDisposable, ISupportInitialize
    {
        private bool _forceV1;
        internal static readonly bool _hasV2 = (Environment.OSVersion.Version >= new Version(6, 0));
        private bool _initializing;
        private Version _maxVer;
        private bool _maxVerSet;
        private string _targetServer;
        private bool _targetServerSet;
        private string _userDomain;
        private bool _userDomainSet;
        private string _userName;
        private bool _userNameSet;
        private string _userPassword;
        private bool _userPasswordSet;
        private WindowsImpersonatedIdentity _v1Impersonation;
        internal ITaskScheduler _v1TaskScheduler;
        internal static readonly Version _v1Ver = new Version(1, 1);
        internal TaskSchedulerClass _v2TaskService;

        public TaskService()
        {
            ResetHighestSupportedVersion();
            Connect();
        }

        public TaskService(string targetServer, string userName = null, string accountDomain = null, string password = null, bool forceV1 = false)
        {
            BeginInit();
            TargetServer = targetServer;
            UserName = userName;
            UserAccountDomain = accountDomain;
            UserPassword = password;
            _forceV1 = forceV1;
            ResetHighestSupportedVersion();
            EndInit();
        }

        public Task AddTask(string path, Trigger trigger, Action action, string UserId = null, string Password = null, TaskLogonType LogonType = (TaskLogonType)3)
        {
            var definition = NewTask();
            definition.Triggers.Add(trigger);
            definition.Actions.Add(action);
            return RootFolder.RegisterTaskDefinition(path, definition, TaskCreation.CreateOrUpdate, UserId, Password, LogonType, null);
        }

        public void BeginInit()
        {
            _initializing = true;
        }

        private void Connect()
        {
            ResetUnsetProperties();
            if ((!_initializing && !base.DesignMode) && (((!string.IsNullOrEmpty(_userDomain) && !string.IsNullOrEmpty(_userName)) && !string.IsNullOrEmpty(_userPassword)) || ((string.IsNullOrEmpty(_userDomain) && string.IsNullOrEmpty(_userName)) && string.IsNullOrEmpty(_userPassword))))
            {
                if (_v2TaskService != null || _v1TaskScheduler != null)
                    Dispose(true);
                if (_hasV2 && !_forceV1)
                {
                    _v2TaskService = new TaskSchedulerClass();
                    if (!string.IsNullOrEmpty(_targetServer))
                    {
                        if (_targetServer.StartsWith(@"\"))
                            _targetServer = _targetServer.TrimStart(new char[] { '\\' });
                        if (_targetServer.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
                            _targetServer = null;
                    }
                    _v2TaskService.Connect(_targetServer, _userName, _userDomain, _userPassword);
                    _targetServer = _v2TaskService.TargetServer;
                    _userName = _v2TaskService.ConnectedUser;
                    _userDomain = _v2TaskService.ConnectedDomain;
                    _maxVer = GetV2Version();
                }
                else
                {
                    _v1Impersonation = new WindowsImpersonatedIdentity(_userName, _userDomain, _userPassword);
                    var scheduler = new CTaskScheduler();
                    _v1TaskScheduler = (ITaskScheduler)scheduler;
                    if (!string.IsNullOrEmpty(_targetServer))
                    {
                        if (!_targetServer.StartsWith(@"\\"))
                            _targetServer = @"\\" + _targetServer;
                    }
                    else
                        _targetServer = null;
                    _v1TaskScheduler.SetTargetComputer(_targetServer);
                    _targetServer = (string)_v1TaskScheduler.GetTargetComputer();
                    _maxVer = _v1Ver;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_v2TaskService != null)
            {
                Marshal.ReleaseComObject(_v2TaskService);
                _v2TaskService = null;
            }
            if (_v1TaskScheduler != null)
            {
                Marshal.ReleaseComObject(_v1TaskScheduler);
                _v1TaskScheduler = null;
            }
            if (_v1Impersonation != null)
            {
                _v1Impersonation.Dispose();
                _v1Impersonation = null;
            }
            base.Dispose(disposing);
        }

        public void EndInit()
        {
            _initializing = false;
            Connect();
        }

        public Task[] FindAllTasks(Regex name, bool searchAllFolders = true)
        {
            var results = new List<Task>();
            FindTaskInFolder(RootFolder, name, ref results, searchAllFolders);
            return results.ToArray();
        }

        public Task FindTask(string name, bool searchAllFolders = true)
        {
            var taskArray = FindAllTasks(new Wildcard(name, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase), searchAllFolders);
            if (taskArray.Length > 0)
                return taskArray[0];
            return null;
        }

        private bool FindTaskInFolder(TaskFolder fld, Regex taskName, ref List<Task> results, bool recurse = true)
        {
            results.AddRange(fld.GetTasks(taskName));
            if (recurse)
                foreach (TaskFolder folder in fld.SubFolders)
                    if (FindTaskInFolder(folder, taskName, ref results, recurse))
                        return true;
            return false;
        }

        public TaskFolder GetFolder(string folderName)
        {
            if (_v2TaskService == null)
                return new TaskFolder(this);
            return new TaskFolder(this, _v2TaskService.GetFolder(folderName));
        }

        public RunningTaskCollection GetRunningTasks(bool includeHidden = true)
        {
            if (_v2TaskService == null)
                return new RunningTaskCollection(this);
            return new RunningTaskCollection(this, _v2TaskService.GetRunningTasks(includeHidden ? 1 : 0));
        }

        public Task GetTask(string taskPath)
        {
            Task task = null;
            if (_v2TaskService != null)
            {
                var task2 = GetTask(_v2TaskService, taskPath);
                if (task2 != null)
                    task = new Task(this, task2);
                return task;
            }
            var iTask = GetTask(_v1TaskScheduler, taskPath);
            if (iTask != null)
                task = new Task(this, iTask);
            return task;
        }

        internal static ITask GetTask(ITaskScheduler iSvc, string name)
        {
            var riid = Marshal.GenerateGuidForType(typeof(ITask));
            try { return iSvc.Activate(name, ref riid); }
            catch { }
            return null;
        }

        internal static IRegisteredTask GetTask(ITaskService iSvc, string name)
        {
            ITaskFolder o = null;
            IRegisteredTask task;
            try
            {
                o = iSvc.GetFolder(@"\");
                task = o.GetTask(name);
            }
            catch { task = null; }
            finally { if (o != null) Marshal.ReleaseComObject(o); }
            return task;
        }

        private Version GetV2Version()
        {
            var highestVersion = _v2TaskService.HighestVersion;
            return new Version((int)(highestVersion >> 0x10), ((int)highestVersion) & 0xffff);
        }

        public TaskDefinition NewTask()
        {
            if (_v2TaskService != null)
                return new TaskDefinition(_v2TaskService.NewTask(0));
            var riid = Marshal.GenerateGuidForType(typeof(ITask));
            var rclsid = Marshal.GenerateGuidForType(typeof(CTask));
            var name = "Temp" + Guid.NewGuid().ToString("B");
            return new TaskDefinition(_v1TaskScheduler.NewWorkItem(name, ref rclsid, ref riid), name);
        }

        private void ResetHighestSupportedVersion()
        {
            if (Connected)
                _maxVer = (_v2TaskService != null) ? GetV2Version() : _v1Ver;
            else
                _maxVer = _hasV2 ? ((Environment.OSVersion.Version.Minor > 0) ? new Version(1, 3) : new Version(1, 2)) : _v1Ver;
        }

        private void ResetUnsetProperties()
        {
            if (!_maxVerSet)
                ResetHighestSupportedVersion();
            if (!_targetServerSet)
                _targetServer = null;
            if (!_userDomainSet)
                _userDomain = null;
            if (!_userNameSet)
                _userName = null;
            if (!_userPasswordSet)
                _userPassword = null;
        }

        private bool ShouldSerializeHighestSupportedVersion() { return (_hasV2 && (_maxVer <= _v1Ver)); }

        private bool ShouldSerializeTargetServer() { return (_targetServer != null && !_targetServer.Trim(new char[] { '\\' }).Equals(Environment.MachineName.Trim(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase)); }

        private bool ShouldSerializeUserAccountDomain() { return (_userDomain != null && !_userDomain.Equals(Environment.UserDomainName, StringComparison.InvariantCultureIgnoreCase)); }

        private bool ShouldSerializeUserName() { return (_userName != null && !_userName.Equals(Environment.UserName, StringComparison.InvariantCultureIgnoreCase)); }

        public void StartSystemTaskSchedulerManager()
        {
            if (Environment.UserInteractive)
                Process.Start("control.exe", "schedtasks");
        }

        protected override bool CanRaiseEvents
        {
            get { return false; }
        }

        [Browsable(false)]
        public bool Connected
        {
            get { return (_v2TaskService != null && _v2TaskService.Connected || _v1TaskScheduler != null); }
        }

        [DefaultValue((string)null), Browsable(false), Obsolete("This property has been superceded by the UserAccountDomin property and may not be available in future releases.")]
        public string ConnectedDomain
        {
            get
            {
                if (_v2TaskService != null)
                    return _v2TaskService.ConnectedDomain;
                var strArray = _v1Impersonation.Name.Split(new char[] { '\\' });
                if (strArray.Length == 2)
                    return strArray[0];
                return string.Empty;
            }
        }

        [Browsable(false), DefaultValue((string)null), Obsolete("This property has been superceded by the UserName property and may not be available in future releases.")]
        public string ConnectedUser
        {
            get
            {
                if (_v2TaskService != null)
                    return _v2TaskService.ConnectedUser;
                var strArray = _v1Impersonation.Name.Split(new char[] { '\\' });
                if (strArray.Length == 2)
                    return strArray[1];
                return strArray[0];
            }
        }

        [Description("Highest version of library that should be used."), TypeConverter(typeof(VersionConverter)), Category("Data")]
        public Version HighestSupportedVersion
        {
            get { return _maxVer; }
            set
            {
                _maxVer = value;
                _maxVerSet = true;
                var flag = value <= _v1Ver;
                if (flag != _forceV1)
                {
                    _forceV1 = flag;
                    Connect();
                }
            }
        }

        [Browsable(false)]
        public TaskFolder RootFolder
        {
            get { return GetFolder(@"\"); }
        }

        [Description("The name of the computer to connect to."), Category("Data"), DefaultValue((string)null)]
        public string TargetServer
        {
            get
            {
                if (!ShouldSerializeTargetServer())
                    return null;
                return _targetServer;
            }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    value = null;
                if (string.Compare(value, _targetServer, true) != 0)
                {
                    _targetServerSet = true;
                    _targetServer = value;
                    Connect();
                }
            }
        }

        [Description("The user account domain to be used when connecting."), Category("Data"), DefaultValue((string)null)]
        public string UserAccountDomain
        {
            get
            {
                if (!ShouldSerializeUserAccountDomain())
                    return null;
                return _userDomain;
            }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    value = null;
                if (string.Compare(value, _userDomain, true) != 0)
                {
                    _userDomainSet = true;
                    _userDomain = value;
                    Connect();
                }
            }
        }

        [DefaultValue((string)null), Description("The user name to be used when connecting."), Category("Data")]
        public string UserName
        {
            get
            {
                if (!ShouldSerializeUserName())
                    return null;
                return _userName;
            }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    value = null;
                if (string.Compare(value, _userName, true) != 0)
                {
                    _userNameSet = true;
                    _userName = value;
                    Connect();
                }
            }
        }

        [Category("Data"), DefaultValue((string)null), Description("The user password to be used when connecting.")]
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    value = null;
                if (string.Compare(value, _userPassword, true) != 0)
                {
                    _userPasswordSet = true;
                    _userPassword = value;
                    Connect();
                }
            }
        }

        private class VersionConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) { return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType)); }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                    return new Version(value as string);
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}

