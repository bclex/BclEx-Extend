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
namespace System.Patterns.UI.Forms
{
    /// <summary>
    /// Provides an object-oriented encapsulation of the primary attributes of a command to be executed against
    /// a database.
    /// </summary>
    public class FormClientCommand : ICloneable
    {
        /// <summary>
        /// Empty
        /// </summary>
        public static FormClientCommand Empty = new FormClientCommand();

        /// <summary>
        /// Type
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// ServerSide
            /// </summary>
            ServerSide,
            /// <summary>
            /// ClientSide
            /// </summary>
            ClientSide,
            /// <summary>
            /// LinkClientSide
            /// </summary>
            LinkClientSide,
            /// <summary>
            /// CustomType
            /// </summary>
            CustomType,
            /// <summary>
            /// Static
            /// </summary>
            Static
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormClientCommand"/> class.
        /// </summary>
        private FormClientCommand() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormClientCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="commandId">The command id.</param>
        public FormClientCommand(string name, string commandId)
            : this(name, commandId, string.Empty, (Nparams)null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormClientCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="commandId">The command id.</param>
        /// <param name="commandText">The command text.</param>
        public FormClientCommand(string name, string commandId, string commandText)
            : this(name, commandId, commandText, (Nparams)null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormClientCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="commandId">The command id.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="args">The args.</param>
        public FormClientCommand(string name, string commandId, string commandText, params string[] args)
            : this(name, commandId, commandText, Nparams.Parse(args)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormClientCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="commandId">The command id.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="args">The attrib.</param>
        public FormClientCommand(string name, string commandId, string commandText, Nparams args)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (commandId == null)
                throw new ArgumentNullException("commandId");
            if (commandText == null)
                throw new ArgumentNullException("commandText");
            Name = name;
            CommandId = commandId;
            CommandText = commandText;
            Nparams = args;
        }

        /// <summary>
        /// Gets or sets the attrib.
        /// </summary>
        /// <value>The attrib.</value>
        public Nparams Nparams { get; set; }

        /// <summary>
        /// Gets or sets the command id.
        /// </summary>
        /// <value>The command id.</value>
        public string CommandId { get; set; }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public Type CommandType { get; set; }

        /// <summary>
        /// Gets or sets the type of the custom.
        /// </summary>
        /// <value>The type of the custom.</value>
        public Type CustomType { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            if (this == Empty)
                return Empty;
            // clone
            return new FormClientCommand(Name, CommandId, CommandText)
            {
                Key = Key,
                CommandType = CommandType,
                Nparams = null, //new Nattrib(Attrib),
                CustomType = CustomType,
            };
        }
        #endregion
    }
}