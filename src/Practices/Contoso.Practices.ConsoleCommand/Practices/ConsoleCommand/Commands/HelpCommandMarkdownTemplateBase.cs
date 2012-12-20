using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    /// <summary>
    /// HelpCommandMarkdownTemplateBase
    /// </summary>
    public class HelpCommandMarkdownTemplateBase
    {
        private StringBuilder _generatingEnvironment = new StringBuilder();

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(object value)
        {
            string valueAsString;
            if (value == null)
                throw new ArgumentNullException("value");
            var method = value.GetType().GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
            if (method == null)
                valueAsString = value.ToString();
            else
                valueAsString = (string)method.Invoke(value, new object[] { CultureInfo.InvariantCulture });
            WriteLiteral(valueAsString);
        }

        /// <summary>
        /// Writes the literal.
        /// </summary>
        /// <param name="textToAppend">The text to append.</param>
        public void WriteLiteral(string textToAppend)
        {
            if (!string.IsNullOrEmpty(textToAppend))
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        /// Gets or sets the generation environment.
        /// </summary>
        /// <value>
        /// The generation environment.
        /// </value>
        protected StringBuilder GenerationEnvironment
        {
            get { return _generatingEnvironment; }
            set { _generatingEnvironment = value; }
        }
    }
}

