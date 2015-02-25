using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Text.Lalr.Emitters
{
    public partial class EmitterC
    {
        // The following routine emits code for the destructor for the symbol sp
        private static void EmitDestructor(StreamWriter w, Symbol symbol, Context ctx, ref int lineno, string filePath)
        {
            string z;
            if (symbol.Type == SymbolType.Terminal)
            {
                z = ctx.TokenDestructor;
                if (z == null) return;
                w.WriteLine(ref lineno, "{");
            }
            else if (symbol.Destructor != null)
            {
                z = symbol.Destructor;
                w.WriteLine(ref lineno, "{");
                if (!ctx.NoShowLinenos) Template.WriteLineInfo(ref lineno, w, symbol.DestructorLineno, ctx.InputFilePath);
            }
            else if (ctx.DefaultDestructor != null)
            {
                z = ctx.DefaultDestructor;
                if (z == null) return;
                w.WriteLine(ref lineno, "{");
            }
            else
                throw new InvalidOperationException();
            var cp = 0;
            for (; cp < z.Length; cp++)
            {
                if (z[cp] == '$' && z[cp + 1] == '$')
                {
                    w.Write("(yypminor->yy{0})", symbol.DataTypeID);
                    cp++;
                    continue;
                }
                if (z[cp] == '\n') lineno++;
                w.Write(z[cp]);
            }
            w.WriteLine(ref lineno);
            if (!ctx.NoShowLinenos) Template.WriteLineInfo(ref lineno, w, lineno, filePath);
            w.WriteLine(ref lineno, "}");
            return;
        }

        // Return TRUE (non-zero) if the given symbol has a destructor.
        private static bool HasDestructor(Symbol symbol, Context ctx)
        {
            return (symbol.Type == SymbolType.Terminal ? ctx.TokenDestructor != null : ctx.DefaultDestructor != null || symbol.Destructor != null);
        }

        // zCode is a string that is the action associated with a rule.  Expand the symbols in this string so that the refer to elements of the parser stack.
        private static void TranslateRuleCode(Context ctx, Rule rule)
        {
            var used = new bool[rule.RHSymbols.Length]; /* True for each RHS element which is used */
            var lhsused = false; /* True if the LHS element has been used */
            if (rule.Code == null) { rule.Code = "\n"; rule.Lineno = rule.RuleLineno; }
            var b = new StringBuilder();
            var z = rule.Code;
            for (var cp = 0; cp < z.Length; cp++)
            {
                if (char.IsLetter(z[cp]) && (cp == 0 || (!char.IsLetterOrDigit(z[cp - 1]) && z[cp - 1] != '_')))
                {
                    var xp = cp + 1;
                    for (; char.IsLetterOrDigit(z[xp]) || z[xp] == '_'; xp++) ;
                    //var saved = z[xp];
                    var xpLength = xp - cp;
                    if (rule.LHSymbolAlias != null && z.Substring(cp, xpLength) == rule.LHSymbolAlias)
                    {
                        b.AppendFormat("yygotominor.yy{0}", rule.LHSymbol.DataTypeID);
                        cp = xp;
                        lhsused = true;
                    }
                    else
                    {
                        for (var i = 0; i < rule.RHSymbols.Length; i++)
                            if (rule.RHSymbolsAlias[i] != null && z.Substring(cp, xpLength) == rule.RHSymbolsAlias[i])
                            {
                                // If the argument is of the form @X then substituted the token number of X, not the value of X
                                if (cp > 0 && z[cp - 1] == '@')
                                {
                                    b.Length--;
                                    b.AppendFormat("yymsp[{0}].major", i - rule.RHSymbols.Length + 1);
                                }
                                else
                                {
                                    var symbol = rule.RHSymbols[i];
                                    var dataTypeID = (symbol.Type == SymbolType.MultiTerminal ? symbol.Children[0].DataTypeID : symbol.DataTypeID);
                                    b.AppendFormat("yymsp[{0}].minor.yy{1}", i - rule.RHSymbols.Length + 1, dataTypeID);
                                }
                                cp = xp;
                                used[i] = true;
                                break;
                            }
                    }
                }
                b.Append(z[cp]);
            }
            // Check to make sure the LHS has been used
            if (rule.LHSymbolAlias != null && !lhsused)
                ctx.RaiseError(ref ctx.Errors, rule.RuleLineno, "Label \"{0}\" for \"{1}({2})\" is never used.", rule.LHSymbolAlias, rule.LHSymbol.Name, rule.LHSymbolAlias);
            // Generate destructor code for RHS symbols which are not used in the reduce code
            for (var i = 0; i < rule.RHSymbols.Length; i++)
            {
                if (rule.RHSymbolsAlias[i] != null && !used[i])
                    ctx.RaiseError(ref ctx.Errors, rule.RuleLineno, "Label {0} for \"{1}({2})\" is never used.", rule.RHSymbolsAlias[i], rule.RHSymbols[i].Name, rule.RHSymbolsAlias[i]);
                else if (rule.RHSymbolsAlias[i] == null && HasDestructor(rule.RHSymbols[i], ctx))
                    b.AppendFormat("    yy_destructor(yyparser, {0}, &yymsp[{1}].minor);\n", rule.RHSymbols[i].ID, i - rule.RHSymbols.Length + 1);
            }
            if (rule.Code != null)
                rule.Code = (b.ToString() ?? string.Empty);
        }

        // Generate code which executes when the rule "rp" is reduced.  Write the code to "@out".  Make sure lineno stays up-to-date.
        private static void EmitRuleCode(StreamWriter w, Rule rule, Context ctx, ref int lineno, string filePath)
        {
            /* Generate code to do the reduce action */
            if (rule.Code != null)
            {
                if (!ctx.NoShowLinenos) Template.WriteLineInfo(ref lineno, w, rule.Lineno, ctx.InputFilePath);
                w.CheckedWriteLine(ref lineno, "{{{0}}}", rule.Code);
                if (!ctx.NoShowLinenos) Template.WriteLineInfo(ref lineno, w, lineno, filePath);
            }
        }

        // Print the definition of the union used for the parser's data stack. This union contains fields for every possible data type for tokens
        // and nonterminals.  In the process of computing and printing this union, also set the ".dtnum" field of every terminal and nonterminal symbol.
        private static void EmitStackUnion(StreamWriter w, Context ctx, ref int lineno, bool makeHeaders)
        {
            // Print out the definition of YYTOKENTYPE and YYMINORTYPE
            var name = (ctx.Name ?? "Parse");
            if (makeHeaders) { w.WriteLine(ref lineno, "#if INTERFACE"); }
            w.WriteLine(ref lineno, "#define {0}TOKENTYPE {1}", name, (ctx.TokenType ?? "void *"));
            if (makeHeaders) { w.WriteLine(ref lineno, "#endif"); }
            w.WriteLine(ref lineno, "typedef union");
            w.WriteLine(ref lineno, "{");
            w.WriteLine(ref lineno, "   int yyinit;");
            w.WriteLine(ref lineno, "   {0}TOKENTYPE yy0;", name);
            foreach (var type in ctx.DataTypes)
                w.WriteLine(ref lineno, "   {0} yy{1};", type.Key, type.Value);
            if (ctx.ErrorSymbol.Uses > 0)
                w.WriteLine(ref lineno, "   int yy{0};", ctx.ErrorSymbol.DataTypeID);
            w.WriteLine(ref lineno, "} YYMINORTYPE;");
        }

        // Return the name of a C datatype able to represent values between lwr and upr, inclusive.
        private static string GetMinimumSizeType(int lwr, int upr)
        {
            if (lwr >= 0)
            {
                if (upr <= 255)
                    return "unsigned char";
                else if (upr < 65535)
                    return "unsigned short int";
                else
                    return "unsigned int";
            }
            else if (lwr >= -127 && upr <= 127)
                return "signed char";
            else if (lwr >= -32767 && upr < 32767)
                return "short";
            else
                return "int";
        }

        // Write text on "w" that describes the rule "rule".
        private static void WriteRuleText(StreamWriter w, Rule rule)
        {
            w.Write("{0} ::=", rule.LHSymbol.Name);
            for (var j = 0; j < rule.RHSymbols.Length; j++)
            {
                var symbol = rule.RHSymbols[j];
                w.Write(" {0}", symbol.Name);
                if (symbol.Type == SymbolType.MultiTerminal)
                    for (var k = 1; k < symbol.Children.Length; k++)
                        w.Write("|{0}", symbol.Children[k].Name);
            }
        }

        /// <summary>
        /// Emits the table.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="r">The r.</param>
        /// <param name="w">The w.</param>
        /// <param name="makeHeaders">if set to <c>true</c> [make headers].</param>
        /// <param name="filePath">The file path.</param>
        public static void EmitTable(Context ctx, StreamReader r, StreamWriter w, bool makeHeaders, string filePath)
        {
            var lineno = 1;
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate the include code, if any
            Template.Write(w, ctx, ctx.Include, ref lineno, filePath);
            if (makeHeaders)
            {
                // TODO: get headerfile
                var filePathH = filePath;
                w.WriteLine(ref lineno, "#include \"{0}\"", filePathH);
            }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate #defines for all tokens
            if (makeHeaders)
            {
                w.WriteLine(ref lineno, "#if INTERFACE");
                var prefix = (ctx.TokenPrefix ?? string.Empty);
                for (var i = 1; i < ctx.Terminals; i++)
                    w.WriteLine(ref lineno, "#define {0}{1,-30} {2,2}", prefix, ctx.Symbols[i].Name, i);
                w.WriteLine(ref lineno, "#endif");
            }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate the defines
            w.WriteLine(ref lineno, "#define YYCODETYPE {0}", GetMinimumSizeType(0, ctx.Symbols.Length));
            w.WriteLine(ref lineno, "#define YYNOCODE {0}", ctx.Symbols.Length);
            w.WriteLine(ref lineno, "#define YYACTIONTYPE {0}", GetMinimumSizeType(0, ctx.States + ctx.Rules + 5));
            if (ctx.Wildcard != null)
                w.WriteLine(ref lineno, "#define YYWILDCARD {0}", ctx.Wildcard.ID);
            EmitStackUnion(w, ctx, ref lineno, makeHeaders);
            w.WriteLine(ref lineno, "#ifndef YYSTACKDEPTH");
            if (ctx.StackSize != null)
                w.WriteLine(ref lineno, "#define YYSTACKDEPTH {0}", ctx.StackSize);
            else
                w.WriteLine(ref lineno, "#define YYSTACKDEPTH 100");
            w.WriteLine(ref lineno, "#endif");
            if (makeHeaders)
                w.WriteLine(ref lineno, "#if INTERFACE");
            var name = (ctx.Name ?? "Parse");
            if (!string.IsNullOrEmpty(ctx.ExtraArg))
            {
                var i = ctx.ExtraArg.Length;
                while (i >= 1 && char.IsWhiteSpace(ctx.ExtraArg[i - 1])) i--;
                while (i >= 1 && (char.IsLetterOrDigit(ctx.ExtraArg[i - 1]) || ctx.ExtraArg[i - 1] == '_')) i--;
                var extraArg = ctx.ExtraArg;
                var extraArg2 = ctx.ExtraArg.Substring(i);
                //var extraArg3 = char.ToUpperInvariant(extraArg2[0]) + extraArg2.Substring(1);
                w.WriteLine(ref lineno, "#define {0}ARG_SDECL {1};", name, extraArg);
                w.WriteLine(ref lineno, "#define {0}ARG_PDECL ,{1}", name, extraArg);
                w.WriteLine(ref lineno, "#define {0}ARG_FETCH {1} = yyparser->{2}", name, extraArg, extraArg2);
                w.WriteLine(ref lineno, "#define {0}ARG_STORE yyparser->{1} = {2}", name, extraArg2, extraArg2);
            }
            else
            {
                w.WriteLine(ref lineno, "#define {0}ARG_SDECL", name);
                w.WriteLine(ref lineno, "#define {0}ARG_PDECL", name);
                w.WriteLine(ref lineno, "#define {0}ARG_FETCH", name);
                w.WriteLine(ref lineno, "#define {0}ARG_STORE", name);
            }
            if (makeHeaders)
                w.WriteLine(ref lineno, "#endif");
            w.WriteLine(ref lineno, "#define YYNSTATE {0}", ctx.States);
            w.WriteLine(ref lineno, "#define YYNRULE {0}", ctx.Rules);
            if (ctx.ErrorSymbol.Uses > 0)
            {
                w.WriteLine(ref lineno, "#define YYERRORSYMBOL {0}", ctx.ErrorSymbol.ID);
                w.WriteLine(ref lineno, "#define YYERRSYMDT yy{0}", ctx.ErrorSymbol.DataTypeID);
            }
            if (ctx.HasFallback)
                w.WriteLine(ref lineno, "#define YYFALLBACK 1");
            Template.Transfer(ctx.Name, r, w, ref lineno);

            int maxTokenOffset;
            int minTokenOffset;
            int maxNonTerminalOffset;
            int minNonTerminalOffset;
            var actionTable = EmitterActionTable.Make(ctx, out maxTokenOffset, out minTokenOffset, out maxNonTerminalOffset, out minNonTerminalOffset);

            // Output the yy_action table
            var n = actionTable.Size;
            w.WriteLine(ref lineno, "#define YY_ACTTAB_COUNT ({0})", n);
            w.WriteLine(ref lineno, "__constant__ static const YYACTIONTYPE yy_action[] = {");
            for (int i = 0, j = 0; i < n; i++)
            {
                var action = actionTable.GetAction(i);
                if (action < 0)
                    action = ctx.States + ctx.Rules + 2;
                if (j == 0)
                    w.Write(" /* {0,5} */ ", i);
                w.Write(" {0,4},", action);
                if (j == 9 || i == n - 1)
                {
                    w.WriteLine(ref lineno);
                    j = 0;
                }
                else
                    j++;
            }
            w.WriteLine(ref lineno, "};");

            // Output the yy_lookahead table
            w.WriteLine(ref lineno, "__constant__ static const YYCODETYPE yy_lookahead[] = {");
            for (int i = 0, j = 0; i < n; i++)
            {
                var lookahead = actionTable.GetLookahead(i);
                if (lookahead < 0)
                    lookahead = ctx.Symbols.Length - 1;
                if (j == 0)
                    w.Write(" /* {0,5} */ ", i);
                w.Write(" {0,4},", lookahead);
                if (j == 9 || i == n - 1)
                {
                    w.WriteLine(ref lineno);
                    j = 0;
                }
                else
                    j++;
            }
            w.WriteLine(ref lineno, "};");

            // Output the yy_shift_ofst[] table
            w.WriteLine(ref lineno, "#define YY_SHIFT_USE_DFLT ({0})", minTokenOffset - 1);
            n = ctx.States;
            while (n > 0 && ctx.Sorted[n - 1].TokenOffset == State.NO_OFFSET) n--;
            w.WriteLine(ref lineno, "#define YY_SHIFT_COUNT ({0})", n - 1);
            w.WriteLine(ref lineno, "#define YY_SHIFT_MIN   ({0})", minTokenOffset);
            w.WriteLine(ref lineno, "#define YY_SHIFT_MAX   ({0})", maxTokenOffset);
            w.WriteLine(ref lineno, "__constant__ static const {0} yy_shift_ofst[] = {{", GetMinimumSizeType(minTokenOffset - 1, maxTokenOffset));
            for (int i = 0, j = 0; i < n; i++)
            {
                var state = ctx.Sorted[i];
                var offset = state.TokenOffset;
                if (offset == State.NO_OFFSET) offset = minTokenOffset - 1;
                if (j == 0) w.Write(" /* {0,5} */ ", i);
                w.Write(" {0,4},", offset);
                if (j == 9 || i == n - 1)
                {
                    w.WriteLine(ref lineno);
                    j = 0;
                }
                else
                    j++;
            }
            w.WriteLine(ref lineno, "};");

            // Output the yy_reduce_ofst[] table
            w.WriteLine(ref lineno, "#define YY_REDUCE_USE_DFLT ({0})", minNonTerminalOffset - 1);
            n = ctx.States;
            while (n > 0 && ctx.Sorted[n - 1].NonTerminalOffset == State.NO_OFFSET) n--;
            w.WriteLine(ref lineno, "#define YY_REDUCE_COUNT ({0})", n - 1);
            w.WriteLine(ref lineno, "#define YY_REDUCE_MIN   ({0})", minNonTerminalOffset);
            w.WriteLine(ref lineno, "#define YY_REDUCE_MAX   ({0})", maxNonTerminalOffset);
            w.WriteLine(ref lineno, "__constant__ static const {0} yy_reduce_ofst[] = {{", GetMinimumSizeType(minNonTerminalOffset - 1, maxNonTerminalOffset));
            for (int i = 0, j = 0; i < n; i++)
            {
                var state = ctx.Sorted[i];
                var offset = state.NonTerminalOffset;
                if (offset == State.NO_OFFSET) offset = minNonTerminalOffset - 1;
                if (j == 0) w.Write(" /* {0,5} */ ", i);
                w.Write(" {0,4},", offset);
                if (j == 9 || i == n - 1)
                {
                    w.WriteLine(ref lineno);
                    j = 0;
                }
                else
                    j++;
            }
            w.WriteLine(ref lineno, "};");

            // Output the default action table
            w.WriteLine(ref lineno, "__constant__ static const YYACTIONTYPE yy_default[] = {");
            n = ctx.States;
            for (int i = 0, j = 0; i < n; i++)
            {
                var state = ctx.Sorted[i];
                if (j == 0) w.Write(" /* {0,5} */ ", i);
                w.Write(" {0,4},", state.Default);
                if (j == 9 || i == n - 1)
                {
                    w.WriteLine(ref lineno);
                    j = 0;
                }
                else
                    j++;
            }
            w.WriteLine(ref lineno, "};");
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate the table of fallback tokens.
            if (ctx.HasFallback)
            {
                var mx = ctx.Terminals - 1;
                while (mx > 0 && ctx.Symbols[mx].Fallback == null) mx--;
                for (var i = 0; i <= mx; i++)
                {
                    var symbol = ctx.Symbols[i];
                    if (symbol.Fallback == null)
                        w.WriteLine(ref lineno, "    0, // {0,10} => nothing", symbol.Name);
                    else
                        w.WriteLine(ref lineno, "  {0,3},   // {1,10} => {2}", symbol.Fallback.ID, symbol.Name, symbol.Fallback.Name);
                }
            }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate a table containing the symbolic name of every symbol
            var i_ = 0;
            for (i_ = 0; i_ < ctx.Symbols.Length - 1; i_++)
            {
                w.Write("  {0,-15}", string.Format("\"{0}\",", ctx.Symbols[i_].Name));
                if ((i_ & 3) == 3) { w.WriteLine(ref lineno); }
            }
            if ((i_ & 3) != 0) { w.WriteLine(ref lineno); }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate a table containing a text string that describes every rule in the rule set of the grammar.  This information is used when tracing REDUCE actions.
            i_ = 0;
            for (var rule = ctx.Rule; rule != null; rule = rule.Next, i_++)
            {
                Debug.Assert(rule.ID == i_);
                w.Write(" /* {0,3} */ \"", i_);
                WriteRuleText(w, rule);
                w.WriteLine(ref lineno, "\",");
            }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which executes every time a symbol is popped from the stack while processing errors or while destroying the parser.  (In other words, generate the %destructor actions)
            if (ctx.TokenDestructor != null)
            {
                var once = true;
                for (var i = 0; i < ctx.Symbols.Length - 1; i++)
                {
                    var symbol = ctx.Symbols[i];
                    if (symbol == null || symbol.Type != SymbolType.Terminal) continue;
                    if (once)
                    {
                        w.WriteLine(ref lineno, "      // TERMINAL Destructor");
                        once = false;
                    }
                    w.WriteLine(ref lineno, "    case {0}: // {1}", symbol.ID, symbol.Name);
                }
                var i2 = 0;
                for (; i2 < (ctx.Symbols.Length - 1) && (ctx.Symbols[i2].Type != SymbolType.Terminal); i2++) ;
                if (i2 < ctx.Symbols.Length - 1)
                {
                    EmitDestructor(w, ctx.Symbols[i2], ctx, ref lineno, filePath);
                    w.WriteLine(ref lineno, "      break;");
                }
            }
            if (ctx.DefaultDestructor != null)
            {
                var dflt_sp = (Symbol)null;
                var once = true;
                for (var i = 0; i < ctx.Symbols.Length - 1; i++)
                {
                    var symbol = ctx.Symbols[i];
                    if (symbol == null || symbol.Type == SymbolType.Terminal || symbol.ID <= 0 || symbol.Destructor != null) continue;
                    if (once)
                    {
                        w.WriteLine(ref lineno, "      // Default NON-TERMINAL Destructor");
                        once = false;
                    }
                    w.WriteLine(ref lineno, "    case {0}: // {1}", symbol.ID, symbol.Name);
                    dflt_sp = symbol;
                }
                if (dflt_sp != null)
                    EmitDestructor(w, dflt_sp, ctx, ref lineno, filePath);
                w.WriteLine(ref lineno, "      break;");
            }
            for (var i = 0; i < ctx.Symbols.Length - 1; i++)
            {
                var symbol = ctx.Symbols[i];
                if (symbol == null || symbol.Type == SymbolType.Terminal || symbol.Destructor == null) continue;
                w.WriteLine(ref lineno, "    case {0}: // {1}", symbol.ID, symbol.Name);
                // Combine duplicate destructors into a single case
                for (var j = i + 1; j < ctx.Symbols.Length - 1; j++)
                {
                    var symbol2 = ctx.Symbols[j];
                    if (symbol2 != null && symbol2.Type != SymbolType.Terminal && symbol2.Destructor != null && symbol2.DataTypeID == symbol.DataTypeID && symbol.Destructor == symbol2.Destructor)
                    {
                        w.WriteLine(ref lineno, "    case {0}: // {1}", symbol2.ID, symbol2.Name);
                        symbol2.Destructor = null;
                    }
                }
                EmitDestructor(w, ctx.Symbols[i], ctx, ref lineno, filePath);
                w.WriteLine(ref lineno, "      break;");
            }
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which executes whenever the parser stack overflows
            Template.Write(w, ctx, ctx.StackOverflow, ref lineno, filePath);
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate the table of rule information 
            // Note: This code depends on the fact that rules are number sequentually beginning with 0.
            for (var rule = ctx.Rule; rule != null; rule = rule.Next)
                w.WriteLine(ref lineno, "  {{ {0}, {1} }},", rule.LHSymbol.ID, rule.RHSymbols.Length);
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which execution during each REDUCE action
            for (var rule = ctx.Rule; rule != null; rule = rule.Next)
                TranslateRuleCode(ctx, rule);
            // First output rules other than the default: rule
            for (var rule = ctx.Rule; rule != null; rule = rule.Next)
            {
                if (rule.Code == null) continue;
                if (rule.Code[0] == '\n' && rule.Code.Length == 1) continue; // Will be default:
                w.Write("      case {0}: /* ", rule.ID);
                WriteRuleText(w, rule);
                w.WriteLine(ref lineno, " */");
                for (var rule2 = rule.Next; rule2 != null; rule2 = rule2.Next)
                    if (rule2.Code == rule.Code)
                    {
                        w.Write("      case {0}: /* ", rule2.ID);
                        WriteRuleText(w, rule2);
                        w.WriteLine(ref lineno, " */ yyASSERTCOVERAGE(yyruleno == {0});", rule2.ID);
                        rule2.Code = null;
                    }
                EmitRuleCode(w, rule, ctx, ref lineno, filePath);
                w.WriteLine(ref lineno, "        break;");
                rule.Code = null;
            }
            // Finally, output the default: rule.  We choose as the default: all empty actions.
            w.WriteLine(ref lineno, "      default:");
            for (var rule = ctx.Rule; rule != null; rule = rule.Next)
            {
                if (rule.Code == null) continue;
                Debug.Assert(rule.Code[0] == '\n' && rule.Code.Length == 1);
                w.Write("      /* ({0}) ", rule.ID);
                WriteRuleText(w, rule);
                w.WriteLine(ref lineno, " */ yyASSERTCOVERAGE(yyruleno == {0});", rule.ID);
            }
            w.WriteLine(ref lineno, "        break;");
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which executes if a parse fails
            Template.Write(w, ctx, ctx.ParseFailure, ref lineno, filePath);
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which executes when a syntax error occurs
            Template.Write(w, ctx, ctx.SyntaxError, ref lineno, filePath);
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Generate code which executes when the parser accepts its input
            Template.Write(w, ctx, ctx.ParseAccept, ref lineno, filePath);
            Template.Transfer(ctx.Name, r, w, ref lineno);

            // Append any addition code the user desires
            Template.Write(w, ctx, ctx.ExtraCode, ref lineno, filePath);
        }

        /// <summary>
        /// Emits the header.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="w">The w.</param>
        public static void EmitHeader(Context ctx, StreamWriter w)
        {
            var prefix = (ctx.TokenPrefix ?? string.Empty);
            w.Write("typedef {0} {1}\n", GetMinimumSizeType(0, ctx.Terminals), prefix);
            for (var i = 1; i < ctx.Terminals; i++)
                w.Write("#define {0}{1,-30} {2,2}\n", prefix, ctx.Symbols[i].Name, i);
        }
    }
}
