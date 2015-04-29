using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Lalr;
using System.Text.Lalr.Emitters;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Designer.Interfaces;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// Lalr
    /// </summary>
    [ComVisible(true)]
#if VS10
    [Guid("10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA0")]
    [ProgId("Contoso.VisualStudio.Generators.Lalr.10")]
#elif VS11
    [Guid("11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA0")]
    [ProgId("Contoso.VisualStudio.Generators.Lalr.11")]
#endif
    [ClassInterface(ClassInterfaceType.None)]
    public class Lalr : BaseCodeMultipleGeneratorWithSite
    {
        private Context _ctx;

        /// <summary>
        /// Pres the code of the generate.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        protected void PreGenerateCode(string inputFilePath, string inputFileContents)
        {
            _ctx = new Context((a, b, c, d) => GeneratorErrorCallback(false, a, b, c, d), (a, b, c, d) => GeneratorErrorCallback(true, a, b, c, d));
            Parser.Parse(_ctx, inputFilePath, inputFileContents, null);
            if (_ctx.Errors > 0)
                return;
            if (_ctx.Rules == 0)
            {
                GeneratorErrorCallback(false, 1, "Empty grammar.", 0, 0);
                return;
            }
            _ctx.Process();
            if (_ctx.Conflicts > 0)
                GeneratorErrorCallback(true, 1, string.Format("{0} parsing conflicts.", _ctx.Conflicts), 0, 0);
        }

        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateCode(string inputFilePath, string inputFileContents)
        {
            PreGenerateCode(inputFilePath, inputFileContents);
            var codeNamespace = new CodeNamespace(FileNamespace);
            var codeUnit = Emitter.Emit(_ctx, codeNamespace, inputFilePath);
            using (var w = new StringWriter())
            {
                GetCodeDomProvider().GenerateCodeFromCompileUnit(codeUnit, w, new CodeGeneratorOptions
                {
                    BlankLinesBetweenMembers = false,
                });
                return Encoding.ASCII.GetBytes(w.ToString());
            }
        }

        /// <summary>
        /// Generates the content of the child.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateChildCode(string inputFilePath, string inputFileContents)
        {
            using (var s = new MemoryStream())
                switch (Path.GetExtension(inputFilePath))
                {
                    case ".out":
                        using (var w = new StreamWriter(s))
                            Reporter.EmitOutput(_ctx, w, false);
                        return s.ToArray();
                    default: throw new InvalidOperationException();
                }
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

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator() { return new[] { "*.out" }.GetEnumerator(); }

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
