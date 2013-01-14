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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    /// <summary>
    /// HelpCommand
    /// </summary>
    [Command(typeof(Local), "help", "HelpCommandDescription", AltName = "?", MaxArgs = 1, UsageSummaryResourceName = "HelpCommandUsageSummary", UsageDescriptionResourceName = "HelpCommandUsageDescription", UsageExampleResourceName = "HelpCommandUsageExamples"), Export(typeof(HelpCommand))]
    public class HelpCommand : Command
    {
        private readonly string _commandExe;
        private readonly ICommandManager _commandManager;
        private readonly string _helpUrl;
        private readonly string _productName;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        [ImportingConstructor]
        public HelpCommand(ICommandManager commandManager)
            : this(commandManager, Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name, "REFERENCE") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        /// <param name="commandExe">The command exe.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="helpUrl">The help URL.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "We don't use the Url for anything besides printing, so it's ok to represent it as a string.")]
        public HelpCommand(ICommandManager commandManager, string commandExe, string productName, string helpUrl)
        {
            _commandManager = commandManager;
            _commandExe = commandExe;
            _productName = productName;
            _helpUrl = helpUrl;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void ExecuteCommand()
        {
            if (!string.IsNullOrEmpty(CommandName))
                ViewHelpForCommand(CommandName);
            else if (All && Markdown)
                ViewMarkdownHelp();
            else if (All)
                ViewHelpForAllCommands();
            else
                ViewHelp();
        }

        private static string GetAltText(string altNameText) { return (string.IsNullOrEmpty(altNameText) ? string.Empty : string.Format(CultureInfo.CurrentCulture, " ({0})", altNameText)); }
        private static string GetCommandText(CommandAttribute commandAttribute) { return (commandAttribute.CommandName + GetAltText(commandAttribute.AltName)); }

        private void PrintCommand(int maxWidth, CommandAttribute commandAttribute)
        {
            Console.Write(" {0, -" + maxWidth + "}   ", GetCommandText(commandAttribute));
            var startIndex = maxWidth + 4;
            Console.PrintJustified(startIndex, commandAttribute.Description);
        }

        /// <summary>
        /// Views the help.
        /// </summary>
        public void ViewHelp()
        {
            Console.WriteLine("{0} Version: {1}", _productName, GetType().Assembly.GetName().Version);
            Console.WriteLine("usage: {0} <command> [args] [options] ", _commandExe);
            Console.WriteLine("Type '{0} help <command>' for help on a specific command.", _commandExe);
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            var commands = _commandManager.GetCommands().OrderBy(x => x.CommandAttribute.CommandName).Select(x => x.CommandAttribute);
            var maxWidth = commands.Max(x => x.CommandName.Length + GetAltText(x.AltName).Length);
            foreach (var command in commands)
                PrintCommand(maxWidth, command);
            if (_helpUrl != null)
            {
                Console.WriteLine();
                Console.WriteLine("For more information, visit {0}", _helpUrl);
            }
        }

        private void ViewHelpForAllCommands()
        {
            var commands = _commandManager.GetCommands().OrderBy(x => x.CommandAttribute.CommandName).Select(x => x.CommandAttribute);
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            foreach (var command in commands)
            {
                Console.WriteLine(textInfo.ToTitleCase(command.CommandName) + " Command");
                ViewHelpForCommand(command.CommandName);
            }
        }

        /// <summary>
        /// Views the help for command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        public void ViewHelpForCommand(string commandName)
        {
            var command = _commandManager.GetCommand(commandName);
            var attribute = command.CommandAttribute;
            Console.WriteLine("usage: {0} {1} {2}", _commandExe, attribute.CommandName, attribute.UsageSummary);
            Console.WriteLine();
            if (!string.IsNullOrEmpty(attribute.AltName))
            {
                Console.WriteLine("alias: {0}", attribute.AltName);
                Console.WriteLine();
            }
            Console.WriteLine(attribute.Description);
            Console.WriteLine();
            if (attribute.UsageDescription != null)
            {
                Console.PrintJustified(5, attribute.UsageDescription);
                Console.WriteLine();
            }
            var options = _commandManager.GetCommandOptions(command);
            if (options.Count > 0)
            {
                Console.WriteLine("options:");
                Console.WriteLine();
                var maxOptionWidth = options.Max(x => x.Value.Name.Length) + 2;
                var maxAltOptionWidth = options.Max(x => x.Key.AltName != null ? x.Key.AltName.Length : 0);
                foreach (KeyValuePair<OptionAttribute, PropertyInfo> pair in options)
                {
                    Console.Write(" -{0, -" + (maxOptionWidth + 2) + "}", pair.Value.Name + (pair.Value.PropertyType.GetInterface(typeof(ICollection<>), typeof(IDictionary<,>)) != null ? " +" : string.Empty));
                    Console.Write(" {0, -" + (maxAltOptionWidth + 4) + "}", GetAltText(pair.Key.AltName));
                    Console.PrintJustified((10 + maxAltOptionWidth) + maxOptionWidth, pair.Key.Description);
                }
                if (_helpUrl != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("For more information, visit {0}", _helpUrl);
                }
                Console.WriteLine();
            }
        }

        private void ViewMarkdownHelp()
        {
            var commands = _commandManager.GetCommands().OrderBy(x => x.CommandAttribute.CommandName);
            foreach (var command in commands)
            {
                var template = new HelpCommandMarkdownTemplate
                {
                    CommandAttribute = command.CommandAttribute,
                    Options = _commandManager.GetCommandOptions(command).Select(x => new HelpCommandMarkdownTemplate.dynamic1 { Name = x.Value.Name, Description = x.Key.Description })
                };
                Console.WriteLine(template.TransformText());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HelpCommand"/> is all.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all; otherwise, <c>false</c>.
        /// </value>
        [Option(typeof(Local), "HelpCommandAll")]
        public bool All { get; set; }

        private string CommandName
        {
            get { return (Arguments != null && Arguments.Count > 0 ? Arguments[0] : null); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HelpCommand"/> is markdown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if markdown; otherwise, <c>false</c>.
        /// </value>
        [Option(typeof(Local), "HelpCommandMarkdown")]
        public bool Markdown { get; set; }
    }
}

