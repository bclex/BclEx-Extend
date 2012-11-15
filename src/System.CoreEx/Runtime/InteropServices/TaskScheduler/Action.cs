#region Foreign-License
// x
#endregion
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public abstract class Action : IDisposable, ICloneable
    {
        internal IAction _iAction;
        protected Dictionary<string, object> _unboundValues = new Dictionary<string, object>();

        protected Action() { }

        internal virtual void Bind(ITask iTask) { }
        internal virtual void Bind(ITaskDefinition iTaskDef)
        {
            var o = iTaskDef.Actions;
            var name = base.GetType().Name;
            if (name != null)
            {
                if (!(name == "ComHandlerAction"))
                {
                    if (name == "ExecAction")
                    {
                        _iAction = o.Create(TaskActionType.Execute);
                        goto Label_008E;
                    }
                    if (name == "EmailAction")
                    {
                        _iAction = o.Create(TaskActionType.SendEmail);
                        goto Label_008E;
                    }
                    if (name == "ShowMessageAction")
                    {
                        _iAction = o.Create(TaskActionType.ShowMessage);
                        goto Label_008E;
                    }
                }
                else
                {
                    _iAction = o.Create(TaskActionType.ComHandler);
                    goto Label_008E;
                }
            }
            throw new ArgumentException();
        Label_008E:
            Marshal.ReleaseComObject(o);
            foreach (string str in _unboundValues.Keys)
            {
                try { _iAction.GetType().InvokeMember(str, BindingFlags.SetProperty, null, _iAction, new object[] { _unboundValues[str] }); }
                catch (TargetInvocationException exception) { throw exception.InnerException; }
                catch { }
            }
            _unboundValues.Clear();
        }

        public object Clone()
        {
            var action = CreateAction(ActionType);
            action.CopyProperties(this);
            return action;
        }

        protected virtual void CopyProperties(Action sourceAction)
        {
            Id = sourceAction.Id;
        }

        public static Action CreateAction(TaskActionType actionType)
        {
            switch (actionType)
            {
                case TaskActionType.ComHandler: return new ComHandlerAction();
                case TaskActionType.SendEmail: return new EmailAction();
                case TaskActionType.ShowMessage: return new ShowMessageAction();
            }
            return new ExecAction();
        }

        internal static Action CreateAction(IAction iAction)
        {
            switch (iAction.Type)
            {
                case TaskActionType.ComHandler: return new ComHandlerAction((IComHandlerAction)iAction);
                case TaskActionType.SendEmail: return new EmailAction((IEmailAction)iAction);
                case TaskActionType.ShowMessage: return new ShowMessageAction((IShowMessageAction)iAction);
            }
            return new ExecAction((IExecAction)iAction);
        }

        public virtual void Dispose()
        {
            if (_iAction != null)
                Marshal.ReleaseComObject(_iAction);
        }

        public override string ToString() { return Id; }

        public TaskActionType ActionType
        {
            get
            {
                if (_iAction != null) return _iAction.Type;
                if (this is ComHandlerAction) return TaskActionType.ComHandler;
                if (this is ShowMessageAction) return TaskActionType.ShowMessage;
                if (this is EmailAction) return TaskActionType.SendEmail;
                return TaskActionType.Execute;
            }
        }

        internal virtual bool Bound
        {
            get { return (_iAction != null); }
        }

        public virtual string Id
        {
            get
            {
                if (_iAction != null) return _iAction.Id;
                if (!_unboundValues.ContainsKey("Id")) return null;
                return (string)_unboundValues["Id"];
            }
            set
            {
                if (_iAction == null) _unboundValues["Id"] = value;
                else _iAction.Id = value;
            }
        }
    }
}

