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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace System.IO
{
    /// <summary>
    /// IOExtensions
    /// </summary>
    public static class IOExtensions
    {
        /// <summary>
        /// Reads the text stream provide and returns the results as a string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static string ReadAsTextStream(this Stream stream) { return ReadAsTextStream(stream, Encoding.UTF8); }
        /// <summary>
        /// Reads the text stream provided and returns the result in a StringBuilder.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="b">The b.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static StringBuilder ReadAsTextStream(this Stream stream, StringBuilder b, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            // rewind the stream to the beginning
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            using (var r = new StreamReader(stream, encoding))
            {
                char[] buffer = new char[8192];
                int byteReadCount;
                while ((byteReadCount = r.Read(buffer, 0, buffer.Length)) > 0)
                    b.Append(buffer, 0, byteReadCount);
            }
            return b;
        }
        /// <summary>
        /// Reads the text stream provided and returns the result in a string using the encoding specified.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string ReadAsTextStream(this Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            // rewind the stream to the beginning
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            using (var streamReader = new StreamReader(stream, encoding))
                return streamReader.ReadToEnd();
        }

        /// <summary>
        /// To the base64.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string ToBase64(this Stream s)
        {
            using (var ms = new MemoryStream())
            using (var cryptStream = new CryptoStream(ms, new ToBase64Transform(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cryptStream))
            {
                sw.Write(s);
                sw.Flush();
                return ms.ReadAsTextStream();
            }
        }

        ///// <summary>
        ///// Froms the base64.
        ///// </summary>
        ///// <param name="s">The s.</param>
        ///// <returns></returns>
        //public static byte[] FromBase64(this string s)
        //{
        //    using (var ms = new MemoryStream(s))
        //    using (var cryptStream = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Read))
        //    using (var sr = new StreamReader(cryptStream))
        //        return sr.ReadToEnd();
        //}


#if EXPERIMENTAL
        /*
        private FileStream GetStream(string fileName, FileAccess fileAccess, int numberOfTries, int timeIntervalBetweenTries)
        {
            var tries = 0;
            while (true)
            {
                try
                {
                    return File.Open(fileName, FileMode.Open, fileAccess, FileShare.None);
                }
                catch (IOException e)
                {
                    if (!IsFileLocked(e))
                        throw;
                    if (++tries > numberOfTries)
                        throw new InvalidOperationException("The file is locked too long: " + e.Message, e);
                    Thread.Sleep(timeIntervalBetweenTries);
                }
            }
        }

        private static bool IsFileLocked(IOException exception)
        {
            int errorCode = (Marshal.GetHRForException(exception) & ((1 << 16) - 1));
            return (errorCode == 32) || (errorCode == 33);
        }
        */

        #region Serialize
        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="text">The text.</param>
        public static void ReadObject(object value, string contentType, string text)
        {
            ReadObject(value, contentType, new StringReader(text));
        }
        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="textReader">The text reader.</param>
        public static void ReadObject(object value, string contentType, TextReader textReader)
        {
            switch (contentType)
            {
                case "text/xml":
                    var xmlValue = (System.Xml.Serialization.IXmlSerializable)value;
                    using (var r = XmlReader.Create(textReader))
                        xmlValue.ReadXml(r);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="textBuilder">The text builder.</param>
        public static void WriteObject(object value, string contentType, StringBuilder textBuilder)
        {
            WriteObject(value, contentType, new StringWriter(textBuilder));
        }
        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="textWriter">The text writer.</param>
        public static void WriteObject(object value, string contentType, TextWriter textWriter)
        {
            switch (contentType)
            {
                case "text/xml":
                    var xmlValue = (System.Xml.Serialization.IXmlSerializable)value;
                    using (var w = XmlWriter.Create(textWriter))
                        xmlValue.WriteXml(w);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion
#endif
    }
}