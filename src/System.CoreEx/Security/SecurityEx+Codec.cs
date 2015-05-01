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
using System.Configuration;
using System.Net;
namespace System.Security
{
    public partial class SecurityEx
    {
        /// <summary>
        /// StringCodec
        /// </summary>
        public static readonly ICodec<string, string> StringCodec = new CodecString();
        /// <summary>
        /// BytesCodec
        /// </summary>
        public static readonly ICodec<byte[], byte[]> BytesCodec = new CodecBytes();
        /// <summary>
        /// CredentialCodec
        /// </summary>
        public static readonly ICodec<NetworkCredential, string> CredentialCodec = new CodecCredential();

        internal class CodecString : ICodec<string, string>
        {
            #region ICodec

            /// <summary>
            /// Decodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            /// <exception cref="System.InvalidOperationException">Unable to read credential store</exception>
            string ICodec<string, string>.Decode(string data, object tag) { return SecurityEx.SymmetricDecrypt(data); }

            /// <summary>
            /// Encodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException"></exception>
            string ICodec<string, string>.Encode(string data, object tag) { return SecurityEx.SymmetricEncrypt(data); }

            #endregion
        }

        internal class CodecBytes : ICodec<byte[], byte[]>
        {
            #region ICodec

            /// <summary>
            /// Decodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            byte[] ICodec<byte[], byte[]>.Decode(byte[] data, object tag) { return SecurityEx.SymmetricDecrypt(data); }

            /// <summary>
            /// Encodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            byte[] ICodec<byte[], byte[]>.Encode(byte[] data, object tag) { return SecurityEx.SymmetricEncrypt(data); }

            #endregion
        }

        internal class CodecCredential : ICodec<NetworkCredential, string>
        {
            #region ICodec

            /// <summary>
            /// Decodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            /// <exception cref="System.InvalidOperationException">Unable to read credential store</exception>
            NetworkCredential ICodec<NetworkCredential, string>.Decode(string data, object tag)
            {
                if (string.IsNullOrEmpty(data))
                    return null;
                CredentialManagerEx.Credential credential;
                if (CredentialManagerEx.Read(data, CredentialManagerEx.CredentialType.GENERIC, out credential) != 0)
                    throw new InvalidOperationException("Unable to read credential store");
                return new NetworkCredential { UserName = credential.UserName, Password = credential.CredentialBlob };
            }

            /// <summary>
            /// Encodes the specified tag.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="tag">The tag.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException"></exception>
            NetworkCredential ICodec<NetworkCredential, string>.Encode(string data, object tag)
            {
                throw new NotSupportedException();
            }

            #endregion
        }
    }
}