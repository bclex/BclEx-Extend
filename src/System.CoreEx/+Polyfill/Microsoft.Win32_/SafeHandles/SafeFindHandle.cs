#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Security;
namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// SafeFindHandle
    /// </summary>
    [SecurityCritical]
    internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        internal SafeFindHandle()
            : base(true) { }

        [SecurityCritical]
        protected override bool ReleaseHandle() { return Win32Native.FindClose(handle); }
    }
}
#endif