#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class IdleSettings : IDisposable
    {
        private ITask _v1Task;
        private IIdleSettings _v2Settings;

        internal IdleSettings(ITask iTask)
        {
            _v1Task = iTask;
        }

        internal IdleSettings(IIdleSettings iSettings)
        {
            _v2Settings = iSettings;
        }

        public void Dispose()
        {
            if (_v2Settings != null)
                Marshal.ReleaseComObject(_v2Settings);
            _v1Task = null;
        }

        public TimeSpan IdleDuration
        {
            get
            {
                if (_v2Settings != null)
                    return Task.StringToTimeSpan(_v2Settings.IdleDuration);
                ushort num;
                ushort num2;
                _v1Task.GetIdleWait(out num, out num2);
                return TimeSpan.FromMinutes((double)num2);
            }
            set
            {
                if (_v2Settings != null)
                    _v2Settings.IdleDuration = Task.TimeSpanToString(value);
                else
                    _v1Task.SetIdleWait((ushort)WaitTimeout.TotalMinutes, (ushort)value.TotalMinutes);
            }
        }

        public bool RestartOnIdle
        {
            get
            {
                if (_v2Settings != null)
                    return _v2Settings.RestartOnIdle;
                return ((_v1Task.GetFlags() & TaskFlags.RestartOnIdleResume) == TaskFlags.RestartOnIdleResume);
            }
            set
            {
                if (_v2Settings != null)
                    _v2Settings.RestartOnIdle = value;
                else
                {
                    var flags = _v1Task.GetFlags();
                    if (value)
                        _v1Task.SetFlags(flags |= TaskFlags.RestartOnIdleResume);
                    else
                        _v1Task.SetFlags(flags &= ~TaskFlags.RestartOnIdleResume);
                }
            }
        }

        public bool StopOnIdleEnd
        {
            get
            {
                if (_v2Settings != null)
                    return _v2Settings.StopOnIdleEnd;
                return ((_v1Task.GetFlags() & TaskFlags.KillOnIdleEnd) == TaskFlags.KillOnIdleEnd);
            }
            set
            {
                if (_v2Settings != null)
                    _v2Settings.StopOnIdleEnd = value;
                else
                {
                    var flags = _v1Task.GetFlags();
                    if (value)
                        _v1Task.SetFlags(flags |= TaskFlags.KillOnIdleEnd);
                    else
                        _v1Task.SetFlags(flags &= ~TaskFlags.KillOnIdleEnd);
                }
            }
        }

        public TimeSpan WaitTimeout
        {
            get
            {
                if (_v2Settings != null)
                    return Task.StringToTimeSpan(_v2Settings.WaitTimeout);
                ushort num;
                ushort num2;
                _v1Task.GetIdleWait(out num, out num2);
                return TimeSpan.FromMinutes((double)num);
            }
            set
            {
                if (_v2Settings != null)
                    _v2Settings.WaitTimeout = Task.TimeSpanToString(value);
                else
                    _v1Task.SetIdleWait((ushort)value.TotalMinutes, (ushort)IdleDuration.TotalMinutes);
            }
        }
    }
}

