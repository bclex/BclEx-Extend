#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.IO;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class ExecAction : Action
    {
        private ITask _v1Task;

        public ExecAction() { }
        internal ExecAction(ITask task)
        {
            _v1Task = task;
        }
        internal ExecAction(IExecAction action)
        {
            base._iAction = action;
        }
        public ExecAction(string path, string arguments = null, string workingDirectory = null)
        {
            Path = path;
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
        }

        internal override void Bind(ITask v1Task)
        {
            object obj2 = null;
            base._unboundValues.TryGetValue("Path", out obj2);
            v1Task.SetApplicationName(obj2 == null ? string.Empty : obj2.ToString());
            obj2 = null;
            base._unboundValues.TryGetValue("Arguments", out obj2);
            v1Task.SetParameters(obj2 == null ? string.Empty : obj2.ToString());
            obj2 = null;
            base._unboundValues.TryGetValue("WorkingDirectory", out obj2);
            v1Task.SetWorkingDirectory(obj2 == null ? string.Empty : obj2.ToString());
        }

        protected override void CopyProperties(Action sourceAction)
        {
            if (sourceAction.GetType() == base.GetType())
            {
                base.CopyProperties(sourceAction);
                Path = ((ExecAction)sourceAction).Path;
                Arguments = ((ExecAction)sourceAction).Arguments;
                WorkingDirectory = ((ExecAction)sourceAction).WorkingDirectory;
            }
        }

        public override string ToString()
        {
            return string.Format(Local.ExecAction, new object[] { Path, Arguments, WorkingDirectory, Id });
        }

        public string Arguments
        {
            get
            {
                if (_v1Task != null)
                    return (string)_v1Task.GetParameters();
                if (base._iAction != null)
                    return ((IExecAction)base._iAction).Arguments;
                if (!base._unboundValues.ContainsKey("Arguments"))
                    return null;
                return (string)base._unboundValues["Arguments"];
            }
            set
            {
                if (_v1Task != null)
                    _v1Task.SetParameters(value);
                else if (base._iAction != null)
                    ((IExecAction)base._iAction).Arguments = value;
                else
                    base._unboundValues["Arguments"] = value;
            }
        }

        internal override bool Bound
        {
            get { return (_v1Task != null || base.Bound); }
        }

        public override string Id
        {
            get
            {
                if (_v1Task != null)
                    return (System.IO.Path.GetFileNameWithoutExtension(Task.GetV1Path(_v1Task)) + "_Action");
                return base.Id;
            }
            set
            {
                if (_v1Task != null)
                    throw new NotV1SupportedException();
                base.Id = value;
            }
        }

        public string Path
        {
            get
            {
                if (_v1Task != null)
                    return (string)_v1Task.GetApplicationName();
                if (base._iAction != null)
                    return ((IExecAction)base._iAction).Path;
                if (!base._unboundValues.ContainsKey("Path"))
                    return null;
                return (string)base._unboundValues["Path"];
            }
            set
            {
                if (_v1Task != null)
                    _v1Task.SetApplicationName(value);
                else if (base._iAction != null)
                    ((IExecAction)base._iAction).Path = value;
                else
                    base._unboundValues["Path"] = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                if (_v1Task != null)
                    return (string)_v1Task.GetWorkingDirectory();
                if (base._iAction != null)
                    return ((IExecAction)base._iAction).WorkingDirectory;
                if (!base._unboundValues.ContainsKey("WorkingDirectory"))
                    return null;
                return (string)base._unboundValues["WorkingDirectory"];
            }
            set
            {
                if (_v1Task != null)
                    _v1Task.SetWorkingDirectory(value);
                else if (base._iAction != null)
                    ((IExecAction)base._iAction).WorkingDirectory = value;
                else
                    base._unboundValues["WorkingDirectory"] = value;
            }
        }
    }
}

