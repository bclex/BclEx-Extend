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
using System.Text;
using System.IO;
using System.Xml.XPath;
using System.Collections.Generic;
namespace System.Xml
{
    /// <summary>
    /// Provides an advanced façade pattern that facilitates a large range of xml-oriented text checking, parsing, and calculation
    /// functions into a single wrapper class.
    /// #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]
    /// </summary>
    public static class XmlNodeEx
    {
        /// <summary>
        /// BeginCData
        /// </summary>
        public const string BeginCData = "<![CDATA[";
        /// <summary>
        /// EndCData
        /// </summary>
        public const string EndCData = "]]>";
        /// <summary>
        /// DefaultListCompositeKeyDivider
        /// </summary>
        public static readonly string[] DefaultListCompositeKeyDivider = new string[] { ";" };
        private static XmlDocument _xmlDocument = new XmlDocument();

        /// <summary>
        /// XmlToListOptions
        /// </summary>
        [Flags]
        public enum XmlToListOptions
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// HasCompositeKey
            /// </summary>
            HasCompositeKey = 0x01,
        }

        /// <summary>
        /// Creates an XmlElement instance with the name provided from the underlying xml document in use.
        /// </summary>
        /// <param name="name">The name of the element to create.</param>
        /// <returns></returns>
        public static XmlElement CreateXmlElement(string name) { return _xmlDocument.CreateElement(name); }

        /// <summary>
        /// Encodes the xpath partial text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeXpathPartialText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var b = new StringBuilder(value.Length);
            EscapeXpathText(b, value);
            return b.ToString();
        }

        /// <summary>
        /// Encodes the xpath text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EncodeXpathText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";
            var b = new StringBuilder("\"", value.Length);
            EscapeXpathText(b, value);
            b.Append("\"");
            return b.ToString();
        }

        /// <summary>
        /// Escapes the xpath text.
        /// </summary>
        /// <param name="b">The String Builder.</param>
        /// <param name="value">The value.</param>
        private static void EscapeXpathText(StringBuilder b, string value)
        {
            foreach (var c in value)
            {
                int code = (int)c;
                switch (c)
                {
                    case '"':
                        b.Append("&quot;");
                        break;
                    default:
                        if ((code >= 32) && (code < 128))
                            b.Append(c);
                        else
                            b.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "\\u{0:X4}", code);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts the supplied list value representing a comma-delimited list of distinct values into an equivalent xml-based representation.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string EnumerableToXml(IEnumerable<string> items) { return EnumerableToXml("root", "item", "key", items, XmlToListOptions.None); }
        /// <summary>
        /// Lists to XML.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public static string EnumerableToXml(IEnumerable<string> items, XmlToListOptions flag) { return EnumerableToXml("root", "item", "key", items, flag); }
        /// <summary>
        /// Converts the supplied list value representing a comma-delimited list of distinct values into an equivalent xml-based representation.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string EnumerableToXml(string rootName, string nodeName, string attributeName, IEnumerable<string> items) { return EnumerableToXml(rootName, nodeName, attributeName, items, XmlToListOptions.None); }
        /// <summary>
        /// Lists to XML.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="items">The items.</param>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public static string EnumerableToXml(string rootName, string nodeName, string attributeName, IEnumerable<string> items, XmlToListOptions flag)
        {
            if (string.IsNullOrEmpty(rootName))
                throw new ArgumentNullException("rootName");
            if (string.IsNullOrEmpty(nodeName))
                throw new ArgumentNullException("nodeName");
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName");
            if (items == null)
                throw new ArgumentNullException("list");
            var b = new StringBuilder();
            var w = XmlWriter.Create(b, new XmlWriterSettings() { OmitXmlDeclaration = true });
            w.WriteStartElement(rootName);
            bool hasCompositeKey = ((flag & XmlToListOptions.HasCompositeKey) == XmlToListOptions.HasCompositeKey);
            foreach (string value in items)
            {
                w.WriteStartElement(nodeName);
                if (!hasCompositeKey)
                    w.WriteAttributeString(attributeName, value);
                else if (value.Length > 0)
                {
                    int compositePropertyIndex = 1;
                    string compositePropertyName = attributeName;
                    foreach (string compositeValue in value.Split(DefaultListCompositeKeyDivider, StringSplitOptions.None))
                    {
                        w.WriteAttributeString(compositePropertyName, compositeValue);
                        compositePropertyIndex++;
                        compositePropertyName = attributeName + compositePropertyIndex.ToString(CultureInfo.InvariantCulture.NumberFormat);
                    }
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.Flush();
            w.Close();
            return b.ToString();
        }

        ///// <summary>
        ///// Converts the supplied xml string into a list representing a comma-delimited list of distinct values.
        ///// </summary>
        ///// <param name="xml">The xml string.</param>
        ///// <returns></returns>
        //public static string XmlToList(string xml) { return XmlToList("root", "item", "key", xml, XmlToListOptions.None); }
        ///// <summary>
        ///// XMLs to list.
        ///// </summary>
        ///// <param name="xml">The XML.</param>
        ///// <param name="flag">The flag.</param>
        ///// <returns></returns>
        //public static string XmlToList(string xml, XmlToListOptions flag) { return XmlToList("root", "item", "key", xml, flag); }
        ///// <summary>
        ///// Converts the supplied xml string into a list representing a comma-delimited list of distinct values.
        ///// </summary>
        ///// <param name="rootName">Name of the root.</param>
        ///// <param name="nodeName">Name of the node.</param>
        ///// <param name="attributeName">Name of the attribute.</param>
        ///// <param name="xml">The xml string.</param>
        ///// <returns></returns>
        //public static string XmlToList(string rootName, string nodeName, string attributeName, string xml) { return XmlToList(rootName, nodeName, attributeName, xml, XmlToListOptions.None); }
        ///// <summary>
        ///// XMLs to list.
        ///// </summary>
        ///// <param name="rootName">Name of the root.</param>
        ///// <param name="nodeName">Name of the node.</param>
        ///// <param name="attributeName">Name of the attribute.</param>
        ///// <param name="xml">The XML.</param>
        ///// <param name="flag">The flag.</param>
        ///// <returns></returns>
        //public static string XmlToList(string rootName, string nodeName, string attributeName, string xml, XmlToListOptions flag)
        //{
        //    string listDivider = ";";
        //    if (string.IsNullOrEmpty(xml))
        //        return string.Empty;
        //    bool hasCompositeKey = ((flag & XmlToListOptions.HasCompositeKey) == XmlToListOptions.HasCompositeKey);
        //    var b = new StringBuilder();
        //    var z = new XPathDocument(new StringReader(xml));
        //    var xpathNavigator = z.CreateNavigator();
        //    var iterator = xpathNavigator.Select("/" + rootName + "/" + nodeName);
        //    while (iterator.MoveNext())
        //    {
        //        if (!hasCompositeKey)
        //            b.Append(iterator.Current.GetAttribute(attributeName, string.Empty) + listDivider);
        //        else
        //        {
        //            var pathNavigator = iterator.Current;
        //            int compositeIndex = 1;
        //            string compositeAttributeName = attributeName;
        //            string propertyValue;
        //            while ((propertyValue = pathNavigator.GetAttribute(compositeAttributeName, string.Empty)) != string.Empty)
        //            {
        //                b.Append(propertyValue + listDivider);
        //                compositeIndex++;
        //                compositeAttributeName = attributeName + compositeIndex.ToString(CultureInfo.InvariantCulture.NumberFormat);
        //            }
        //            if (b.Length > 0)
        //                b.Length -= listDivider.Length;
        //            b.Append(listDivider);
        //        }
        //    }
        //    if (b.Length > 0)
        //        b.Length -= listDivider.Length;
        //    return b.ToString();
        //}

        /// <summary>
        /// XMLs to enumerable.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static IEnumerable<string> XmlToEnumerable(string xml) { return XmlToEnumerable("root", "item", "key", xml, XmlToListOptions.None); }
        /// <summary>
        /// XMLs to enumerable.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public static IEnumerable<string> XmlToEnumerable(string xml, XmlToListOptions flag) { return XmlToEnumerable("root", "item", "key", xml, flag); }
        /// <summary>
        /// XMLs to enumerable.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public static IEnumerable<string> XmlToEnumerable(string rootName, string nodeName, string attributeName, string xml, XmlToListOptions flag)
        {
            if (string.IsNullOrEmpty(xml))
                yield break;
            bool hasCompositeKey = ((flag & XmlToListOptions.HasCompositeKey) == XmlToListOptions.HasCompositeKey);
            var b = new StringBuilder();
            var z = new XPathDocument(new StringReader(xml));
            var xpathNavigator = z.CreateNavigator();
            var iterator = xpathNavigator.Select("/" + rootName + "/" + nodeName);
            while (iterator.MoveNext())
            {
                if (!hasCompositeKey)
                    yield return iterator.Current.GetAttribute(attributeName, string.Empty);
                else
                {
                    var pathNavigator = iterator.Current;
                    int compositeIndex = 1;
                    string compositeAttributeName = attributeName;
                    string attributeValue;
                    while ((attributeValue = pathNavigator.GetAttribute(compositeAttributeName, string.Empty)) != string.Empty)
                    {
                        yield return attributeValue;
                        compositeIndex++;
                        compositeAttributeName = attributeName + compositeIndex.ToString(CultureInfo.InvariantCulture.NumberFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Encodes the text into XML by passing through an XmlElement instance.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The InnerXml of the resulting XmlElement</returns>
        public static string XmlTextEncode(string text)
        {
            var xmlElement = _xmlDocument.CreateElement("Item");
            xmlElement.InnerText = text;
            return xmlElement.InnerXml;
        }

        /// <summary>
        /// Encodes the Xml into text by passing through an XmlElement instance.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The InnerText of the XmlElement</returns>
        public static string XmlTextDecode(string text)
        {
            var xmlElement = _xmlDocument.CreateElement("Item");
            xmlElement.InnerXml = text;
            return xmlElement.InnerText;
        }
    }
}
