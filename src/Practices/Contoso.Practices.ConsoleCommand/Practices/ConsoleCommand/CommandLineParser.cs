using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Contoso.Practices.ConsoleCommand.Commands;

namespace Contoso.Practices.ConsoleCommand
{
    /// <summary>
    /// CommandLineParser
    /// </summary>
    public class CommandLineParser
    {
        private readonly ICommandManager _commandManager;
        // On Unix or MacOSX slash as a switch indicator would interfere with the path separator
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        private static readonly bool _supportSlashAsSwitch = (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX);

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public CommandLineParser(ICommandManager manager)
        {
            _commandManager = manager;
        }

        /// <summary>
        /// Assigns the value.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="property">The property.</param>
        /// <param name="option">The option.</param>
        /// <param name="value">One or more semi-colon separated items that might support values also.</param>
        /// <example>
        /// Example of a list value : nuget pack -option "foo;bar;baz"
        /// Example of a keyvalue value: nuget pack -option "foo=bar;baz=false"
        /// </example>
        internal static void AssignValue(object command, PropertyInfo property, string option, object value)
        {
            try
            {
                if (property.PropertyType.GetInterface(typeof(ICollection<>), typeof(IDictionary<,>)) != null)
                {
                    var valueAsString = (value as string);
                    dynamic obj = property.GetValue(command, null);
                    foreach (string item in valueAsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        if (property.PropertyType.GetInterface(typeof(IDictionary<,>)) != null)
                        {
                            int index = item.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                            if (index > -1)
                                obj.Add(item.Substring(0, index), item.Substring(index + 1));
                            else
                                obj.Add(item, item);
                        }
                        else
                            obj.Add(item);
                }
                else if (property.PropertyType.IsEnum)
                {
                    value = GetPartialOptionMatch(Enum.GetValues(property.PropertyType).Cast<object>(), x => x.ToString(), x => x.ToString(), option, value.ToString());
                    property.SetValue(command, value, null);
                }
                else
                    property.SetValue(command, TypeHelper.ChangeType(value, property.PropertyType), null);
            }
            catch (CommandLineException) { throw; }
            catch { throw new CommandLineException(Local.InvalidOptionValueError, new object[] { option, value }); }
        }

        /// <summary>
        /// Extracts the options.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="argsE">The args.</param>
        public void ExtractOptions(ICommand command, IEnumerator<string> argsE)
        {
            var arguments = new List<string>();
            var properties = _commandManager.GetCommandOptions(command);
            for (var option = GetNextCommandLineItem(argsE); option != null; option = GetNextCommandLineItem(argsE))
            {
                if ((!option.StartsWith("/", StringComparison.OrdinalIgnoreCase) || !_supportSlashAsSwitch) && !option.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                    arguments.Add(option);
                else
                {
                    string value = null;
                    var optionText = option.Substring(1);
                    if (optionText.EndsWith("-", StringComparison.OrdinalIgnoreCase))
                    {
                        optionText = optionText.TrimEnd('-');
                        value = "false";
                    }
                    var property = GetPartialOptionMatch(properties, x => x.Value.Name, x => x.Key.AltName, option, optionText).Value;
                    if (property.PropertyType != typeof(bool))
                        value = GetNextCommandLineItem(argsE);
                    else if (value == null)
                        value = "true";
                    if (value == null)
                        throw new CommandLineException(Local.MissingOptionValueError, new object[] { option });
                    AssignValue(command, property, option, value);
                }
            }
            foreach (var argument in command.Arguments)
                command.Arguments.Add(argument);
        }

        /// <summary>
        /// Gets the next command line item.
        /// </summary>
        /// <param name="argsE">The args.</param>
        /// <returns></returns>
        public static string GetNextCommandLineItem(IEnumerator<string> argsE) { return (argsE != null && argsE.MoveNext() ? argsE.Current : null); }

        private static TVal GetPartialOptionMatch<TVal>(IEnumerable<TVal> source, Func<TVal, string> getDisplayName, Func<TVal, string> getAltName, string option, string value)
        {
            var r = source.Where(x =>
            {
                var altName = getAltName(x);
                return (getDisplayName(x).StartsWith(value, StringComparison.OrdinalIgnoreCase) || (altName != null && altName.StartsWith(value, StringComparison.OrdinalIgnoreCase)));
            });
            if (!r.Any<TVal>())
                throw new CommandLineException(Local.UnknownOptionError, new object[] { option });
            var match = r.FirstOrDefault();
            if (!r.Skip(1).Any())
                return match;
            match = r.FirstOrDefault(x => value.Equals(getDisplayName(x), StringComparison.OrdinalIgnoreCase) || value.Equals(getAltName(x), StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture, Local.AmbiguousCommand, new object[] { value, string.Join(" ", source.Select(x => getDisplayName(x))) }));
        }

        /// <summary>
        /// Parses the command line.
        /// </summary>
        /// <param name="args">The command line args.</param>
        /// <returns></returns>
        public ICommand ParseCommandLine(IEnumerable<string> args)
        {
            var argsE = args.GetEnumerator();
            var commandName = GetNextCommandLineItem(argsE);
            if (commandName == null)
                return null;
            var command = _commandManager.GetCommand(commandName);
            if (command == null)
                throw new CommandLineException(Local.UnknownCommandError, new object[] { commandName });
            ExtractOptions(command, argsE);
            return command;
        }
    }
}

