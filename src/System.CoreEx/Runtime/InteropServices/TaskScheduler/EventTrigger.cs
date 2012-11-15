#region Foreign-License
// x
#endregion
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class EventTrigger : Trigger, ITriggerDelay
    {
        private NamedValueCollection _nvc;

        public EventTrigger()
            : base(TaskTriggerType.Event) { }
        internal EventTrigger(ITrigger iTrigger)
            : base(iTrigger) { }
        public EventTrigger(string log, string source, int? eventId)
            : this()
        {
            SetBasic(log, source, eventId);
        }

        internal override void Bind(ITaskDefinition iTaskDef)
        {
            base.Bind(iTaskDef);
            if (_nvc != null)
                _nvc.Bind(((IEventTrigger)base._v2Trigger).ValueQueries);
        }

        public override void CopyProperties(Trigger sourceTrigger)
        {
            base.CopyProperties(sourceTrigger);
            if (sourceTrigger.GetType() == base.GetType())
                Subscription = ((EventTrigger)sourceTrigger).Subscription;
            ((EventTrigger)sourceTrigger).ValueQueries.CopyTo(this.ValueQueries);
        }

        public bool GetBasic(out string log, out string source, out int? eventId)
        {
            log = (string)(source = null);
            eventId = 0;
            if (!string.IsNullOrEmpty(this.Subscription))
                using (var s = new MemoryStream(Encoding.UTF8.GetBytes(this.Subscription)))
                using (var r = new XmlTextReader(s))
                {
                    r.MoveToContent();
                    r.ReadStartElement("QueryList");
                    if (r.Name == "Query" && r.MoveToAttribute("Path"))
                    {
                        var str = r.Value;
                        if (r.MoveToElement() && r.ReadToDescendant("Select") && str.Equals(r["Path"], StringComparison.InvariantCultureIgnoreCase))
                        {
                            var match = Regex.Match(r.ReadString(), @"\*(?:\[System\[(?:Provider\[\@Name='(?<s>[^']+)'\])?(?:\s+and\s+)?(?:EventID=(?<e>\d+))?\]\])", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                log = str;
                                if (match.Groups["s"].Success)
                                    source = match.Groups["s"].Value;
                                if (match.Groups["e"].Success)
                                    eventId = new int?(Convert.ToInt32(match.Groups["e"].Value));
                                return true;
                            }
                        }
                    }
                }
            return false;
        }

        public void SetBasic(string log, string source, int? eventId)
        {
            var b = new StringBuilder();
            if (string.IsNullOrEmpty(log))
                throw new ArgumentNullException("log");
            b.AppendFormat("<QueryList><Query Id=\"0\" Path=\"{0}\"><Select Path=\"{0}\">*", log);
            var flag = !string.IsNullOrEmpty(source);
            var hasValue = eventId.HasValue;
            if (flag || hasValue)
            {
                b.Append("[System[");
                if (flag)
                    b.AppendFormat("Provider[@Name='{0}']", source);
                if (flag && hasValue)
                    b.Append(" and ");
                if (hasValue)
                    b.AppendFormat("EventID={0}", eventId.Value);
                b.Append("]]");
            }
            b.Append("</Select></Query></QueryList>");
            ValueQueries.Clear();
            Subscription = b.ToString();
        }

        protected override string V2GetTriggerString()
        {
            string str;
            string str2;
            int? nullable;
            if (!this.GetBasic(out str, out str2, out nullable))
                return Local.TriggerEvent1;
            var b = new StringBuilder();
            b.AppendFormat(Local.TriggerEventBasic1, str);
            if (!string.IsNullOrEmpty(str2))
                b.AppendFormat(Local.TriggerEventBasic2, str2);
            if (nullable.HasValue)
                b.AppendFormat(Local.TriggerEventBasic3, nullable.Value);
            return b.ToString();
        }

        [DefaultValue(0)]
        public TimeSpan Delay
        {
            get
            {
                if (base._v2Trigger != null)
                    return Task.StringToTimeSpan(((IEventTrigger)base._v2Trigger).Delay);
                if (!base.unboundValues.ContainsKey("Delay"))
                    return TimeSpan.Zero;
                return (TimeSpan)base.unboundValues["Delay"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IEventTrigger)base._v2Trigger).Delay = Task.TimeSpanToString(value);
                else
                    base.unboundValues["Delay"] = value;
            }
        }

        [DefaultValue((string)null)]
        public string Subscription
        {
            get
            {
                if (base._v2Trigger != null)
                    return ((IEventTrigger)base._v2Trigger).Subscription;
                if (!base.unboundValues.ContainsKey("Subscription"))
                    return null;
                return (string)base.unboundValues["Subscription"];
            }
            set
            {
                if (base._v2Trigger != null)
                    ((IEventTrigger)base._v2Trigger).Subscription = value;
                else
                    base.unboundValues["Subscription"] = value;
            }
        }

        public NamedValueCollection ValueQueries
        {
            get
            {
                if (this._nvc == null)
                {
                    if (base._v2Trigger == null)
                        _nvc = new NamedValueCollection();
                    else
                        _nvc = new NamedValueCollection(((IEventTrigger)base._v2Trigger).ValueQueries);
                }
                return _nvc;
            }
        }
    }
}

