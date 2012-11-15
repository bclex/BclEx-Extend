#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.AccessControl;
using System.Xml;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public class Task : IDisposable
    {
        private TaskDefinition _myTD;
        internal ITask _v1Task;
        private static readonly DateTime _v2InvalidDate = new DateTime(0x76b, 12, 30);
        private IRegisteredTask _v2Task;

        internal Task(TaskService svc, ITask iTask)
        {
            TaskService = svc;
            _v1Task = iTask;
        }
        internal Task(TaskService svc, IRegisteredTask iTask)
        {
            TaskService = svc;
            _v2Task = iTask;
        }

        public void Dispose()
        {
            if (_v2Task != null)
                Marshal.ReleaseComObject(_v2Task);
            _v1Task = null;
        }

        public DateTime[] GetRunTimes(DateTime start, DateTime end, uint count = 0)
        {
            var pstStart = new SystemTime(start);
            var pstEnd = new SystemTime(end);
            var zero = IntPtr.Zero;
            if (_v2Task != null)
                _v2Task.GetRunTimes(ref pstStart, ref pstEnd, ref count, ref zero);
            else
            {
                var num = (count > 0 && count <= 0x5a0 ? (ushort)count : (ushort)0x5a0);
                _v1Task.GetRunTimes(ref pstStart, ref pstEnd, ref num, ref zero);
                count = num;
            }
            var timeArray = new DateTime[count];
            for (var i = 0; i < count; i++)
            {
                var ptr = new IntPtr(zero.ToInt64() + (i * Marshal.SizeOf(typeof(SystemTime))));
                timeArray[i] = (DateTime)((SystemTime)Marshal.PtrToStructure(ptr, typeof(SystemTime)));
            }
            Marshal.FreeCoTaskMem(zero);
            return timeArray;
        }

        public string GetSecurityDescriptorSddlForm(AccessControlSections includeSections)
        {
            if (_v2Task == null)
                throw new NotV1SupportedException();
            return _v2Task.GetSecurityDescriptor((int)includeSections);
        }

        internal static string GetV1Path(ITask v1Task)
        {
            var ppszFileName = string.Empty;
            try { ((IPersistFile)v1Task).GetCurFile(out ppszFileName); }
            catch (Exception ex) { throw ex; }
            return ppszFileName;
        }

        public void RegisterChanges()
        {
            if (Definition.Principal.LogonType == TaskLogonType.InteractiveTokenOrPassword || Definition.Principal.LogonType == TaskLogonType.Password)
                throw new SecurityException("Tasks which have been registered previously with stored passwords must use the TaskFolder.RegisterTaskDefinition method for updates.");
            TaskService.GetFolder(System.IO.Path.GetDirectoryName(Path)).RegisterTaskDefinition(Name, Definition);
        }

        public RunningTask Run(params string[] parameters)
        {
            if (_v2Task != null)
            {
                if (parameters != null && parameters.Length > 0x20)
                    throw new ArgumentOutOfRangeException("parameters", "A maximum of 32 values is allowed.");
                return new RunningTask(TaskService, _v2Task, _v2Task.Run(parameters));
            }
            _v1Task.Run();
            return new RunningTask(TaskService, _v1Task);
        }

        public RunningTask RunEx(TaskRunFlags flags, int sessionID, string user, params string[] parameters)
        {
            if (_v2Task == null)
                throw new NotV1SupportedException();
            return new RunningTask(TaskService, _v2Task, _v2Task.RunEx(parameters, (int)flags, sessionID, user));
        }

        public void SetSecurityDescriptorSddlForm(string sddlForm, AccessControlSections includeSections)
        {
            if (_v2Task != null)
                _v2Task.SetSecurityDescriptor(sddlForm, (int)includeSections);
            throw new NotV1SupportedException();
        }

        public bool ShowEditor()
        {
            try
            {
                var assembly = Assembly.LoadFrom("Microsoft.Win32.TaskSchedulerEditor.dll");
                if (assembly != null)
                {
                    var type = assembly.GetType("Microsoft.Win32.TaskScheduler.TaskEditDialog", false, false);
                    if (type != null)
                    {
                        var obj2 = Activator.CreateInstance(type, new object[] { this, true, true });
                        if (obj2 != null)
                        {
                            var method = type.GetMethod("ShowDialog", Type.EmptyTypes);
                            if (method != null)
                                return (Convert.ToInt32(method.Invoke(obj2, null)) == 1);
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public void ShowPropertyPage()
        {
            if (_v1Task == null)
                throw new NotV2SupportedException();
            _v1Task.EditWorkItem(IntPtr.Zero, 0);
        }

        public void Stop()
        {
            if (_v2Task != null)
                _v2Task.Stop(0);
            else
                _v1Task.Terminate();
        }

        internal static TimeSpan StringToTimeSpan(string input)
        {
            if (!string.IsNullOrEmpty(input))
                try { return XmlConvert.ToTimeSpan(input); }
                catch { }
            return TimeSpan.Zero;
        }

        internal static string TimeSpanToString(TimeSpan span)
        {
            if (span != TimeSpan.Zero)
                try { return XmlConvert.ToString(span); }
                catch { }
            return null;
        }

        public override string ToString() { return Name; }

        public TaskDefinition Definition
        {
            get
            {
                if (_myTD == null)
                    if (_v2Task != null)
                        _myTD = new TaskDefinition(_v2Task.Definition);
                    else
                        _myTD = new TaskDefinition(_v1Task, Name);
                return _myTD;
            }
        }

        public bool Enabled
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.Enabled;
                return ((_v1Task.GetFlags() & TaskFlags.Disabled) != TaskFlags.Disabled);
            }
            set
            {
                if (_v2Task != null)
                    _v2Task.Enabled = value;
                else
                {
                    var flags = _v1Task.GetFlags();
                    if (!value)
                        _v1Task.SetFlags(flags |= TaskFlags.Disabled);
                    else
                        _v1Task.SetFlags(flags &= ~TaskFlags.Disabled);
                }
            }
        }

        public DateTime LastRunTime
        {
            get
            {
                if (_v2Task == null)
                    return (DateTime)_v1Task.GetMostRecentRunTime();
                var lastRunTime = _v2Task.LastRunTime;
                if (!(lastRunTime == _v2InvalidDate))
                    return lastRunTime;
                return DateTime.MinValue;
            }
        }

        public int LastTaskResult
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.LastTaskResult;
                return (int)_v1Task.GetExitCode();
            }
        }

        public string Name
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.Name;
                return System.IO.Path.GetFileNameWithoutExtension(GetV1Path(_v1Task));
            }
        }

        public DateTime NextRunTime
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.NextRunTime;
                return (DateTime)_v1Task.GetNextRunTime();
            }
        }

        public int NumberOfMissedRuns
        {
            get
            {
                if (_v2Task == null)
                    throw new NotV1SupportedException();
                return _v2Task.NumberOfMissedRuns;
            }
        }

        public string Path
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.Path;
                return GetV1Path(_v1Task);
            }
        }

        public GenericSecurityDescriptor SecurityDescriptor
        {
            get { return new RawSecurityDescriptor(GetSecurityDescriptorSddlForm(AccessControlSections.All)); }
            set { SetSecurityDescriptorSddlForm(value.GetSddlForm(AccessControlSections.All), AccessControlSections.All); }
        }

        public TaskState State
        {
            get
            {
                if (_v2Task != null)
                    return _v2Task.State;
                switch (_v1Task.GetStatus())
                {
                    case TaskStatus.Ready:
                    case TaskStatus.NeverRun:
                    case TaskStatus.NoMoreRuns:
                    case TaskStatus.Terminated:
                        return TaskState.Ready;
                    case TaskStatus.Running:
                        return TaskState.Running;
                    case TaskStatus.Disabled:
                        return TaskState.Disabled;
                }
                return TaskState.Unknown;
            }
        }

        public TaskService TaskService { get; private set; }

        public string Xml
        {
            get
            {
                if (_v2Task == null)
                    throw new NotV1SupportedException();
                return _v2Task.Xml;
            }
        }
    }
}

