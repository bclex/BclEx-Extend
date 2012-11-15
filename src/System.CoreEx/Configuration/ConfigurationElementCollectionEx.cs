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
using System.Collections;
#if DEFINENAMEHERE
using System.Configuration;
namespace NAMESPACEHERE
{
    /// <summary>
    /// An abstract class representing a simplified configuration setting object collection.
    /// </summary>
    public partial class NAMEHERE : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEHERE"/> class.
        /// </summary>
        public NAMEHERE() { _collectionType = ConfigurationElementCollectionType.AddRemoveClearMap; _attributeIndex = new AttributeIndex(this); }
#else
namespace System.Configuration
{
    /// <summary>
    /// An abstract class representing a simplified configuration setting object collection. This provides a basic
    /// facade over the <see cref="T:System.Configuration.ConfigurationElementCollection">ConfigurationElement</see> class.
    /// </summary>
    /// <typeparam name="T">Specific type contained within the generic hash instance.</typeparam>
    public class ConfigurationElementCollectionEx<T> : ConfigurationElementCollection
        where T : ConfigurationElementEx, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationElementCollectionEx&lt;T&gt;"/> class.
        /// </summary>
        protected ConfigurationElementCollectionEx()
            : this(ConfigurationElementCollectionType.AddRemoveClearMap) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationElementCollectionEx&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collectionType">Type of the collection.</param>
        protected ConfigurationElementCollectionEx(ConfigurationElementCollectionType collectionType)
        {
            _collectionType = collectionType;
            _attributeIndex = new AttributeIndex(this);
        }
#endif
        private AttributeIndex _attributeIndex;
        private ConfigurationElementCollectionType _collectionType;


        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        public T this[int index]
        {
            get { return (T)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        public new T this[string name]
        {
            get { return (T)BaseGet(name); }
        }

        /// <summary>
        /// Adds if undefined.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T AddIfUndefined(string name)
        {
            bool isNew;
            return AddIfUndefined(name, out isNew);
        }
        /// <summary>
        /// Adds if undefined.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="wasAdded">if set to <c>true</c> [is new].</param>
        /// <returns></returns>
        public T AddIfUndefined(string name, out bool wasAdded)
        {
            T t = (T)BaseGet(name);
            if (t == null)
            {
                t = (T)CreateNewElement();
                t.Name = name;
                BaseAdd(t);
                wasAdded = true;
                return t;
            }
            wasAdded = false;
            return t;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly() { return false; }

        ///// <summary>
        ///// Gets the property collection.
        ///// </summary>
        ///// <value>The property collection.</value>
        //public ConfigurationPropertyCollection PropertyCollection
        //{
        //    get { return base.Properties; }
        //}

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        ///   <c>true</c> if the Collection contains an element with the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetValue(object key, out T value)
        {
            value = (T)BaseGet(key);
            return (value != null);
        }

        #region Inheriting

        /// <summary>
        /// Applies the configuration.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        public void ApplyConfiguration(ConfigurationElementCollection inheritConfiguration)
        {
            ApplyConfigurationValues(inheritConfiguration);
            ApplyConfigurationElements(inheritConfiguration);
        }

        /// <summary>
        /// Applies the configuration values.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        protected virtual void ApplyConfigurationValues(ConfigurationElementCollection inheritConfiguration) { }

        /// <summary>
        /// Applies the configuration elements.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        protected virtual void ApplyConfigurationElements(ConfigurationElementCollection inheritConfiguration) { }

        /// <summary>
        /// Applies the default values.
        /// </summary>
        protected virtual void ApplyDefaultValues() { }

        #endregion

        #region ConfigurationElementCollection

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection">ConfigurationElementCollection</see>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to add.</param>
        protected override void BaseAdd(ConfigurationElement element) { BaseAdd(element, false); }

        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection">ConfigurationElementCollection</see>
        /// returned by accessing <see cref="P:System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap">AddRemoveClearMap</see>.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType"></see> of this collection.</returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return _collectionType; }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement">ConfigurationElement</see>
        /// of the generic parameter type T.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement() { return new T(); }

        /// <summary>
        /// Gets the Name for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element) { return ((T)element).Name; }

        #endregion

        #region Attribute

#if DEFINENAMEHERE
        /// <summary>
        /// Gets the AttributeIndex of this class.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public AttributeIndex Attribute
        {
            get { return _attributeIndex; }
        }
        /// <summary>
        /// AttributeIndex
        /// </summary>
        public class AttributeIndex
        {
            private NAMEHERE _parent;
            /// <summary>
            /// Initializes a new instance of the <see cref="AttributeIndex"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public AttributeIndex(NAMEHERE parent) { _parent = parent; }
#else
        /// <summary>
        /// Gets the AttributeIndex of this class.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        protected IIndexer<string, object> Attribute
        {
            get { return _attributeIndex; }
        }
        private class AttributeIndex : IIndexer<string, object>
        {
            private ConfigurationElementCollectionEx<T> _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurationElementCollectionEx&lt;T&gt;.AttributeIndex"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public AttributeIndex(ConfigurationElementCollectionEx<T> parent) { _parent = parent; }
#endif
            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            /// <value></value>
            public object this[string key]
            {
                get { return _parent.GetBaseItem(key); }
                set { _parent.SetBaseItem(key, value); }
            }
        }

        /// <summary>
        /// Gets the item associated with the key provided from the underlying base config collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Configuration value.
        /// </returns>
        protected object GetBaseItem(string key) { return base[key]; }

        /// <summary>
        /// Sets the item associated with the key provided from the underlying base config collection.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to set.</param>
        protected void SetBaseItem(string key, object value) { base[key] = value; }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object GetAttribute(string name) { return (!Properties.Contains(name) ? base[name] : null); }

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetAttribute(string name, object value)
        {
            if (!Properties.Contains(name))
                Properties.Add(new ConfigurationProperty(name, typeof(string)));
            base[name] = value;
        }

        #endregion
    }
}