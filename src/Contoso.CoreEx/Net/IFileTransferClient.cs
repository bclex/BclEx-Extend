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
using System;
using System.Net;
using System.Collections.Generic;
namespace Contoso.Net
{
    /// <summary>
    /// IFileTransferClient
    /// </summary>
    public interface IFileTransferClient
    {
        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        NetworkCredential Credentials { get; }
        /// <summary>
        /// Gets the remote host.
        /// </summary>
        /// <value>
        /// The remote host.
        /// </value>
        string RemoteHost { get; }
        /// <summary>
        /// Existses the specified remote file.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <returns></returns>
        bool FileExists(string remoteFile);
        /// <summary>
        /// Tries the delete.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        bool TryDeleteFile(string remoteFile, out Exception ex);
        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        bool TryGetFile(string remoteFile, string localFile, out Exception ex);
        /// <summary>
        /// Tries the put.
        /// </summary>
        /// <param name="localFile">The local file.</param>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        bool TryPutFile(string localFile, string remoteFile, out Exception ex);

        /// <summary>
        /// Lists the files in the directory.
        /// </summary>
        /// <param name="remoteDirectory">The remote directory.</param>
        /// <returns></returns>
        IEnumerable<FtpFile> ListDirectory(string remoteDirectory);
    }
}
