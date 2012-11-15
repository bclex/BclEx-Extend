//#region License
// /*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography;
//namespace System.Security
//{
//    /// <summary>
//    /// SecurityEx
//    /// </summary>
//    public static class SecurityEx
//    {
//        public static X509CertificateValidator CreateCertificateValidator(X509CertificateValidationMode certificateValidationMode, X509RevocationMode revocationMode, StoreLocation trustedStoreLocation)
//        {
//            return new X509CertificateValidatorEx(certificateValidationMode, revocationMode, trustedStoreLocation);
//        }

//        public static RSA EnsureAndGetPrivateRSAKey(X509Certificate2 certificate)
//        {
//            AsymmetricAlgorithm privateKey;
//            if (!certificate.HasPrivateKey)
//                throw new ArgumentException(SR.GetString("ID1001", new object[] { certificate.Thumbprint }));
//            try
//            {
//                privateKey = certificate.PrivateKey;
//            }
//            catch (CryptographicException exception) { throw new ArgumentException(SR.GetString("ID1039", new object[] { certificate.Thumbprint }), exception); }
//            var rsa = (privateKey as RSA);
//            if (rsa == null)
//                throw new ArgumentException(SR.GetString("ID1002", new object[] { certificate.Thumbprint }));
//            return rsa;
//        }

//        public static string GetCertificateId(X509Certificate2 certificate)
//        {
//            if (certificate == null)
//                throw new ArgumentNullException("certificate");
//            string name = certificate.SubjectName.Name;
//            if (string.IsNullOrEmpty(name))
//                name = certificate.Thumbprint;
//            return name;
//        }

//        public static string GetCertificateIssuerName(X509Certificate2 certificate, IssuerNameRegistry issuerNameRegistry)
//        {
//            int num;
//            if (certificate == null)
//            {
//                throw DiagnosticUtil.ExceptionUtil.ThrowHelperArgumentNull("certificate");
//            }
//            if (issuerNameRegistry == null)
//            {
//                throw DiagnosticUtil.ExceptionUtil.ThrowHelperArgumentNull("issuerNameRegistry");
//            }
//            X509Chain chain = new X509Chain
//            {
//                ChainPolicy = { RevocationMode = X509RevocationMode.NoCheck }
//            };
//            chain.Build(certificate);
//            X509ChainElementCollection chainElements = chain.ChainElements;
//            string issuerName = null;
//            if (chainElements.Count > 1)
//            {
//                using (X509SecurityToken token = new X509SecurityToken(chainElements[1].Certificate))
//                {
//                    issuerName = issuerNameRegistry.GetIssuerName(token);
//                    goto Label_008D;
//                }
//            }
//            using (X509SecurityToken token2 = new X509SecurityToken(certificate))
//            {
//                issuerName = issuerNameRegistry.GetIssuerName(token2);
//            }
//        Label_008D:
//            num = 1;
//            while (num < chainElements.Count)
//            {
//                chainElements[num].Certificate.Reset();
//                num++;
//            }
//            return issuerName;
//        }

//        //public static bool TryResolveCertificate(CertificateReferenceElement element, out X509Certificate2 certificate) { return TryResolveCertificate(element.StoreName, element.StoreLocation, element.X509FindType, element.FindValue, out certificate); }
//        public static bool TryResolveCertificate(StoreName storeName, StoreLocation storeLocation, X509FindType findType, object findValue, out X509Certificate2 certificate)
//        {
//            X509Store store = new X509Store(storeName, storeLocation);
//            store.Open(OpenFlags.ReadOnly);
//            certificate = null;
//            X509Certificate2Collection certificates = null;
//            X509Certificate2Collection certificates2 = null;
//            try
//            {
//                certificates = store.Certificates;
//                certificates2 = certificates.Find(findType, findValue, false);
//                if (certificates2.Count == 1)
//                {
//                    certificate = new X509Certificate2(certificates2[0]);
//                    return true;
//                }
//            }
//            finally
//            {
//                CryptoUtil.ResetAllCertificates(certificates2);
//                CryptoUtil.ResetAllCertificates(certificates);
//                store.Close();
//            }
//            return false;
//        }
//    }
//}
