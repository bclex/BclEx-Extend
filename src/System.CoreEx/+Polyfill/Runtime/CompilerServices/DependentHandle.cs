#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Security;
using System.Runtime.InteropServices;
namespace System.Runtime.CompilerServices
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    internal struct DependentHandle
    {
        private WeakReference _primary;
        private object _secondary;

        [SecurityCritical]
        public DependentHandle(object primary, object secondary)
        {
            _primary = new WeakReference(primary);
            _secondary = secondary;
        }

        public bool IsAllocated
        {
            get { return ((_primary != null) && _primary.IsAlive); }
        }

        [SecurityCritical]
        public object GetPrimary() { return _primary.Target; }

        [SecurityCritical]
        public void GetPrimaryAndSecondary(out object primary, out object secondary)
        {
            primary = _primary.Target;
            secondary = _secondary;
        }

        // Forces dependentHandle back to non-allocated state (if not already there) and frees the handle if needed.
        [SecurityCritical]
        public void Free()
        {
            _primary = null;
            _secondary = null;
        }
    }

    //[StructLayout(LayoutKind.Sequential), ComVisible(false)]
    //public struct DependentHandle
    //{
    //    private IntPtr _handle;

    //    [SecurityCritical]
    //    public DependentHandle(object primary, object secondary)
    //    {
    //        var zero = IntPtr.Zero;
    //        // no need to check for null result: nInitialize expected to throw OOM.
    //        nInitialize(primary, secondary, out zero);
    //        _handle = zero;
    //    }

    //    public bool IsAllocated
    //    {
    //        get { return (_handle != IntPtr.Zero); }
    //    }

    //    // Getting the secondary object is more expensive than getting the first so we provide a separate primary-only accessor for those times we only want the primary.
    //    [SecurityCritical]
    //    public object GetPrimary()
    //    {
    //        object obj2;
    //        nGetPrimary(_handle, out obj2);
    //        return obj2;
    //    }

    //    [SecurityCritical]
    //    public void GetPrimaryAndSecondary(out object primary, out object secondary) { nGetPrimaryAndSecondary(_handle, out primary, out secondary); }

    //    // Forces dependentHandle back to non-allocated state (if not already there) and frees the handle if needed.
    //    [SecurityCritical]
    //    public void Free()
    //    {
    //        if (_handle != IntPtr.Zero)
    //        {
    //            IntPtr dependentHandle = _handle;
    //            _handle = IntPtr.Zero;
    //            nFree(dependentHandle);
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical]
    //    private static extern void nInitialize(object primary, object secondary, out IntPtr dependentHandle);
    //    [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical]
    //    private static extern void nGetPrimary(IntPtr dependentHandle, out object primary);
    //    [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical]
    //    private static extern void nGetPrimaryAndSecondary(IntPtr dependentHandle, out object primary, out object secondary);
    //    [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical]
    //    private static extern void nFree(IntPtr dependentHandle);
    //}
}
#endif