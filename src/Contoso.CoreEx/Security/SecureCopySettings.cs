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
namespace Contoso.Security
{
    /// <summary>
    /// SecureCopySettings
    /// </summary>
    public class SecureCopySettings : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureCopySettings"/> class.
        /// </summary>
        public SecureCopySettings()
        {
            ProcessTimeoutInMilliseconds = 6000;
            ProcessPutTimeoutInMilliseconds = 360000;
            PuTtyPath = ".";
        }

        /// <summary>
        /// Gets or sets the process timeout in milliseconds.
        /// </summary>
        /// <value>
        /// The process timeout in milliseconds.
        /// </value>
        public int ProcessTimeoutInMilliseconds { get; set; }
        /// <summary>
        /// Gets or sets the process put timeout in milliseconds.
        /// </summary>
        /// <value>
        /// The process put timeout in milliseconds.
        /// </value>
        public int ProcessPutTimeoutInMilliseconds { get; set; }
        /// <summary>
        /// Gets or sets the pu tty path.
        /// </summary>
        /// <value>
        /// The pu tty path.
        /// </value>
        public string PuTtyPath { get; set; }
        /// <summary>
        /// Gets or sets the name of the session.
        /// </summary>
        /// <value>
        /// The name of the session.
        /// </value>
        public string SessionName { get; set; }
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int? Port { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the private key path.
        /// </summary>
        /// <value>
        /// The private key path.
        /// </value>
        public string PrivateKeyPath { get; set; }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public SecureCopySettingsOptions Options { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}