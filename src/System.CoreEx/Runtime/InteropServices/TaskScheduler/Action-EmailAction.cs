#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class EmailAction : Action
    {
        private NamedValueCollection _nvc;

        public EmailAction() { }
        internal EmailAction(IEmailAction action)
        {
            base._iAction = action;
        }
        public EmailAction(string subject, string from, string to, string body, string mailServer)
        {
            Subject = subject;
            From = from;
            To = to;
            Body = body;
            Server = mailServer;
        }

        internal override void Bind(ITaskDefinition iTaskDef)
        {
            base.Bind(iTaskDef);
            if (_nvc != null)
                _nvc.Bind(((IEmailAction)base._iAction).HeaderFields);
        }

        protected override void CopyProperties(Action sourceAction)
        {
            if (sourceAction.GetType() == base.GetType())
            {
                base.CopyProperties(sourceAction);
                if (((EmailAction)sourceAction).Attachments != null)
                    Attachments = (object[])((EmailAction)sourceAction).Attachments.Clone();
                Bcc = ((EmailAction)sourceAction).Bcc;
                Body = ((EmailAction)sourceAction).Body;
                Cc = ((EmailAction)sourceAction).Cc;
                From = ((EmailAction)sourceAction).From;
                if (((EmailAction)sourceAction)._nvc != null)
                    ((EmailAction)sourceAction).HeaderFields.CopyTo(HeaderFields);
                ReplyTo = ((EmailAction)sourceAction).ReplyTo;
                Server = ((EmailAction)sourceAction).Server;
                Subject = ((EmailAction)sourceAction).Subject;
                To = ((EmailAction)sourceAction).To;
            }
        }

        public override string ToString()
        {
            return string.Format(Local.EmailAction, new object[] { Subject, To, Cc, Bcc, From, ReplyTo, Body, Server, Id });
        }

        public object[] Attachments
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Attachments;
                if (!base._unboundValues.ContainsKey("Attachments"))
                    return null;
                return (object[])base._unboundValues["Attachments"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Attachments"] = value;
                else
                    ((IEmailAction)base._iAction).Attachments = value;
            }
        }

        public string Bcc
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Bcc;
                if (!base._unboundValues.ContainsKey("Bcc"))
                    return null;
                return (string)base._unboundValues["Bcc"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Bcc"] = value;
                else
                    ((IEmailAction)base._iAction).Bcc = value;
            }
        }

        public string Body
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Body;
                if (!base._unboundValues.ContainsKey("Body"))
                    return null;
                return (string)base._unboundValues["Body"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Body"] = value;
                else
                    ((IEmailAction)base._iAction).Body = value;
            }
        }

        public string Cc
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Cc;
                if (!base._unboundValues.ContainsKey("Cc"))
                    return null;
                return (string)base._unboundValues["Cc"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Cc"] = value;
                else
                    ((IEmailAction)base._iAction).Cc = value;
            }
        }

        public string From
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).From;
                if (!base._unboundValues.ContainsKey("From"))
                    return null;
                return (string)base._unboundValues["From"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["From"] = value;
                else
                    ((IEmailAction)base._iAction).From = value;
            }
        }

        public NamedValueCollection HeaderFields
        {
            get
            {
                if (_nvc == null)
                    if (base._iAction != null)
                        _nvc = new NamedValueCollection(((IEmailAction)base._iAction).HeaderFields);
                    else
                        _nvc = new NamedValueCollection();
                return _nvc;
            }
        }

        public string ReplyTo
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).ReplyTo;
                if (!base._unboundValues.ContainsKey("ReplyTo"))
                    return null;
                return (string)base._unboundValues["ReplyTo"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["ReplyTo"] = value;
                else
                    ((IEmailAction)base._iAction).ReplyTo = value;
            }
        }

        public string Server
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Server;
                if (!base._unboundValues.ContainsKey("Server"))
                    return null;
                return (string)base._unboundValues["Server"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Server"] = value;
                else
                    ((IEmailAction)base._iAction).Server = value;
            }
        }

        public string Subject
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).Subject;
                if (!base._unboundValues.ContainsKey("Subject"))
                    return null;
                return (string)base._unboundValues["Subject"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["Subject"] = value;
                else
                    ((IEmailAction)base._iAction).Subject = value;
            }
        }

        public string To
        {
            get
            {
                if (base._iAction != null)
                    return ((IEmailAction)base._iAction).To;
                if (!base._unboundValues.ContainsKey("To"))
                    return null;
                return (string)base._unboundValues["To"];
            }
            set
            {
                if (base._iAction == null)
                    base._unboundValues["To"] = value;
                else
                    ((IEmailAction)base._iAction).To = value;
            }
        }
    }
}

