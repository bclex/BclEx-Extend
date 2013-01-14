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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Contoso.Practices.ConsoleCommand.Commands
{
    [GeneratedCode("RazorGenerator", "1.1.0.0")]
    internal class HelpCommandMarkdownTemplate : HelpCommandMarkdownTemplateBase
    {
        public struct dynamic1
        {
            public string Name;
            public string Description;
        }

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
            foreach (dynamic1 option in Options)
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

        public IEnumerable<dynamic1> Options { get; set; }

        private TextInfo TextInfo
        {
            get { return CultureInfo.CurrentCulture.TextInfo; }
        }
    }
}

