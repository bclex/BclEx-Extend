#region Foreign-License
// x
#endregion
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V1Interop
{
    [Guid("148BD528-A2AB-11CE-B11F-00AA00530503"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
    internal interface IEnumWorkItems
    {
        [PreserveSig]
        int Next([In] uint RequestCount, out IntPtr Names, out uint Fetched);
        void Skip([In] uint Count);
        void Reset();
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumWorkItems Clone();
    }
}

