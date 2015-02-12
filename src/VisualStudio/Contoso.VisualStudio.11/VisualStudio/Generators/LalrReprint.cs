using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Lalr;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// LalrReprint
    /// </summary>
    [ComVisible(true)]
#if VS10
    [Guid("10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA1")]
    [ProgId("Contoso.VisualStudio.Generators.LalrReprint.10")]
#elif VS11
    [Guid("11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA1")]
    [ProgId("Contoso.VisualStudio.Generators.LalrReprint.11")]
#endif
    [ClassInterface(ClassInterfaceType.None)]
    public class LalrReprint : BaseCodeGeneratorWithSite
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
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateCode(string inputFilePath, string inputFileContents)
        {
            PreGenerateCode(inputFilePath, inputFileContents);
            using (var s = new MemoryStream())
            {
                using (var w = new StreamWriter(s))
                    _ctx.Reprint(w);
                return s.ToArray();
            }
        }

        /// <summary>
        /// Gets the default extension.
        /// </summary>
        /// <returns></returns>
        public override string GetDefaultExtension() { return ".out"; }
    }
}
