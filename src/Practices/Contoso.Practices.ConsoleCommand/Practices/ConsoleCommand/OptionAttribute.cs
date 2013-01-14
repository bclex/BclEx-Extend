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
using System.Resources;

namespace Contoso.Practices.ConsoleCommand
{
    /// <summary>
    /// OptionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public OptionAttribute(string description)
        {
            Description = description;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="descriptionResourceName">Name of the description resource.</param>
        public OptionAttribute(Type resourceType, string descriptionResourceName)
        {
            ResourceType = resourceType;
            DescriptionResourceName = descriptionResourceName;
        }

        /// <summary>
        /// Gets or sets the name of the alt.
        /// </summary>
        /// <value>
        /// The name of the alt.
        /// </value>
        public string AltName { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return ResourceManagerEx.GetStringOrDefault(ResourceType, DescriptionResourceName, _description); }
            private set { _description = value; }
        }

        /// <summary>
        /// Gets the name of the description resource.
        /// </summary>
        /// <value>
        /// The name of the description resource.
        /// </value>
        public string DescriptionResourceName { get; private set; }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        /// <value>
        /// The type of the resource.
        /// </value>
        public Type ResourceType { get; private set; }
    }
}

