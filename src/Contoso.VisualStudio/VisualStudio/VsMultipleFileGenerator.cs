using System;
using System.Collections.Generic;
using EnvDTE;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Designer.Interfaces;

namespace Contoso.VisualStudio
{
    /// <summary>
    /// VsMultipleFileGenerator
    /// </summary>
    public abstract class VsMultipleFileGenerator : VsSingleFileGenerator, IEnumerable, IVsSingleFileGenerator
    {
        private List<string> _newFileNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VsMultipleFileGenerator"/> class.
        /// </summary>
        protected VsMultipleFileGenerator() { }

        /// <summary>
        /// Pres the content of the generate.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        protected override void PreGenerateContent(string inputFilePath, string inputFileContents)
        {
            _newFileNames.Clear();
            var vsProject = VsHelper.ToVsProject(Project);
            int iFound;
            uint itemId;
            vsProject.IsDocumentInProject(inputFilePath, out iFound, new VSDOCUMENTPRIORITY[1], out itemId);
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
                    var inputFileName = GetFileName(i);
                    _newFileNames.Add(inputFileName);
                    var path = Path.Combine(inputFilePath.Substring(0, inputFilePath.LastIndexOf(Path.DirectorySeparatorChar)), inputFileName);
                    var inputFileContent = string.Empty;
                    if (File.Exists(path))
                        try { inputFileContent = File.ReadAllText(path); }
                        catch (Exception) { inputFileContent = string.Empty; }
                    var s = File.Create(path);
                    try
                    {
                        var data = GenerateChildContent(path, inputFileContent);
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
        /// Generates the content of the child.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="inputFileContents">The input file contents.</param>
        /// <returns></returns>
        protected abstract byte[] GenerateChildContent(string inputFilePath, string inputFileContents);
    }
}
