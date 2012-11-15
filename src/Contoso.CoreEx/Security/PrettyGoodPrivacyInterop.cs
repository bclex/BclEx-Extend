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
using System.IO;
using System.Diagnostics;
using System;
namespace Contoso.Security
{
    /// <summary>
    /// PrettyGoodPrivacyInterop
    /// </summary>
    public class PrettyGoodPrivacyInterop
    {
        private const string DecryptArgumentsXAB = "{0} --always-trust --output \"{1}\" --decrypt \"{2}\"";
        private const string EncryptArgumentsXABC = "{0} --always-trust --recipient \"{1}\" --output \"{2}\" --encrypt \"{3}\"";
        private const string ImportArgumentsXA = "{0} --import \"{1}\"";

        /// <summary>
        /// Encrypts the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="outputFilePath">The output file path.</param>
        public static void Encrypt(PrettyGoodPrivacySettings settings, string recipient, string inputFilePath, string outputFilePath)
        {
            string executablePath;
            var arguments = string.Format(EncryptArgumentsXABC, Get(settings, out executablePath), recipient, outputFilePath, inputFilePath);
            var process = Process.Start(new ProcessStartInfo(executablePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments,
            });
            if (process != null)
                process.WaitForExit(60000);
        }

        /// <summary>
        /// Decrypts the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="outputFilePath">The output file path.</param>
        public void Decrypt(PrettyGoodPrivacySettings settings, string inputFilePath, string outputFilePath)
        {
            string executablePath;
            string arguments = string.Format(DecryptArgumentsXAB, Get(settings, out executablePath), outputFilePath, inputFilePath);
            var process = Process.Start(new ProcessStartInfo(executablePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments,
            });
            if (process != null)
                process.WaitForExit(60000);
        }

        /// <summary>
        /// Imports the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="keyFilePath">The key file path.</param>
        public void Import(PrettyGoodPrivacySettings settings, string keyFilePath)
        {
            string executablePath;
            string arguments = string.Format(ImportArgumentsXA, Get(settings, out executablePath), keyFilePath);
            var process = Process.Start(new ProcessStartInfo(executablePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments,
            });
            if (process != null)
                process.WaitForExit(60000);
        }

        private static string Get(PrettyGoodPrivacySettings settings, out string executablePath)
        {
            executablePath = EnsureEndsWith(settings.GnuPGPath, "\\") + "gpg.exe";
            if (!File.Exists(executablePath))
                throw new InvalidOperationException(string.Format("'gpg.exe' not found at '{0}'.", executablePath));
            var homeDirectory = EnsureEndsWith(settings.GnuPGPath, "\\") + "store";
            if (!Directory.Exists(homeDirectory))
                Directory.CreateDirectory(homeDirectory);
            return string.Format("--homedir \"{0}\" --passphrase \"{1}\" --yes ", homeDirectory, settings.Passphase);
        }

        private static string EnsureEndsWith(string text, string suffix)
        {
            return ((!string.IsNullOrEmpty(text)) && (!text.EndsWith(suffix)) ? text + suffix : text);
        }
    }
}