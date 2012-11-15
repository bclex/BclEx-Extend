#region Foreign-License
// x
#endregion
using System.Diagnostics;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskSettings : IDisposable
    {
        private IdleSettings _idleSettings;
        private NetworkSettings _networkSettings;
        private ITask _v1Task;
        private ITaskSettings _v2Settings;
        private ITaskSettings2 _v2Settings2;

        internal TaskSettings(ITask iTask)
        {
            this._v1Task = iTask;
        }
        internal TaskSettings(ITaskSettings iSettings)
        {
            this._v2Settings = iSettings;
            try { this._v2Settings2 = (ITaskSettings2)this._v2Settings; }
            catch { }
        }

        public void Dispose()
        {
            if (this._v2Settings != null)
                Marshal.ReleaseComObject(this._v2Settings);
            this._idleSettings = null;
            this._networkSettings = null;
            this._v1Task = null;
        }

        public bool AllowDemandStart
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.AllowDemandStart;
                return true;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.AllowDemandStart = value;
            }
        }

        public bool AllowHardTerminate
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.AllowHardTerminate;
                return true;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.AllowHardTerminate = value;
            }
        }

        public TaskCompatibility Compatibility
        {
            get
            {
                if (this._v2Settings == null)
                    return TaskCompatibility.V1;
                if (this._v2Settings.Compatibility < TaskCompatibility.V2)
                    return this._v2Settings.Compatibility;
                return TaskCompatibility.V2;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.Compatibility = value;
            }
        }

        public TimeSpan DeleteExpiredTaskAfter
        {
            get
            {
                if (this._v2Settings != null)
                    return Task.StringToTimeSpan(this._v2Settings.DeleteExpiredTaskAfter);
                if ((this._v1Task.GetFlags() & TaskFlags.DeleteWhenDone) != TaskFlags.DeleteWhenDone)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(1.0);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.DeleteExpiredTaskAfter = Task.TimeSpanToString(value);
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value >= TimeSpan.FromSeconds(1.0))
                        this._v1Task.SetFlags(flags |= TaskFlags.DeleteWhenDone);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.DeleteWhenDone);
                }
            }
        }

        public bool DisallowStartIfOnBatteries
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.DisallowStartIfOnBatteries;
                return ((this._v1Task.GetFlags() & TaskFlags.DontStartIfOnBatteries) == TaskFlags.DontStartIfOnBatteries);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.DisallowStartIfOnBatteries = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.DontStartIfOnBatteries);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.DontStartIfOnBatteries);
                }
            }
        }

        public bool DisallowStartOnRemoteAppSession
        {
            get { return ((this._v2Settings2 != null) && this._v2Settings2.DisallowStartOnRemoteAppSession); }
            set
            {
                if (this._v2Settings2 == null)
                    throw new NotV1SupportedException();
                this._v2Settings2.DisallowStartOnRemoteAppSession = value;
            }
        }

        public bool Enabled
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.Enabled;
                return ((this._v1Task.GetFlags() & TaskFlags.Disabled) != TaskFlags.Disabled);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.Enabled = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (!value)
                        this._v1Task.SetFlags(flags |= TaskFlags.Disabled);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.Disabled);
                }
            }
        }

        public TimeSpan ExecutionTimeLimit
        {
            get
            {
                if (this._v2Settings != null)
                    return Task.StringToTimeSpan(this._v2Settings.ExecutionTimeLimit);
                return TimeSpan.FromMilliseconds((double)this._v1Task.GetMaxRunTime());
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.ExecutionTimeLimit = (value == TimeSpan.Zero) ? "PT0S" : Task.TimeSpanToString(value);
                else
                    this._v1Task.SetMaxRunTime(Convert.ToUInt32(value.TotalMilliseconds));
            }
        }

        public bool Hidden
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.Hidden;
                return ((this._v1Task.GetFlags() & TaskFlags.Hidden) == TaskFlags.Hidden);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.Hidden = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.Hidden);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.Hidden);
                }
            }
        }

        public IdleSettings IdleSettings
        {
            get
            {
                if (this._idleSettings == null)
                    if (this._v2Settings != null)
                        this._idleSettings = new IdleSettings(this._v2Settings.IdleSettings);
                    else
                        this._idleSettings = new IdleSettings(this._v1Task);
                return this._idleSettings;
            }
        }

        public TaskInstancesPolicy MultipleInstances
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.MultipleInstances;
                return TaskInstancesPolicy.IgnoreNew;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.MultipleInstances = value;
            }
        }

        public NetworkSettings NetworkSettings
        {
            get
            {
                if (this._networkSettings == null)
                    if (this._v2Settings != null)
                        this._networkSettings = new NetworkSettings(this._v2Settings.NetworkSettings);
                    else
                        this._networkSettings = new NetworkSettings();
                return this._networkSettings;
            }
        }

        public ProcessPriorityClass Priority
        {
            get
            {
                if (this._v2Settings == null)
                    return (ProcessPriorityClass)this._v1Task.GetPriority();
                switch (this._v2Settings.Priority)
                {
                    case 0:
                        return ProcessPriorityClass.RealTime;
                    case 1:
                        return ProcessPriorityClass.High;
                    case 2:
                    case 3:
                        return ProcessPriorityClass.AboveNormal;
                    case 7:
                    case 8:
                        return ProcessPriorityClass.BelowNormal;
                    case 9:
                    case 10:
                        return ProcessPriorityClass.Idle;
                }
                return ProcessPriorityClass.Normal;
            }
            set
            {
                if (this._v2Settings == null)
                {
                    if (value == ProcessPriorityClass.AboveNormal || value == ProcessPriorityClass.BelowNormal)
                        throw new NotV1SupportedException("Unsupported priority level on Task Scheduler 1.0.");
                    this._v1Task.SetPriority((uint)value);
                }
                else
                {
                    var num = 7;
                    var class2 = value;
                    if (class2 <= ProcessPriorityClass.High)
                        switch (class2)
                        {
                            case ProcessPriorityClass.Normal:
                                num = 5;
                                break;
                            case ProcessPriorityClass.Idle:
                                num = 10;
                                break;
                            case ProcessPriorityClass.High:
                                num = 1;
                                break;
                        }
                    else if (class2 == ProcessPriorityClass.RealTime)
                        num = 0;
                    else if ((class2 != ProcessPriorityClass.BelowNormal) && (class2 == ProcessPriorityClass.AboveNormal))
                        num = 3;
                    this._v2Settings.Priority = num;
                }
            }
        }

        public int RestartCount
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.RestartCount;
                return 0;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.RestartCount = value;
            }
        }

        public TimeSpan RestartInterval
        {
            get
            {
                if (this._v2Settings != null)
                    return Task.StringToTimeSpan(this._v2Settings.RestartInterval);
                return TimeSpan.Zero;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.RestartInterval = Task.TimeSpanToString(value);
            }
        }

        public bool RunOnlyIfIdle
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.RunOnlyIfIdle;
                return ((this._v1Task.GetFlags() & TaskFlags.StartOnlyIfIdle) == TaskFlags.StartOnlyIfIdle);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.RunOnlyIfIdle = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.StartOnlyIfIdle);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.StartOnlyIfIdle);
                }
            }
        }

        public bool RunOnlyIfLoggedOn
        {
            get { return (this._v2Settings != null || (this._v1Task.GetFlags() & TaskFlags.RunOnlyIfLoggedOn) == TaskFlags.RunOnlyIfLoggedOn); }
            set
            {
                if (this._v1Task != null)
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.RunOnlyIfLoggedOn);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.RunOnlyIfLoggedOn);
                }
                else if (this._v2Settings != null)
                    throw new NotV2SupportedException("Task Scheduler 2.0 (1.2) does not support setting this property. You must use an InteractiveToken in order to have the task run in the current user session.");
            }
        }

        public bool RunOnlyIfNetworkAvailable
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.RunOnlyIfNetworkAvailable;
                return ((this._v1Task.GetFlags() & TaskFlags.RunIfConnectedToInternet) == TaskFlags.RunIfConnectedToInternet);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.RunOnlyIfNetworkAvailable = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.RunIfConnectedToInternet);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.RunIfConnectedToInternet);
                }
            }
        }

        public bool StartWhenAvailable
        {
            get { return (this._v2Settings != null && this._v2Settings.StartWhenAvailable); }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.StartWhenAvailable = value;
            }
        }

        public bool StopIfGoingOnBatteries
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.StopIfGoingOnBatteries;
                return ((this._v1Task.GetFlags() & TaskFlags.KillIfGoingOnBatteries) == TaskFlags.KillIfGoingOnBatteries);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.StopIfGoingOnBatteries = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.KillIfGoingOnBatteries);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.KillIfGoingOnBatteries);
                }
            }
        }

        public bool UseUnifiedSchedulingEngine
        {
            get { return (this._v2Settings2 != null && this._v2Settings2.UseUnifiedSchedulingEngine); }
            set
            {
                if (this._v2Settings2 == null)
                    throw new NotV1SupportedException();
                this._v2Settings2.UseUnifiedSchedulingEngine = value;
            }
        }

        public bool WakeToRun
        {
            get
            {
                if (this._v2Settings != null)
                    return this._v2Settings.WakeToRun;
                return ((this._v1Task.GetFlags() & TaskFlags.SystemRequired) == TaskFlags.SystemRequired);
            }
            set
            {
                if (this._v2Settings != null)
                    this._v2Settings.WakeToRun = value;
                else
                {
                    var flags = this._v1Task.GetFlags();
                    if (value)
                        this._v1Task.SetFlags(flags |= TaskFlags.SystemRequired);
                    else
                        this._v1Task.SetFlags(flags &= ~TaskFlags.SystemRequired);
                }
            }
        }

        public string XmlText
        {
            get
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                return this._v2Settings.XmlText;
            }
            set
            {
                if (this._v2Settings == null)
                    throw new NotV1SupportedException();
                this._v2Settings.XmlText = value;
            }
        }
    }
}

