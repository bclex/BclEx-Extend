using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Contoso.VisualStudio.Generators
{
    /// <summary>
    /// ExampleMultipleFile
    /// </summary>
    [ComVisible(true)]
    [Guid("52A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC9")]
    public class ExampleMultipleFile : VsMultipleFileGeneratorWithSite
    {
        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="inputFileContent">Content of the input file.</param>
        /// <returns></returns>
        protected override byte[] GenerateContent(string inputFileName, string inputFileContent)
        {
            return Encoding.ASCII.GetBytes("Example:" + inputFileName);
        }

        /// <summary>
        /// Generates the content of the child.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="inputFileContent">Content of the input file.</param>
        /// <returns></returns>
        protected override byte[] GenerateChildContent(string inputFileName, string inputFileContent)
        {
            return Encoding.ASCII.GetBytes("Example:" + inputFileName);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator() { return new[] { "*.h", "*.out" }.GetEnumerator(); }
    }
}
