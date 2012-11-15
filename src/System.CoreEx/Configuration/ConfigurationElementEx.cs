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
    /// An abstract class representing a simplified configuration setting object. This provides a basic
    /// facade over the <see cref="T:System.Configuration.ConfigurationElement">ConfigurationElement</see> class.
    /// </summary>
    public partial class NAMEHERE : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEHERE"/> class.
        /// </summary>
        public NAMEHERE() { _attributeIndex = new AttributeIndex(this); }
#else
namespace System.Configuration
{
    /// <summary>
    /// An abstract class representing a simplified configuration setting object. This provides a basic
    /// facade over the <see cref="T:System.Configuration.ConfigurationElement">ConfigurationElement</see> class.
    /// </summary>
    public abstract class ConfigurationElementEx : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationElementEx"/> class.
        /// </summary>
        protected ConfigurationElementEx() { _attributeIndex = new AttributeIndex(this); }
#endif
        private AttributeIndex _attributeIndex;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly() { return false; }

#if !COREINTERNAL
        /// <summary>
        /// Gets or sets the name of the configuration setting.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name
        {
            get { return string.Empty; }
            set { }
        }
#endif

        ///// <summary>
        ///// Gets the property collection of the underlying ConfigurationElement instance.
        ///// </summary>
        ///// <value>The property collection.</value>
        //public ConfigurationPropertyCollection PropertyCollection
        //{
        //    get { return base.Properties; }
        //}

        #region Inheriting

        /// <summary>
        /// Applies the configuration.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        public void ApplyConfiguration(ConfigurationElement inheritConfiguration)
        {
            ApplyConfigurationValues(inheritConfiguration);
            ApplyConfigurationElements(inheritConfiguration);
        }

        /// <summary>
        /// Applies the configuration values.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        protected virtual void ApplyConfigurationValues(ConfigurationElement inheritConfiguration) { }

        /// <summary>
        /// Applies the configuration elements.
        /// </summary>
        /// <param name="inheritConfiguration">The inherit configuration.</param>
        protected virtual void ApplyConfigurationElements(ConfigurationElement inheritConfiguration) { }

        /// <summary>
        /// Applies the default values.
        /// </summary>
        protected virtual void ApplyDefaultValues() { }

        #endregion

        #region Attribute

#if DEFINENAMEHERE
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        protected AttributeIndex Attribute
        {
            get { return _attributeIndex; }
        }
        /// <summary>
        /// AttributeIndex
        /// </summary>
        protected class AttributeIndex
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
        protected IIndexer<ConfigurationProperty, object> Attribute
        {
            get { return _attributeIndex; }
        }
        private class AttributeIndex : IIndexer<ConfigurationProperty, object>
        {
            private ConfigurationElementEx _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="AttributeIndex"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public AttributeIndex(ConfigurationElementEx parent)
            {
                _parent = parent;
            }
#endif
            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> with the specified key.
            /// </summary>
            public object this[ConfigurationProperty key]
            {
                get { return _parent[key]; }
                set { _parent[key] = value; }
            }
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected object GetAttribute(string name)
        {
            return (!Properties.Contains(name) ? this[name] : null);
        }

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected void SetAttribute(string name, object value)
        {
            if (!Properties.Contains(name))
                Properties.Add(new ConfigurationProperty(name, typeof(string)));
            this[name] = value;
        }

        #endregion
    }
}