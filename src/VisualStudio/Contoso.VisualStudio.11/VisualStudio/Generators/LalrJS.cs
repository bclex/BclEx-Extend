using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Lalr;
using System.Text.Lalr.Emitters;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// LalrJS
    /// </summary>
    [ComVisible(true)]
    [Guid("52A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA3")]
    public class LalrJS : VsMultipleFileGeneratorWithSite
    {
        private Context _ctx;
        private byte[] _templateFileContents;

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
            _ctx.Process();
            if (_ctx.Conflicts > 0)
                Error(0, "{0} parsing conflicts.", _ctx.Conflicts);
            base.PreGenerateContent(inputFilePath, inputFileContents);
        }

        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateContent(string inputFilePath, string inputFileContents)
        {
            var newFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), Path.GetFileNameWithoutExtension(inputFilePath) + GetDefaultExtension());
            using (var s = new MemoryStream())
            {
                using (var rS = new MemoryStream(_templateFileContents))
                using (var r = new StreamReader(rS))
                using (var w = new StreamWriter(s))
                    EmitterJS.EmitTable(_ctx, r, w, false, newFilePath);
                return s.ToArray();
            }
        }

        /// <summary>
        /// Generates the content of the child.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected override byte[] GenerateChildContent(string inputFilePath, string inputFileContents)
        {
            using (var s = new MemoryStream())
                switch (Path.GetExtension(inputFilePath))
                {
                    case ".template":
                        if (!string.IsNullOrWhiteSpace(inputFileContents))
                            return (_templateFileContents = Encoding.ASCII.GetBytes(inputFileContents));
                        using (var rS = typeof(LalrJS).Assembly.GetManifestResourceStream("Contoso.Resource_.Lempar.js"))
                        using (var r = new StreamReader(rS))
                            return (_templateFileContents = Encoding.ASCII.GetBytes(r.ReadToEnd()));
                    case ".out":
                        using (var w = new StreamWriter(s))
                            Reporter.EmitOutput(_ctx, w, false);
                        return s.ToArray();
                    default: throw new InvalidOperationException();
                }
        }

        /// <summary>
        /// Gets the default extension.
        /// </summary>
        /// <returns></returns>
        public override string GetDefaultExtension() { return ".js"; }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator() { return new[] { "*.template", "*.out" }.GetEnumerator(); }
    }
}
