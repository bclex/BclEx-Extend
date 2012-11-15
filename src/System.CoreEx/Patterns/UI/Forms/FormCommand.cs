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
    public class FormCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public const string InitCommandId = "Init";
        /// <summary>
        /// 
        /// </summary>
        public const string ResetCommandId = "Reset";
        private string _commandId = "Init";
        private string _commandText = string.Empty;
        private object _commandArgument;
        private string _commandTargetId = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormCommand"/> class.
        /// </summary>
        public FormCommand() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormCommand"/> class.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        public FormCommand(string commandId)
        {
            if (string.IsNullOrEmpty(commandId))
                throw new ArgumentNullException("commandId");
            _commandId = commandId;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormCommand"/> class.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        /// <param name="commandText">The command text.</param>
        public FormCommand(string commandId, string commandText)
        {
            if (string.IsNullOrEmpty(commandId))
                throw new ArgumentNullException("commandId");
            _commandId = commandId;
            _commandText = (commandText ?? string.Empty);
        }

        /// <summary>
        /// Resets this instance to a non-initalized state.
        /// </summary>
        public void Clear()
        {
            _commandId = InitCommandId;
            _commandTargetId = string.Empty;
            _commandArgument = null;
            _commandText = string.Empty;
        }

        /// <summary>
        /// Sets the CommandId property based on the name and argument values provided.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="argument">The argument.</param>
        public void SetCommand(string name, object argument)
        {
            _commandId = name.ParseBoundedPrefix("[]", out _commandTargetId);
            if (string.IsNullOrEmpty(_commandId))
                throw new ArgumentNullException("name", "Core_.Local.UndefinedCommandId");
            CommandArgument = argument;
        }

        /// <summary>
        /// Gets or sets the command argument.
        /// </summary>
        /// <value>The command argument.</value>
        public object CommandArgument
        {
            get { return _commandArgument; }
            set
            {
                string valueText = (value as string);
                if (valueText != null)
                {
                    _commandArgument = valueText.Split(';');
                    _commandText = valueText;
                }
                else
                {
                    _commandArgument = value;
                    _commandText = (value != null ? value.ToString() : string.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the command id.
        /// </summary>
        /// <value>The command id.</value>
        public string CommandId
        {
            get { return _commandId; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                _commandId = value;
            }
        }

        /// <summary>
        /// Gets or sets the command target id.
        /// </summary>
        /// <value>The command target id.</value>
        public string CommandTargetId
        {
            get { return _commandTargetId; }
            set { _commandTargetId = (value ?? string.Empty); }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get { return _commandText; }
            set
            {
                _commandArgument = null;
                _commandText = (value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CommandId for this class begins with "@".
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is check data; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckData
        {
            get { return ((!string.IsNullOrEmpty(_commandId)) && (_commandId.Substring(0, 1) == "@")); }
        }

        /// <summary>
        /// Gets a value indicating whether the CommandId for this instance is either 'Init' or 'Reset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the CommandId is either 'Init' or 'Reset'; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew
        {
            get { return ((!string.IsNullOrEmpty(_commandId)) && ((_commandId == InitCommandId) || (_commandId == ResetCommandId) || (_commandId[0] == 'i'))); }
        }

        /// <summary>
        /// Gets a value indicating whether the CommandId is equal 'Init'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is equal to 'Init'; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewByInit
        {
            get { return (_commandId == InitCommandId); }
        }

        /// <summary>
        /// Gets a value indicating whether the CommandId is equal 'Reset'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is equal to 'Reset'; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewByReset
        {
            get { return (_commandId == ResetCommandId); }
        }
    }
}