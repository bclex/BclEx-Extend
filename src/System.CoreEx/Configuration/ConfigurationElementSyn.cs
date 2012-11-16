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
namespace System.Configuration
{
    /// <summary>
    /// ConfigurationElementSyn
    /// </summary>
    /// <typeparam name="TSyn">The type of the syn.</typeparam>
    public class ConfigurationElementSyn<TSyn>
        where TSyn : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationElementSyn&lt;TSyn&gt;"/> class.
        /// </summary>
        /// <param name="syn">The syn.</param>
        public ConfigurationElementSyn(TSyn syn)
        {
            Syn = syn;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified property name.
        /// </summary>
        protected internal object this[string propertyName]
        {
            get
            {
                var property = Properties[propertyName];
                //if (property == null)
                //{
                //    property = Properties[string.Empty];
                //    if (property.ProvidedName != propertyName)
                //        return null;
                //}
                return this[property];
            }
            set
            {
                if (Syn == null)
                    throw new InvalidOperationException();
                SetPropertyValue(Properties[propertyName], value, false);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified prop.
        /// </summary>
        protected internal object this[ConfigurationProperty prop]
        {
            get
            {
                if (Syn == null)
                    throw new InvalidOperationException();
                return ConfigurationElementSynHelper.ItemProperty.GetValue(Syn, new[] { prop });
            }
            set
            {
                if (Syn == null)
                    throw new InvalidOperationException();
                SetPropertyValue(prop, value, false);
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        protected internal virtual ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection result = null;
                if (ConfigurationElementSynHelper.PropertiesFromType(GetType(), out result))
                {
                    ConfigurationElementSynHelper.ApplyInstanceAttributesMethod.Invoke(null, new[] { this });
                    ConfigurationElementSynHelper.ApplyValidatorsRecursiveMethod.Invoke(null, new[] { this });
                }
                return result;
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="value">The value.</param>
        /// <param name="ignoreLocks">if set to <c>true</c> [ignore locks].</param>
        protected void SetPropertyValue(ConfigurationProperty prop, object value, bool ignoreLocks)
        {
            if (prop == null)
                throw new ArgumentNullException("prop");
            ConfigurationElementSynHelper.SetPropertyValueMethod.Invoke(Syn, new[] { prop, value, ignoreLocks });
        }

        /// <summary>
        /// Gets the syn.
        /// </summary>
        public TSyn Syn { get; private set; }
    }
}
