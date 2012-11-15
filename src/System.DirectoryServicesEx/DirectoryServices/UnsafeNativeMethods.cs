using System.Runtime.InteropServices;
using System.Security;

namespace System.DirectoryServices
{
    [SuppressUnmanagedCodeSecurity, ComVisible(false)]
    internal class UnsafeNativeMethods
    {
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("FD8256D0-FD15-11CE-ABC4-02608C9E7553")]
        internal interface IAds
        {
            string Name { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            string Class { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            string GUID { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            string ADsPath { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            string Parent { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            string Schema { [return: MarshalAs(UnmanagedType.BStr)] [SuppressUnmanagedCodeSecurity] get; }
            [SuppressUnmanagedCodeSecurity]
            void GetInfo();
            [SuppressUnmanagedCodeSecurity]
            void SetInfo();
            [return: MarshalAs(UnmanagedType.Struct)]
            [SuppressUnmanagedCodeSecurity]
            object Get([In, MarshalAs(UnmanagedType.BStr)] string bstrName);
            [SuppressUnmanagedCodeSecurity]
            void Put([In, MarshalAs(UnmanagedType.BStr)] string bstrName, [In, MarshalAs(UnmanagedType.Struct)] object vProp);
            [PreserveSig, SuppressUnmanagedCodeSecurity]
            int GetEx([In, MarshalAs(UnmanagedType.BStr)] string bstrName, [MarshalAs(UnmanagedType.Struct)] out object value);
            [SuppressUnmanagedCodeSecurity]
            void PutEx([In, MarshalAs(UnmanagedType.U4)] int lnControlCode, [In, MarshalAs(UnmanagedType.BStr)] string bstrName, [In, MarshalAs(UnmanagedType.Struct)] object vProp);
            [SuppressUnmanagedCodeSecurity]
            void GetInfoEx([In, MarshalAs(UnmanagedType.Struct)] object vProperties, [In, MarshalAs(UnmanagedType.U4)] int lnReserved);
        }

        //[ComImport, Guid("451a0030-72ec-11cf-b03b-00aa006e0975"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsDual)]
        //internal interface IADsMembers : IEnumerable
        //{
        //    int Count { [return: MarshalAs(UnmanagedType.U4)] get; }
        //    [return: ComAliasName("get_NewEnum"), MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        //    new IEnumerator GetEnumerator();
        //    object Filter { [return: MarshalAs(UnmanagedType.Struct)] get; [param: MarshalAs(UnmanagedType.Struct)] set; }
        //}

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D592AED4-F420-11D0-A36E-00C04FB950DC")]
        internal interface IAdsPathname
        {
            [SuppressUnmanagedCodeSecurity]
            int Set([In, MarshalAs(UnmanagedType.BStr)] string bstrADsPath, [In, MarshalAs(UnmanagedType.U4)] int lnSetType);
            int SetDisplayType([In, MarshalAs(UnmanagedType.U4)] int lnDisplayType);
            [return: MarshalAs(UnmanagedType.BStr)]
            [SuppressUnmanagedCodeSecurity]
            string Retrieve([In, MarshalAs(UnmanagedType.U4)] int lnFormatType);
            [return: MarshalAs(UnmanagedType.U4)]
            int GetNumElements();
            [return: MarshalAs(UnmanagedType.BStr)]
            string GetElement([In, MarshalAs(UnmanagedType.U4)] int lnElementIndex);
            void AddLeafElement([In, MarshalAs(UnmanagedType.BStr)] string bstrLeafElement);
            void RemoveLeafElement();
            [return: MarshalAs(UnmanagedType.Interface)]
            object CopyPath();
            [return: MarshalAs(UnmanagedType.BStr)]
            [SuppressUnmanagedCodeSecurity]
            string GetEscapedElement([In, MarshalAs(UnmanagedType.U4)] int lnReserved, [In, MarshalAs(UnmanagedType.BStr)] string bstrInStr);
            int EscapedMode { get; [SuppressUnmanagedCodeSecurity] set; }
        }

        [ComImport, Guid("080d0d78-f421-11d0-a36e-00c04fb950dc")]
        internal class Pathname
        {
        }
    }
}
