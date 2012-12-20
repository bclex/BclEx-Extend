using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    [GeneratedCode("RazorGenerator", "1.1.0.0")]
    internal class HelpCommandMarkdownTemplate : HelpCommandMarkdownTemplateBase
    {
        public override void Execute()
        {
            WriteLiteral("\r\n");
            WriteLiteral("\r\n\r\n##  ");
            Write(TextInfo.ToTitleCase(CommandAttribute.CommandName));
            WriteLiteral(" Command\r\n\r\n");
            Write(CommandAttribute.Description);
            WriteLiteral("\r\n\r\n### Usage\r\n    nuget ");
            Write(TextInfo.ToLower(CommandAttribute.CommandName));
            WriteLiteral(" ");
            Write(CommandAttribute.UsageSummary);
            WriteLiteral("\r\n");
            if (!string.IsNullOrEmpty(CommandAttribute.UsageDescription))
            {
                WriteLiteral("\r\n");
                if (CommandAttribute.UsageDescription == null) { }
                Write("");
            }
            WriteLiteral("\r\n### Options\r\n<table>\r\n");
            foreach (dynamic option in Options)
            {
                WriteLiteral("    <tr>\r\n        <td>");
                Write(option.Name);
                WriteLiteral("</td>\r\n        <td>");
                Write(option.Description);
                WriteLiteral("</td>\r\n    </tr>\r\n");
            }
            WriteLiteral("</table>\r\n");
            if (!string.IsNullOrEmpty(CommandAttribute.UsageExample))
            {
                WriteLiteral("\r\n### Examples\r\n\r\n    ");
                Write(CommandAttribute.UsageExample.Replace(Environment.NewLine, Environment.NewLine + "    "));
                WriteLiteral("\r\n");
                WriteLiteral("\r\n");
            }
        }

        public string TransformText()
        {
            Execute();
            return GenerationEnvironment.ToString();
        }

        public CommandAttribute CommandAttribute { get; set; }

        public IEnumerable<dynamic> Options { get; set; }

        private TextInfo TextInfo
        {
            get { return CultureInfo.CurrentCulture.TextInfo; }
        }
    }
}

