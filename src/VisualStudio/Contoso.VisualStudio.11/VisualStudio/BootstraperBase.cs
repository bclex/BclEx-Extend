using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Contoso.VisualStudio
{
    // register : regasm /codebase [AssemblyFullPath].dll
    // unregister : regasm /codebase [AssemblyFullPath].dll /u
    /// <summary>
    /// BootstraperBase
    /// </summary>
    public abstract class BootstraperBase
    {
        /// <summary>
        /// CSharpCategory
        /// </summary>
        protected static readonly Guid CSharpCategory = new Guid("{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}");
        /// <summary>
        /// VBCategory
        /// </summary>
        protected static readonly Guid VBCategory = new Guid("{164B10B9-B200-11D0-8C61-00A0C91E29D5}");
        private const string KeyFormat = @"SOFTWARE\Microsoft\VisualStudio\{0}\Generators\{1}\{2}";

        /// <summary>
        /// Registers the specified vs version.
        /// </summary>
        /// <param name="vsVersion">The vs version.</param>
        /// <param name="customToolGuid">The custom tool GUID.</param>
        /// <param name="customToolName">Name of the custom tool.</param>
        /// <param name="customToolDescription">The custom tool description.</param>
        /// <param name="categoryGuids">The category guids.</param>
        protected static void Register(Version vsVersion, Guid customToolGuid, string customToolName, string customToolDescription, params Guid[] categoryGuids)
        {
            foreach (var categoryGuid in categoryGuids)
            {
                var subKey = string.Format(KeyFormat, vsVersion, categoryGuid.ToString("B"), customToolName);
                using (var key = Registry.LocalMachine.CreateSubKey(subKey))
                {
                    key.SetValue(string.Empty, customToolDescription);
                    key.SetValue("CLSID", customToolGuid.ToString("B"));
                    key.SetValue("GeneratesDesignTimeSource", 1);
                }
            }
        }

        /// <summary>
        /// Unregisters the specified vs version.
        /// </summary>
        /// <param name="vsVersion">The vs version.</param>
        /// <param name="customToolName">Name of the custom tool.</param>
        /// <param name="categoryGuids">The category guids.</param>
        protected static void Unregister(Version vsVersion, string customToolName, params Guid[] categoryGuids)
        {
            foreach (var categoryGuid in categoryGuids)
            {
                var subKey = string.Format(KeyFormat, vsVersion, categoryGuid.ToString("B"), customToolName);
                Registry.LocalMachine.DeleteSubKey(subKey, false);
            }
        }
    }
}
