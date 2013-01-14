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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Linq;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    /// <summary>
    /// ICommand
    /// </summary>
    [InheritedExport]
    public interface ICommand
    {
        /// <summary>
        /// Executes this instance.
        /// </summary>
        void Execute();
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        IList<string> Arguments { get; }
        /// <summary>
        /// Gets the command attribute.
        /// </summary>
        CommandAttribute CommandAttribute { get; }
    }

    /// <summary>
    /// Command
    /// </summary>
    public abstract class Command : ICommand
    {
        private CommandAttribute _commandAttribute;
        private const string CommandSuffix = "Command";

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        protected Command()
        {
            Arguments = new List<string>();
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            if (Help)
                HelpCommand.ViewHelpForCommand(CommandAttribute.CommandName);
            else
                ExecuteCommand();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void ExecuteCommand();

        /// <summary>
        /// Gets the command attribute.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does quite a bit of processing.")]
        public virtual CommandAttribute GetCommandAttribute()
        {
            var customAttributes = base.GetType().GetCustomAttributes(typeof(CommandAttribute), true);
            if (customAttributes.Any())
                return (CommandAttribute)customAttributes.FirstOrDefault();
            var name = base.GetType().Name;
            var length = name.LastIndexOf("Command", StringComparison.OrdinalIgnoreCase);
            if (length >= 0)
                name = name.Substring(0, length);
            return (!string.IsNullOrEmpty(name) ? new CommandAttribute(name, Local.DefaultCommandDescription) : null);
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IList<string> Arguments { get; private set; }

        /// <summary>
        /// Gets the command attribute.
        /// </summary>
        public CommandAttribute CommandAttribute
        {
            get
            {
                if (_commandAttribute == null)
                    _commandAttribute = GetCommandAttribute();
                return _commandAttribute;
            }
        }

        /// <summary>
        /// Gets or sets the console.
        /// </summary>
        /// <value>
        /// The console.
        /// </value>
        [Import]
        public IConsoleEx Console { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Command"/> is help.
        /// </summary>
        /// <value>
        ///   <c>true</c> if help; otherwise, <c>false</c>.
        /// </value>
        [Option("help", AltName = "?")]
        public bool Help { get; set; }

        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>
        /// The help command.
        /// </value>
        [Import]
        public HelpCommand HelpCommand { get; set; }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        [Import]
        public ICommandManager Manager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non interactive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non interactive]; otherwise, <c>false</c>.
        /// </value>
        [Option(typeof(Local), "Option_NonInteractive")]
        public bool NonInteractive { get; set; }

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        /// <value>
        /// The verbosity.
        /// </value>
        [Option(typeof(Local), "Option_Verbosity")]
        public ConsoleVerbosity Verbosity { get; set; }
    }
}
