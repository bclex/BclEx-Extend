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
    /// VsMultipleFileGeneratorWithSite
    /// </summary>
    public abstract class VsMultipleFileGeneratorWithSite : VsMultipleFileGenerator, IObjectWithSite
    {
        private static Guid CodeDomInterfaceGuid = new Guid("{73E59688-C7C4-4a85-AF64-A538754784C5}");
        private object _site;
        private ServiceProvider _serviceProvider;
        private CodeDomProvider _codeDomProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsMultipleFileGeneratorWithSite"/> class.
        /// </summary>
        protected VsMultipleFileGeneratorWithSite() { }

        /// <summary>
        /// Gets the default extension.
        /// </summary>
        /// <returns></returns>
        public override string GetDefaultExtension()
        {
            var fileExtension = CodeProvider.FileExtension;
            if (!string.IsNullOrEmpty(fileExtension) && fileExtension[0] != '.')
                fileExtension = "." + fileExtension;
            return fileExtension;
        }

        #region Site

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceGuid">The service GUID.</param>
        /// <returns></returns>
        protected object GetService(Guid serviceGuid) { return SiteServiceProvider.GetService(serviceGuid); }
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected object GetService(Type serviceType) { return SiteServiceProvider.GetService(serviceType); }

        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="riid">The riid.</param>
        /// <param name="ppvSite">The PPV site.</param>
        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (_site == null)
                throw new COMException("object is not sited", -2147467259);
            var siteAsIUnknown = Marshal.GetIUnknownForObject(_site);
            try
            {
                Marshal.QueryInterface(siteAsIUnknown, ref riid, out ppvSite);
                if (ppvSite == IntPtr.Zero)
                    throw new COMException("object is not interface", -2147467262);
            }
            finally
            {
                if (siteAsIUnknown != IntPtr.Zero)
                {
                    Marshal.Release(siteAsIUnknown);
                    siteAsIUnknown = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Sets the site.
        /// </summary>
        /// <param name="pUnkSite">The p unk site.</param>
        public void SetSite(object pUnkSite)
        {
            _site = pUnkSite;
            _codeDomProvider = null;
            _serviceProvider = null;
        }

        /// <summary>
        /// Gets or sets the code provider.
        /// </summary>
        /// <value>
        /// The code provider.
        /// </value>
        protected virtual CodeDomProvider CodeProvider
        {
            get
            {
                if (_codeDomProvider == null)
                {
                    var service = (IVSMDCodeDomProvider)GetService(CodeDomInterfaceGuid);
                    if (service != null)
                        _codeDomProvider = (CodeDomProvider)service.CodeDomProvider;
                }
                return _codeDomProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _codeDomProvider = value;
            }
        }

        private ServiceProvider SiteServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                    _serviceProvider = new ServiceProvider(_site as IServiceProvider);
                return _serviceProvider;
            }
        }

        #endregion
    }
}
