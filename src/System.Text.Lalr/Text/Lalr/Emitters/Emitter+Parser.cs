using System.Collections.Generic;
using System.Diagnostics;
using System.CodeDom;

namespace System.Text.Lalr.Emitters
{
    public partial class Emitter
    {
        private CodeExpression _idx = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_idx");
        private CodeExpression _idxMax = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_idxMax");
        private CodeExpression _errors = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_errors");
        private CodeExpression _stackLength = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_stackLength");
        private CodeExpression _stack = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_stack");

        private void EmitBase(CodeTypeDeclaration target)
        {
            var destructor = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Destructor");
            var stackOverflow = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "StackOverflow");
            var reduce = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Reduce");
            var syntaxError = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "SyntaxError");
            var parseFailed = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "ParseFailed");
            CodeConditionStatement[] cs = new CodeConditionStatement[3];
            CodeCommentStatementCollection c;
            CodeStatementCollection s;
            CodeMethodReturnStatement r;
            CodeMemberField field;

            // struct StackEntry { }
            CodeTypeDeclaration stackEntry;
            var stackEntryType = new CodeTypeReference("StackEntry");
            var stackEntryArrayType = new CodeTypeReference("StackEntry[]");
            target.Members.Add(stackEntry = new CodeTypeDeclaration("StackEntry") { Attributes = MemberAttributes.Family, IsStruct = true });
            stackEntry.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Types: StackEntry"));
            stackEntry.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Types: StackEntry"));
            c = stackEntry.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("The following structure represents a single element of the parser's stack.  Information stored includes:", true));
            c.Add(new CodeCommentStatement(" +  The state number for the parser at this level of the stack.", true));
            c.Add(new CodeCommentStatement(" +  The value of the token stored at this level of the stack. (In other words, the \"major\" token.)", true));
            c.Add(new CodeCommentStatement(" +  The semantic value stored at this level of the stack.  This is the information used by the action routines in the grammar. It is sometimes called the \"minor\" token.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            stackEntry.Members.Add(field = new CodeMemberField(_emitCtx.ACTIONTYPE, "stateno") { Attributes = MemberAttributes.Public }); field.Comments.Add(new CodeCommentStatement("The state-number"));
            stackEntry.Members.Add(field = new CodeMemberField(_emitCtx.CODETYPE, "major") { Attributes = MemberAttributes.Public }); field.Comments.Add(new CodeCommentStatement("The major token value.  This is the code number for the token at this stack level"));
            stackEntry.Members.Add(field = new CodeMemberField(_emitCtx.MINORTYPE, "minor") { Attributes = MemberAttributes.Public }); field.Comments.Add(new CodeCommentStatement("The user-supplied minor token value.  This is the value of the token"));

            // fields { }
            target.Members.Add(field = new CodeMemberField(typeof(int), "_idx") { Attributes = MemberAttributes.Family, InitExpression = new CodePrimitiveExpression(-1) }); field.Comments.Add(new CodeCommentStatement("Index of top element in stack"));
            if (_emitCtx.TRACKMAXSTACKDEPTH)
            {
                target.Members.Add(field = new CodeMemberField(typeof(int), "_idxMax") { Attributes = MemberAttributes.Family }); field.Comments.Add(new CodeCommentStatement("Maximum value of _idx"));
            }
            target.Members.Add(field = new CodeMemberField(typeof(int), "_errors") { Attributes = MemberAttributes.Family }); field.Comments.Add(new CodeCommentStatement("Shifts left before out of the error"));
            if (!string.IsNullOrEmpty(_emitCtx.ParseArg))
            {
                target.Members.Add(field = new CodeMemberField(_emitCtx.ParseArgType, "_" + _emitCtx.ParseArg)); field.Comments.Add(new CodeCommentStatement("A place to hold %extra_argument"));
            }
            if (_emitCtx.STACKDEPTH <= 0)
            {
                target.Members.Add(field = new CodeMemberField(typeof(int), "_stackLength") { Attributes = MemberAttributes.Family }); field.Comments.Add(new CodeCommentStatement("Current side of the stack"));
                target.Members.Add(field = new CodeMemberField(stackEntryArrayType, "_stack") { Attributes = MemberAttributes.Family }); field.Comments.Add(new CodeCommentStatement("The parser's stack"));
            }
            else
            {
                target.Members.Add(field = new CodeMemberField(stackEntryArrayType, "_stack") { Attributes = MemberAttributes.Family, InitExpression = new CodeArrayCreateExpression(stackEntryType, _emitCtx.STACKDEPTH) }); field.Comments.Add(new CodeCommentStatement("The parser's stack"));
            }
            var popParserStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "PopParserStack");

            // #
            // public void Dispose();
            var disposeMethod = new CodeMemberMethod { Attributes = MemberAttributes.Public, Name = "Dispose" };
            disposeMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Dispose"));
            disposeMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Dispose"));
            target.Members.Add(disposeMethod);
            c = disposeMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("Deallocate and destroy a parser.  Destructors are all called for all stack elements before shutting the parser down.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            s = disposeMethod.Statements;
            {
                // while (_idx >= 0) PopParserStack();
                s.Add(new CodeIterationStatement(new CodeSnippetStatement(),
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(popParserStack))
                ));
            }

            // #
            // protected void GrowStack();
            var growStack = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GrowStack");
            if (_emitCtx.STACKDEPTH <= 0)
            {
                var growStackMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, Name = "GrowStack" };
                growStackMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "GrowStack"));
                growStackMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "GrowStack"));
                c = growStackMethod.Comments;
                c.Add(new CodeCommentStatement("<summary>", true));
                c.Add(new CodeCommentStatement("Try to increase the size of the parser stack.", true));
                c.Add(new CodeCommentStatement("</summary>", true));
                s = growStackMethod.Statements;
                {
                    // int newSize = _stackLength * 2 + 100;
                    var newSize = new CodeVariableReferenceExpression("newSize");
                    s.Add(new CodeVariableDeclarationStatement(typeof(int), "newSize", new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(_stackLength, CodeBinaryOperatorType.Multiply, new CodePrimitiveExpression(2)), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(100))));
                    // Array.Resize(ref _stack, newSize);
                    s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Array"), "Resize", new CodeSnippetExpression("ref _stack"), newSize)));
                    // _stackLength = newSize;
                    s.Add(new CodeAssignStatement(_stackLength, newSize));
                    //#ifndef NDEBUG
                    if (!_emitCtx.NDEBUG)
                    {
                        // if (_tracePrompt != null)
                        //    Trace.WriteLine(string.Format("{0}Stack grows to {1} entries!", _tracePrompt, _stackLength));                
                        s.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Stack grows to {1} entries!"), _tracePrompt, _stackLength)))));
                    }
                }
            }

            // #
            // protected int PopParserStack();
            var popParserStackMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, ReturnType = new CodeTypeReference(typeof(int)), Name = "PopParserStack" };
            popParserStackMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "PopParserStack"));
            popParserStackMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "PopParserStack"));
            target.Members.Add(popParserStackMethod);
            c = popParserStackMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("Pop the parser's stack once.", true));
            c.Add(new CodeCommentStatement("If there is a destructor routine associated with the token which is popped from the stack, then call it.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            c.Add(new CodeCommentStatement("<returns>Return the major token number for the symbol popped.</returns>", true));
            s = popParserStackMethod.Statements;
            {
                // var tos = _stack[_idx];
                var tos = new CodeVariableReferenceExpression("tos");
                s.Add(new CodeVariableDeclarationStatement(stackEntryType, "tos", new CodeIndexerExpression(_stack, _idx)));
                // if (_idx < 0) return 0;            
                s.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                    r = new CodeMethodReturnStatement(new CodePrimitiveExpression(0))
                ));
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null && _idx >= 0)
                    //    Trace.WriteLine(string.Format("{0}Popping {1}", _tracePrompt, _tokenNames[tos.major]));                
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0))),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Popping {1}"), _tracePrompt, new CodeIndexerExpression(_tokenNames, new CodeFieldReferenceExpression(tos, "major")))))));
                }
                // var major = tos.major;
                // Destructor(major, tos.minor);
                // _idx--;
                // return major;
                var major = new CodeVariableReferenceExpression("major");
                s.Add(new CodeVariableDeclarationStatement(_emitCtx.CODETYPE, "major", new CodeFieldReferenceExpression(tos, "major")));
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(destructor, major, new CodeFieldReferenceExpression(tos, "minor"))));
                s.Add(new CodeAssignStatement(_idx, new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));
                s.Add(new CodeMethodReturnStatement(major));
            }

            // #
            // protected int ParseStackPeak();
            if (_emitCtx.TRACKMAXSTACKDEPTH)
            {
                var parseStackPeakMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, ReturnType = new CodeTypeReference(typeof(int)), Name = "ParseStackPeak" };
                parseStackPeakMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "ParseStackPeak"));
                parseStackPeakMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "ParseStackPeak"));
                target.Members.Add(parseStackPeakMethod);
                c = parseStackPeakMethod.Comments;
                c.Add(new CodeCommentStatement("<summary>", true));
                c.Add(new CodeCommentStatement("Return the peak depth of the stack for a parser.", true));
                c.Add(new CodeCommentStatement("</summary>", true));
                s = parseStackPeakMethod.Statements;
                {
                    // return _idxMax;
                    s.Add(new CodeMethodReturnStatement(_idxMax));
                }
            }

            // #
            // protected void FindShiftAction();
            var findShiftAction = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "FindShiftAction");
            var findShiftActionMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, ReturnType = new CodeTypeReference(typeof(int)), Name = "FindShiftAction" };
            findShiftActionMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.CODETYPE, "lookahead"));
            findShiftActionMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "FindShiftAction"));
            findShiftActionMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "FindShiftAction"));
            target.Members.Add(findShiftActionMethod);
            c = findShiftActionMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>", true));
            c.Add(new CodeCommentStatement("Find the appropriate action for a parser given the terminal look-ahead token lookahead.", true));
            c.Add(new CodeCommentStatement("If the look-ahead token is NOCODE, then check to see if the action is independent of the look-ahead.  If it is, return the action, otherwise return NO_ACTION.", true));
            c.Add(new CodeCommentStatement("</summary>", true));
            c.Add(new CodeCommentStatement("<param name=\"lookahead\"></param>", true));
            c.Add(new CodeCommentStatement("<returns></returns>", true));
            s = findShiftActionMethod.Statements;
            {
                var lookahead = new CodeVariableReferenceExpression("lookahead");
                // int i;
                // int stateno = _stack[_idx].stateno;
                // if (stateno > SHIFT_COUNT || (i = _shift_ofsts[stateno]) == SHIFT_USE_DFLT)
                //    return _default[stateno];
                var i = new CodeVariableReferenceExpression("i");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "i"));
                var stateno = new CodeVariableReferenceExpression("stateno");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "stateno", new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, _idx), "stateno")));
                s.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(stateno, CodeBinaryOperatorType.GreaterThan, _emitCtx.pSHIFT_COUNT), CodeBinaryOperatorType.BooleanOr,
                        new CodeBinaryOperatorExpression(new CodeSnippetExpression("(i = _shift_ofsts[stateno])"), CodeBinaryOperatorType.IdentityEquality, _emitCtx.pSHIFT_USE_DFLT)),
                    new CodeMethodReturnStatement(new CodeIndexerExpression(_defaults, stateno))));
                /* other way of inlineing
                s.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(stateno, CodeBinaryOperatorType.LessThanOrEqual, _emitCtx.pSHIFT_COUNT),
                    new CodeAssignStatement(i, new CodeIndexerExpression(_shift_ofsts, stateno))));
                s.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(stateno, CodeBinaryOperatorType.GreaterThan, _emitCtx.pSHIFT_COUNT), CodeBinaryOperatorType.BooleanOr,
                        new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.IdentityEquality, _emitCtx.pSHIFT_USE_DFLT)),
                    new CodeMethodReturnStatement(new CodeIndexerExpression(_defaults, stateno))));
                */
                // Debug.Assert(lookahead != NOCODE);
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                    new CodeBinaryOperatorExpression(lookahead, CodeBinaryOperatorType.IdentityInequality, _emitCtx.pNOCODE))));
                // i += lookahead;
                // if (i < 0 || i >= ACTTAB_COUNT || _lookahead[i] != lookahead) {
                //     _CS0_
                //    if (lookahead > 0) { _CS1_ }
                // }
                s.Add(new CodeAssignStatement(i, new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.Add, lookahead)));
                s.Add(cs[0] = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanOr,
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.GreaterThanOrEqual, _emitCtx.pACTIONS), CodeBinaryOperatorType.BooleanOr,
                            new CodeBinaryOperatorExpression(new CodeIndexerExpression(_lookaheads, i), CodeBinaryOperatorType.IdentityInequality, lookahead)))
                ));
                cs[0].TrueStatements.Add(cs[1] = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(lookahead, CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0))));
                // #ifdef FALLBACK
                if (_emitCtx.FALLBACK)
                {
                    // CODETYPE fallback; /* Fallback token */
                    // if (lookahead < _fallback.Length && (fallback = _fallbacks[lookahead]) != 0) { _CS2_ }
                    var fallback = new CodeVariableReferenceExpression("fallback");
                    cs[1].TrueStatements.Add(new CodeVariableDeclarationStatement(_emitCtx.CODETYPE, "fallback"));
                    cs[1].TrueStatements.Add(cs[2] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(lookahead, CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(_fallbacks, "Length")), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(new CodeSnippetExpression("(fallback = _fallbacks[lookahead])"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)))));
                    // #ifndef NDEBUG
                    if (!_emitCtx.NDEBUG)
                    {
                        // if (_tracePrompt != null)
                        //    Trace.WriteLine(string.Format("{0}FALLBACK {1} => {2}", _tracePrompt, _tokenNames[lookahead], _tokenNames[fallback]));
                        cs[2].TrueStatements.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}FALLBACK {1} => {2}"), _tracePrompt, new CodeIndexerExpression(_tokenNames, lookahead), new CodeIndexerExpression(_tokenNames, fallback))))));
                    }
                    // return FindShiftAction(fallback);
                    cs[2].TrueStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(findShiftAction, fallback)));
                }
                // #ifdef WILDCARD
                if (_emitCtx.WILDCARD > 0)
                {
                    // int j = i - lookahead + WILDCARD;
                    var j = new CodeVariableReferenceExpression("j");
                    cs[1].TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "j", new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(lookahead, CodeBinaryOperatorType.Add, _emitCtx.pWILDCARD))));
                    // if ( _b_ ) { _CS2_ }                    
                    {
                        // _lookaheads[j] == WILDCARD
                        var b = new CodeBinaryOperatorExpression(new CodeIndexerExpression(_lookaheads, j), CodeBinaryOperatorType.IdentityEquality, _emitCtx.pWILDCARD);
                        // #if SHIFT_MAX+WILDCARD >= ACTTAB_COUNT
                        // j < ACTTAB_COUNT &&
                        if (_emitCtx.SHIFT_MAX + _emitCtx.WILDCARD >= _emitCtx.ACTIONS)
                            b = new CodeBinaryOperatorExpression(b, CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(j, CodeBinaryOperatorType.LessThan, _emitCtx.pACTIONS));
                        // #if SHIFT_MIN+WILDCARD < 0
                        // j >= 0 &&
                        if (_emitCtx.SHIFT_MIN + _emitCtx.WILDCARD < 0)
                            b = new CodeBinaryOperatorExpression(b, CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(j, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)));
                        cs[1].TrueStatements.Add(cs[2] = new CodeConditionStatement(b));
                    }
                    // #ifndef NDEBUG
                    if (!_emitCtx.NDEBUG)
                    {
                        // if (_tracePrompt != null)
                        //    Trace.WriteLine(string.Format("{0}WILDCARD {1} => {2}", _tracePrompt, _tokenNames[lookahead], _tokenNames[WILDCARD]));
                        cs[2].TrueStatements.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}WILDCARD {1} => {2}"), _tracePrompt, new CodeIndexerExpression(_tokenNames, lookahead), new CodeIndexerExpression(_tokenNames, _emitCtx.pWILDCARD))))));
                    }
                    // return _actions[j];
                    cs[2].TrueStatements.Add(new CodeMethodReturnStatement(new CodeIndexerExpression(_actions, j)));
                }
                // return _defaults[stateno];
                cs[0].TrueStatements.Add(new CodeMethodReturnStatement(new CodeIndexerExpression(_defaults, stateno)));
                // return _actions[i];
                cs[0].FalseStatements.Add(new CodeMethodReturnStatement(new CodeIndexerExpression(_actions, i)));
            }

            // #
            // protected int FindReduceAction();
            var findReduceAction = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "FindReduceAction");
            var findReduceActionMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, ReturnType = new CodeTypeReference(typeof(int)), Name = "FindReduceAction" };
            findReduceActionMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "stateno"));
            findReduceActionMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.CODETYPE, "lookahead"));
            findReduceActionMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "FindReduceAction"));
            findReduceActionMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "FindReduceAction"));
            target.Members.Add(findReduceActionMethod);
            c = findReduceActionMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>"));
            c.Add(new CodeCommentStatement("Find the appropriate action for a parser given the non-terminal look-ahead token lookahead.", false));
            c.Add(new CodeCommentStatement("If the look-ahead token is NOCODE, then check to see if the action is independent of the look-ahead.  If it is, return the action, otherwise return NO_ACTION.", false));
            c.Add(new CodeCommentStatement("</summary>"));
            s = findReduceActionMethod.Statements;
            {
                var stateno = new CodeVariableReferenceExpression("stateno");
                var lookahead = new CodeVariableReferenceExpression("lookahead");
                // #ifdef ERRORSYMBOL
                if (_emitCtx.ERRORSYMBOL > 0)
                {
                    // if (stateno > REDUCE_COUNT)
                    //    return _defaults[stateno];
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(stateno, CodeBinaryOperatorType.GreaterThan, _emitCtx.pREDUCE_COUNT),
                        new CodeMethodReturnStatement(new CodeIndexerExpression(_defaults, stateno))
                    ));
                }
                // #else
                else
                    // Debug.Assert(stateno <= REDUCE_COUNT);
                    s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                        new CodeBinaryOperatorExpression(stateno, CodeBinaryOperatorType.LessThanOrEqual, _emitCtx.pREDUCE_COUNT))));
                // int i = _reduce_ofsts[stateno];
                var i = new CodeVariableReferenceExpression("i");
                s.Add(new CodeVariableDeclarationStatement(typeof(int), "i", new CodeIndexerExpression(_reduce_ofsts, stateno)));
                // Debug.Assert(i != REDUCE_USE_DFLT);
                // Debug.Assert(lookahead != NOCODE);
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                    new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.IdentityInequality, _emitCtx.pREDUCE_USE_DFLT))));
                s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                    new CodeBinaryOperatorExpression(lookahead, CodeBinaryOperatorType.IdentityInequality, _emitCtx.pNOCODE))));
                // i += lookahead;
                s.Add(new CodeAssignStatement(i, new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.Add, lookahead)));
                // #ifdef ERRORSYMBOL
                if (_emitCtx.ERRORSYMBOL > 0)
                    // if (i < 0 || i >= ACTTAB_COUNT || _lookahead[i] != lookahead)
                    //    return _defaults[stateno];
                    s.Add(cs[0] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanOr,
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.GreaterThanOrEqual, _emitCtx.pACTIONS), CodeBinaryOperatorType.BooleanOr,
                                new CodeBinaryOperatorExpression(new CodeIndexerExpression(_lookaheads, i), CodeBinaryOperatorType.IdentityInequality, lookahead))),
                        new CodeMethodReturnStatement(new CodeIndexerExpression(_defaults, stateno))
                    ));
                // #else
                else
                {
                    // Debug.Assert(i >= 0 && i < ACTTAB_COUNT);
                    // Debug.Assert(_lookahead[i] == lookahead);
                    s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.LessThan, _emitCtx.pACTIONS)))));
                    s.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                        new CodeBinaryOperatorExpression(new CodeIndexerExpression(_lookaheads, i), CodeBinaryOperatorType.IdentityEquality, lookahead))));
                }
                // return _actions[i];
                s.Add(new CodeMethodReturnStatement(new CodeIndexerExpression(_actions, i)));
            }

            // #
            // protected void Shift();
            var shift = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Shift");
            var shiftMethod = new CodeMemberMethod { Attributes = MemberAttributes.Family, Name = "Shift" };
            shiftMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "newState"));
            shiftMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "major"));
            shiftMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.MINORTYPE, "minor"));
            shiftMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Shift"));
            shiftMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Shift"));
            target.Members.Add(shiftMethod);
            c = shiftMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>"));
            c.Add(new CodeCommentStatement("Perform a shift action."));
            c.Add(new CodeCommentStatement("</summary>"));
            s = shiftMethod.Statements;
            {
                var newState = new CodeVariableReferenceExpression("newState");
                var major = new CodeVariableReferenceExpression("major");
                var minor = new CodeVariableReferenceExpression("minor");
                // _idx++;
                s.Add(new CodeAssignStatement(_idx, new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
                // #ifdef TRACKMAXSTACKDEPTH
                if (_emitCtx.TRACKMAXSTACKDEPTH)
                {
                    // if (_idx > _idxMax) _idxMax = _idx;
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThan, _idxMax),
                        new CodeAssignStatement(_idxMax, _idx)));
                }
                // #ifdef STACKDEPTH > 0
                if (_emitCtx.STACKDEPTH > 0)
                {
                    // if (_idx >= STACKDEPTH) { StackOverflow(minor); return; }
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, _emitCtx.pSTACKDEPTH),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(stackOverflow, minor)),
                        new CodeMethodReturnStatement()));
                }
                // #else
                else
                {
                    // if (_idx >= _stackLength) {
                    //    GrowStack();
                    //    _CS0_
                    // }
                    s.Add(cs[0] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, _stackLength),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(growStack))));
                    // if (_idx >= _stackLength) { StackOverflow(minor); return; }
                    cs[0].TrueStatements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, _stackLength),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(stackOverflow, minor)),
                        new CodeMethodReturnStatement()));
                }
                // var tos = _stack[_idx];
                // tos.stateno = (ACTIONTYPE)newState;
                // tos.major = (CODETYPE)major;
                // tos.minor = minor;
                // _stack[_idx] = tos;
                var tos = new CodeVariableReferenceExpression("tos");
                s.Add(new CodeVariableDeclarationStatement(stackEntryType, "tos", new CodeIndexerExpression(_stack, _idx)));
                s.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(tos, "stateno"), new CodeCastExpression(_emitCtx.ACTIONTYPE, newState)));
                s.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(tos, "major"), new CodeCastExpression(_emitCtx.CODETYPE, major)));
                s.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(tos, "minor"), minor));
                s.Add(new CodeAssignStatement(new CodeIndexerExpression(_stack, _idx), tos));
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null && _idx > 0) {
                    //    Trace.WriteLine(string.Format("{0}Shift {1}", _tracePrompt, newState));
                    //    _CS0_
                    // }
                    s.Add(cs[0] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0))),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Shift {1}"), _tracePrompt, newState)))));
                    // var b = new StringBuilder(string.Format("{0}Stack:", _tracePrompt));
                    var b = new CodeVariableReferenceExpression("b");
                    cs[0].TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(StringBuilder), "b",
                        new CodeObjectCreateExpression(typeof(StringBuilder),
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Stack:"), _tracePrompt))));
                    // for (int i = 1; i <= _idx; i++)
                    //    b.AppendFormat(" {0}", _tokenNames[_stack[i].major]);
                    var i = new CodeVariableReferenceExpression("i");
                    cs[0].TrueStatements.Add(new CodeIterationStatement(
                        new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(1)),
                        new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.LessThanOrEqual, _idx),
                        new CodeAssignStatement(i, new CodeBinaryOperatorExpression(i, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(b, "AppendFormat",
                            new CodeIndexerExpression(_tokenNames, new CodeMethodReferenceExpression(new CodeIndexerExpression(_stack, i), "major"))))));
                    // b.AppendLine();
                    cs[0].TrueStatements.Add(new CodeMethodInvokeExpression(b, "ToString"));
                    // Trace.WriteLine(b.ToString());
                    cs[0].TrueStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(b, "ToString")));
                }
            }

            // #
            // public void Parse(int major, TOKENTYPE minor);
            var parseMethod = new CodeMemberMethod { Attributes = MemberAttributes.Public, Name = "Parse" };
            parseMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "major"));
            parseMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.TOKENTYPE, "minor"));
            if (!string.IsNullOrEmpty(_emitCtx.ParseArg))
                parseMethod.Parameters.Add(new CodeParameterDeclarationExpression(_emitCtx.ParseArgType, _emitCtx.ParseArg));
            parseMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Parse"));
            parseMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Parse"));
            target.Members.Add(parseMethod);
            c = parseMethod.Comments;
            c.Add(new CodeCommentStatement("<summary>"));
            c.Add(new CodeCommentStatement("The main parser."));
            c.Add(new CodeCommentStatement("</summary>"));
            s = parseMethod.Statements;
            {
                var major = new CodeVariableReferenceExpression("major");
                var minor = new CodeVariableReferenceExpression("minor");
                var extraArg = new CodeVariableReferenceExpression("extraArg");
                // (re)initialize the parser, if necessary
                // if (_idx < 0) { _CS0_ }
                s.Add(cs[0] = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0))));
                // MINORTYPE minorunion;
                var minorUnion = new CodeVariableReferenceExpression("minorUnion");
                s.Add(new CodeVariableDeclarationStatement(_emitCtx.MINORTYPE, "minorUnion", _zeroMinor));
                // #if STACKDEPTH <= 0
                if (_emitCtx.STACKDEPTH <= 0)
                {
                    // if (_stackLength <= 0) {
                    //    minorUnion = _zeroMinor;
                    //    StackOverflow(minorUnion);
                    //    return;
                    // }
                    cs[0].TrueStatements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_stackLength, CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(0)),
                        new CodeAssignStatement(minorUnion, _zeroMinor),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(stackOverflow, minorUnion)),
                        new CodeMethodReturnStatement()));
                }
                // _idx = 0;
                // _errors = -1;
                // _stack[0].stateno = 0;
                // _stack[0].major = 0;
                cs[0].TrueStatements.Add(new CodeAssignStatement(_idx, new CodePrimitiveExpression(0)));
                cs[0].TrueStatements.Add(new CodeAssignStatement(_errors, new CodePrimitiveExpression(-1)));
                cs[0].TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, new CodePrimitiveExpression(0)), "stateno"), new CodePrimitiveExpression(0)));
                cs[0].TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, new CodePrimitiveExpression(0)), "major"), new CodePrimitiveExpression(0)));
                // minorunion.yy0 = minor;
                s.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(minorUnion, "yy0"), minor));
                // var endOfInput = (major == 0);
                var endOfInput = new CodeVariableReferenceExpression("endOfInput");
                s.Add(new CodeVariableDeclarationStatement(typeof(bool), "endOfInput", new CodeBinaryOperatorExpression(major, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0))));
                // ParseARG_STORE
                if (_emitCtx.ParseARG_STORE != null)
                    _emitCtx.ParseARG_STORE(s);
                // #ifndef NDEBUG
                if (!_emitCtx.NDEBUG)
                {
                    // if (_tracePrompt != null) {
                    //    Trace.WriteLine(string.Format("{0}Input {1}", _tracePrompt, _tokenNames[major]));
                    s.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Input {1}"), _tracePrompt, new CodeIndexerExpression(_tokenNames, major))))));
                }
                var errorHit = new CodeVariableReferenceExpression("errorHit");
                // #ifdef YYERRORSYMBOL
                if (_emitCtx.ERRORSYMBOL > 0)
                    // var errorHit = false;
                    s.Add(new CodeVariableDeclarationStatement(typeof(bool), "errorHit", new CodePrimitiveExpression(false)));
                // do { _S2_ } while (major != NOCODE && _idx >= 0);
                // for (bool _do1 = true; _do; _do = (major != NOCODE && _idx >= 0)) { _S2_ }
                CodeIterationStatement loop;
                s.Add(loop = new CodeIterationStatement(
                    new CodeVariableDeclarationStatement(typeof(bool), "do1", new CodePrimitiveExpression(true)),
                    new CodeVariableReferenceExpression("do1"),
                    new CodeAssignStatement(new CodeVariableReferenceExpression("do1"),
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(major, CodeBinaryOperatorType.IdentityInequality, _emitCtx.pNOCODE), CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0))))));
                var s2 = loop.Statements;
                {
                    // int action = FindShiftAction((CODETYPE)major);
                    var action = new CodeVariableReferenceExpression("action");
                    s2.Add(new CodeVariableDeclarationStatement(typeof(int), "action", new CodeMethodInvokeExpression(findShiftAction, new CodeCastExpression(_emitCtx.CODETYPE, major))));
                    // if (act < NSTATE) { _CS0_ }
                    s2.Add(cs[0] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.LessThan, _emitCtx.pSTATES)));
                    // Debug.Assert(!endOfInput);
                    // Shift(act, major, minorUnion);
                    // _errors++;
                    // major = NOCODE;
                    cs[0].TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                        new CodeBinaryOperatorExpression(endOfInput, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false)))));
                    cs[0].TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(shift, action, major, minorUnion)));
                    cs[0].TrueStatements.Add(new CodeAssignStatement(_errors, new CodeBinaryOperatorExpression(_errors, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
                    cs[0].TrueStatements.Add(new CodeAssignStatement(major, _emitCtx.pNOCODE));
                    // } else if (act < NSTATE + NRULE) { Reduce(act - NSTATE); _CS0_ }
                    cs[0].FalseStatements.Add(cs[0] = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(_emitCtx.STATES + _emitCtx.RULES)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(reduce, new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.Subtract, _emitCtx.pSTATES)))));
                    // } else { _S2_ }
                    s2 = cs[0].FalseStatements;
                    // Debug.Assert(act == ERROR_ACTION);
                    s2.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Debug"), "Assert",
                        new CodeBinaryOperatorExpression(action, CodeBinaryOperatorType.IdentityEquality, _emitCtx.pERROR_ACTION))));
                    // #ifndef NDEBUG
                    if (!_emitCtx.NDEBUG)
                    {
                        // if (_tracePrompt != null) {
                        //    Trace.WriteLine(string.Format("{0}Syntax Error!", _tracePrompt));
                        s2.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Syntax Error!"), _tracePrompt)))));
                    }
                    // #ifdef ERRORSYMBOL
                    if (_emitCtx.ERRORSYMBOL > 0)
                    {
                        // if (_errors < 0)
                        //    SyntaxError(major, minorUnion);
                        s2.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_errors, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(syntaxError, major, minorUnion))));
                        // var mx = _stack[_idx].major;
                        var mx = new CodeVariableReferenceExpression("mx");
                        s2.Add(new CodeVariableDeclarationStatement(_emitCtx.CODETYPE, "mx", new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, _idx), "major")));
                        // if (mx == ERRORSYMBOL || errorHit) { _CS0_ }
                        s2.Add(cs[0] = new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(mx, CodeBinaryOperatorType.IdentityEquality, _emitCtx.pERRORSYMBOL), CodeBinaryOperatorType.BooleanOr,
                                new CodeBinaryOperatorExpression(errorHit, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)))));
                        // #ifndef NDEBUG
                        if (!_emitCtx.NDEBUG)
                        {
                            // if (_tracePrompt != null) {
                            //    Trace.WriteLine(string.Format("{0}Discard input token {1}", _tracePrompt, _tokenNames[major]));
                            cs[0].TrueStatements.Add(new CodeConditionStatement(
                                new CodeBinaryOperatorExpression(_tracePrompt, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                                new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Trace"), "WriteLine",
                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("String"), "Format", new CodePrimitiveExpression("{0}Discard input token {1}"), _tracePrompt, new CodeIndexerExpression(_tokenNames, major))))));
                        }
                        // Destructor((CODETYPE)major, minorUnion);
                        // major = NOCODE;
                        cs[0].TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(destructor, new CodeCastExpression(_emitCtx.CODETYPE, major), minorUnion)));
                        cs[0].TrueStatements.Add(new CodeAssignStatement(major, _emitCtx.pNOCODE));
                        // while (_idx >= 0 && mx != ERRORSYMBOL && (action = FindReduceAction(_stack[_idx].stateno, ERRORSYMBOL)) >= NSTATE)
                        //    PopParserStack();
                        cs[0].FalseStatements.Add(new CodeIterationStatement(new CodeStatement(),
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(
                                    new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanAnd,
                                    new CodeBinaryOperatorExpression(mx, CodeBinaryOperatorType.IdentityInequality, _emitCtx.pERRORSYMBOL)), CodeBinaryOperatorType.BooleanAnd,
                                new CodeBinaryOperatorExpression(new CodeSnippetExpression("action = FindReduceAction(_stack[_idx].stateno, ERRORSYMBOL"), CodeBinaryOperatorType.GreaterThanOrEqual, _emitCtx.pSTATES)),
                            new CodeStatement(),
                            //new CodeAssignStatement(act, new CodeMethodInvokeExpression(findReduceAction, new CodeFieldReferenceExpression(new CodeIndexerExpression(_stack, _idx), "stateno"), emitCtx.pERRORSYMBOL),
                            //new CodeConditionStatement(new CodeBinaryOperatorExpression(act, CodeBinaryOperatorType.GreaterThanOrEqual, emitCtx.pNSTATE), new CodeSnippetExpression("break")
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(popParserStack))));
                        // if (_idx < 0 || major == 0) {
                        //    Destructor((CODETYPE)major, minorUnion);
                        //    ParseFailed();
                        //    major = NOCODE;
                        //    _CS1_ }
                        cs[0].FalseStatements.Add(cs[1] = new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(_idx, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)), CodeBinaryOperatorType.BooleanOr,
                                new CodeBinaryOperatorExpression(major, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0))),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(destructor, new CodeCastExpression(_emitCtx.CODETYPE, major), minorUnion)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(parseFailed)),
                            new CodeAssignStatement(major, _emitCtx.pNOCODE)));
                        // } else if (mx != ERRORSYMBOL) {
                        //    MINORTYPE u2;
                        //    u2.YYERRSYMDT = 0;
                        //    Shift(act, ERRORSYMBOL, &u2);
                        // }
                        var u2 = new CodeVariableReferenceExpression("u2");
                        cs[1].FalseStatements.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(mx, CodeBinaryOperatorType.LessThanOrEqual, _emitCtx.pERRORSYMBOL),
                            new CodeVariableDeclarationStatement(_emitCtx.MINORTYPE, "u2"),
                            new CodeAssignStatement(new CodeFieldReferenceExpression(u2, "ERRSYMDT"), new CodePrimitiveExpression(0)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(shift, action, _emitCtx.pERRORSYMBOL, u2))
                            ));
                        // _errors = 3;
                        // errorHit = true;
                        s2.Add(new CodeAssignStatement(_errors, new CodePrimitiveExpression(3)));
                        s2.Add(new CodeAssignStatement(errorHit, new CodePrimitiveExpression(true)));

                    }
                    // #elif defined(NOERRORRECOVERY)
                    else if (_emitCtx.NOERRORRECOVERY)
                    {
                        // SyntaxError(major, minorUnion);
                        // Destructor((CODETYPE)major, minorUnion);
                        // major = NOCODE;
                        s2.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(syntaxError, major, minorUnion)));
                        s2.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(destructor, new CodeCastExpression(_emitCtx.CODETYPE, major), minorUnion)));
                        s2.Add(new CodeAssignStatement(major, _emitCtx.pNOCODE));
                    }
                    else
                    {
                        // if (_errors <= 0)
                        //    SyntaxError(major, minorUnion);
                        // _errors = 3;
                        // Destructor((CODETYPE)major, minorUnion);
                        s2.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(_errors, CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(0)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(syntaxError, major, minorUnion))));
                        s2.Add(new CodeAssignStatement(_errors, new CodePrimitiveExpression(3)));
                        s2.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(destructor, new CodeCastExpression(_emitCtx.CODETYPE, major), minorUnion)));
                        // if (endOfInput)
                        //    ParseFailed();
                        // major = NOCODE;
                        s2.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(endOfInput, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(parseFailed))));
                        s2.Add(new CodeAssignStatement(major, _emitCtx.pNOCODE));
                    }
                }
            }
        }
    }
}
