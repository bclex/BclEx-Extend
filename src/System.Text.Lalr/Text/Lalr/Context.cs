using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Text.Lalr
{
    /// <summary>
    /// Context
    /// </summary>
    public class Context
    {
        internal readonly SymbolCollection AllSymbols = new SymbolCollection();
        internal readonly ConfigCollection AllConfigs = new ConfigCollection();
        internal readonly StateCollection AllStates = new StateCollection();
        private static readonly Action.KeyComparer _actionComparer = new Action.KeyComparer();
        private static readonly State.KeyComparer _stateComparer = new State.KeyComparer();
        /// <summary>
        /// Sorted
        /// </summary>
        public State[] Sorted;
        /// <summary>
        /// Rule
        /// </summary>
        public Rule Rule;
        /// <summary>
        /// States
        /// </summary>
        public int States;
        /// <summary>
        /// Rules
        /// </summary>
        public int Rules;
        /// <summary>
        /// Terminals
        /// </summary>
        public int Terminals;
        /// <summary>
        /// Symbols
        /// </summary>
        public Symbol[] Symbols;
        /// <summary>
        /// DataTypes
        /// </summary>
        public Dictionary<string, int> DataTypes;
        /// <summary>
        /// Errors
        /// </summary>
        public int Errors;
        /// <summary>
        /// ErrorSymbol
        /// </summary>
        public Symbol ErrorSymbol;
        /// <summary>
        /// Wildcard
        /// </summary>
        public Symbol Wildcard;
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
        /// <summary>
        /// ExtraArg
        /// </summary>
        public string ExtraArg;
        /// <summary>
        /// TokenType
        /// </summary>
        public string TokenType;
        /// <summary>
        /// DefaultDataType
        /// </summary>
        public string DefaultDataType;
        internal string StartSymbol;
        /// <summary>
        /// StackSize
        /// </summary>
        public string StackSize;
        /// <summary>
        /// Include
        /// </summary>
        public string Include;
        /// <summary>
        /// SyntaxError
        /// </summary>
        public string SyntaxError;
        /// <summary>
        /// StackOverflow
        /// </summary>
        public string StackOverflow;
        /// <summary>
        /// ParseFailure
        /// </summary>
        public string ParseFailure;
        /// <summary>
        /// ParseAccept
        /// </summary>
        public string ParseAccept;
        /// <summary>
        /// ExtraCode
        /// </summary>
        public string ExtraCode;
        /// <summary>
        /// TokenDestructor
        /// </summary>
        public string TokenDestructor;
        /// <summary>
        /// DefaultDestructor
        /// </summary>
        public string DefaultDestructor;
        /// <summary>
        /// InputFilePath
        /// </summary>
        public string InputFilePath;
        /// <summary>
        /// TokenPrefix
        /// </summary>
        public string TokenPrefix;
        /// <summary>
        /// Conflicts
        /// </summary>
        public int Conflicts;
        /// <summary>
        /// HasFallback
        /// </summary>
        public bool HasFallback;
        /// <summary>
        /// NoShowLinenos
        /// </summary>
        public bool NoShowLinenos;
        internal Action<int, string, object[]> _errorCallback;
        internal Action<int, string, object[]> _warningCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="errorCallback">The error callback.</param>
        /// <param name="warningCallback">The warning callback.</param>
        public Context(Action<int, string, object[]> errorCallback, Action<int, string, object[]> warningCallback)
        {
            _errorCallback = errorCallback;
            _warningCallback = warningCallback;
            Symbol.New(this, "$", false);
            ErrorSymbol = Symbol.New(this, "error", false);
        }

        /// <summary>
        /// Ensures this instance.
        /// </summary>
        public void Ensure()
        {
            if (string.IsNullOrEmpty(TokenPrefix))
                TokenPrefix = "Token";
        }

        /// <summary>
        /// Raises the error.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="lineno">The lineno.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void RaiseError(ref int errors, int lineno, string format, params object[] args) { errors++; _errorCallback(lineno, format, args); }

        /// <summary>
        /// Raises the warning.
        /// </summary>
        /// <param name="lineno">The lineno.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void RaiseWarning(int lineno, string format, params object[] args) { _warningCallback(lineno, format, args); }

        #region Process

        /// <summary>
        /// Processes this instance.
        /// </summary>
        public void Process()
        {
            // Find the precedence for every production rule (that has one)
            BuildRulesPrecedences();
            // Compute the lambda-nonterminals and the first-sets for every nonterminal
            BuildFirstSets();
            // Compute all LR(0) states.  Also record follow-set propagation links so that the follow-set can be computed later
            BuildStates();
            // Tie up loose ends on the propagation links
            BuildLinks();
            // Compute the follow set of every reducible configuration
            BuildFollowSets();
            // Compute the action tables
            BuildActions();
            // Compute the DataTypeID
            BuildDataTypes();
            // Compress the action tables
            CompressTables();
            // Reorder and renumber the states so that states with fewer choices occur at the end.  This is an optimization that helps make the generated parser tables smaller.
            ResortStates();
        }

        private void BuildDataTypes()
        {
            /* Build a hash table of datatypes. The ".dtnum" field of each symbol is filled in with the hash index plus 1.  A ".dtnum" value of 0 is
            ** used for terminal symbols.  If there is no %default_type defined then 0 is also used as the .dtnum value for nonterminals which do not specify
            ** a datatype using the %type directive. */
            var types = new Dictionary<string, int>();
            for (var i = 0; i < Symbols.Length - 1; i++)
            {
                var symbol = Symbols[i];
                if (symbol == ErrorSymbol)
                {
                    symbol.DataTypeID = int.MaxValue;
                    continue;
                }
                if (symbol.Type != SymbolType.NonTerminal || (symbol.DataType == null && DefaultDataType == null))
                {
                    symbol.DataTypeID = 0;
                    continue;
                }
                var z = (symbol.DataType ?? DefaultDataType);
                var cp = 0;
                while (char.IsWhiteSpace(z[cp])) cp++;
                var dataType = z.Substring(cp).TrimEnd();
                if (TokenType != null && dataType == TokenType)
                {
                    symbol.DataTypeID = 0;
                    continue;
                }
                int value;
                if (types.TryGetValue(dataType, out value))
                    symbol.DataTypeID = value;
                else
                    types.Add(dataType, symbol.DataTypeID = types.Count + 1);
            }
            DataTypes = types;
        }

        private void BuildRulesPrecedences()
        {
            for (var rule = Rule; rule != null; rule = rule.Next)
                if (rule.PrecedenceSymbol == null)
                    for (var index = 0; index < rule.RHSymbols.Length && rule.PrecedenceSymbol == null; index++)
                    {
                        var symbol = rule.RHSymbols[index];
                        if (symbol.Type == SymbolType.MultiTerminal)
                        {
                            for (var childenIndex = 0; childenIndex < symbol.Children.Length; childenIndex++)
                                if (symbol.Children[childenIndex].Precedence >= 0)
                                {
                                    rule.PrecedenceSymbol = symbol.Children[childenIndex];
                                    break;
                                }
                        }
                        else if (symbol.Precedence >= 0)
                            rule.PrecedenceSymbol = rule.RHSymbols[index];
                    }
        }

        private void BuildFirstSets()
        {
            for (var i = 0; i < Symbols.Length - 1; i++)
                Symbols[i].Lambda = false;
            for (var i = Terminals; i < Symbols.Length - 1; i++)
                Symbols[i].FirstSet = new HashSet<int>();
            /* First compute all lambdas */
            bool progress = false;
            do
            {
                progress = false;
                for (var rule = Rule; rule != null; rule = rule.Next)
                {
                    if (rule.LHSymbol.Lambda)
                        continue;
                    var i = 0;
                    for (; i < rule.RHSymbols.Length; i++)
                    {
                        var symbol = rule.RHSymbols[i];
                        if (symbol.Type != SymbolType.Terminal || !symbol.Lambda)
                            break;
                    }
                    if (i == rule.RHSymbols.Length)
                    {
                        rule.LHSymbol.Lambda = true;
                        progress = true;
                    }
                }
            } while (progress);
            /* Now compute all first sets */
            do
            {
                progress = false;
                for (var rule = Rule; rule != null; rule = rule.Next)
                {
                    var lhSymbol = rule.LHSymbol;
                    for (var i = 0; i < rule.RHSymbols.Length; i++)
                    {
                        var rhSymbol = rule.RHSymbols[i];
                        if (rhSymbol.Type == SymbolType.Terminal)
                        {
                            progress |= lhSymbol.FirstSet.Add(rhSymbol.ID);
                            break;
                        }
                        else if (rhSymbol.Type == SymbolType.MultiTerminal)
                        {
                            for (var j = 0; j < rhSymbol.Children.Length; j++)
                                progress |= lhSymbol.FirstSet.Add(rhSymbol.Children[j].ID);
                            break;
                        }
                        else if (lhSymbol == rhSymbol)
                        {
                            if (!lhSymbol.Lambda)
                                break;
                        }
                        else
                        {
                            progress |= lhSymbol.FirstSet.AddRange(rhSymbol.FirstSet);
                            if (!rhSymbol.Lambda)
                                break;
                        }
                    }
                }
            } while (progress);
        }

        private void BuildStates()
        {
            States = 0;
            /* Find the start symbol */
            Symbol symbol;
            if (!string.IsNullOrEmpty(StartSymbol))
            {
                symbol = AllSymbols[StartSymbol];
                if (symbol == null)
                {
                    RaiseError(ref Errors, 0, "The specified start symbol \"{0}\" is not in a nonterminal of the grammar.  \"{1}\" will be used as the start symbol instead.", StartSymbol, Rule.LHSymbol.Name);
                    symbol = Rule.LHSymbol;
                }
            }
            else
                symbol = Rule.LHSymbol;
            /* Make sure the start symbol doesn't occur on the right-hand side of any rule.  Report an error if it does.  (YACC would generate a new start symbol in this case.) */
            for (var rule = Rule; rule != null; rule = rule.Next)
                for (var i = 0; i < rule.RHSymbols.Length; i++)
                    if (rule.RHSymbols[i] == symbol)
                        /* FIX ME:  Deal with multiterminals */
                        RaiseError(ref Errors, 0, "The start symbol \"{0}\" occurs on the right-hand side of a rule. This will result in a parser which does not work properly.", symbol.Name);
            /* The basis configuration set for the first state is all rules which have the start symbol as their left-hand side */
            for (var rule = symbol.Rule; rule != null; rule = rule.NextLHSymbol)
            {
                rule.LHSymbolStart = 1;
                var newConfig = AllConfigs.AddBasis(rule, 0);
                newConfig.FwSet.Add(0);
            }
            /* Compute the first state.  All other states will be computed automatically during the computation of the first one. The returned pointer to the first state is not used. */
            AllStates.GetState(this);
            Sorted = AllStates.ToStateArray();
        }

        private void BuildLinks()
        {
            for (var i = 0; i < States; i++)
            {
                var state = Sorted[i];
                foreach (var config in state.Configs)
                {
                    /* Add to every propagate link a pointer back to the state to which the link is attached. */
                    config.State = state;
                    /* Convert all backlinks into forward links.  Only the forward links are used in the follow-set computation. */
                    foreach (var other in config.Basises)
                        other.Forwards.Add(config);
                }
            }
        }

        private void BuildFollowSets()
        {
            for (var i = 0; i < States; i++)
                foreach (var config in Sorted[i].Configs)
                    config.Complete = false;
            bool progress = false;
            do
            {
                progress = false;
                for (var i = 0; i < States; i++)
                    foreach (var config in Sorted[i].Configs)
                    {
                        if (config.Complete)
                            continue;
                        foreach (var config2 in config.Forwards)
                        {
                            var change = config2.FwSet.AddRange(config.FwSet);
                            if (change)
                            {
                                config2.Complete = false;
                                progress = true;
                            }
                        }
                        config.Complete = true;
                    }
            } while (progress);
        }

        private void BuildActions()
        {
            /* Add all of the reduce actions. A reduce action is added for each element of the followset of a configuration which has its dot at the extreme right. */
            for (var i = 0; i < States; i++)
            {
                var state = Sorted[i];
                foreach (var config in state.Configs)
                    if (config.Rule.RHSymbols.Length == config.Dot)
                        for (var j = 0; j < Terminals; j++)
                            if (config.FwSet.Contains(j))
                                /* Add a reduce action to the state "stp" which will reduce by the rule "cfp->rp" if the lookahead symbol is "lemp->symbols[j]" */
                                state.Actions.Add(new Action
                                {
                                    Type = ActionType.Reduce,
                                    Symbol = Symbols[j],
                                    Rule = config.Rule,
                                });
            }
            /* Add the accepting token */
            var symbol2 = ((!string.IsNullOrEmpty(StartSymbol) ? AllSymbols[StartSymbol] : null) ?? Rule.LHSymbol);
            /* Add to the first state (which is always the starting state of the finite state machine) an action to ACCEPT if the lookahead is the start nonterminal.  */
            Sorted[0].Actions.Add(new Action
            {
                Type = ActionType.Accept,
                Symbol = symbol2,
            });
            /* Resolve conflicts */
            for (var i = 0; i < States; i++)
            {
                var symbol = Sorted[i];
                var symbolActions = symbol.Actions;
                Debug.Assert(symbolActions.Count > 0);
                symbolActions.Sort(_actionComparer);
                Action action, action2;
                for (var actionIndex = 0; actionIndex < symbolActions.Count && (action = symbolActions[actionIndex]) != null; actionIndex++)
                    for (var actionIndex2 = actionIndex + 1; actionIndex2 < symbolActions.Count && (action2 = symbolActions[actionIndex2]) != null && action2.Symbol == action.Symbol; actionIndex2++)
                        /* The two actions "ap" and "nap" have the same lookahead. Figure out which one should be used */
                        Conflicts += Action.ResolveConflict(action, action2, ErrorSymbol);
            }
            /* Report an error for each rule that can never be reduced. */
            for (var rule = Rule; rule != null; rule = rule.Next)
                rule.CanReduce = false;
            for (var i = 0; i < States; i++)
                foreach (var action in Sorted[i].Actions)
                    if (action.Type == ActionType.Reduce)
                        action.Rule.CanReduce = true;
            for (var rule = Rule; rule != null; rule = rule.Next)
            {
                if (rule.CanReduce)
                    continue;
                RaiseError(ref Errors, rule.RuleLineno, "This rule can not be reduced.\n");
            }
        }

        private void CompressTables()
        {
            for (var i = 0; i < States; i++)
            {
                var state = Sorted[i];
                var stateActions = state.Actions;
                //
                var bestN = 0;
                Rule bestRule = null;
                var usesWildcard = false;
                Action action;
                for (var actionIndex = 0; actionIndex < stateActions.Count && (action = stateActions[actionIndex]) != null; actionIndex++)
                {
                    if (action.Type == ActionType.Shift && action.Symbol == Wildcard) usesWildcard = true;
                    if (action.Type != ActionType.Reduce) continue;
                    var rule = action.Rule;
                    if (rule.LHSymbolStart > 0) continue;
                    if (rule == bestRule) continue;
                    var n = 1;
                    Action action2;
                    for (var actionIndex2 = actionIndex + 1; actionIndex2 < stateActions.Count && (action2 = stateActions[actionIndex2]) != null; actionIndex2++)
                    {
                        if (action2.Type != ActionType.Reduce) continue;
                        var rule2 = action2.Rule;
                        if (rule2 == bestRule) continue;
                        if (rule2 == rule) n++;
                    }
                    if (n > bestN)
                    {
                        bestN = n;
                        bestRule = rule;
                    }
                }
                /* Do not make a default if the number of rules to default is not at least 1 or if the wildcard token is a possible lookahead. */
                if (bestN < 1 || usesWildcard) continue;
                /* Combine matching REDUCE actions into a single default */
                Action action3 = null;
                var actionIndex3 = 0;
                for (; actionIndex3 < stateActions.Count && (action3 = stateActions[actionIndex3]) != null; actionIndex3++)
                    if (action3.Type == ActionType.Reduce && action3.Rule == bestRule)
                        break;
                Debug.Assert(action3 != null);
                action3.Symbol = Symbol.New(this, "{default}");
                for (actionIndex3++; actionIndex3 < stateActions.Count && (action3 = stateActions[actionIndex3]) != null; actionIndex3++)
                    if (action3.Type == ActionType.Reduce && action3.Rule == bestRule)
                        action3.Type = ActionType.NotUsed;
                state.Actions.Sort(_actionComparer);
            }
        }

        private void ResortStates()
        {
            for (var i = 0; i < States; i++)
            {
                var state = Sorted[i];
                state.TokenActions = state.NonTerminalActions = 0;
                state.Default = States + Rules;
                state.TokenOffset = State.NO_OFFSET;
                state.NonTerminalOffset = State.NO_OFFSET;
                int computeID;
                foreach (var action in state.Actions)
                    if ((computeID = action.ComputeID(this)) >= 0)
                    {
                        if (action.Symbol.ID < Terminals)
                            state.TokenActions++;
                        else if (action.Symbol.ID < Symbols.Length - 1)
                            state.NonTerminalActions++;
                        else
                            state.Default = computeID;
                    }
            }
            Array.Sort(Sorted, 1, Sorted.Length - 1, _stateComparer);
            for (var i = 0; i < States; i++)
                Sorted[i].ID = i;
        }

        #endregion

        /// <summary>
        /// Reprints the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        public void Reprint(StreamWriter w)
        {
            w.WriteLine("// Symbols:");
            var maxlen = 10;
            for (var i = 0; i < Symbols.Length; i++)
            {
                var symbol = Symbols[i];
                var len = symbol.Name.Length;
                if (len > maxlen)
                    maxlen = len;
            }
            var columns = 76 / (maxlen + 5);
            if (columns < 1)
                columns = 1;
            var skip = (Symbols.Length + columns - 1) / columns;
            for (var i = 0; i < skip; i++)
            {
                w.Write("//");
                for (var j = i; j < Symbols.Length; j += skip)
                {
                    var symbol = Symbols[j];
                    Debug.Assert(symbol.ID == j);
                    var symbolName = (symbol.Name.Length < maxlen ? symbol.Name : symbol.Name.Substring(0, maxlen));
                    w.Write(" {0,3} {1,-" + maxlen.ToString() + "}", j, symbolName);
                }
                w.WriteLine();
            }
            for (var rule = Rule; rule != null; rule = rule.Next)
            {
                w.Write("{0}", rule.LHSymbol.Name);
                if (rule.LHSymbolAlias != null)
                    w.Write("({0})", rule.LHSymbolAlias);
                w.Write(" ::=");
                for (var i = 0; i < rule.RHSymbols.Length; i++)
                {
                    var symbol = rule.RHSymbols[i];
                    w.Write(" {0}", symbol.Name);
                    if (symbol.Type == SymbolType.MultiTerminal)
                        for (var j = 1; j < symbol.Children.Length; j++)
                            w.Write("|{0}", symbol.Children[j].Name);
                    //if (rule.RHSymbolsAlias[i] != null) w.Write("({0})", rule.RHSymbolsAlias[i]);
                }
                w.Write(".");
                if (rule.PrecedenceSymbol != null) w.Write(" [{0}]", rule.PrecedenceSymbol.Name);
                //if (rule.Code != null) { b.WriteLine(); w.Write("    {0}", rule.Code);
                w.WriteLine();
            }
        }
    }
}
