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
using System.ComponentModel;
using System.Reflection;
namespace System.Configuration
{
    /// <summary>
    /// ConfigurationElementCollectionEx
    /// </summary>
    public class ConfigurationElementCollectionEx : ConfigurationElementCollectionEx<ConfigurationElementCollectionEx.NameValueElement>
    {
        /// <summary>
        /// TypeElement
        /// </summary>
        public class TypeElement : ConfigurationElementEx
        {
            /// <summary>
            /// Gets or sets the name of the configuration setting.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            [ConfigurationProperty("name")]
            public override string Name
            {
                get { string name; return (!string.IsNullOrEmpty(name = (string)base["name"]) ? name : (Type != null ? Type.ToString() : null)); }
                set { base["name"] = value; }
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            [ConfigurationProperty("type", IsRequired = true)]
            [TypeConverter(typeof(TypeNameConverter))]
            public Type Type
            {
                get { return (Type)this["type"]; }
                set { base["type"] = value; }
            }
        }

        /// <summary>
        /// AssemblyElement
        /// </summary>
        public class AssemblyElement : ConfigurationElementEx
        {
            /// <summary>
            /// Gets or sets the name of the configuration setting.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            [ConfigurationProperty("name")]
            public override string Name
            {
                get { string name; return (!string.IsNullOrEmpty(name = (string)base["name"]) ? name : (Assembly != null ? Assembly.FullName : null)); }
                set { base["name"] = value; }
            }

            /// <summary>
            /// Gets or sets the assembly.
            /// </summary>
            /// <value>
            /// The assembly.
            /// </value>
            [ConfigurationProperty("assembly", IsRequired = true)]
            [TypeConverter(typeof(AssemblyNameConverter))]
            public Assembly Assembly
            {
                get { return (Assembly)this["assembly"]; }
                set { base["assembly"] = value; }
            }
        }

        /// <summary>
        /// NameValueElement
        /// </summary>
        public class NameValueElement : ConfigurationElementEx
        {
            /// <summary>
            /// Gets or sets the name of the configuration setting.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            [ConfigurationProperty("name")]
            public override string Name
            {
                get { string name; return (!string.IsNullOrEmpty(name = (string)base["name"]) ? name : (Type != null ? Type.ToString() : null)); }
                set { base["name"] = value; }
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            [ConfigurationProperty("value")]
            public object Type
            {
                get { return (object)this["type"]; }
                set { base["type"] = value; }
            }
        }
    }
}