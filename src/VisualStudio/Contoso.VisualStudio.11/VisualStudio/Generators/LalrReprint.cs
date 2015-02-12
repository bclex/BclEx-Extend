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
    public class LalrReprint : VsSingleFileGeneratorWithSite
    {
        private Context _ctx;

        /// <summary>
        /// Pres the content of the generate.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        protected override void PreGenerateContent(string inputFilePath, string inputFileContents)
        {
            _ctx = new Context(Error, Warning);
            Parser.Parse(_ctx, inputFilePath, inputFileContents, null);
            if (_ctx.Errors > 0)
                return;
            if (_ctx.Rules == 0)
            {
                Warning(0, "Empty grammar.");
                return;
            }
        }

        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateContent(string inputFilePath, string inputFileContents)
        {
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
