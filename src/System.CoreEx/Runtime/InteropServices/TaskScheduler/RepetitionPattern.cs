#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.ComponentModel;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class RepetitionPattern : IDisposable
    {
        private Trigger _pTrigger;
        private IRepetitionPattern _v2Pattern;

        internal RepetitionPattern(Trigger parent)
        {
            _pTrigger = parent;
            if (_pTrigger._v2Trigger != null)
                _v2Pattern = _pTrigger._v2Trigger.Repetition;
        }

        internal void Bind()
        {
            if (_pTrigger._v1Trigger != null)
                _pTrigger._v1Trigger.SetTrigger(ref _pTrigger._v1TriggerData);
            else if (_pTrigger._v2Trigger != null)
            {
                if (_pTrigger._v1TriggerData.MinutesInterval != 0)
                    _v2Pattern.Interval = string.Format("PT{0}M", _pTrigger._v1TriggerData.MinutesInterval);
                if (_pTrigger._v1TriggerData.MinutesDuration != 0)
                    _v2Pattern.Duration = string.Format("PT{0}M", _pTrigger._v1TriggerData.MinutesDuration);
                _v2Pattern.StopAtDurationEnd = (_pTrigger._v1TriggerData.Flags & TaskTriggerFlags.KillAtDurationEnd) == TaskTriggerFlags.KillAtDurationEnd;
            }
        }

        public void Dispose()
        {
            if (_v2Pattern != null)
                Marshal.ReleaseComObject(_v2Pattern);
        }

        [DefaultValue(0)]
        public TimeSpan Duration
        {
            get
            {
                if (_v2Pattern != null)
                    return Task.StringToTimeSpan(_v2Pattern.Duration);
                return TimeSpan.FromMinutes((double)_pTrigger._v1TriggerData.MinutesDuration);
            }
            set
            {
                if (_v2Pattern != null)
                    _v2Pattern.Duration = Task.TimeSpanToString(value);
                else
                {
                    _pTrigger._v1TriggerData.MinutesDuration = (uint)value.TotalMinutes;
                    Bind();
                }
            }
        }

        [DefaultValue(0)]
        public TimeSpan Interval
        {
            get
            {
                if (_v2Pattern != null)
                    return Task.StringToTimeSpan(_v2Pattern.Interval);
                return TimeSpan.FromMinutes((double)_pTrigger._v1TriggerData.MinutesInterval);
            }
            set
            {
                if (_v2Pattern != null)
                    _v2Pattern.Interval = Task.TimeSpanToString(value);
                else
                {
                    _pTrigger._v1TriggerData.MinutesInterval = (uint)value.TotalMinutes;
                    Bind();
                }
            }
        }

        [DefaultValue(false)]
        public bool StopAtDurationEnd
        {
            get
            {
                if (_v2Pattern != null)
                    return _v2Pattern.StopAtDurationEnd;
                return ((_pTrigger._v1TriggerData.Flags & TaskTriggerFlags.KillAtDurationEnd) == TaskTriggerFlags.KillAtDurationEnd);
            }
            set
            {
                if (_v2Pattern != null)
                    _v2Pattern.StopAtDurationEnd = value;
                else
                {
                    if (value)
                        _pTrigger._v1TriggerData.Flags |= TaskTriggerFlags.KillAtDurationEnd;
                    else
                        _pTrigger._v1TriggerData.Flags &= ~TaskTriggerFlags.KillAtDurationEnd;
                    Bind();
                }
            }
        }
    }
}

