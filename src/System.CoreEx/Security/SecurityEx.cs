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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Security.Principal;
namespace System.Security
{
    /// <summary>
    /// SecurityEx
    /// </summary>
    public static class SecurityEx
    {
        //private static readonly byte[] _genericIv = ConvertEx.FromBase16String("C9DCF37AED8574A1441FD82DB743765C");

        /// <summary>
        /// Impersonates the windows user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static WindowsImpersonationContext ImpersonateWindowsUser(string userName, string domain, string password)
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;
            try
            {
                if (CoreExtensions.RevertToSelf())
                    if (CoreExtensions.LogonUserA(userName, domain, password, CoreExtensions.LOGON32_LOGON_INTERACTIVE, CoreExtensions.LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                        if (CoreExtensions.DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            var impersonationContext = tempWindowsIdentity.Impersonate();
                            if (impersonationContext != null)
                            {
                                CoreExtensions.CloseHandle(token);
                                CoreExtensions.CloseHandle(tokenDuplicate);
                                return impersonationContext;
                            }
                        }
            }
            finally
            {
                if (token != IntPtr.Zero)
                    CoreExtensions.CloseHandle(token);
                if (tokenDuplicate != IntPtr.Zero)
                    CoreExtensions.CloseHandle(tokenDuplicate);
            }
            throw new Exception("Unable to login.");
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] ComputeHash(byte[] data) { using (var algorithm2 = new SHA1Managed()) return ComputeHash(algorithm2, data); }
        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string algorithm, byte[] data) { using (var algorithm2 = (HashAlgorithm)CryptoConfig.CreateFromName(algorithm)) return ComputeHash(algorithm2, data); }
        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithm algorithm, byte[] data)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            return algorithm.ComputeHash(data);
        }

        /// <summary>
        /// Symmetrics the transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="transformer">The transformer.</param>
        /// <returns></returns>
        public static byte[] SymmetricTransform(byte[] data, ICryptoTransform transformer)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (transformer == null)
                throw new ArgumentNullException("transformer");
            using (var ms = new MemoryStream())
            {
                using (var s = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
                    s.Write(data, 0, data.Length);
                return ms.ToArray();
            }
        }

        #region Encrypt

        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(SymmetricAlgorithm algorithm, string data) { return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data))); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            using (var encryptor = algorithm.CreateEncryptor())
                return SymmetricTransform(data, encryptor);
        }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(SymmetricAlgorithm algorithm, string data, int saltSize) { return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), MakeRandomBytes(saltSize))); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="salt">The salt.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(SymmetricAlgorithm algorithm, string data, byte[] salt) { return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), salt)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data, int saltSize) { return SymmetricEncrypt(algorithm, data, MakeRandomBytes(saltSize)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="salt">The salt.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data, byte[] salt)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (salt == null)
                throw new ArgumentOutOfRangeException("salt");
            if (data == null)
                throw new ArgumentNullException("data");
            using (var encryptor = algorithm.CreateEncryptor())
                return SymmetricEncryptTransform(data, salt, encryptor);
        }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(string data, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), key, iv)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(SymmetricAlgorithm algorithm, string data, byte[] key, byte[] iv) { return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), key, iv)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(byte[] data, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return SymmetricEncrypt(algorithm, data, key, iv); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data, byte[] key, byte[] iv)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            if (key == null)
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            using (var encryptor = algorithm.CreateEncryptor(key, iv))
                return SymmetricTransform(data, encryptor);
        }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(string data, int saltSize, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), saltSize, key, iv)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricEncrypt(SymmetricAlgorithm algorithm, string data, int saltSize, byte[] key, byte[] iv) { return Convert.ToBase64String(SymmetricEncrypt(algorithm, Encoding.UTF8.GetBytes(data), saltSize, key, iv)); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(byte[] data, int saltSize, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return SymmetricEncrypt(algorithm, data, saltSize, key, iv); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data, int saltSize, byte[] key, byte[] iv) { return SymmetricEncrypt(algorithm, data, MakeRandomBytes(saltSize), key, iv); }
        /// <summary>
        /// Symmetrics the encrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(SymmetricAlgorithm algorithm, byte[] data, byte[] salt, byte[] key, byte[] iv)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (salt == null)
                throw new ArgumentOutOfRangeException("salt");
            if (data == null)
                throw new ArgumentNullException("data");
            if (key == null)
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            using (var encryptor = algorithm.CreateEncryptor(key, iv))
                return SymmetricEncryptTransform(data, salt, encryptor);
        }

        /// <summary>
        /// Symmetrics the encrypt transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="transformer">The transformer.</param>
        /// <returns></returns>
        public static byte[] SymmetricEncryptTransform(byte[] data, byte[] salt, ICryptoTransform transformer)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (salt == null)
                throw new ArgumentOutOfRangeException("salt");
            if (transformer == null)
                throw new ArgumentNullException("transformer");
            using (var ms = new MemoryStream())
            {
                using (var s = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
                {
                    byte[] data2;
                    if (salt.Length > 0)
                    {
                        data2 = new byte[salt.Length + data.Length];
                        Buffer.BlockCopy(salt, 0, data2, 0, salt.Length);
                        Buffer.BlockCopy(data, 0, data2, salt.Length, data.Length);
                    }
                    else
                        data2 = data;
                    s.Write(data2, 0, data2.Length);
                }
                return ms.ToArray();
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(SymmetricAlgorithm algorithm, string data) { return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data))); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(SymmetricAlgorithm algorithm, byte[] data)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            using (var decryptor = algorithm.CreateDecryptor())
                return SymmetricTransform(data, decryptor);
        }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(SymmetricAlgorithm algorithm, string data, int saltSize) { return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data), saltSize)); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(SymmetricAlgorithm algorithm, byte[] data, int saltSize)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            if (saltSize < 0)
                throw new ArgumentOutOfRangeException("saltSize");
            using (var decryptor = algorithm.CreateDecryptor())
                return SymmetricDecryptTransform(data, saltSize, decryptor);
        }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(string data, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data), key, iv)); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(SymmetricAlgorithm algorithm, string data, byte[] key, byte[] iv) { return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data), key, iv)); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(byte[] data, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return SymmetricDecrypt(algorithm, data, key, iv); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(SymmetricAlgorithm algorithm, byte[] data, byte[] key, byte[] iv)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            if (key == null)
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            using (var decryptor = algorithm.CreateDecryptor(key, iv))
                return SymmetricTransform(data, decryptor);
        }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(string data, int saltSize, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data), saltSize, key, iv)); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string SymmetricDecrypt(SymmetricAlgorithm algorithm, string data, int saltSize, byte[] key, byte[] iv) { return Encoding.UTF8.GetString(SymmetricDecrypt(algorithm, Convert.FromBase64String(data), saltSize, key, iv)); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(byte[] data, int saltSize, byte[] key, byte[] iv) { using (var algorithm = SymmetricAlgorithm.Create()) return SymmetricDecrypt(algorithm, data, saltSize, key, iv); }
        /// <summary>
        /// Symmetrics the decrypt.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecrypt(SymmetricAlgorithm algorithm, byte[] data, int saltSize, byte[] key, byte[] iv)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            if (data == null)
                throw new ArgumentNullException("data");
            if (saltSize < 0)
                throw new ArgumentOutOfRangeException("saltSize");
            if (key == null)
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            using (var decryptor = algorithm.CreateDecryptor(key, iv))
                return SymmetricDecryptTransform(data, saltSize, decryptor);
        }

        /// <summary>
        /// Symmetrics the decrypt transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="saltSize">Size of the salt.</param>
        /// <param name="transformer">The transformer.</param>
        /// <returns></returns>
        public static byte[] SymmetricDecryptTransform(byte[] data, int saltSize, ICryptoTransform transformer)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (saltSize < 0)
                throw new ArgumentOutOfRangeException("saltSize");
            if (transformer == null)
                throw new ArgumentNullException("transformer");
            byte[] transformedData;
            using (var ms = new MemoryStream())
            {
                using (var s = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
                    s.Write(data, 0, data.Length);
                transformedData = ms.ToArray();
            }
            if (saltSize > 0)
            {
                var data2 = new byte[transformedData.Length - saltSize];
                Buffer.BlockCopy(transformedData, saltSize, data2, 0, data2.Length);
                transformedData = data2;
            }
            return transformedData;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Makes the random bytes.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static byte[] MakeRandomBytes(int length)
        {
            var bytes = new byte[length];
            RNGCryptoServiceProvider.Create().GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Generates the symmetric key.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSymmetricKey() { using (var algorithm = SymmetricAlgorithm.Create()) return GenerateSymmetricKey(algorithm); }
        /// <summary>
        /// Generates the symmetric key.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <returns></returns>
        public static byte[] GenerateSymmetricKey(SymmetricAlgorithm algorithm)
        {
            algorithm.GenerateKey();
            return algorithm.Key;
        }

        /// <summary>
        /// Generates the symmetric iv.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSymmetricIv() { using (var algorithm = SymmetricAlgorithm.Create()) return GenerateSymmetricIv(algorithm); }
        /// <summary>
        /// Generates the symmetric iv.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <returns></returns>
        public static byte[] GenerateSymmetricIv(SymmetricAlgorithm algorithm)
        {
            algorithm.GenerateIV();
            return algorithm.IV;
        }

        #endregion
    }
}
