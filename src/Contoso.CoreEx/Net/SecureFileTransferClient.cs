#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Net;
using System.IO;
using System;
using Contoso.Security;
using System.Collections.Generic;
namespace Contoso.Net
{
    /// <summary>
    /// SecureFileTransferClient
    /// </summary>
    public class SecureFileTransferClient : IFileTransferClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureFileTransferClient"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="remoteHost">The remote host.</param>
        /// <param name="userName">Name of the user.</param>
        public SecureFileTransferClient(SecureCopySettings settings, string remoteHost, string userName)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (string.IsNullOrEmpty(remoteHost))
                throw new ArgumentNullException("remoteHost");
            SecureCopySettings = (SecureCopySettings)settings.Clone();
            SecureCopySettings.Options = SecureCopySettingsOptions.ForceSftp;
            Credentials = new NetworkCredential(userName, (string)null);
            RemoteHost = remoteHost;
        }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public NetworkCredential Credentials { get; protected set; }

        /// <summary>
        /// Gets or sets the remote host.
        /// </summary>
        /// <value>
        /// The remote host.
        /// </value>
        public string RemoteHost { get; protected set; }

        /// <summary>
        /// Gets or sets the secure copy settings.
        /// </summary>
        /// <value>
        /// The secure copy settings.
        /// </value>
        public SecureCopySettings SecureCopySettings { get; protected set; }

        /// <summary>
        /// Existses the specified remote file.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <returns></returns>
        public virtual bool FileExists(string remoteFile)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tries the delete.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public virtual bool TryDeleteFile(string remoteFile, out Exception ex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public virtual bool TryGetFile(string remoteFile, string localFile, out Exception ex)
        {
            return SecureCopyInterop.TryGet(SecureCopySettings, RemoteHost, Credentials.UserName, remoteFile, localFile, out ex);
        }

        /// <summary>
        /// Tries the put.
        /// </summary>
        /// <param name="localFile">The local file.</param>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public virtual bool TryPutFile(string localFile, string remoteFile, out Exception ex)
        {
            SecureCopySettings.Options |= SecureCopySettingsOptions.ForceSftp;
            return SecureCopyInterop.TryPut(SecureCopySettings, RemoteHost, Credentials.UserName, new[] { localFile }, remoteFile, out ex);
        }

        /// <summary>
        /// Lists the files in the directory.
        /// </summary>
        /// <param name="remoteDirectory">The remote directory.</param>
        /// <returns></returns>
        public IEnumerable<FtpFile> ListDirectory(string remoteDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
