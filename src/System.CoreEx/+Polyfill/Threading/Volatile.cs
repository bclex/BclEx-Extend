#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR45
using System.Security;
using System.Runtime;
using System.Runtime.ConstrainedExecution;
namespace System.Threading
{
    /// <summary>
    /// Volatile
    /// </summary>
    public static class Volatile
    {
        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), SecuritySafeCritical]
        public static T Read<T>(ref T location)
            where T : class { var local = location; Thread.MemoryBarrier(); return local; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">if set to <c>true</c> [location].</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool Read(ref bool location) { var flag = location; Thread.MemoryBarrier(); return flag; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static byte Read(ref byte location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static double Read(ref double location) { return Interlocked.CompareExchange(ref location, 0.0, 0.0); }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static short Read(ref short location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static int Read(ref int location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static long Read(ref long location) { return Interlocked.CompareExchange(ref location, 0L, 0L); }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static IntPtr Read(ref IntPtr location) { var ptr = location; Thread.MemoryBarrier(); return ptr; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static sbyte Read(ref sbyte location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static float Read(ref float location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static ushort Read(ref ushort location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static uint Read(ref uint location) { var num = location; Thread.MemoryBarrier(); return num; }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecuritySafeCritical, TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static unsafe ulong Read(ref ulong location)
        {
            fixed (ulong* numRef = &location)
            {
                var num = (long)numRef;
                return (ulong)Interlocked.CompareExchange(ref num, 0L, 0L);
            }
        }

        /// <summary>
        /// Reads the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static UIntPtr Read(ref UIntPtr location) { var ptr = location; Thread.MemoryBarrier(); return ptr; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecuritySafeCritical, TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write<T>(ref T location, T value)
            where T : class { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">if set to <c>true</c> [location].</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref bool location, bool value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref byte location, byte value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref double location, double value) { Interlocked.Exchange(ref location, value); }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref short location, short value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref int location, int value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref long location, long value) { Interlocked.Exchange(ref location, value); }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref IntPtr location, IntPtr value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref sbyte location, sbyte value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref float location, float value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref ushort location, ushort value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref uint location, uint value) { Thread.MemoryBarrier(); location = value; }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static unsafe void Write(ref ulong location, ulong value)
        {
            fixed (ulong* numRef = &location)
            {
                var num = (long)numRef;
                Interlocked.Exchange(ref num, (long)value);
            }
        }

        /// <summary>
        /// Writes the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static void Write(ref UIntPtr location, UIntPtr value) { Thread.MemoryBarrier(); location = value; }
    }
}
#endif