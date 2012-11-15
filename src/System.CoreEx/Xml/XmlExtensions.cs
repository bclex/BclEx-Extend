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
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Collections;
namespace System.Xml
{
	/// <summary>
	/// XmlExtensions
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Writes the attribute.
		/// </summary>
		/// <param name="w">The XML writer.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public static void WriteAttributeStringIf(this XmlWriter w, string name, string value)
		{
			if (w == null)
				throw new ArgumentNullException("w");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			if (string.IsNullOrEmpty(value))
				w.WriteAttributeString(name, value);
		}

		/// <summary>
		/// Writes the collection.
		/// </summary>
		/// <param name="w">The XML writer.</param>
		/// <param name="name">The name.</param>
		/// <param name="collection">The collection.</param>
		public static void WriteCollectionIf(this XmlWriter w, string name, ICollection collection)
		{
			if (w == null)
				throw new ArgumentNullException("w");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			var xmlSerializable = (collection as IXmlSerializable);
			if (xmlSerializable == null)
				throw new ArgumentNullException("collection");
			if ((collection != null) && (collection.Count != 0))
			{
				w.WriteStartElement(name);
				xmlSerializable.WriteXml(w);
				w.WriteEndElement();
			}
		}

        ///// <summary>
        ///// Creates an XmlReader instance for a new document using the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="r">The text reader.</param>
        ///// <param name="settings">The settings.</param>
        ///// <returns></returns>
        ////? REMOVE
        //public static XmlReader CreateXmlReader(this string text)
        //{
        //    return XmlReader.Create(new StringReader(text), new XmlReaderSettings());
        //}
        ///// <summary>
        ///// Creates an XmlReader instance for a new document using the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="r">The text reader.</param>
        ///// <param name="settings">The settings.</param>
        ///// <returns></returns>
        ////? REMOVE
        //public static XmlReader CreateXmlReader(this TextReader r)
        //{
        //    return XmlReader.Create(r, new XmlReaderSettings());
        //}

        ///// <summary>
        ///// Creates an XmlReader instance for a new document using the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="r">The text reader.</param>
        ///// <param name="settings">The settings.</param>
        ///// <returns></returns>
        ////? REMOVE
        //public static XmlReader CreateXmlReader(this TextReader r, XmlReaderSettings settings)
        //{
        //    return XmlReader.Create(r, settings);
        //}

        ///// <summary>
        ///// Creates an XmlWriter instance used to append to the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="b">The <see cref="StringBuilder"/> to which to write to.
        ///// Content written by the <see cref="XmlWriter"/> is appended to the <c>StringBuilder</c>.</param>
        ///// <returns>An <c>XmlWriter</c> object.</returns>
        ////? REMOVE
        //public static XmlWriter CreateXmlWriter(this StringBuilder b)
        //{
        //    return XmlWriter.Create(b, new XmlWriterSettings() { OmitXmlDeclaration = true });
        //}

        ///// <summary>
        ///// Creates an XmlWriter instance used to append to the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="b">The <see cref="StringBuilder"/> to which to write to.
        ///// Content written by the <see cref="XmlWriter"/> is appended to the <c>StringBuilder</c>.</param>
        ///// <param name="omitXmlDeclaration">if set to <c>true</c> [omit XML declaration].</param>
        ///// <returns>An <c>XmlWriter</c> object.</returns>
        ////? REMOVE
        //public static XmlWriter CreateXmlWriter(this StringBuilder b, bool omitXmlDeclaration)
        //{
        //    return XmlWriter.Create(b, new XmlWriterSettings() { OmitXmlDeclaration = omitXmlDeclaration });
        //}

        ///// <summary>
        ///// Creates an XmlWriter instance for a new document using the provided <see cref="StringBuilder"/> underlying buffer.
        ///// </summary>
        ///// <param name="b">The b.</param>
        ///// <param name="settings">The settings.</param>
        ///// <returns></returns>
        ////? REMOVE
        //public static XmlWriter CreateXmlWriter(this StringBuilder b, XmlWriterSettings settings)
        //{
        //    return XmlWriter.Create(b, settings);
        //}
	}
}