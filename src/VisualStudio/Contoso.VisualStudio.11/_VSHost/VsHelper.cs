using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.VisualStudio.TextTemplating.VSHost
{
    internal static class VsHelper
    {
        public static IVsHierarchy ToHierarchy(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            string projectGuid = null;
            // DTE does not expose the project GUID that exists at in the msbuild project file.
            // Cannot use MSBuild object model because it uses a static instance of the Engine, and using the Project will cause it to be unloaded from the engine when the GC collects the variable that we declare.       
            using (var projectReader = XmlReader.Create(project.FileName))
            {
                projectReader.MoveToContent();
                var nodeName = projectReader.NameTable.Add("ProjectGuid");
                while (projectReader.Read())
                    if (object.Equals(projectReader.LocalName, nodeName))
                    {
                        projectGuid = (string)projectReader.ReadElementContentAsString();
                        break;
                    }
            }
            Debug.Assert(!string.IsNullOrEmpty(projectGuid));
            var serviceProvider = new ServiceProvider(project.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            return VsShellUtilities.GetHierarchy(serviceProvider, new Guid(projectGuid));
        }

        public static IVsProject ToVsProject()
        {
            var dte = (Package.GetGlobalService(typeof(SDTE)) as DTE);
            var projects = (dte.ActiveSolutionProjects as Array);
            if (projects == null || projects.Length == 0)
                throw new NullReferenceException();
            return ToVsProject((Project)projects.GetValue(0));
        }

        public static IVsProject ToVsProject(EnvDTE.Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            var vsProject = (ToHierarchy(project) as IVsProject);
            if (vsProject == null)
                throw new ArgumentException("Project is not a VS project.");
            return vsProject;
        }
    }
}
