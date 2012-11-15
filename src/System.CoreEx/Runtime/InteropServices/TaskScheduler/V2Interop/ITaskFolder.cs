#region Foreign-License
// x
#endregion
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, Guid("8CFAC062-A080-4C15-9A88-AA7C2AF80DFC"), DefaultMember("Path"), TypeLibType((short)0x10c0), SuppressUnmanagedCodeSecurity]
    internal interface ITaskFolder
    {
        [DispId(1)]
        string Name { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        string Path { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        ITaskFolder GetFolder([MarshalAs(UnmanagedType.BStr)] string Path);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        ITaskFolderCollection GetFolders(int flags);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        ITaskFolder CreateFolder([In, MarshalAs(UnmanagedType.BStr)] string subFolderName, [In, Optional, MarshalAs(UnmanagedType.Struct)] object sddl);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        void DeleteFolder([MarshalAs(UnmanagedType.BStr)] string subFolderName, [In] int flags);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)]
        IRegisteredTask GetTask([MarshalAs(UnmanagedType.BStr)] string Path);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)]
        IRegisteredTaskCollection GetTasks(int flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)]
        void DeleteTask([In, MarshalAs(UnmanagedType.BStr)] string Name, [In] int flags);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)]
        IRegisteredTask RegisterTask([In, MarshalAs(UnmanagedType.BStr)] string Path, [In, MarshalAs(UnmanagedType.BStr)] string XmlText, [In] int flags, [In, MarshalAs(UnmanagedType.Struct)] object UserId, [In, MarshalAs(UnmanagedType.Struct)] object password, [In] TaskLogonType LogonType, [In, Optional, MarshalAs(UnmanagedType.Struct)] object sddl);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
        IRegisteredTask RegisterTaskDefinition([In, MarshalAs(UnmanagedType.BStr)] string Path, [In, MarshalAs(UnmanagedType.Interface)] ITaskDefinition pDefinition, [In] int flags, [In, MarshalAs(UnmanagedType.Struct)] object UserId, [In, MarshalAs(UnmanagedType.Struct)] object password, [In] TaskLogonType LogonType, [In, Optional, MarshalAs(UnmanagedType.Struct)] object sddl);
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
        string GetSecurityDescriptor(int securityInformation);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
        void SetSecurityDescriptor([In, MarshalAs(UnmanagedType.BStr)] string sddl, [In] int flags);
    }
}

