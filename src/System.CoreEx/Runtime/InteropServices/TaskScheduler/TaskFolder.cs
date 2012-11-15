#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskFolder : IDisposable
    {
        private ITaskScheduler _v1List;
        private ITaskFolder _v2Folder;

        internal TaskFolder(TaskService svc)
        {
            TaskService = svc;
            _v1List = svc._v1TaskScheduler;
        }
        internal TaskFolder(TaskService svc, ITaskFolder iFldr)
        {
            TaskService = svc;
            _v2Folder = iFldr;
        }

        public TaskFolder CreateFolder(string subFolderName, GenericSecurityDescriptor sd) { return CreateFolder(subFolderName, sd.GetSddlForm(AccessControlSections.All)); }
        public TaskFolder CreateFolder(string subFolderName, string sddlForm = null)
        {
            if (_v2Folder == null)
                throw new NotV1SupportedException();
            return new TaskFolder(TaskService, _v2Folder.CreateFolder(subFolderName, sddlForm));
        }

        public void DeleteFolder(string subFolderName)
        {
            if (_v2Folder == null)
                throw new NotV1SupportedException();
            _v2Folder.DeleteFolder(subFolderName, 0);
        }

        public void DeleteTask(string Name)
        {
            if (_v2Folder != null)
                _v2Folder.DeleteTask(Name, 0);
            else
            {
                if (!Name.EndsWith(".job", StringComparison.CurrentCultureIgnoreCase))
                    Name = Name + ".job";
                _v1List.Delete(Name);
            }
        }

        public void Dispose()
        {
            if (_v2Folder != null)
                Marshal.ReleaseComObject(_v2Folder);
            _v1List = null;
        }

        public GenericSecurityDescriptor GetSecurityDescriptor(AccessControlSections includeSections) { return new RawSecurityDescriptor(GetSecurityDescriptorSddlForm(includeSections)); }
        public string GetSecurityDescriptorSddlForm(AccessControlSections includeSections)
        {
            if (_v2Folder == null)
                throw new NotV1SupportedException();
            return _v2Folder.GetSecurityDescriptor((int)includeSections);
        }

        public TaskCollection GetTasks(Regex filter = null)
        {
            if (_v2Folder != null)
                return new TaskCollection(this, _v2Folder.GetTasks(1), filter);
            return new TaskCollection(TaskService, filter);
        }

        public Task RegisterTask(string Path, string XmlText, TaskCreation createType, string UserId, string password, TaskLogonType LogonType, string sddl)
        {
            if (_v2Folder == null)
                throw new NotV1SupportedException();
            return new Task(TaskService, _v2Folder.RegisterTask(Path, XmlText, (int)createType, UserId, password, LogonType, sddl));
        }

        public Task RegisterTaskDefinition(string Path, TaskDefinition definition) { return RegisterTaskDefinition(Path, definition, TaskCreation.CreateOrUpdate, (definition.Principal.LogonType == TaskLogonType.Group) ? definition.Principal.GroupId : definition.Principal.UserId, null, definition.Principal.LogonType, null); }
        public Task RegisterTaskDefinition(string Path, TaskDefinition definition, TaskCreation createType, string UserId, string password = null, TaskLogonType LogonType = (TaskLogonType)2, string sddl = null)
        {
            if (_v2Folder != null)
                return new Task(TaskService, _v2Folder.RegisterTaskDefinition(Path, definition._v2Def, (int)createType, UserId, password, LogonType, sddl));
            var flags = definition._v1Task.GetFlags();
            if (LogonType == TaskLogonType.InteractiveTokenOrPassword && string.IsNullOrEmpty(password))
                LogonType = TaskLogonType.InteractiveToken;
            switch (LogonType)
            {
                case TaskLogonType.None:
                case TaskLogonType.S4U:
                case TaskLogonType.Group:
                    throw new NotV1SupportedException("This LogonType is not supported on Task Scheduler 1.0.");
                case TaskLogonType.Password:
                    using (var str2 = new CoTaskMemString(password))
                        definition._v1Task.SetAccountInformation(UserId, str2.DangerousGetHandle());
                    break;
                case TaskLogonType.InteractiveToken:
                    flags |= TaskFlags.RunOnlyIfLoggedOn | TaskFlags.Interactive;
                    if (string.IsNullOrEmpty(UserId))
                        UserId = WindowsIdentity.GetCurrent().Name;
                    definition._v1Task.SetAccountInformation(UserId, IntPtr.Zero);
                    break;
                case TaskLogonType.ServiceAccount:
                    flags &= ~(TaskFlags.RunOnlyIfLoggedOn | TaskFlags.Interactive);
                    definition._v1Task.SetAccountInformation((string.IsNullOrEmpty(UserId) || UserId.Equals("SYSTEM", StringComparison.CurrentCultureIgnoreCase)) ? string.Empty : UserId, IntPtr.Zero);
                    break;
                case TaskLogonType.InteractiveTokenOrPassword:
                    flags |= TaskFlags.Interactive;
                    using (var str = new CoTaskMemString(password))
                    {
                        definition._v1Task.SetAccountInformation(UserId, str.DangerousGetHandle());
                        break;
                    }
            }
            definition._v1Task.SetFlags(flags);
            switch (createType)
            {
                case TaskCreation.ValidateOnly:
                    throw new NotV1SupportedException("Xml validation not available on Task Scheduler 1.0.");
                case TaskCreation.Create:
                case TaskCreation.Update:
                case TaskCreation.CreateOrUpdate:
                case TaskCreation.Disable:
                    if (createType == TaskCreation.Disable)
                        definition.Settings.Enabled = false;
                    definition.V1Save(Path);
                    break;
                case TaskCreation.DontAddPrincipalAce:
                    throw new NotV1SupportedException("Security settings are not available on Task Scheduler 1.0.");
                case TaskCreation.IgnoreRegistrationTriggers:
                    throw new NotV1SupportedException("Registration triggers are not available on Task Scheduler 1.0.");
            }
            return new Task(TaskService, definition._v1Task);
        }

        public void SetSecurityDescriptor(GenericSecurityDescriptor sd, AccessControlSections includeSections) { SetSecurityDescriptorSddlForm(sd.GetSddlForm(includeSections), includeSections); }
        public void SetSecurityDescriptorSddlForm(string sddlForm, AccessControlSections includeSections)
        {
            if (_v2Folder == null)
                throw new NotV1SupportedException();
            _v2Folder.SetSecurityDescriptor(sddlForm, (int)includeSections);
        }

        public override string ToString() { return Path; }

        public string Name
        {
            get
            {
                if (_v2Folder != null)
                    return _v2Folder.Name;
                return @"\";
            }
        }

        public string Path
        {
            get
            {
                if (_v2Folder != null)
                    return _v2Folder.Path;
                return @"\";
            }
        }

        public GenericSecurityDescriptor SecurityDescriptor
        {
            get { return GetSecurityDescriptor(AccessControlSections.All); }
            set { SetSecurityDescriptor(value, AccessControlSections.All); }
        }

        public TaskFolderCollection SubFolders
        {
            get
            {
                if (_v2Folder != null)
                    return new TaskFolderCollection(this, _v2Folder.GetFolders(0));
                return new TaskFolderCollection();
            }
        }

        public TaskCollection Tasks
        {
            get { return GetTasks(null); }
        }

        public TaskService TaskService { get; private set; }
    }
}

