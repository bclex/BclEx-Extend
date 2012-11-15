#region Foreign-License
// x
#endregion
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
namespace System.Runtime.InteropServices.TaskScheduler.V2Interop
{
    [ComImport, SuppressUnmanagedCodeSecurity, TypeLibType((short)0x10c0), Guid("02820E19-7B98-4ED2-B2E8-FDCCCEFF619B")]
    internal interface IActionCollection : IEnumerable
    {
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(0)]
        IAction this[int index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4)]
        IEnumerator GetEnumerator();
        [DispId(2)]
        string XmlText { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        IAction Create([In] TaskActionType Type);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void Remove([In, MarshalAs(UnmanagedType.Struct)] object index);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void Clear();
        [DispId(6)]
        string Context { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] set; }
    }
}

