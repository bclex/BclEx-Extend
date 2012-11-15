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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Contoso.Primitives.TextPacks
{
    /// <summary>
    /// <root key="value">
    ///    <sub key2="value2">text</sub>
    /// </root>
    /// Key = value {attrib on root}
    /// Sub:: = text {node in root}
    /// Sub::Key2 = value2 {attrib on node}
    /// </summary>
    public class XmlTextPack : ITextPack
    {
        /// <summary>
        /// Delimiter
        /// </summary>
        public static string Delimiter = "::";

        #region Private

        ///// <summary>
        ///// Context
        ///// </summary>
        //private class Context
        //{
        //    public string Key;
        //    public string ScopeKey;
        //    public XmlElement XmlElement;
        //    public XmlDocument XmlDocument;

        //    public static Context GetContext(string pack, string contextKey, ref object context)
        //    {
        //        if (contextKey == null)
        //            throw new ArgumentNullException("contextKey");
        //        if (context == null)
        //            context = new Context();
        //        var context2 = (Context)context;
        //        if (context2.Key == contextKey)
        //            return context2;
        //        context2.Key = contextKey;
        //        var xmlDocument = new XmlDocument();
        //        if (pack.Length > 0)
        //            xmlDocument.LoadXml(pack);
        //        context2.XmlDocument = xmlDocument;
        //        return context2;
        //    }
        //}

        private static string EncodeKey(string key)
        {
            return (key.IndexOf(Delimiter) == -1 ? "\x01" + key.Replace(Delimiter, "\x01") + "-" : key.Replace(Delimiter, "\x01") + "_") ;
        }

        private static string DecodeKey(string key)
        {
            return (key.EndsWith("-") ? key.Substring(1, key.Length - 2).Replace("\x01", Delimiter) : key.Substring(0, key.Length - 1).Replace("\x01", Delimiter));
        }

        private static void ParseEncodedKey(string key, out bool isValue, out string scopeKey, out string itemKey)
        {
            int keyLength = key.Length;
            isValue = (key[keyLength - 2] == '\x01');
            if (!isValue)
            {
                int scopeIndex = key.LastIndexOf("\x01", keyLength - 2);
                if (scopeIndex == 0)
                {
                    scopeKey = string.Empty;
                    itemKey = key.Substring(1, keyLength - 2);
                }
                else
                {
                    scopeIndex += 1;
                    scopeKey = key.Substring(0, scopeIndex);
                    itemKey = key.Substring(scopeIndex, keyLength - scopeIndex - 1);
                }
            }
            else
            {
                scopeKey = key.Substring(0, keyLength - 1);
                itemKey = string.Empty;
            }
        }

        private static void ParseKey(string key, out bool isValue, out string scopeKey, out string itemKey)
        {
            if (key.IndexOf(Delimiter) == -1)
                key = Delimiter + key;
            isValue = key.EndsWith(Delimiter);
            if (!isValue)
            {
                int scopeIndex = key.LastIndexOf(Delimiter);
                if (scopeIndex == 0)
                {
                    scopeKey = string.Empty;
                    itemKey = key.Substring(2);
                }
                else
                {
                    scopeIndex += 2;
                    scopeKey = key.Substring(0, scopeIndex);
                    itemKey = key.Substring(scopeIndex);
                }
            }
            else
            {
                scopeKey = key;
                itemKey = string.Empty;
            }
        }

        #endregion

        ///// <summary>
        ///// Gets the value associated with the specified key out of the packed representation provided.
        ///// </summary>
        //public override string GetValue(string pack, string key, string contextKey, ref object context)
        //{
        //    if (string.IsNullOrEmpty(pack))
        //        return string.Empty;
        //    // context
        //    var context2 = Context.GetContext(pack, contextKey, ref context);
        //    var xmlDocument = context2.XmlDocument;
        //    // parse key
        //    bool isValue;
        //    string scopeKey;
        //    string itemKey;
        //    ParseKey(key, out isValue, out scopeKey, out itemKey);
        //    // xmlelement
        //    XmlElement xmlElement;
        //    if (context2.ScopeKey != scopeKey)
        //    {
        //        context2.ScopeKey = scopeKey;
        //        // find element
        //        xmlElement = xmlDocument.DocumentElement;
        //        if (xmlElement == null)
        //        {
        //            context2.XmlElement = null;
        //            return string.Empty;
        //        }
        //        string xpath = "/" + xmlElement.Name;
        //        if (scopeKey.Length > 0)
        //            // singhj: scopeKey doesn't include trailing "::"
        //            xpath += "/" + scopeKey.Replace(Delimiter, "/"); // was: xpath += "/" + scopeKey.Substring(0, scopeKey.Length - 2).Replace(KernelText.Scope, "/");
        //        xmlElement = (xmlDocument.SelectSingleNode(xpath) as XmlElement);
        //        context2.XmlElement = xmlElement;
        //    }
        //    else
        //        xmlElement = context2.XmlElement;
        //    // get value
        //    return (xmlElement != null ? (!isValue ? xmlElement.GetAttribute(itemKey) : xmlElement.InnerText) : string.Empty);
        //}

        ///// <summary>
        ///// Sets the value within the packed string specified that is associated with the key provided.
        ///// </summary>
        //public override string SetValue(string pack, string key, string value, string contextKey, ref object context)
        //{
        //    // context
        //    var context2 = Context.GetContext(pack, contextKey, ref context);
        //    var xmlDocument = context2.XmlDocument;
        //    // parse key
        //    bool isValue;
        //    string scopeKey;
        //    string itemKey;
        //    ParseKey(key, out isValue, out scopeKey, out itemKey);
        //    // xmlelement
        //    XmlElement xmlElement;
        //    if (context2.ScopeKey != scopeKey)
        //    {
        //        context2.ScopeKey = scopeKey;
        //        // find element
        //        xmlElement = xmlDocument.DocumentElement;
        //        if (xmlElement == null)
        //        {
        //            xmlElement = xmlDocument.CreateElement("root");
        //            xmlDocument.AppendChild(xmlElement);
        //        }
        //        if (scopeKey.Length > 0)
        //        {
        //            // singhj: scopeKey doesn't include trailing "::"
        //            string[] scopeKeyArray = scopeKey.Split(new string[] { Delimiter }, StringSplitOptions.None); //was: string[] scopeKeyArray = scopeKey.Substring(0, scopeKey.Length - 2).Split(new string[] { KernelText.Scope }, StringSplitOptions.None);
        //            foreach (string scopeKey2 in scopeKeyArray)
        //            {
        //                var xmlElement2 = xmlElement[scopeKey2];
        //                if (xmlElement2 == null)
        //                {
        //                    xmlElement2 = xmlDocument.CreateElement(scopeKey2);
        //                    xmlElement.AppendChild(xmlElement2);
        //                }
        //                xmlElement = xmlElement2;
        //            }
        //        }
        //        context2.XmlElement = xmlElement;
        //    }
        //    else
        //        xmlElement = context2.XmlElement;
        //    // set value
        //    if (!isValue)
        //        xmlElement.SetAttribute(itemKey, value);
        //    else
        //        xmlElement.Value = value;
        //    return xmlDocument.InnerXml;
        //}

        /// <summary>
        /// Provides the ability to decode the contents of the pack provided into the hash instance provided, based on the logic
        /// provided by <see cref="M:PackDecodeRecurse">PackDecodeRecurse</see>. The result is contained in the hash provided.
        /// </summary>
        /// <param name="pack">The packed string to process.</param>
        /// <param name="namespaceID">The namespace ID.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static IDictionary<string, string> Decode(string pack, string namespaceID, Predicate<string> predicate)
        {
            var values = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(pack))
                return values;
            using (var r = XmlReader.Create(new StringReader(pack)))
            {
                if (r.IsStartElement())
                    DecodeInternalRecurse(string.Empty, r, (namespaceID ?? string.Empty), values, predicate);
            }
            return values;
        }

        private static void DecodeInternalRecurse(string scope, XmlReader r, string namespaceID, IDictionary<string, string> values, Predicate<string> predicate)
        {
            bool inNamespace = scope.StartsWith(namespaceID, StringComparison.OrdinalIgnoreCase);
            if (inNamespace)
            {
                // parse attributes
                if (r.HasAttributes)
                {
                    while (r.MoveToNextAttribute())
                    {
                        string key = scope + r.Name;
                        // check validkeyindex and commit
                        if ((predicate == null) || predicate(key))
                            values[scope + r.Name] = r.Value;
                    }
                    // move the reader back to the element node.
                    r.MoveToElement();
                }
            }
            if (!r.IsEmptyElement)
            {
                // read the start tag.
                r.Read();
                bool isRead = true;
                while (isRead)
                    switch (r.MoveToContent())
                    {
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                            if (inNamespace)
                            {
                                string key = (scope.Length > 0 ? scope : Delimiter);
                                // check validkeyindex and commit
                                if ((predicate == null) || predicate(key))
                                    values[key] = r.Value;
                            }
                            r.Read();
                            break;
                        case XmlNodeType.Element:
                            // handle nested elements.
                            if (r.IsStartElement())
                            {
                                DecodeInternalRecurse(scope + r.Name + Delimiter, r, namespaceID, values, predicate);
                                r.Read();
                            }
                            break;
                        default:
                            isRead = false;
                            break;
                    }
            }
        }

        /// <summary>
        /// Provides the ability to pack the contents in the hash provided into a different string representation.
        /// Results is contained in the provided StringBuilder instance.
        /// </summary>
        public static string Encode(IDictionary<string, string> values, string namespaceID, Predicate<string> predicate)
        {
            if (values == null)
                throw new ArgumentNullException("set");
            if (values.Count == 0)
                return string.Empty;
            var b = new StringBuilder();
            // pull keys from existing hash and validate against provided IDictionary, and encode into tree key structure
            // field is prepended with identifyer for unencoding at the end to find original key
            var keys = new List<string>(values.Keys);
            for (int keyIndex = keys.Count - 1; keyIndex >= 0; keyIndex--)
            {
                string key = keys[keyIndex];
                // check for validkeyindex
                if ((predicate != null) && !predicate(key))
                {
                    keys.RemoveAt(keyIndex);
                    continue;
                }
                // encode key
                keys[keyIndex] = EncodeKey(key);
            }
            keys.Sort(0, keys.Count, StringComparer.OrdinalIgnoreCase);
            //
            using (var w = XmlTextWriter.Create(b))
            {
                w.WriteStartElement("r");
                //
                string lastScopeKey = string.Empty;
                string elementValue = null;
                foreach (string key in keys)
                {
                    // parse encoded key
                    bool isValue;
                    string scopeKey;
                    string itemKey;
                    ParseEncodedKey(key, out isValue, out scopeKey, out itemKey);
                    // process element
                    if ((scopeKey.Length > 1) && (lastScopeKey != scopeKey))
                    {
                        // write latched value
                        if (elementValue != null)
                        {
                            w.WriteString(elementValue);
                            elementValue = null;
                        }
                        // element
                        if (scopeKey.StartsWith(lastScopeKey))
                        {
                            // start elements
                            int lastScopeKeyLength = lastScopeKey.Length;
                            var createScopeKeyArray = scopeKey.Substring(lastScopeKeyLength, scopeKey.Length - lastScopeKeyLength - 1).Split('\x01');
                            foreach (string createScopeKey in createScopeKeyArray)
                                w.WriteStartElement(createScopeKey);
                        }
                        else
                        {
                            // end and start elements
                            var lastScopeKeyArray = lastScopeKey.Substring(0, lastScopeKey.Length - 1).Split('\x01');
                            var scopeKeyArray = scopeKey.Substring(0, scopeKey.Length - 1).Split('\x01');
                            int scopeKeyArrayLength = scopeKeyArray.Length;
                            // skip existing elements
                            int index;
                            for (index = 0; index < lastScopeKeyArray.Length; index++)
                                if ((index >= scopeKeyArrayLength) || (scopeKeyArray[index] != lastScopeKeyArray[index]))
                                    break;
                            // end elements
                            for (int lastScopeKeyIndex = lastScopeKeyArray.Length - 1; lastScopeKeyIndex >= index; lastScopeKeyIndex--)
                                w.WriteEndElement();
                            // start elements
                            for (int scopeKeyIndex = index; scopeKeyIndex < scopeKeyArray.Length; scopeKeyIndex++)
                                w.WriteStartElement(scopeKeyArray[scopeKeyIndex]);
                        }
                        lastScopeKey = scopeKey;
                    }
                    // decode key and set value
                    string value = values[DecodeKey(key)];
                    if (!isValue)
                        w.WriteAttributeString(itemKey, (value ?? string.Empty));
                    else
                        if (!string.IsNullOrEmpty(value))
                            elementValue = value;
                }
                // overflow close, write latched value
                if (elementValue != null)
                    w.WriteString(elementValue);
                w.WriteEndDocument();
            }
            return b.ToString();
        }

        #region ITextPack

        IDictionary<string, string> ITextPack.Decode(string pack, string namespaceID, Predicate<string> predicate) { return Decode(pack, namespaceID, predicate); }
        string ITextPack.Encode(IDictionary<string, string> values, string namespaceID, Predicate<string> predicate) { return Encode(values, namespaceID, predicate); }

        #endregion
    }
}
