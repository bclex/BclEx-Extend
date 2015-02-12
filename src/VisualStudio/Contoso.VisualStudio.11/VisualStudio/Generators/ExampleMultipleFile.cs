using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// ExampleMultipleFile
    /// </summary>
    [ComVisible(true)]
#if VS10
    [Guid("10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC9")]
    [ProgId("Contoso.VisualStudio.Generators.ExampleMultipleFile.10")]
#elif VS11
    [Guid("11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC9")]
    [ProgId("Contoso.VisualStudio.Generators.ExampleMultipleFile.11")]
#endif
    [ClassInterface(ClassInterfaceType.None)]
    public class ExampleMultipleFile : BaseCodeMultipleGeneratorWithSite
    {
        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="inputFileContent">Content of the input file.</param>
        /// <returns></returns>
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            return Encoding.ASCII.GetBytes("Example:" + inputFileName);
        }

        /// <summary>
        /// Generates the content of the child.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="inputFileContent">Content of the input file.</param>
        /// <returns></returns>
        protected override byte[] GenerateChildCode(string inputFileName, string inputFileContent)
        {
            return Encoding.ASCII.GetBytes("Example:" + inputFileName);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator() { return new[] { "*.h", "*.out" }.GetEnumerator(); }

        /// <summary>
        /// gets the default extension for this generator
        /// </summary>
        /// <returns>
        /// string with the default extension for this generator
        /// </returns>
        public override string GetDefaultExtension()
        {
            var fileExtension = GetCodeDomProvider().FileExtension;
            if (!string.IsNullOrEmpty(fileExtension) && fileExtension[0] != '.')
                fileExtension = "." + fileExtension;
            return fileExtension;
        }

        private CodeDomProvider _codeDomProvider;
        /// <summary>
        /// Gets the code DOM provider.
        /// </summary>
        /// <returns></returns>
        protected CodeDomProvider GetCodeDomProvider()
        {
            if (_codeDomProvider == null)
            {
                var service = (IVSMDCodeDomProvider)GetService(new Guid("{73E59688-C7C4-4a85-AF64-A538754784C5}")); //: CodeDomInterfaceGuid
                if (service != null)
                    _codeDomProvider = (CodeDomProvider)service.CodeDomProvider;
            }
            return _codeDomProvider;
        }
    }
}
