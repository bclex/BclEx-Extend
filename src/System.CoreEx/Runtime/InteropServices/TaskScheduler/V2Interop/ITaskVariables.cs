#region Foreign-License
// x
#endregion
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("3E4C9351-D966-4B8B-BB87-CEBA68BB0107"), InterfaceType((short)1), SuppressUnmanagedCodeSecurity]
    internal interface ITaskVariables
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetInput();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOutput([In, MarshalAs(UnmanagedType.BStr)] string input);
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetContext();
    }
}

