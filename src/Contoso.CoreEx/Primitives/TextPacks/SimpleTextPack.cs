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
using System.Text;
using System.Collections.Generic;
namespace Contoso.Primitives.TextPacks
{
    /// <summary>
    /// A subclass of TextPackBase used to provide a basic encoding and decoding function-set.
    /// </summary>
    public class SimpleTextPack : ITextPack
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetValue(string pack, string key)
        {
            pack += "\x01";
            string packKey = "\x01" + key + "=";
            int packIndex = pack.IndexOf(packKey, StringComparison.OrdinalIgnoreCase);
            if (packIndex > -1)
            {
                packIndex += packKey.Length;
                return pack.Substring(packIndex, pack.IndexOf("\x01", packIndex) - packIndex).Replace("\xDE\xDE", "\x01");
            }
            return string.Empty;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SetValue(string pack, string key, string value)
        {
            if (value != null)
                value = value.Replace("\x01", "\xDE\xDE");
            pack += "\x01";
            string packKey = "\x01" + key + "=";
            int packIndex = pack.IndexOf(packKey, StringComparison.OrdinalIgnoreCase);
            if (packIndex > -1)
                pack = pack.Replace(pack.Substring(packIndex, pack.IndexOf("\x01", packIndex + 1) - packIndex), string.Empty);
            return pack + key + "=" + value;
        }

        /// <summary>
        /// Provides the ability to decode the contents of the pack provided into the hash instance provided, based on the logic
        /// provided by the implementating class. The result is contained in the hash provided.
        /// </summary>
        public static IDictionary<string, string> Decode(string pack, string namespaceID, Predicate<string> predicate)
        {
            var values = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(pack))
                return values;
            if (namespaceID == null)
                namespaceID = string.Empty;
            string search = (namespaceID.Length == 0 ? "\x01" : "\x01" + namespaceID + "::");
            int searchLength = search.Length;
            int packIndex = 0;
            do
            {
                packIndex = pack.IndexOf(search, packIndex, StringComparison.OrdinalIgnoreCase);
                if (packIndex > -1)
                {
                    packIndex += searchLength;
                    int packIndex2 = pack.IndexOf("=", packIndex);
                    int packIndex3 = pack.IndexOf("\x01", packIndex);
                    string key = pack.Substring(packIndex, packIndex2 - packIndex);
                    packIndex = packIndex3;
                    // check validkeyindex and commit
                    if ((predicate == null) || predicate(key))
                        values[key] = (packIndex3 > -1 ? pack.Substring(packIndex2 + 1, packIndex3 - packIndex2 - 1) : pack.Substring(packIndex2 + 1)).Replace("\xDE\xDE", "\x01");
                }
            } while (packIndex > -1);
            return values;
        }

        /// <summary>
        /// Provides the ability to pack the contents in the hash provided into a different string representation based on the logic
        /// provided by the implementating class. Results is contained in the provided StringBuilder instance.
        /// The packing format uses "\x01[key]=[value]\x01".
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="namespaceID">The namespace ID.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static string Encode(IDictionary<string, string> values, string namespaceID, Predicate<string> predicate)
        {
            if (values == null)
                throw new ArgumentNullException("set");
            if (values.Count == 0)
                return string.Empty;
            // check validkeyindex and commit
            var b = new StringBuilder();
            foreach (string key in values.Keys)
                if ((predicate == null) || predicate(key))
                {
                    b.Append("\x01" + key + "=");
                    b.Append(values[key].Replace("\x01", "\xDE\xDE"));
                }
            return b.ToString();
        }

        #region ITextPack

        IDictionary<string, string> ITextPack.Decode(string pack, string namespaceID, Predicate<string> predicate) { return Decode(pack, namespaceID, predicate); }
        string ITextPack.Encode(IDictionary<string, string> values, string namespaceID, Predicate<string> predicate) { return Encode(values, namespaceID, predicate); }

        #endregion
    }
}
