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
using System.Globalization;
using System.Reflection;
namespace System
{
    /// <summary>
    /// Provides a basic façade pattern that facilitates common numeric-based calculations into a single class.
    /// </summary>
    public static class ConvertEx
    {
        private static readonly byte[] _hexValues = { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };

        /// <summary>
        /// Returns a Hex string represenation of the text specified.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static byte[] FromBase16String(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new byte[] { };
            var bytes = new byte[text.Length >> 1];
            for (int byteIndex = 0, textIndex = 0; byteIndex < bytes.Length; byteIndex++, textIndex += 2)
                bytes[byteIndex] = byte.Parse(text.Substring(textIndex, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return bytes;
        }

        /// <summary>
        /// Returns a Hex string of the byte array provided.
        /// </summary>
        /// <param name="data">The array.</param>
        /// <returns></returns>
        public static string ToBase16String(byte[] data)
        {
            return (data != null && data.Length != 0 ? BitConverter.ToString(data).Replace("-", string.Empty).ToUpper() : string.Empty);
        }

        #region ToStrings

        /// <summary>
        /// Creates the text array based on the object array provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static string[] ToStrings<T>(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            return ToStringsDelegateFactory<T>.Builder(array);
        }

        private static class ToStringsDelegateFactory<T>
        {
            internal static readonly Func<T[], string[]> Builder = Create(typeof(T));

            static ToStringsDelegateFactory() { }

            private static Func<T[], string[]> Create(Type type)
            {
                if (type == CoreExtensions.ObjectType)
                    return (Func<T[], string[]>)Delegate.CreateDelegate(typeof(Func<T[], string[]>), typeof(ToStringsDelegateFactory<T>).GetMethod("ReferenceField_Object", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { CoreExtensions.ObjectType }, null));
                return new Func<T[], string[]>(GenericField_T);
            }

            private static string[] ReferenceField_Object(object[] array)
            {
                var newArray = new string[array.Length];
                array.CopyTo(newArray, 0);
                return newArray;
            }

            private static string[] GenericField_T(T[] array)
            {
                var newArray = new string[array.Length];
                for (var index = 0; index < newArray.Length; index++)
                    newArray[index] = array[index].ToString();
                return newArray;
            }
        }

        #endregion

        //public static string ToMd5Hash(string val)
        //{
        //    if (val == null) throw new ArgumentNullException("val");
        //    byte[] data = Encoding.ASCII.GetBytes(val);
        //    data = MD5.Create().ComputeHash(data);
        //    string ret = "";
        //    for (int i = 0; i < data.Length; i++)
        //        ret += data[i].ToString("x2").ToLower();
        //    return ret;
        //}
    }
}
