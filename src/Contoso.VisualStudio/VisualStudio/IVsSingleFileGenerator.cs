using System;
using System.Runtime.InteropServices;

namespace Contoso.VisualStudio
{
    /// <summary>
    /// IVsSingleFileGenerator
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3634494C-492F-4F91-8009-4541234E4E99")]
    public interface IVsSingleFileGenerator
    {
        /// <summary>
        /// Gets the default extension.
        /// </summary>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDefaultExtension();
        /// <summary>
        /// Generates the specified WSZ input file path.
        /// </summary>
        /// <param name="wszInputFilePath">The WSZ input file path.</param>
        /// <param name="bstrInputFileContents">The BSTR input file contents.</param>
        /// <param name="wszDefaultNamespace">The WSZ default namespace.</param>
        /// <param name="rgbOutputFileContents">The RGB output file contents.</param>
        /// <param name="pcbOutput">The PCB output.</param>
        /// <param name="pGenerateProgress">The p generate progress.</param>
        void Generate([MarshalAs(UnmanagedType.LPWStr)] string wszInputFilePath, [MarshalAs(UnmanagedType.BStr)] string bstrInputFileContents, [MarshalAs(UnmanagedType.LPWStr)] string wszDefaultNamespace, out IntPtr rgbOutputFileContents, [MarshalAs(UnmanagedType.U4)] out int pcbOutput, IVsGeneratorProgress pGenerateProgress);
    }
}
