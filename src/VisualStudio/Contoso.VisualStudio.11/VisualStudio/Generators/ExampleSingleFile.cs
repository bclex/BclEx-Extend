using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// ExampleSingleFile
    /// </summary>
    [ComVisible(true)]
#if VS10
    [Guid("10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC8")]
    [ProgId("Contoso.VisualStudio.Generators.ExampleSingleFile.10")]
#elif VS11
    [Guid("11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC8")]
    [ProgId("Contoso.VisualStudio.Generators.ExampleSingleFile.11")]
#endif
    public class ExampleSingleFile : BaseCodeGeneratorWithSite
    {
        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="inputFileContent">Content of the input file.</param>
        /// <returns></returns>
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            return Encoding.ASCII.GetBytes("Example:" + inputFileName);
            //var code = new CodeCompileUnit();
            //var codeNamespace = new CodeNamespace(DefaultNamespace);
            //code.Namespaces.Add(codeNamespace);
            //codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            //codeNamespace.Comments.Add(new CodeCommentStatement { Comment = new CodeComment { Text = "TEST" } });
            //using (var w = new StringWriter())
            //{
            //    CodeProvider.GenerateCodeFromCompileUnit(code, w, null);
            //    return Encoding.ASCII.GetBytes(w.ToString());
            //}
        }

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
