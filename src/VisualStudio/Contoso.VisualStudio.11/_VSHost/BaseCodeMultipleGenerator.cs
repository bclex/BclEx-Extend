using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.TextTemplating.VSHost
{
    /// <summary>
    /// BaseCodeMultipleGenerator
    /// </summary>
    public abstract class BaseCodeMultipleGenerator : BaseCodeGenerator, IEnumerable
    {
        private List<string> _newFileNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCodeMultipleGenerator"/> class.
        /// </summary>
        protected BaseCodeMultipleGenerator() { }

        /// <summary>
        /// Pres the content of the generate.
        /// </summary>
        /// <param name="inputFileName">The input file path.</param>
        /// <param name="inputFileContent">The input file contents.</param>
        protected void PreGenerateContent(IVsProject vsProject, string inputFileName)
        {
            _newFileNames.Clear();
            int iFound;
            uint itemId;
            vsProject.IsDocumentInProject(inputFileName, out iFound, new VSDOCUMENTPRIORITY[1], out itemId);
            if (iFound == 0 || itemId == 0)
                throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");
            IServiceProvider sp;
            vsProject.GetItemContext(itemId, out sp);
            if (sp == null)
                throw new ApplicationException("Unable to retrieve Visual Studio ProjectItem");
            var item = (new ServiceProvider(sp).GetService(typeof(ProjectItem)) as ProjectItem);
            foreach (string i in this)
            {
                try
                {
                    var inputFileName2 = GetFileName(i);
                    _newFileNames.Add(inputFileName2);
                    var path = Path.Combine(inputFileName.Substring(0, inputFileName.LastIndexOf(Path.DirectorySeparatorChar)), inputFileName2);
                    var inputFileContent2 = string.Empty;
                    if (File.Exists(path))
                        try { inputFileContent2 = File.ReadAllText(path); }
                        catch (Exception) { inputFileContent2 = string.Empty; }
                    var s = File.Create(path);
                    try
                    {
                        var data = GenerateChildCode(path, inputFileContent2);
                        s.Write(data, 0, data.Length);
                        s.Close();
                        item.ProjectItems.AddFromFile(path);
                    }
                    catch (Exception)
                    {
                        s.Close();
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            foreach (ProjectItem childItem in item.ProjectItems)
            {
                if (!(childItem.Name.EndsWith(GetDefaultExtension()) || _newFileNames.Contains(childItem.Name)))
                    childItem.Delete();
            }
        }

        /// <summary>
        /// the method that does the actual work of generating code given the input
        /// file.
        /// </summary>
        /// <param name="inputFileName">input file name</param>
        /// <param name="inputFileContent">file contents as a string</param>
        /// <returns>
        /// the generated code file as a byte-array
        /// </returns>
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            PreGenerateContent(VsHelper.ToVsProject(), inputFileName);
            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected virtual string GetFileName(string element)
        {
            if (string.IsNullOrEmpty(element))
                throw new ArgumentNullException("element");
            return element.Replace("*", Path.GetFileNameWithoutExtension(InputFilePath));
        }

        /// <summary>
        /// Generates the code of the child.
        /// </summary>
        /// <param name="inputFileName">The input file path.</param>
        /// <param name="inputFileContent">The input file contents.</param>
        /// <returns></returns>
        protected abstract byte[] GenerateChildCode(string inputFileName, string inputFileContent);
    }
}
