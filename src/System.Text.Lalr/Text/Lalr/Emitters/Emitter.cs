using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Text.Lalr.Emitters
{
    /// <summary>
    /// Emitter
    /// </summary>
    public partial class Emitter
    {
        private Context _ctx;
        private EmitContext _emitCtx;

        /// <summary>
        /// Initializes a new instance of the <see cref="Emitter"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="target">The target.</param>
        public Emitter(Context ctx, CodeTypeDeclaration target)
        {
            _ctx = ctx;
            _emitCtx = new EmitContext(ctx, target);
        }

        private void AddLineInfo(CodeStatementCollection s, int sourceLineno, string source) { s.Add(new CodeSnippetStatement(string.Format("#line {0} \"{1}\"", sourceLineno, source))); }

        /// <summary>
        /// Emits the specified CTX.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="inputFilePath">The input file path.</param>
        /// <returns></returns>
        public static CodeCompileUnit Emit(Context ctx, CodeNamespace codeNamespace, string inputFilePath)
        {
            var codeUnit = new CodeCompileUnit();
            codeUnit.Namespaces.Add(codeNamespace);
            codeNamespace.Imports.AddRange(new[]{
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport("System.Diagnostics"),
                new CodeNamespaceImport("System.Runtime.InteropServices") });
            CodeTypeDeclaration target;
            var name = Path.GetFileNameWithoutExtension(inputFilePath);
            codeNamespace.Types.Add(target = new CodeTypeDeclaration(ctx.Name ?? name));
            target.BaseTypes.Add(typeof(IDisposable));
            //
            var e = new Emitter(ctx, target);
            //e.EmitIncludes(target);
            e.EmitTrace(target);
            e.EmitTokens(target);
            e.EmitMinor(target);
            e.EmitActionsTable(target);
            e.EmitBase(target);
            e.EmitDestructors(target);
            e.EmitRuleTable(target);
            e.EmitReducedRules(target);
            e.EmitStackOverflow(target);
            e.EmitParseFailure(target);
            e.EmitSyntaxError(target);
            e.EmitParseAccept(target);
            e.EmitExtraCode(target);
            return codeUnit;
        }

        #region Trace

        private CodeExpression _tracePrompt = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_tracePrompt");
        private CodeExpression _tokenNames = new CodeVariableReferenceExpression("_tokenNames");
        private CodeExpression _ruleNames = new CodeVariableReferenceExpression("_ruleNames");

        private static void WriteRuleText(StringBuilder b, Rule rule)
        {
            b.AppendFormat("{0} ::=", rule.LHSymbol.Name);
            for (var j = 0; j < rule.RHSymbols.Length; j++)
            {
                var symbol = rule.RHSymbols[j];
                b.AppendFormat(" {0}", symbol.Name);
                if (symbol.Type == SymbolType.MultiTerminal)
                    for (var k = 1; k < symbol.Children.Length; k++)
                        b.AppendFormat("|{0}", symbol.Children[k].Name);
            }
        }

        private void EmitTrace(CodeTypeDeclaration target)
        {
            if (_emitCtx.NDEBUG)
                return;
            CodeMemberField field;
            target.Members.Add(field = new CodeMemberField(typeof(string), "_tracePrompt") { Attributes = MemberAttributes.Private });
            // Generate a table containing the symbolic name of every symbol
            var n = _ctx.Symbols.Length - 1;
            var tokenNameInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
                tokenNameInits[i] = new CodePrimitiveExpression(_ctx.Symbols[i].Name);
            // For tracing shifts, the names of all terminals and nonterminals are required.  The following table supplies these names
            target.Members.Add(field = new CodeMemberField(typeof(string[]), "_tokenNames") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(typeof(string), tokenNameInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Trace: Token Names"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Trace: Token Names"));
            // Generate a table containing a text string that describes every rule in the rule set of the grammar.  This information is used when tracing REDUCE actions.
            var ruleNameInits = new CodePrimitiveExpression[_ctx.Rules];
            n = 0;
            var b = new StringBuilder();
            for (var rule = _ctx.Rule; rule != null; rule = rule.Next, n++)
            {
                b.Length = 0; WriteRuleText(b, rule);
                ruleNameInits[n] = new CodePrimitiveExpression(b.ToString());
            }
            // For tracing reduce actions, the names of all rules are required.
            target.Members.Add(field = new CodeMemberField(typeof(string[]), "_ruleNames") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(typeof(string), ruleNameInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Trace: Rule Names"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Trace: Rule Names"));
            // public string TracePrompt { get; set; }
            var tracePromptProperty = new CodeMemberProperty { Attributes = MemberAttributes.Public, Type = new CodeTypeReference(typeof(string)), Name = "TracePrompt" };
            tracePromptProperty.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "TracePrompt"));
            tracePromptProperty.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "TracePrompt"));
            target.Members.Add(tracePromptProperty);
            var c = tracePromptProperty.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("Turn parser tracing on by giving a stream to which to write the trace and a prompt to preface each trace message.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            tracePromptProperty.GetStatements.Add(new CodeMethodReturnStatement(_tracePrompt));
            tracePromptProperty.SetStatements.Add(new CodeAssignStatement(_tracePrompt, new CodePropertySetValueReferenceExpression()));
        }

        #endregion

        #region Minor

        private CodeExpression _zeroMinor = new CodeVariableReferenceExpression("_zeroMinor");

        private void EmitMinor(CodeTypeDeclaration target)
        {
            CodeMemberField field;
            target.Members.Add(field = new CodeMemberField(new CodeTypeReference("Minor"), "_zeroMinor") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final });
            CodeTypeDeclaration type;
            target.Members.Add(type = new CodeTypeDeclaration("Minor") { IsStruct = true });
            type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Types: Minor"));
            type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Types: Minor"));
            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("StructLayout"), new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("LayoutKind"), "Explicit"))));
            type.Members.Add(field = new CodeMemberField(typeof(int), "yyinit") { Attributes = MemberAttributes.Public }); field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("FieldOffset"), new CodeAttributeArgument(new CodePrimitiveExpression(0))));
            type.Members.Add(field = new CodeMemberField(_emitCtx.TOKENTYPE, "yy0") { Attributes = MemberAttributes.Public }); field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("FieldOffset"), new CodeAttributeArgument(new CodePrimitiveExpression(0))));
            if (_ctx.DataTypes != null)
                foreach (var dataType in _ctx.DataTypes)
                {
                    type.Members.Add(field = new CodeMemberField(EmitContext.FixupType(dataType.Key), "yy" + dataType.Value) { Attributes = MemberAttributes.Public }); field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("FieldOffset"), new CodeAttributeArgument(new CodePrimitiveExpression(0))));
                }
            if (_ctx.ErrorSymbol.Uses > 0)
            {
                type.Members.Add(field = new CodeMemberField(typeof(int), "yy" + _ctx.ErrorSymbol.DataTypeID) { Attributes = MemberAttributes.Public }); field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("FieldOffset"), new CodeAttributeArgument(new CodePrimitiveExpression(0))));
            }
        }

        #endregion

        #region Includes/ExtraCode

        private void EmitIncludes(CodeTypeDeclaration target)
        {
            if (string.IsNullOrEmpty(_ctx.Include))
                return;
            var targetMembers = target.Members;
            targetMembers.Add(new CodeSnippetTypeMember(_ctx.Include));
        }

        private void EmitExtraCode(CodeTypeDeclaration target)
        {
            if (string.IsNullOrEmpty(_ctx.ExtraCode))
                return;
            var targetMembers = target.Members;
            // Append any addition code the user desires            
            targetMembers.Add(new CodeSnippetTypeMember(_ctx.ExtraCode));
        }

        #endregion

        #region Tokens

        private void EmitTokens(CodeTypeDeclaration target)
        {
            var type = new CodeTypeDeclaration(_ctx.TokenPrefix) { IsEnum = true };
            type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Types: Tokens"));
            type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Types: Tokens"));
            var typeComments = type.Comments;
            typeComments.Add(new CodeCommentStatement("<summary>", true));
            typeComments.Add(new CodeCommentStatement("These constants (all generated automatically by the parser generator) specify the various kinds of tokens (terminals) that the parser understands.", true));
            typeComments.Add(new CodeCommentStatement("Each symbol here is a terminal symbol in the grammar.", true));
            typeComments.Add(new CodeCommentStatement("</summary>", true));
            var typeMembers = type.Members;
            for (var i = 1; i < _ctx.Terminals; i++)
                typeMembers.Add(new CodeMemberField { Name = _ctx.Symbols[i].Name, InitExpression = new CodePrimitiveExpression(i) });
            target.Members.Add(type);
        }

        #endregion

        #region Actions

        private CodeExpression _defaults = new CodeVariableReferenceExpression("_defaults");
        private CodeExpression _lookaheads = new CodeVariableReferenceExpression("_lookaheads");
        private CodeExpression _actions = new CodeVariableReferenceExpression("_actions");
        private CodeExpression _shift_ofsts = new CodeVariableReferenceExpression("_shift_ofsts");
        private CodeExpression _reduce_ofsts = new CodeVariableReferenceExpression("_reduce_ofsts");
        private CodeExpression _fallbacks = new CodeVariableReferenceExpression("_fallbacks");

        private void EmitActionsTable(CodeTypeDeclaration target)
        {
            // Next are the tables used to determine what action to take based on the current state and lookahead token.  These tables are used to implement
            // functions that take a state number and lookahead value and return an action integer.  

            // Suppose the action integer is N.  Then the action is determined as follows
            //  0 <= N < NSTATE                  Shift N.  That is, push the lookahead token onto the stack and goto state N.
            //  NSTATE <= N < NSTATE+NRULE   Reduce by rule N-NSTATE.
            //  N == NSTATE+NRULE              A syntax error has occurred.
            //  N == NSTATE+NRULE+1            The parser accepts its input.
            //  N == NSTATE+NRULE+2            No such action.  Denotes unused slots in the _actions[] table.

            // The action table is constructed as a single large table named yy_action[].
            // Given state S and lookahead X, the action is computed as
            //     _actions[_shift_ofsts[S] + X]

            // If the index value _shift_ofsts[S]+X is out of range or if the value _lookaheads[_shift_ofsts[S]+X] is not equal to X or if _shift_ofsts[S]
            // is equal to SHIFT_USE_DFLT, it means that the action is not in the table and that _defaults[S] should be used instead.  

            // The formula above is for computing the action when the lookahead is a terminal symbol.  If the lookahead is a non-terminal (as occurs after
            // a reduce action) then the _reduce_ofsts[] array is used in place of the _shift_ofsts[] array and REDUCE_USE_DFLT is used in place of SHIFT_USE_DFLT.

            // The following are the tables generated in this section:
            // _actions[]        A single table containing all actions.
            // _lookaheads[]     A table containing the lookahead for each entry in yy_action.  Used to detect hash collisions.
            // _shift_ofsts[]    For each state, the offset into yy_action for shifting terminals.
            // _reduce_ofsts[]   For each state, the offset into yy_action for shifting non-terminals after a reduce.
            // _defaults[]       Default action for each state.
            int maxTokenOffset;
            int minTokenOffset;
            int maxNonTerminalOffset;
            int minNonTerminalOffset;
            var actionTable = EmitterActionTable.Make(_ctx, out maxTokenOffset, out minTokenOffset, out maxNonTerminalOffset, out minNonTerminalOffset);
            CodeMemberField field;
            #region Actions
            var n = actionTable.Size;
            _emitCtx.ACTIONS = actionTable.Size; target.Members.Add(new CodeMemberField(typeof(int), "ACTIONS") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.ACTIONS) });
            //
            var actionInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
            {
                var action = actionTable.GetAction(i);
                if (action < 0)
                    action = _ctx.States + _ctx.Rules + 2;
                actionInits[i] = new CodePrimitiveExpression(action);
            }
            Type actionArrayType;
            _emitCtx.ACTIONTYPE = GetMinimumSizeType(0, _ctx.States + _ctx.Rules + 5, out actionArrayType);
            target.Members.Add(field = new CodeMemberField(actionArrayType, "_actions") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(_emitCtx.ACTIONTYPE, actionInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Actions"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Actions"));
            // #define NO_ACTION      (NSTATE+NRULE+2)
            // #define ACCEPT_ACTION  (NSTATE+NRULE+1)
            // #define ERROR_ACTION   (NSTATE+NRULE)
            target.Members.Add(new CodeMemberField(typeof(int), "NO_ACTION") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_ctx.States + _ctx.Rules + 2) });
            target.Members.Add(new CodeMemberField(typeof(int), "ACCEPT_ACTION") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_ctx.States + _ctx.Rules + 1) });
            target.Members.Add(new CodeMemberField(typeof(int), "ERROR_ACTION") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_ctx.States + _ctx.Rules) });

            #endregion
            #region Lookahead
            var lookaheadInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
            {
                var lookahead = actionTable.GetLookahead(i);
                if (lookahead < 0)
                    lookahead = _ctx.Symbols.Length - 1;
                lookaheadInits[i] = new CodePrimitiveExpression(lookahead);
            }
            Type codeArrayType;
            _emitCtx.CODETYPE = GetMinimumSizeType(0, _ctx.Symbols.Length, out codeArrayType);
            target.Members.Add(field = new CodeMemberField(codeArrayType, "_lookaheads") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(_emitCtx.CODETYPE, lookaheadInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Lookaheads"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Lookaheads"));
            #endregion
            #region Shift Offsets
            n = _ctx.States;
            _emitCtx.SHIFT_USE_DFLT = minTokenOffset - 1; target.Members.Add(new CodeMemberField(typeof(int), "SHIFT_USE_DFLT") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.SHIFT_USE_DFLT) });
            while (n > 0 && _ctx.Sorted[n - 1].TokenOffset == State.NO_OFFSET) n--;
            _emitCtx.SHIFT_COUNT = n - 1; target.Members.Add(new CodeMemberField(typeof(int), "SHIFT_COUNT") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.SHIFT_COUNT) });
            _emitCtx.SHIFT_MIN = minTokenOffset; target.Members.Add(new CodeMemberField(typeof(int), "SHIFT_MIN") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.SHIFT_MIN) });
            _emitCtx.SHIFT_MAX = maxTokenOffset; target.Members.Add(new CodeMemberField(typeof(int), "SHIFT_MAX") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.SHIFT_MAX) });
            var shiftInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
            {
                var state = _ctx.Sorted[i];
                var offset = state.TokenOffset;
                if (offset == State.NO_OFFSET) offset = minTokenOffset - 1;
                shiftInits[i] = new CodePrimitiveExpression(offset);
            }
            Type shiftArrayType;
            var shiftType = GetMinimumSizeType(minTokenOffset - 1, maxTokenOffset, out shiftArrayType);
            target.Members.Add(field = new CodeMemberField(shiftArrayType, "_shift_ofsts") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(shiftType, shiftInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Shift Offsets"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Shift Offsets"));
            #endregion
            #region Reduce Offsets
            n = _ctx.States;
            _emitCtx.REDUCE_USE_DFLT = minNonTerminalOffset - 1; target.Members.Add(new CodeMemberField(typeof(int), "REDUCE_USE_DFLT") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.REDUCE_USE_DFLT) });
            while (n > 0 && _ctx.Sorted[n - 1].NonTerminalOffset == State.NO_OFFSET) n--;
            _emitCtx.REDUCE_COUNT = n - 1; target.Members.Add(new CodeMemberField(typeof(int), "REDUCE_COUNT") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(n - 1) });
            _emitCtx.REDUCE_MIN = minNonTerminalOffset; target.Members.Add(new CodeMemberField(typeof(int), "REDUCE_MIN") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.REDUCE_MIN) });
            _emitCtx.REDUCE_MAX = maxNonTerminalOffset; target.Members.Add(new CodeMemberField(typeof(int), "REDUCE_MAX") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(_emitCtx.REDUCE_MAX) });
            var reduceInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
            {
                var state = _ctx.Sorted[i];
                var offset = state.NonTerminalOffset;
                if (offset == State.NO_OFFSET) offset = minNonTerminalOffset - 1;
                reduceInits[i] = new CodePrimitiveExpression(offset);
            }
            Type reduceArrayType;
            var reduceType = GetMinimumSizeType(minNonTerminalOffset - 1, maxNonTerminalOffset, out reduceArrayType);
            target.Members.Add(field = new CodeMemberField(reduceArrayType, "_reduce_ofsts") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(reduceType, reduceInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Reduce Offsets"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Reduce Offsets"));
            #endregion
            #region Defaults
            n = _ctx.States;
            var defaultInits = new CodePrimitiveExpression[n];
            for (var i = 0; i < n; i++)
            {
                var state = _ctx.Sorted[i];
                defaultInits[i] = new CodePrimitiveExpression(state.Default);
            }
            target.Members.Add(field = new CodeMemberField(actionArrayType, "_defaults") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(_emitCtx.ACTIONTYPE, defaultInits) { Size = n } });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Defaults"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Defaults"));
            #endregion
            #region Fallbacks
            if (_ctx.HasFallback)
            {
                n = _ctx.Terminals - 1;
                while (n > 0 && _ctx.Symbols[n].Fallback == null) n--;
                var fallbackInits = new CodeExpression[n + 1];
                for (var i = 0; i <= n; i++)
                {
                    var symbol = _ctx.Symbols[i];
                    if (symbol.Fallback == null)
                        fallbackInits[i] = new CodeDefaultValueExpression(new CodeTypeReference(_emitCtx.CODETYPE)); /* {symbol.Name,10} => nothing */
                    else
                        fallbackInits[i] = new CodePrimitiveExpression(symbol.Fallback.ID); /* {symbol.Name,10} => symbol.Fallback.Name */
                }
                target.Members.Add(field = new CodeMemberField(codeArrayType, "_fallbacks") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(_emitCtx.CODETYPE, fallbackInits) { Size = n } });
                field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Fallbacks"));
                field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Fallbacks"));
            }
            #endregion
        }

        #endregion

        #region Destructors

        private void EmitDestructorBySymbol(CodeStatementCollection s, Symbol symbol)
        {
            string z;
            if (symbol.Type == SymbolType.Terminal)
            {
                z = _ctx.TokenDestructor;
                if (z == null) return;
                s.Add(new CodeSnippetStatement("{"));
            }
            else if (symbol.Destructor != null)
            {
                z = symbol.Destructor;
                s.Add(new CodeSnippetStatement("{"));
                if (!_ctx.NoShowLinenos) AddLineInfo(s, symbol.DestructorLineno, _ctx.InputFilePath);
            }
            else if (_ctx.DefaultDestructor != null)
            {
                z = _ctx.DefaultDestructor;
                if (z == null) return;
                s.Add(new CodeSnippetStatement("{"));
            }
            else
                throw new InvalidOperationException();
            var cp = 0;
            var b = new StringBuilder();
            for (; cp < z.Length; cp++)
            {
                if (z[cp] == '$' && z[cp + 1] == '$')
                {
                    b.AppendFormat("(minor.yy{0})", symbol.DataTypeID);
                    cp++;
                    continue;
                }
                b.Append(z[cp]);
            }
            s.Add(new CodeSnippetStatement(b.ToString()));
            s.Add(new CodeSnippetStatement("}"));
            return;
        }

        private void EmitDestructors(CodeTypeDeclaration target)
        {
            // private void Destructor();
            var destructorMethod = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "Destructor" };
            destructorMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.CODETYPE, "major"));
            destructorMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.MINORTYPE, "minor"));
            destructorMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Destructors (switch)"));
            destructorMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Destructors (switch)"));
            target.Members.Add(destructorMethod);
            var c = destructorMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following function deletes the value associated with a symbol.  The symbol can be either a terminal or nonterminal.", true));
            c.Add(new CodeCommentStatement("\"major\" is the symbol code, and \"minor\" is a pointer to the value.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = destructorMethod.Statements;
            {
                // ParseARG_FETCH
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // switch (major) {
                s.Add(new CodeSnippetStatement("switch (major) {"));
                s.Add(new CodeCommentStatement("Here is inserted the actions which take place when a terminal or non-terminal is destroyed.  This can happen when the symbol is popped"));
                s.Add(new CodeCommentStatement("from the stack during a reduce or during error processing or when a parser is being destroyed before it is finished parsing."));
                s.Add(new CodeCommentStatement("Note: during a reduce, the only symbols destroyed are those which appear on the RHS of the rule, but which are not used inside the C code."));
                // Generate code which executes every time a symbol is popped from the stack while processing errors or while destroying the parser.  (In other words, generate the %destructor actions)
                if (_ctx.TokenDestructor != null)
                {
                    var once = true;
                    for (var i = 0; i < _ctx.Symbols.Length - 1; i++)
                    {
                        var symbol = _ctx.Symbols[i];
                        if (symbol == null || symbol.Type != SymbolType.Terminal) continue;
                        if (once) { s.Add(new CodeCommentStatement("TERMINAL Destructor")); once = false; }
                        s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", symbol.ID, symbol.Name)));
                    }
                    var i2 = 0;
                    for (; i2 < (_ctx.Symbols.Length - 1) && (_ctx.Symbols[i2].Type != SymbolType.Terminal); i2++) ;
                    if (i2 < _ctx.Symbols.Length - 1)
                    {
                        EmitDestructorBySymbol(s, _ctx.Symbols[i2]);
                        s.Add(new CodeSnippetStatement("      break;"));
                    }
                }
            }
            if (_ctx.DefaultDestructor != null)
            {
                var once = true;
                var defaultSymbol = (Symbol)null;
                for (var i = 0; i < _ctx.Symbols.Length - 1; i++)
                {
                    var symbol = _ctx.Symbols[i];
                    if (symbol == null || symbol.Type == SymbolType.Terminal || symbol.ID <= 0 || symbol.Destructor != null) continue;
                    if (once) { s.Add(new CodeCommentStatement("Default NON-TERMINAL Destructor")); once = false; }
                    s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", symbol.ID, symbol.Name)));
                    defaultSymbol = symbol;
                }
                if (defaultSymbol != null)
                    EmitDestructorBySymbol(s, defaultSymbol);
                s.Add(new CodeSnippetStatement("      break;"));
            }
            for (var i = 0; i < _ctx.Symbols.Length - 1; i++)
            {
                var symbol = _ctx.Symbols[i];
                if (symbol == null || symbol.Type == SymbolType.Terminal || symbol.Destructor == null) continue;
                s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", symbol.ID, symbol.Name)));
                // Combine duplicate destructors into a single case
                for (var j = i + 1; j < _ctx.Symbols.Length - 1; j++)
                {
                    var symbol2 = _ctx.Symbols[j];
                    if (symbol2 != null && symbol2.Type != SymbolType.Terminal && symbol2.Destructor != null && symbol2.DataTypeID == symbol.DataTypeID && symbol.Destructor == symbol2.Destructor)
                    {
                        s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", symbol2.ID, symbol2.Name)));
                        symbol2.Destructor = null;
                    }
                }
                EmitDestructorBySymbol(s, _ctx.Symbols[i]);
                s.Add(new CodeSnippetStatement("      break;"));
            }
            // }
            s.Add(new CodeSnippetStatement("}"));
        }

        #endregion

        #region Rules

        private CodeExpression _ruleInfos = new CodeVariableReferenceExpression("_ruleInfos");

        private void EmitRuleTable(CodeTypeDeclaration target)
        {
            var ruleInfo = new CodeTypeReference("RuleInfo");
            CodeTypeDeclaration type;
            // protected struct RuleInfo {CODETYPE LHSymbol; byte RHSymbols; } 
            target.Members.Add(type = new CodeTypeDeclaration("RuleInfo") { Attributes = MemberAttributes.Family, IsStruct = true });
            type.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Types: RuleInfo"));
            type.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Types: RuleInfo"));
            var targetMembers = type.Members;
            targetMembers.Add(new CodeMemberField(_emitCtx.CODETYPE, "LHSymbol") { Attributes = MemberAttributes.Public });
            targetMembers.Add(new CodeMemberField(typeof(byte), "RHSymbols") { Attributes = MemberAttributes.Public });
            CodeConstructor constructor;
            targetMembers.Add(constructor = new CodeConstructor { Attributes = MemberAttributes.Public });
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.CODETYPE, "lhSymbol"));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte), "rhSymbols"));
            var s = constructor.Statements;
            {
                s.Add(new CodeAssignStatement(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "LHSymbol"), new CodeVariableReferenceExpression("lhSymbol")));
                s.Add(new CodeAssignStatement(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "RHSymbols"), new CodeVariableReferenceExpression("rhSymbols")));
            }
            // Generate the table of rule information. Note: This code depends on the fact that rules are number sequentually beginning with 0.
            var ruleInits = new List<CodeObjectCreateExpression>();
            for (var rule = _ctx.Rule; rule != null; rule = rule.Next)
                ruleInits.Add(new CodeObjectCreateExpression(ruleInfo, new CodePrimitiveExpression(rule.LHSymbol.ID), new CodePrimitiveExpression(rule.RHSymbols.Length)));
            target.Comments.Add(new CodeCommentStatement("The following table contains information about every rule that is used during the reduce."));
            // public static readonly RuleInfo[] _ruleInfos[] = { .. }
            CodeMemberField field;
            target.Members.Add(field = new CodeMemberField(new CodeTypeReference("RuleInfo[]"), "_ruleInfos") { Attributes = MemberAttributes.Family | MemberAttributes.Static | MemberAttributes.Final, InitExpression = new CodeArrayCreateExpression(ruleInfo, ruleInits.ToArray()) });
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Tables: Rule Infos"));
            field.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Tables: Rule Infos"));
        }

        private void EmitRuleCodeByRule(CodeStatementCollection s, Rule rule)
        {
            if (rule.Code != null)
            {
                if (!_ctx.NoShowLinenos) AddLineInfo(s, rule.Lineno, _ctx.InputFilePath);
                s.Add(new CodeSnippetStatement("{"));
                s.Add(new CodeSnippetStatement(rule.Code));
                s.Add(new CodeSnippetStatement("}"));
            }
        }

        private static string GetRuleText(Rule rule)
        {
            var b = new StringBuilder();
            b.AppendFormat("{0} ::=", rule.LHSymbol.Name);
            for (var j = 0; j < rule.RHSymbols.Length; j++)
            {
                var symbol = rule.RHSymbols[j];
                b.AppendFormat(" {0}", symbol.Name);
                if (symbol.Type == SymbolType.MultiTerminal)
                    for (var k = 1; k < symbol.Children.Length; k++)
                        b.AppendFormat("|{0}", symbol.Children[k].Name);
            }
            return b.ToString();
        }

        private static bool HasDestructor(Symbol symbol, Context ctx) { return (symbol.Type == SymbolType.Terminal ? ctx.TokenDestructor != null : ctx.DefaultDestructor != null || symbol.Destructor != null); }

        // Expand the symbols in this string so that the refer to elements of the parser stack.
        private static void ExpandRuleCode(Context ctx, Rule rule)
        {
            var lhSymbolUsed = false;
            var rhSymbolsUsed = new bool[rule.RHSymbols.Length];
            if (rule.Code == null) { rule.Code = Environment.NewLine; rule.Lineno = rule.RuleLineno; }
            var b = new StringBuilder();
            var z = rule.Code;
            for (var cp = 0; cp < z.Length; cp++)
            {
                if (char.IsLetter(z[cp]) && (cp == 0 || (!char.IsLetterOrDigit(z[cp - 1]) && z[cp - 1] != '_')))
                {
                    var xp = cp + 1;
                    for (; char.IsLetterOrDigit(z[xp]) || z[xp] == '_'; xp++) ;
                    var xpLength = xp - cp;
                    if (rule.LHSymbolAlias != null && z.Substring(cp, xpLength) == rule.LHSymbolAlias)
                    {
                        b.AppendFormat("gotoMinor.yy{0}", rule.LHSymbol.DataTypeID);
                        cp = xp;
                        lhSymbolUsed = true;
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
                                    b.AppendFormat("_stack[_idx + {0}].major", i - rule.RHSymbols.Length + 1);
                                }
                                else
                                {
                                    var symbol = rule.RHSymbols[i];
                                    var dataTypeID = (symbol.Type == SymbolType.MultiTerminal ? symbol.Children[0].DataTypeID : symbol.DataTypeID);
                                    b.AppendFormat("_stack[_idx + {0}].minor.yy{1}", i - rule.RHSymbols.Length + 1, dataTypeID);
                                }
                                cp = xp;
                                rhSymbolsUsed[i] = true;
                                break;
                            }
                    }
                }
                b.Append(z[cp]);
            }
            // Check to make sure the LHS has been used
            if (rule.LHSymbolAlias != null && !lhSymbolUsed)
                ctx.RaiseError(ref ctx.Errors, rule.RuleLineno, "Label \"{0}\" for \"{1}({2})\" is never used.", rule.LHSymbolAlias, rule.LHSymbol.Name, rule.LHSymbolAlias);
            // Generate destructor code for RHS symbols which are not used in the reduce code
            for (var i = 0; i < rule.RHSymbols.Length; i++)
            {
                if (rule.RHSymbolsAlias[i] != null && !rhSymbolsUsed[i])
                    ctx.RaiseError(ref ctx.Errors, rule.RuleLineno, "Label {0} for \"{1}({2})\" is never used.", rule.RHSymbolsAlias[i], rule.RHSymbols[i].Name, rule.RHSymbolsAlias[i]);
                else if (rule.RHSymbolsAlias[i] == null && HasDestructor(rule.RHSymbols[i], ctx))
                    b.AppendFormat("  Destructor({0}, _stack[_idx - {1}].minor);" + Environment.NewLine, rule.RHSymbols[i].ID, i - rule.RHSymbols.Length + 1);
            }
            if (rule.Code != null)
                rule.Code = (b.ToString() ?? string.Empty);
        }

        private void EmitReducedRules(CodeTypeDeclaration target)
        {
            var findReduceAction = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "FindReduceAction");
            var shift = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Shift");
            var accept = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Accept");
            // private void Reduce();
            var cs = new CodeConditionStatement[2];
            var reduceMethod = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "Reduce" };
            reduceMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "ruleno")); // Number of the rule by which to reduce
            reduceMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Reduce (switch)"));
            reduceMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Reduce (switch)"));
            target.Members.Add(reduceMethod);
            var c = reduceMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("Perform a reduce action and the shift that must immediately follow the reduce.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = reduceMethod.Statements;
            {
                var ruleno = new CodeVariableReferenceExpression("ruleno");
                //  ParseARG_FETCH;
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null && ruleno >= 0 && ruleno < _ruleNames.Length)
                    //    Trace.WriteLine(string.Format("{0}Reduce [{1}].", _tracePrompt, _ruleNames[ruleno]));
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(ruleno, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanAnd,
                                new CodeBinaryOperatorExpression(ruleno, CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(_ruleNames, "Length")))),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Reduce [{1}]."), _tracePrompt, new CodeIndexerExpression(_ruleNames, ruleno))))));
                }
                // var gotoMinor = _zeroMinor;
                var gotoMinor = new CodeVariableReferenceExpression("gotoMinor");
                s.Add(new CodeVariableDeclarationStatement(_emitCtx.MINORTYPE, "gotoMinor", _zeroMinor));
                // switch (ruleno) {
                s.Add(new CodeSnippetStatement("switch (ruleno) {"));
                s.Add(new CodeSnippetStatement(@"/* Beginning here are the reduction cases.  A typical example
** follows:
**   case 0:
**  #line <lineno> <grammarfile>
**     { ... }           // User supplied code
**     break;
*/"));
                // Generate code which execution during each REDUCE action
                for (var rule = _ctx.Rule; rule != null; rule = rule.Next)
                    ExpandRuleCode(_ctx, rule);
                // First output rules other than the default: rule
                for (var rule = _ctx.Rule; rule != null; rule = rule.Next)
                {
                    if (rule.Code == null) continue;
                    if (rule.Code[0] == '\n' && rule.Code.Length == 1) continue; // Will be default:
                    s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", rule.ID, GetRuleText(rule))));
                    for (var rule2 = rule.Next; rule2 != null; rule2 = rule2.Next)
                        if (rule2.Code == rule.Code)
                        {
                            s.Add(new CodeSnippetStatement(string.Format("    case {0}: /* {1} */", rule2.ID, GetRuleText(rule2)))); // testcase(ruleno=={2});", rule2.ID, GetRuleText(rule2), rule2.ID)));
                            rule2.Code = null;
                        }
                    EmitRuleCodeByRule(s, rule);
                    s.Add(new CodeSnippetStatement("      break;"));
                    rule.Code = null;
                }
                // Finally, output the default: rule.  We choose as the default: all empty actions.
                s.Add(new CodeSnippetStatement("      default:"));
                for (var rule = _ctx.Rule; rule != null; rule = rule.Next)
                {
                    if (rule.Code == null) continue;
                    Debug.Assert(rule.Code[0] == '\n' && rule.Code.Length == 1);
                    s.Add(new CodeSnippetStatement(string.Format("    /* {0} */ testcase(ruleno=={1})", rule.ID, GetRuleText(rule), rule.ID)));
                }
                s.Add(new CodeSnippetStatement("      break;"));
                s.Add(new CodeSnippetStatement("}"));
                // var @goto = _ruleInfos[ruleno].LHSymbol;
                // var size = _ruleInfos[ruleno].RHSymbols;
                var @goto = new CodeVariableReferenceExpression("@goto");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "@goto", new CodeFieldReferenceExpression(new CodeIndexerExpression(_ruleInfos, ruleno), "LHSymbol")));
                var size = new CodeVariableReferenceExpression("size");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "size", new CodeFieldReferenceExpression(new CodeIndexerExpression(_ruleInfos, ruleno), "RHSymbols")));
                // _idx -= size;
                s.Add(new CodeAssignStatement(_idx, new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.Subtract, size)));
                // action = FindReduceAction(_stack[_idx].stateno, (CODETYPE)@goto);
                var action = new CodeVariableReferenceExpression("action");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "action",
                    new CodeMethodInvokeExpression(findReduceAction, new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, _idx), "stateno"), new CodeCastExpression(_emitCtx.CODETYPE, @goto))));
                // if (action < NSTATE) { _CS0_ }
                s.Add(cs[0] = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.LessThan, _emitCtx.pSTATES)));
                //#ifdef NDEBUG
                if (_emitCtx.NDEBUG)
                {
                    cs[0].TrueStatements.Add(new CodeCommentStatement("If we are not debugging and the reduce action popped at least one element off the stack, then we can push the new element back"));
                    cs[0].TrueStatements.Add(new CodeCommentStatement("onto the stack here, and skip the stack overflow test in Shift(). That gives a significant speed improvement."));
                    // if (size > 0) {
                    //    _idx++;
                    //    stack = _stack[_idx];
                    //    stack.stateno = (ACTIONTYPE)action;
                    //    stack.major = (CODETYPE)@goto;
                    //    stack.minor = gotoMinor;
                    //    _stack[_idx] = stack;
                    //    return;
                    // }
                    var stack = new CodeVariableReferenceExpression("stack");
                    cs[0].TrueStatements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(size, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                        new CodeAssignStatement(_idx, new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                        new CodeVariableDeclarationStatement(new CodeTypeReference("StackEntry"), "stack", new CodeIndexerExpression(_stack, _idx)),
                        new CodeAssignStatement(new CodeFieldReferenceExpression(stack, "stateno"), new CodeCastExpression(_emitCtx.ACTIONTYPE, action)),
                        new CodeAssignStatement(new CodeFieldReferenceExpression(stack, "major"), new CodeCastExpression(_emitCtx.CODETYPE, @goto)),
                        new CodeAssignStatement(new CodeFieldReferenceExpression(stack, "minor"), gotoMinor),
                        new CodeAssignStatement(new CodeIndexerExpression(_stack, _idx), stack),
                        new CodeMethodReturnStatement()));
                }
                // Shift(action, @goto, gotoMinor);
                // return;
                cs[0].TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(shift, action, @goto, gotoMinor)));
                cs[0].TrueStatements.Add(new CodeMethodReturnStatement());
                // Debug.Assert(action == ACCEPT_ACTION);
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                    new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.IdentityEquality, _emitCtx.pACCEPT_ACTION))));
                // Accept();
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(accept)));
            }
        }

        #endregion

        #region Syntax

        private void EmitStackOverflow(CodeTypeDeclaration target)
        {
            var popParserStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "PopParserStack");
            // private void StackOverflow(Minor minor);
            var stackOverflow = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "StackOverflow" };
            stackOverflow.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.MINORTYPE, "minor"));
            stackOverflow.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Syntax: StackOverflow"));
            stackOverflow.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Syntax: StackOverflow"));
            target.Members.Add(stackOverflow);
            var c = stackOverflow.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following routine is called if the stack overflows.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = stackOverflow.Statements;
            {
                // ParseARG_FETCH
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // _idx--;
                s.Add(new CodeAssignStatement(_idx, new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null)
                    //    Trace.WriteLine(string.Format("{0}Stack Overflow!", _tracePrompt));
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Stack Overflow!"), _tracePrompt)))));
                }
                // while (_idx >= 0) PopParserStack();
                s.Add(new CodeIterationStatement(new CodeSnippetStatement(),
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(popParserStack))));
                s.Add(new CodeCommentStatement("Here code is inserted which will execute if the parser stack every overflows"));
                if (!string.IsNullOrEmpty(_ctx.StackOverflow))
                    s.Add(new CodeSnippetStatement(_ctx.StackOverflow));
                // ParseARG_STORE
                if (_emitCtx.ParseARG_STORE != null)
                    _emitCtx.ParseARG_STORE(s);
            }
        }

        private void EmitParseFailure(CodeTypeDeclaration target)
        {
            var popParserStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "PopParserStack");
            // #ifndef YYNOERRORRECOVERY
            if (_emitCtx.NOERRORRECOVERY)
                return;
            // private void ParseFailed();
            var parseFailedMethod = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "ParseFailed" };
            parseFailedMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Syntax: Parse Failed"));
            parseFailedMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Syntax: Parse Failed"));
            target.Members.Add(parseFailedMethod);
            var c = parseFailedMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following code executes when the parse fails", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = parseFailedMethod.Statements;
            {
                // ParseARG_FETCH;
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null)
                    //    Trace.WriteLine(string.Format("{0}Fail!", _tracePrompt));
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Fail!"), _tracePrompt)))));
                }
                // while (_idx >= 0) PopParserStack();
                s.Add(new CodeIterationStatement(new CodeSnippetStatement(),
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(popParserStack))));
                c.Add(new CodeCommentStatement("Here code is inserted which will be executed whenever the parser fails"));
                if (!string.IsNullOrEmpty(_ctx.ParseFailure))
                    s.Add(new CodeSnippetStatement(_ctx.ParseFailure));
                // ParseARG_STORE
                if (_emitCtx.ParseARG_STORE != null)
                    _emitCtx.ParseARG_STORE(s);
            }
        }

        private void EmitSyntaxError(CodeTypeDeclaration target)
        {
            var popParserStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "PopParserStack");
            // private void SyntaxError(int major, MINORTYPE minor);
            var syntaxErrorMethod = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "SyntaxError" };
            syntaxErrorMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "major"));
            syntaxErrorMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.MINORTYPE, "minor"));
            syntaxErrorMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Syntax: Syntax Error"));
            syntaxErrorMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Syntax: Syntax Error"));
            target.Members.Add(syntaxErrorMethod);
            var c = syntaxErrorMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following code executes when a syntax error first occurs.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = syntaxErrorMethod.Statements;
            {
                // ParseARG_FETCH;
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // #define TOKEN (minor.yy0)
                // Generate code which executes when a syntax error occurs
                if (!string.IsNullOrEmpty(_ctx.SyntaxError))
                    s.Add(new CodeSnippetStatement(_ctx.SyntaxError));
                // ParseARG_STORE
                if (_emitCtx.ParseARG_STORE != null)
                    _emitCtx.ParseARG_STORE(s);
            }
        }

        private void EmitParseAccept(CodeTypeDeclaration target)
        {
            var popParserStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "PopParserStack");
            // private void ParseFailed();
            var acceptMethod = new CodeMemberMethod { Attributes = MemberAttributes.Private, Name = "Accept" };
            acceptMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Syntax: Accept"));
            acceptMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Syntax: Accept"));
            target.Members.Add(acceptMethod);
            var c = acceptMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following is executed when the parser accepts", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            var s = acceptMethod.Statements;
            {
                // ParseARG_FETCH;
                if (_emitCtx.ParseARG_FETCH != null)
                    _emitCtx.ParseARG_FETCH(s);
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null)
                    //    Trace.WriteLine(string.Format("{0}Accept!", _tracePrompt));
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Accept!"), _tracePrompt)))));
                }
                // while (_idx >= 0) PopParserStack();
                s.Add(new CodeIterationStatement(new CodeSnippetStatement(),
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(popParserStack))));
                c.Add(new CodeCommentStatement("Here code is inserted which will be executed whenever the parser accepts"));
                // Generate code which executes when the parser accepts its input
                if (!string.IsNullOrEmpty(_ctx.ParseAccept))
                    s.Add(new CodeSnippetStatement(_ctx.ParseAccept));
                // ParseARG_STORE
                if (_emitCtx.ParseARG_STORE != null)
                    _emitCtx.ParseARG_STORE(s);
            }
        }

        #endregion
    }
}
