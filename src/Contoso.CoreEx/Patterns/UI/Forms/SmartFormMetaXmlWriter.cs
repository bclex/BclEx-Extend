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
//using System.Collections.Generic;
//using System.IO;
//using System.Xml;
//using Contoso.Patterns.UI.Forms.SmartFormElements;
//namespace Contoso.Patterns.UI.Forms
//{
//    //<smartForm>
//    //   <unit name="Process" label="PROCESSING EMAIL">
//    //      <textBox name="FromEmail" label="From Email" type="Email"/>
//    //      <textBox name="FromName" label="From Name" type="100"/>
//    //      <textBox name="ToEmail" label="To Email" type="EmailList"/>
//    //      <textBox name="BccEmail" label="BCC" type="EmailList"/>
//    //      <textBox name="Subject" label="Subject" type="300"/>
//    //      <textBox name="TextBody" label="Body" type="Memo" size="100x15"/>
//    //   </unit>
//    //   <unit name="Response" label="RESPONSE EMAIL">
//    //      <textBox name="FromEmail" label="From Email" type="Email"/>
//    //      <textBox name="FromName" label="From Name" type="100"/>
//    //      <textBox name="Subject" label="Subject" type="300"/>
//    //      <textBox name="TextBody" label="Body" type="Memo" size="100x15"/>
//    //   </unit>
//    //</smartForm>
//    /// <summary>
//    /// Represents a advanced form processing type that maintains state, processing functionality, and TextColumn/Element support.
//    /// </summary>
//    public class SmartFormMetaXmlWriter
//    {
//        /// <summary>
//        /// Writes the meta field.
//        /// </summary>
//        /// <param name="name">The name.</param>
//        /// <param name="element">The element.</param>
//        /// <param name="w">The XML writer.</param>
//        public void WriteMetaField(string name, ElementBase element, XmlWriter w)
//        {
//            // label
//            var labelElement = (element as LabelElement);
//            if (labelElement != null)
//            {
//                w.WriteElementString("label", labelElement.Label);
//                return;
//            }
//            // textbox
//            var textBoxElement = (element as TextBoxElement);
//            if (textBoxElement != null)
//            {
//                w.WriteStartElement(!(element is TextBox2Element) ? "textBox" : "textBox2");
//                w.WriteAttributeString("name", name);
//                w.WriteAttributeString("label", textBoxElement.Label);
//                w.WriteAttributeString("type", textBoxElement.FieldType);
//                //var fieldAttrib = textBoxElement.FieldAttrib;
//                //if (fieldAttrib != null)
//                //    foreach (string fieldAttribKey in fieldAttrib.Keys)
//                //        xmlWriter.WriteAttributeString(fieldAttribKey, fieldAttrib[fieldAttribKey]);
//                w.WriteEndElement();
//                return;
//            }
//            // error
//            w.WriteElementString("error", name);
//        }

//        /// <summary>
//        /// Writes the state of the meta.
//        /// </summary>
//        /// <param name="w">The XML writer.</param>
//        public void WriteMetaState(SmartFormMeta meta, XmlWriter w)
//        {
//            var elements = meta.Elements;
//            if (elements.Count == 0)
//                return;
//            w.WriteStartElement("smartForm");
//            int metaKeyIndex = 0;
//            //elements.Dictionary
//            var metaKeys = new List<string>(_meta.Keys).ToArray();
//            WriteMetaStateRecurse(string.Empty, ref metaKeyIndex, metaKeys, w);
//            w.WriteEndElement();
//        }

//        /// <summary>
//        /// Writes the meta state recurse.
//        /// </summary>
//        /// <param name="scopeKey">The scope key.</param>
//        /// <param name="metaKeyIndex">Index of the meta key.</param>
//        /// <param name="metaKeys">The meta keys.</param>
//        /// <param name="w">The XML writer.</param>
//        private void WriteMetaStateRecurse(string scopeKey, ref int metaKeyIndex, string[] metaKeys, XmlWriter w)
//        {
//            while (metaKeyIndex < metaKeys.Length)
//            {
//                string metaKey = metaKeys[metaKeyIndex];
//                if (!metaKey.StartsWith(scopeKey))
//                    return;
//                int scopeKeyLength = scopeKey.Length;
//                int nextScopeIndex = metaKey.IndexOf(CoreEx.Scope, scopeKeyLength);
//                if (nextScopeIndex > -1)
//                {
//                    // add unit
//                    string scopeName = metaKey.Substring(scopeKeyLength, nextScopeIndex - scopeKeyLength);
//                    w.WriteStartElement("unit");
//                    w.WriteAttributeString("name", scopeName);
//                    var element = _meta[metaKey];
//                    if (element.Type == SmartFormMeta.ElementType.Unit)
//                    {
//                        w.WriteAttributeString("label", element.Label);
//                        metaKeyIndex++;
//                    }
//                    WriteMetaStateRecurse(scopeKey + scopeName + CoreEx.Scope, ref metaKeyIndex, metaKeys, w);
//                    w.WriteEndElement();
//                    continue;
//                }
//                // add leaf node
//                string name = metaKey.Substring(scopeKeyLength);
//                WriteMetaField(name, _meta[metaKey], w);
//                metaKeyIndex++;
//            }
//        }

//        ///// <summary>
//        ///// Writes the state of the value.
//        ///// </summary>
//        ///// <returns></returns>
//        //public string WriteValueState()
//        //{
//        //    return WriteValueState(System.Primitives.TextPacks.XmlTextPack.Instance);
//        //}
//        ///// <summary>
//        ///// Writes the state of the value.
//        ///// </summary>
//        ///// <param name="textPack">The text pack.</param>
//        ///// <returns></returns>
//        //public string WriteValueState(TextPackBase textPack)
//        //{
//        //    if (textPack == null)
//        //        throw new ArgumentNullException("textPack");
//        //    return (_valueHash.Count == 0 ? string.Empty : textPack.PackEncode(_valueHash));
//        //}
//    }
//}
