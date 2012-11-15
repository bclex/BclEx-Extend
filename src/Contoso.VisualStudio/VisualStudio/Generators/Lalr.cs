using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Lalr;
using System.Text.Lalr.Emitters;
using System.CodeDom.Compiler;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// Lalr
    /// </summary>
    [ComVisible(true)]
    [Guid("52A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA0")]
    public class Lalr : VsMultipleFileGeneratorWithSite
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
            var codeNamespace = new CodeNamespace(DefaultNamespace);
            var codeUnit = Emitter.Emit(_ctx, codeNamespace, inputFilePath);
            using (var w = new StringWriter())
            {
                CodeProvider.GenerateCodeFromCompileUnit(codeUnit, w, new CodeGeneratorOptions
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
        protected override byte[] GenerateChildContent(string inputFilePath, string inputFileContents)
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
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator() { return new[] { "*.out" }.GetEnumerator(); }
    }
}
