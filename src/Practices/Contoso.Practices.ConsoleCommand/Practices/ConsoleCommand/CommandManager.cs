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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Contoso.Practices.ConsoleCommand.Commands;

namespace Contoso.Practices.ConsoleCommand
{
    /// <summary>
    /// ICommandManager
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns></returns>
        ICommand GetCommand(string commandName);
        /// <summary>
        /// Gets the command options.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        IDictionary<OptionAttribute, PropertyInfo> GetCommandOptions(ICommand command);
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method would do reflection and a property would be inappropriate.")]
        IEnumerable<ICommand> GetCommands();
        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="command">The command.</param>
        void RegisterCommand(ICommand command);
    }

    /// <summary>
    /// CommandManager
    /// </summary>
    [Export(typeof(ICommandManager))]
    public class CommandManager : ICommandManager
    {
        private readonly IList<ICommand> _commands = new List<ICommand>();

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns></returns>
        public ICommand GetCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentNullException("commandName");
            var r = _commands.Where(x =>
            {
                var altName = x.CommandAttribute.AltName;
                return (x.CommandAttribute.CommandName.StartsWith(commandName, StringComparison.OrdinalIgnoreCase) || (altName != null && altName.StartsWith(commandName, StringComparison.OrdinalIgnoreCase)));
            });
            if (!r.Any())
                throw new CommandLineException(Local.UnknownCommandError, new object[] { commandName });
            var command = r.First();
            if (!r.Skip(1).Any())
                return command;
            command = r.FirstOrDefault(x => commandName.Equals(x.CommandAttribute.CommandName, StringComparison.OrdinalIgnoreCase) || commandName.Equals(x.CommandAttribute.AltName, StringComparison.OrdinalIgnoreCase));
            if (command != null)
                return command;
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture, Local.AmbiguousCommand, new object[] { commandName, string.Join(" ", r.Select(x => x.CommandAttribute.CommandName).ToArray()) }));
        }

        /// <summary>
        /// Gets the command options.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public IDictionary<OptionAttribute, PropertyInfo> GetCommandOptions(ICommand command)
        {
            var dictionary = new Dictionary<OptionAttribute, PropertyInfo>();
            foreach (PropertyInfo propInfo in command.GetType().GetProperties())
                foreach (OptionAttribute attribute in propInfo.GetCustomAttributes(typeof(OptionAttribute), true))
                {
                    if (!propInfo.CanWrite && propInfo.PropertyType.GetInterface(typeof(ICollection<>), typeof(IDictionary<,>)) == null)
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Local.OptionInvalidWithoutSetter, new object[] { command.GetType().FullName + "." + propInfo.Name }));
                    dictionary.Add(attribute, propInfo);
                }
            return dictionary;
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICommand> GetCommands() { return _commands; }

        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void RegisterCommand(ICommand command)
        {
            if (command.CommandAttribute != null)
                _commands.Add(command);
        }
    }
}

