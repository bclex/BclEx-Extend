#region Foreign-License
// x
#endregion
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskDefinition
    {
        private ActionCollection _actions;
        private TaskPrincipal _principal;
        private TaskRegistrationInfo _regInfo;
        private TaskSettings _settings;
        private TriggerCollection _triggers;
        internal string _v1Name;
        internal ITask _v1Task;
        internal ITaskDefinition _v2Def;

        internal TaskDefinition(ITaskDefinition iDef)
        {
            _v1Name = string.Empty;
            _v2Def = iDef;
        }

        internal TaskDefinition(ITask iTask, string name)
        {
            _v1Name = string.Empty;
            _v1Task = iTask;
            _v1Name = name;
        }

        public void Dispose()
        {
            _regInfo = null;
            _triggers = null;
            _settings = null;
            _principal = null;
            _actions = null;
            if (_v2Def != null)
                Marshal.ReleaseComObject(_v2Def);
            _v1Task = null;
        }

        internal void V1Save(string newName)
        {
            if (_v1Task != null)
            {
                Triggers.Bind();
                var file = (IPersistFile)_v1Task;
                if (string.IsNullOrEmpty(newName) || newName == _v1Name)
                    try { file.Save(null, false); file = null; }
                    catch { }
                else
                {
                    string str;
                    file.GetCurFile(out str);
                    File.Delete(str);
                    str = string.Concat(new object[] { Path.GetDirectoryName(str), Path.DirectorySeparatorChar, newName, Path.GetExtension(str) });
                    File.Delete(str);
                    file.Save(str, true);
                    file = null;
                }
            }
        }

        public ActionCollection Actions
        {
            get
            {
                if (_actions == null)
                    if (_v2Def != null)
                        _actions = new ActionCollection(_v2Def);
                    else
                        _actions = new ActionCollection(_v1Task);
                return _actions;
            }
        }

        public string Data
        {
            get
            {
                if (_v2Def != null)
                    return _v2Def.Data;
                return TaskRegistrationInfo.GetTaskData(_v1Task).ToString();
            }
            set
            {
                if (_v2Def != null)
                    _v2Def.Data = value;
                else
                    TaskRegistrationInfo.SetTaskData(_v1Task, value);
            }
        }

        public TaskPrincipal Principal
        {
            get
            {
                if (_principal == null)
                    if (_v2Def != null)
                        _principal = new TaskPrincipal(_v2Def.Principal);
                    else
                        _principal = new TaskPrincipal(_v1Task);
                return _principal;
            }
        }

        public TaskRegistrationInfo RegistrationInfo
        {
            get
            {
                if (_regInfo == null)
                    if (_v2Def != null)
                        _regInfo = new TaskRegistrationInfo(_v2Def.RegistrationInfo);
                    else
                        _regInfo = new TaskRegistrationInfo(_v1Task);
                return _regInfo;
            }
        }

        public TaskSettings Settings
        {
            get
            {
                if (_settings == null)
                    if (_v2Def != null)
                        _settings = new TaskSettings(_v2Def.Settings);
                    else
                        _settings = new TaskSettings(_v1Task);
                return _settings;
            }
        }

        public TriggerCollection Triggers
        {
            get
            {
                if (_triggers == null)
                    if (_v2Def != null)
                        _triggers = new TriggerCollection(_v2Def);
                    else
                        _triggers = new TriggerCollection(_v1Task);
                return _triggers;
            }
        }

        public string XmlText
        {
            get
            {
                if (_v2Def == null)
                    throw new NotV1SupportedException();
                return _v2Def.XmlText;
            }
            set
            {
                if (_v2Def == null)
                    throw new NotV1SupportedException();
                _v2Def.XmlText = value;
            }
        }
    }
}

