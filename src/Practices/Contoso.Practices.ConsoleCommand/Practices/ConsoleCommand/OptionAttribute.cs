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

