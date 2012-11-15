#region Foreign-License
// x
#endregion
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    internal sealed class CoTaskMemString : SafeHandle
    {
        public CoTaskMemString()
            : base(IntPtr.Zero, true) { }
        public CoTaskMemString(IntPtr handle)
            : this() { base.SetHandle(handle); }
        public CoTaskMemString(string text)
            : this() { base.SetHandle(Marshal.StringToCoTaskMemUni(text)); }

        public static implicit operator string(CoTaskMemString cmem) { return cmem.ToString(); }

        protected override bool ReleaseHandle() { Marshal.FreeCoTaskMem(base.handle); return true; }

        public override string ToString() { return Marshal.PtrToStringUni(base.handle); }

        public override bool IsInvalid
        {
            get { return (base.handle == IntPtr.Zero); }
        }
    }
}

