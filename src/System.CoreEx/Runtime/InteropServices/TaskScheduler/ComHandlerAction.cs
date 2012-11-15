#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class ComHandlerAction : Action
    {
        public ComHandlerAction() { }

        internal ComHandlerAction(IComHandlerAction action)
        {
            base._iAction = action;
        }
        public ComHandlerAction(Guid classId, string data)
        {
            ClassId = classId;
            Data = data;
        }

        protected override void CopyProperties(Action sourceAction)
        {
            if (sourceAction.GetType() == base.GetType())
            {
                base.CopyProperties(sourceAction);
                ClassId = ((ComHandlerAction)sourceAction).ClassId;
                Data = ((ComHandlerAction)sourceAction).Data;
            }
        }

        public override string ToString()
        {
            return string.Format(Local.ComHandlerAction, ClassId, Data, Id);
        }

        public Guid ClassId
        {
            get
            {
                if (base._iAction != null)
                    return new Guid(((IComHandlerAction)base._iAction).ClassId);
                if (!base._unboundValues.ContainsKey("ClassId"))
                    return Guid.Empty;
                return (Guid)base._unboundValues["ClassId"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["ClassId"] = value.ToString();
                else
                    ((IComHandlerAction)base._iAction).ClassId = value.ToString();
            }
        }

        public string Data
        {
            get
            {
                if (base._iAction != null)
                    return ((IComHandlerAction)base._iAction).Data;
                if (!base._unboundValues.ContainsKey("Data"))
                    return null;
                return (string)base._unboundValues["Data"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Data"] = value;
                else
                    ((IComHandlerAction)base._iAction).Data = value;
            }
        }
    }
}

