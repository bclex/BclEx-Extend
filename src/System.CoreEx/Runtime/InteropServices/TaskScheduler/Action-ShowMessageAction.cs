#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class ShowMessageAction : Action
    {
        public ShowMessageAction() { }

        internal ShowMessageAction(IShowMessageAction action)
        {
            base._iAction = action;
        }

        public ShowMessageAction(string messageBody, string title)
        {
            MessageBody = messageBody;
            Title = title;
        }

        protected override void CopyProperties(Action sourceAction)
        {
            if (sourceAction.GetType() == base.GetType())
            {
                base.CopyProperties(sourceAction);
                Title = ((ShowMessageAction)sourceAction).Title;
                MessageBody = ((ShowMessageAction)sourceAction).MessageBody;
            }
        }

        public override string ToString() { return string.Format(Local.ShowMessageAction, Title, MessageBody, Id); }

        public string MessageBody
        {
            get
            {
                if (base._iAction != null)
                    return ((IShowMessageAction)base._iAction).MessageBody;
                if (!base._unboundValues.ContainsKey("MessageBody"))
                    return null;
                return (string)base._unboundValues["MessageBody"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["MessageBody"] = value;
                else
                    ((IShowMessageAction)base._iAction).MessageBody = value;
            }
        }

        public string Title
        {
            get
            {
                if (base._iAction != null)
                    return ((IShowMessageAction)base._iAction).Title;
                if (!base._unboundValues.ContainsKey("Title"))
                    return null;
                return (string)base._unboundValues["Title"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Title"] = value;
                else
                    ((IShowMessageAction)base._iAction).Title = value;
            }
        }
    }
}

