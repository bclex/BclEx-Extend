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
//using System;
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
//    public class SmartFormXmlReader
//    {
//        private int _xmlDepth;

//        /// <summary>
//        /// Metas the read error.
//        /// </summary>
//        /// <param name="scopeKey">The scope key.</param>
//        /// <param name="message">The message.</param>
//        private void MetaReadError(string scopeKey, string message)
//        {
//            IsError = true;
//            _meta[scopeKey + "::Error." + _meta.Count.ToString()] = new SmartFormMeta.Element(SmartFormMeta.ElementType.Error, message);
//        }

//        /// <summary>
//        /// Reads the meta field.
//        /// </summary>
//        /// <param name="scopeKey">The scope key.</param>
//        /// <param name="r">The XML reader.</param>
//        private void ReadMetaField(string scopeKey, XmlReader r)
//        {
//            switch (r.LocalName)
//            {
//                case "label":
//                    // <lable>LABEL</label>
//                    _meta[scopeKey + "::Label." + _meta.Count.ToString()] = new SmartFormMeta.Element(SmartFormMeta.ElementType.Label, r.ReadString());
//                    return;
//                case "textBox":
//                case "textBox2":
//                    // <textbox name="" label="" type="" ... />
//                    var elementType = (r.LocalName == "textBox" ? SmartFormMeta.ElementType.TextBox : SmartFormMeta.ElementType.TextBox2);
//                    string fieldName = r.GetAttribute("name");
//                    string fieldLabel = r.GetAttribute("label");
//                    string fieldType = r.GetAttribute("type");
//                    int attributeCount = r.AttributeCount;
//                    if ((attributeCount >= 3) && (fieldName != null) && (fieldLabel != null) && (fieldType != null))
//                    {
//                        if ((fieldName.Length == 0) || (fieldLabel.Length == 0) || (fieldType.Length == 0))
//                        {
//                            MetaReadError(scopeKey, "Name, label and type are required: " + StringEx.Axb(scopeKey, CoreEx.Scope, fieldName));
//                            break;
//                        }
//                        Nattrib fieldAttrib;
//                        if (attributeCount > 3)
//                        {
//                            fieldAttrib = new Nattrib();
//                            //xmlReader.MoveToFirstAttribute();
//                            //do
//                            //{
//                            //    switch (xmlReader.LocalName)
//                            //    {
//                            //        case "name":
//                            //        case "label":
//                            //        case "type":
//                            //            continue;
//                            //    }
//                            //    fieldAttrib[xmlReader.LocalName] = xmlReader.Value;
//                            //} while (xmlReader.MoveToNextAttribute());
//                        }
//                        else
//                            fieldAttrib = null;
//                        string key = StringEx.Axb(scopeKey, CoreEx.Scope, fieldName);
//                        if (!_meta.ContainsKey(key))
//                            _meta[key] = new SmartFormMeta.Element(elementType, fieldLabel, fieldType, fieldAttrib);
//                        else
//                        {
//                            MetaReadError(scopeKey, "Duplicate key: " + key);
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        MetaReadError(scopeKey, "Invalid type: " + scopeKey + CoreEx.Scope + r.LocalName);
//                        return;
//                    }
//                    return;
//            }
//            MetaReadError(scopeKey, "Invalid field type: " + scopeKey + CoreEx.Scope + r.LocalName);
//        }

//        /// <summary>
//        /// Loads the meta state information contained within the string provided by conversion to an XmlDocument and performing
//        /// a recursive load.
//        /// </summary>
//        /// <param name="metaState">State of the meta.</param>
//        public void ReadMetaState(string metaState)
//        {
//            if (string.IsNullOrEmpty(metaState))
//            {
//                _meta.Clear();
//                return;
//            }
//            using (var r = XmlReader.Create(new StringReader(metaState)))
//                ReadMetaState(r);
//        }
//        /// <summary>
//        /// Loads the meta state information contained within the string provided by conversion to an XmlDocument and performing
//        /// a recursive load.
//        /// </summary>
//        /// <param name="r">State of the meta.</param>
//        public void ReadMetaState(XmlReader r)
//        {
//            _meta.Clear();
//            if (r == null)
//                return;
//            // 1. find root element
//            r.ReadToFollowing("smartForm");
//            if (r.EOF)
//                return;
//            _xmlDepth = r.Depth;
//            // 2. parse document
//            ReadMetaStateRecurse(string.Empty, r);
//        }

//        /// <summary>
//        /// Reads the meta state recurse.
//        /// </summary>
//        /// <param name="scopeKey">The scope key.</param>
//        /// <param name="r">The XML reader.</param>
//        private void ReadMetaStateRecurse(string scopeKey, XmlReader r)
//        {
//            while ((r.Read()) && (r.Depth >= _xmlDepth))
//                if (r.LocalName == "unit")
//                    if (r.NodeType == XmlNodeType.Element)
//                    {
//                        string unitName;
//                        if (((unitName = r.GetAttribute("name")) == null) || (unitName.Length == 0))
//                        {
//                            MetaReadError(scopeKey, "Unit missing name attribute");
//                            continue;
//                        }
//                        string key = StringEx.Axb(scopeKey, "::" unitName);
//                        _meta[key + CoreEx.Scope] = new SmartFormMeta.Element(SmartFormMeta.ElementType.Unit, r.GetAttribute("label"));
//                        ReadMetaStateRecurse(key, r);
//                    }
//                    else
//                        return;
//                else if (r.NodeType == XmlNodeType.Element)
//                    ReadMetaField(scopeKey, r);
//        }

//        /// <summary>
//        /// Loads the valueState information using the XmlTextPack instance associated with the underlying TextPackBase.
//        /// </summary>
//        /// <param name="valueState">State of the value.</param>
//        public void ReadValueState(string valueState)
//        {
//            ReadValueState(valueState, XmlTextPack.Instance);
//        }
//        /// <summary>
//        /// 	<summary>
//        /// Loads the valueState information using the TextPackBase provided.
//        /// </summary>
//        /// 	<param name="valueState">State of the value.</param>
//        /// </summary>
//        /// <param name="valueState">State of the value.</param>
//        /// <param name="textPack">The text pack.</param>
//        public void ReadValueState(string valueState, ITextPack textPack)
//        {
//            if (textPack == null)
//                throw new ArgumentNullException("textPack");
//            _valueHash.Clear();
//            if (string.IsNullOrEmpty(valueState))
//                return;
//            textPack.PackDecode(valueState, _valueHash);
//        }
//    }
//}
