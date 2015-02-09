using System;
using System.Runtime.InteropServices;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Contoso.VisualStudio
{
    /// <summary>
    /// VsSingleFileGenerator
    /// </summary>
    public abstract class VsSingleFileGenerator : IVsSingleFileGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VsSingleFileGenerator"/> class.
        /// </summary>
        protected VsSingleFileGenerator() { }

        private void SetProject(string path)
        {
            var dte = (Package.GetGlobalService(typeof(SDTE)) as DTE);
            var projects = (dte.ActiveSolutionProjects as Array);
            if (projects == null || projects.Length == 0)
                throw new NullReferenceException();
            Project = (Project)projects.GetValue(0);
        }

        /// <summary>
        /// Generates the specified WSZ input file path.
        /// </summary>
        /// <param name="wszInputFilePath">The WSZ input file path.</param>
        /// <param name="bstrInputFileContents">The BSTR input file contents.</param>
        /// <param name="wszDefaultNamespace">The WSZ default namespace.</param>
        /// <param name="rgbOutputFileContents">The RGB output file contents.</param>
        /// <param name="pcbOutput">The PCB output.</param>
        /// <param name="pGenerateProgress">The p generate progress.</param>
        public virtual void Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, out IntPtr rgbOutputFileContents, out int pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            SetProject(wszInputFilePath);
            if (bstrInputFileContents == null)
                throw new ArgumentNullException(bstrInputFileContents);
            InputFilePath = wszInputFilePath;
            DefaultNamespace = wszDefaultNamespace;
            GenerateProgress = pGenerateProgress;
            //
            PreGenerateContent(wszInputFilePath, bstrInputFileContents);
            var source = GenerateContent(wszInputFilePath, bstrInputFileContents);
            if (source == null)
            {
                pcbOutput = 0;
                rgbOutputFileContents = IntPtr.Zero;
            }
            else
            {
                pcbOutput = source.Length;
                rgbOutputFileContents = Marshal.AllocCoTaskMem(pcbOutput);
                Marshal.Copy(source, 0, rgbOutputFileContents, pcbOutput);
            }
        }

        /// <summary>
        /// Pres the content of the generate.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        protected virtual void PreGenerateContent(string inputFilePath, string inputFileContents) { }
        /// <summary>
        /// Generates the content.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected abstract byte[] GenerateContent(string inputFilePath, string inputFileContents);

        /// <summary>
        /// Errors the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        protected virtual void Error(int line, string message, params object[] args) { Error(line, 0, 1, message, args); }
        /// <summary>
        /// Errors the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        protected virtual void Error(int line, int column, int level, string message, params object[] args)
        {
            var progress = GenerateProgress;
            if (progress != null)
                progress.GeneratorError(false, level, string.Format(message, args), line, column);
        }

        /// <summary>
        /// Warnings the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        protected virtual void Warning(int line, string message, params object[] args) { Warning(line, 0, 1, message, args); }
        /// <summary>
        /// Warnings the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        protected virtual void Warning(int line, int column, int level, string message, params object[] args)
        {
            var progress = GenerateProgress;
            if (progress != null)
                progress.GeneratorError(true, level, string.Format(message, args), line, column);
        }

        /// <summary>
        /// Gets the default extension.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDefaultExtension();

        /// <summary>
        /// Streams to bytes.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        protected byte[] StreamToBytes(Stream s)
        {
            if (s.Length == 0L)
                return new byte[0];
            var position = s.Position;
            s.Position = 0L;
            var b = new byte[(int)s.Length];
            s.Read(b, 0, b.Length);
            s.Position = position;
            return b;
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        protected Project Project { get; private set; }

        /// <summary>
        /// Gets or sets the generate progress.
        /// </summary>
        /// <value>
        /// The generate progress.
        /// </value>
        protected IVsGeneratorProgress GenerateProgress { get; set; }

        /// <summary>
        /// Gets or sets the input file path.
        /// </summary>
        /// <value>
        /// The input file path.
        /// </value>
        protected string InputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the default namespace.
        /// </summary>
        /// <value>
        /// The default namespace.
        /// </value>
        protected string DefaultNamespace { get; set; }

        /// <summary>
        /// Creates the exception message.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        protected virtual string CreateExceptionMessage(Exception e)
        {
            var str = (e.Message ?? string.Empty);
            for (var exception = e.InnerException; exception != null; exception = exception.InnerException)
            {
                var message = exception.Message;
                if (!string.IsNullOrEmpty(message))
                    str = str + " " + message;
            }
            return str;
        }
    }
}
