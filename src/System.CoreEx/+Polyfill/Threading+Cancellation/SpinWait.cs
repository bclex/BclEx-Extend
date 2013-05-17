#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Runtime;
using System.Collections.Generic;
namespace System.Threading
{
    /// <summary>
    /// SpinWait
    /// </summary>
    [StructLayout(LayoutKind.Sequential), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public struct SpinWait
    {
        internal const int YIELD_THRESHOLD = 10;
        internal const int SLEEP_0_EVERY_HOW_MANY_TIMES = 5;
        internal const int SLEEP_1_EVERY_HOW_MANY_TIMES = 20;
        private int _count;

        public int Count
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _count; }
        }

        public bool NextSpinWillYield
        {
            get { return (_count <= 10 ? PlatformHelper.IsSingleProcessor : true); }
        }

        public void SpinOnce()
        {
            if (NextSpinWillYield)
            {
                CdsSyncEtwBCLProvider.Log.SpinWait_NextSpinWillYield();
                var num = (_count >= 10 ? (_count - 10) : _count);
                if ((num % 20) == 0x13)
                    Thread.Sleep(1);
                else if ((num % 5) == 4)
                    Thread.Sleep(0);
                else
                    Thread.Yield();
            }
            else
                Thread.SpinWait(((int)4) << _count);
            _count = (_count == 0x7fffffff ? 10 : (_count + 1));
        }

        public void Reset()
        {
            _count = 0;
        }

        public static void SpinUntil(Func<bool> condition)
        {
            SpinUntil(condition, -1);
        }

        public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
        {
            var totalMilliseconds = (long)timeout.TotalMilliseconds;
            if (totalMilliseconds < -1L || totalMilliseconds > 0x7fffffffL)
                throw new ArgumentOutOfRangeException("timeout", timeout, SR.GetResourceString("SpinWait_SpinUntil_TimeoutWrong"));
            return SpinUntil(condition, (int)timeout.TotalMilliseconds);
        }

        public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
                throw new ArgumentOutOfRangeException("millisecondsTimeout", millisecondsTimeout, SR.GetResourceString("SpinWait_SpinUntil_TimeoutWrong"));
            if (condition == null)
                throw new ArgumentNullException("condition", SR.GetResourceString("SpinWait_SpinUntil_ArgumentNull"));
            var time = 0U;
            if (millisecondsTimeout != 0 && millisecondsTimeout != -1)
                time = TimeoutHelper.GetTime();
            var wait = new SpinWait();
            while (!condition())
            {
                if (millisecondsTimeout == 0)
                    return false;
                wait.SpinOnce();
                if (millisecondsTimeout != -1 && wait.NextSpinWillYield && millisecondsTimeout <= (TimeoutHelper.GetTime() - time))
                    return false;
            }
            return true;
        }
    }
}
#endif