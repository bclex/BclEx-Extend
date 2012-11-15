#region Foreign-License
// x
#endregion
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Text;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public abstract class Trigger : IDisposable, ICloneable
    {
        private static bool _foundTimeSpan2;
        private RepetitionPattern _repititionPattern;
        private static Type _timeSpan2Type;
        internal TaskTriggerType _ttype;
        protected Dictionary<string, object> unboundValues;
        internal ITaskTrigger _v1Trigger;
        internal TaskTrigger _v1TriggerData;
        internal const string _V2BoundaryDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK";
        internal ITrigger _v2Trigger;

        internal Trigger(TaskTriggerType triggerType)
        {
            _ttype = triggerType;
            unboundValues = new Dictionary<string, object>();
            _v1TriggerData.TriggerSize = (ushort)Marshal.SizeOf(typeof(TaskTrigger));
            try { _v1TriggerData.Type = ConvertToV1TriggerType(_ttype); }
            catch { }
            StartBoundary = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
        }
        internal Trigger(ITrigger iTrigger)
        {
            _v2Trigger = iTrigger;
            _ttype = iTrigger.Type;
            if (string.IsNullOrEmpty(_v2Trigger.StartBoundary))
            {
                StartBoundary = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            }
        }
        internal Trigger(ITaskTrigger trigger, TaskTrigger data)
        {
            _v1Trigger = trigger;
            _v1TriggerData = data;
            _ttype = ConvertFromV1TriggerType(data.Type);
        }
        internal Trigger(ITaskTrigger trigger, V1Interop.TaskTriggerType type)
            : this(trigger, trigger.GetTrigger()) { }

        internal virtual void Bind(ITask iTask)
        {
            if (_v1Trigger == null)
            {
                ushort num;
                _v1Trigger = iTask.CreateTrigger(out num);
            }
            SetV1TriggerData();
        }

        internal virtual void Bind(ITaskDefinition iTaskDef)
        {
            var o = iTaskDef.Triggers;
            _v2Trigger = o.Create(_ttype);
            Marshal.ReleaseComObject(o);
            foreach (string str in unboundValues.Keys)
            {
                try
                {
                    var obj2 = unboundValues[str];
                    CheckBindValue(str, ref obj2);
                    _v2Trigger.GetType().InvokeMember(str, BindingFlags.SetProperty, null, _v2Trigger, new object[] { obj2 });
                }
                catch (TargetInvocationException ex) { throw ex.InnerException; }
                catch { }
            }
            unboundValues.Clear();
            unboundValues = null;
            _repititionPattern = new RepetitionPattern(this);
            _repititionPattern.Bind();
        }

        internal static string BuildEnumString(string preface, object enumValue)
        {
            var strArray = enumValue.ToString().Split(new string[] { ", " }, StringSplitOptions.None);
            if (strArray.Length == 0)
                return string.Empty;
            for (var i = 0; i < strArray.Length; i++)
                strArray[i] = Local.ResourceManager.GetString(preface + strArray[i]);
            return string.Join(", ", strArray);
        }

        internal virtual void CheckBindValue(string key, ref object o)
        {
            if (o is TimeSpan)
                o = Task.TimeSpanToString((TimeSpan)o);
            if (o is DateTime)
            {
                if ((key == "EndBoundary" && ((DateTime)o) == DateTime.MaxValue) || (key == "StartBoundary" && ((DateTime)o) == DateTime.MinValue))
                    o = null;
                else
                    o = ((DateTime)o).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK");
            }
        }

        public object Clone()
        {
            var trigger = CreateTrigger(TriggerType);
            trigger.CopyProperties(this);
            return trigger;
        }

        internal static TaskTriggerType ConvertFromV1TriggerType(V1Interop.TaskTriggerType v1Type)
        {
            var num = ((int)v1Type) + 1;
            if (num > 6)
                num++;
            return (TaskTriggerType)num;
        }

        internal static V1Interop.TaskTriggerType ConvertToV1TriggerType(TaskTriggerType type)
        {
            if (type == TaskTriggerType.Registration || type == TaskTriggerType.Event || type == TaskTriggerType.SessionStateChange)
                throw new NotV1SupportedException();
            var num = ((int)type) - 1;
            if (num >= 7)
                num--;
            return (V1Interop.TaskTriggerType)num;
        }

        public virtual void CopyProperties(Trigger sourceTrigger)
        {
            Enabled = sourceTrigger.Enabled;
            EndBoundary = sourceTrigger.EndBoundary;
            try { ExecutionTimeLimit = sourceTrigger.ExecutionTimeLimit; }
            catch { }
            Repetition.Duration = sourceTrigger.Repetition.Duration;
            Repetition.Interval = sourceTrigger.Repetition.Interval;
            Repetition.StopAtDurationEnd = sourceTrigger.Repetition.StopAtDurationEnd;
            StartBoundary = sourceTrigger.StartBoundary;
            if (sourceTrigger is ITriggerDelay && this is ITriggerDelay)
                try { ((ITriggerDelay)this).Delay = ((ITriggerDelay)sourceTrigger).Delay; }
                catch { }
            if (sourceTrigger is ITriggerUserId && this is ITriggerUserId)
                try { ((ITriggerUserId)this).UserId = ((ITriggerUserId)sourceTrigger).UserId; }
                catch { }
        }

        public static Trigger CreateTrigger(TaskTriggerType triggerType)
        {
            switch (triggerType)
            {
                case TaskTriggerType.Event: return new EventTrigger();
                case TaskTriggerType.Time: return new TimeTrigger();
                case TaskTriggerType.Daily: return new DailyTrigger(1);
                case TaskTriggerType.Weekly: return new WeeklyTrigger(DaysOfTheWeek.Sunday, 1);
                case TaskTriggerType.Monthly: return new MonthlyTrigger(1, MonthsOfTheYear.September | MonthsOfTheYear.August | MonthsOfTheYear.July | MonthsOfTheYear.October | MonthsOfTheYear.December | MonthsOfTheYear.November | MonthsOfTheYear.February | MonthsOfTheYear.January | MonthsOfTheYear.March | MonthsOfTheYear.June | MonthsOfTheYear.May | MonthsOfTheYear.April);
                case TaskTriggerType.MonthlyDOW: return new MonthlyDOWTrigger(DaysOfTheWeek.Sunday, MonthsOfTheYear.September | MonthsOfTheYear.August | MonthsOfTheYear.July | MonthsOfTheYear.October | MonthsOfTheYear.December | MonthsOfTheYear.November | MonthsOfTheYear.February | MonthsOfTheYear.January | MonthsOfTheYear.March | MonthsOfTheYear.June | MonthsOfTheYear.May | MonthsOfTheYear.April, WhichWeek.FirstWeek);
                case TaskTriggerType.Idle: return new IdleTrigger();
                case TaskTriggerType.Registration: return new RegistrationTrigger();
                case TaskTriggerType.Boot: return new BootTrigger();
                case TaskTriggerType.Logon: return new LogonTrigger();
                case TaskTriggerType.SessionStateChange: return new SessionStateChangeTrigger();
            }
            return null;
        }

        internal static Trigger CreateTrigger(ITaskTrigger trigger) { return CreateTrigger(trigger, trigger.GetTrigger().Type); }
        internal static Trigger CreateTrigger(ITrigger iTrigger)
        {
            switch (iTrigger.Type)
            {
                case TaskTriggerType.Event: return new EventTrigger((IEventTrigger)iTrigger);
                case TaskTriggerType.Time: return new TimeTrigger((ITimeTrigger)iTrigger);
                case TaskTriggerType.Daily: return new DailyTrigger((IDailyTrigger)iTrigger);
                case TaskTriggerType.Weekly: return new WeeklyTrigger((IWeeklyTrigger)iTrigger);
                case TaskTriggerType.Monthly: return new MonthlyTrigger((IMonthlyTrigger)iTrigger);
                case TaskTriggerType.MonthlyDOW: return new MonthlyDOWTrigger((IMonthlyDOWTrigger)iTrigger);
                case TaskTriggerType.Idle: return new IdleTrigger((IIdleTrigger)iTrigger);
                case TaskTriggerType.Registration: return new RegistrationTrigger((IRegistrationTrigger)iTrigger);
                case TaskTriggerType.Boot: return new BootTrigger((IBootTrigger)iTrigger);
                case TaskTriggerType.Logon: return new LogonTrigger((ILogonTrigger)iTrigger);
                case TaskTriggerType.SessionStateChange: return new SessionStateChangeTrigger((ISessionStateChangeTrigger)iTrigger);
            }
            return null;
        }

        internal static Trigger CreateTrigger(ITaskTrigger trigger, V1Interop.TaskTriggerType triggerType)
        {
            switch (triggerType)
            {
                case V1Interop.TaskTriggerType.RunOnce: return new TimeTrigger(trigger);
                case V1Interop.TaskTriggerType.RunDaily: return new DailyTrigger(trigger);
                case V1Interop.TaskTriggerType.RunWeekly: return new WeeklyTrigger(trigger);
                case V1Interop.TaskTriggerType.RunMonthly: return new MonthlyTrigger(trigger);
                case V1Interop.TaskTriggerType.RunMonthlyDOW: return new MonthlyDOWTrigger(trigger);
                case V1Interop.TaskTriggerType.OnIdle: return new IdleTrigger(trigger);
                case V1Interop.TaskTriggerType.OnSystemStart: return new BootTrigger(trigger);
                case V1Interop.TaskTriggerType.OnLogon: return new LogonTrigger(trigger);
            }
            return null;
        }

        public virtual void Dispose()
        {
            if (_v2Trigger != null)
                Marshal.ReleaseComObject(_v2Trigger);
            if (_v1Trigger != null)
                Marshal.ReleaseComObject(_v1Trigger);
        }

        private static string GetBestTimeSpanString(TimeSpan span)
        {
            if (!_foundTimeSpan2)
            {
                try
                {
                    var assembly = Assembly.LoadFrom("TimeSpan2.dll");
                    if (assembly != null)
                    {
                        _timeSpan2Type = assembly.GetType("System.TimeSpan2", false, false);
                        if (_timeSpan2Type != null)
                            _foundTimeSpan2 = true;
                    }
                }
                catch { }
            }
            if (_foundTimeSpan2 && (_timeSpan2Type != null))
            {
                try
                {
                    var obj2 = Activator.CreateInstance(_timeSpan2Type, new object[] { span });
                    if (obj2 != null)
                    {
                        var method = _timeSpan2Type.GetMethod("ToString", new Type[] { typeof(string) });
                        if (method != null)
                        {
                            var obj3 = method.Invoke(obj2, new object[] { "f" });
                            if (obj3 != null)
                                return obj3.ToString();
                        }
                    }
                }
                catch { }
            }
            return span.ToString();
        }

        internal void SetV1TriggerData()
        {
            if (_v1TriggerData.MinutesInterval != 0 && _v1TriggerData.MinutesInterval >= _v1TriggerData.MinutesDuration)
                throw new ArgumentException("Trigger.Repetition.Interval must be less than Trigger.Repetition.Duration under Task Scheduler 1.0.");
            if (_v1Trigger != null)
                _v1Trigger.SetTrigger(ref _v1TriggerData);
        }

        public override string ToString()
        {
            if (_v1Trigger != null)
                return (string)_v1Trigger.GetTriggerString();
            return (V2GetTriggerString() + V2BaseTriggerString());
        }

        private string V2BaseTriggerString()
        {
            var b = new StringBuilder();
            if (Repetition.Interval != TimeSpan.Zero)
            {
                b.AppendFormat(" {0} {1}", Local.TriggerBase1, GetBestTimeSpanString(Repetition.Interval));
                if (Repetition.Duration == TimeSpan.Zero)
                    b.Append(" " + Local.TriggerBase2);
                else
                    b.AppendFormat(" {0} {1}", Local.TriggerBase3, GetBestTimeSpanString(Repetition.Duration));
                b.Append(".");
            }
            if (EndBoundary != DateTime.MaxValue)
                b.AppendFormat(" {0} {1:G}.", Local.TriggerBase4, EndBoundary);
            if (b.Length > 0)
                b.Insert(0, " -");
            return b.ToString();
        }

        protected virtual string V2GetTriggerString() { return string.Empty; }

        internal virtual bool Bound
        {
            get
            {
                if (_v1Trigger != null)
                    return _v1Trigger.GetTrigger().Equals(_v1TriggerData);
                return (_v2Trigger != null);
            }
        }

        [DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                if (_v2Trigger != null)
                    return _v2Trigger.Enabled;
                return ((_v1TriggerData.Flags & TaskTriggerFlags.Disabled) != TaskTriggerFlags.Disabled);
            }
            set
            {
                if (_v2Trigger != null)
                    _v2Trigger.Enabled = value;
                else
                {
                    if (!value)
                        _v1TriggerData.Flags |= TaskTriggerFlags.Disabled;
                    else
                        _v1TriggerData.Flags &= ~TaskTriggerFlags.Disabled;
                    if (_v1Trigger != null)
                        SetV1TriggerData();
                    else
                        unboundValues["Enabled"] = value;
                }
            }
        }

        [DefaultValue((long)0x2bca2875f4373fffL)]
        public DateTime EndBoundary
        {
            get
            {
                if (_v2Trigger != null)
                {
                    if (!string.IsNullOrEmpty(_v2Trigger.EndBoundary))
                        return DateTime.Parse(_v2Trigger.EndBoundary);
                    return DateTime.MaxValue;
                }
                if (unboundValues != null && unboundValues.ContainsKey("EndBoundary"))
                    return (DateTime)unboundValues["EndBoundary"];
                return _v1TriggerData.EndDate;
            }
            set
            {
                if (_v2Trigger != null)
                    _v2Trigger.EndBoundary = (value == DateTime.MaxValue ? null : value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK"));
                else
                {
                    _v1TriggerData.EndDate = value;
                    if (_v1Trigger != null)
                        SetV1TriggerData();
                    else
                        unboundValues["EndBoundary"] = value;
                }
            }
        }

        [DefaultValue(0)]
        public TimeSpan ExecutionTimeLimit
        {
            get
            {
                if (_v2Trigger != null)
                    return Task.StringToTimeSpan(_v2Trigger.ExecutionTimeLimit);
                if (_v1Trigger != null)
                    throw new NotV1SupportedException();
                if (unboundValues != null && unboundValues.ContainsKey("ExecutionTimeLimit"))
                    return (TimeSpan)unboundValues["ExecutionTimeLimit"];
                return TimeSpan.Zero;
            }
            set
            {
                if (_v2Trigger != null)
                    _v2Trigger.ExecutionTimeLimit = Task.TimeSpanToString(value);
                else
                {
                    if (_v1Trigger != null)
                        throw new NotV1SupportedException();
                    unboundValues["ExecutionTimeLimit"] = value;
                }
            }
        }

        [DefaultValue((string)null)]
        public string Id
        {
            get
            {
                if (_v2Trigger != null)
                    return _v2Trigger.Id;
                if (_v1Trigger != null)
                    throw new NotV1SupportedException();
                if (!unboundValues.ContainsKey("Id"))
                    return null;
                return (string)unboundValues["Id"];
            }
            set
            {
                if (_v2Trigger != null)
                    _v2Trigger.Id = value;
                else
                {
                    if (_v1Trigger != null)
                        throw new NotV1SupportedException();
                    unboundValues["Id"] = value;
                }
            }
        }

        public RepetitionPattern Repetition
        {
            get
            {
                if (_repititionPattern == null)
                    _repititionPattern = new RepetitionPattern(this);
                return _repititionPattern;
            }
        }

        public DateTime StartBoundary
        {
            get
            {
                if (_v2Trigger != null)
                {
                    if (!string.IsNullOrEmpty(_v2Trigger.StartBoundary))
                        return DateTime.Parse(_v2Trigger.StartBoundary);
                    return DateTime.MinValue;
                }
                if (unboundValues != null && unboundValues.ContainsKey("StartBoundary"))
                    return (DateTime)unboundValues["StartBoundary"];
                return _v1TriggerData.BeginDate;
            }
            set
            {
                if (_v2Trigger != null)
                    _v2Trigger.StartBoundary = (value == DateTime.MinValue ? null : value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK"));
                else
                {
                    _v1TriggerData.BeginDate = value;
                    if (_v1Trigger != null)
                        SetV1TriggerData();
                    else
                        unboundValues["StartBoundary"] = value;
                }
            }
        }

        public TaskTriggerType TriggerType
        {
            get { return _ttype; }
        }
    }
}

