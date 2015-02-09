using System;
using System.Runtime.InteropServices;

namespace Contoso.VisualStudio
{
    /// <summary>
    /// IVsGeneratorProgress
    /// </summary>
    [ComImport, Guid("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVsGeneratorProgress
    {
        /// <summary>
        /// Generators the error.
        /// </summary>
        /// <param name="fWarning">if set to <c>true</c> [f warning].</param>
        /// <param name="dwLevel">The dw level.</param>
        /// <param name="bstrError">The BSTR error.</param>
        /// <param name="dwLine">The dw line.</param>
        /// <param name="dwColumn">The dw column.</param>
        void GeneratorError(bool fWarning, [MarshalAs(UnmanagedType.U4)] int dwLevel, [MarshalAs(UnmanagedType.BStr)] string bstrError, [MarshalAs(UnmanagedType.U4)] int dwLine, [MarshalAs(UnmanagedType.U4)] int dwColumn);
        /// <summary>
        /// Progresses the specified n complete.
        /// </summary>
        /// <param name="nComplete">The n complete.</param>
        /// <param name="nTotal">The n total.</param>
        void Progress([MarshalAs(UnmanagedType.U4)] int nComplete, [MarshalAs(UnmanagedType.U4)] int nTotal);
    }
}
