using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Lalr
{
    /// <summary>
    /// State
    /// </summary>
    public class State
    {
        /// <summary>
        /// NO_OFFSET
        /// </summary>
        public const int NO_OFFSET = int.MaxValue;

        internal int ID;
        internal List<Config> Basises = new List<Config>();
        internal List<Config> Configs = new List<Config>();
        /// <summary>
        /// Actions
        /// </summary>
        public List<Action> Actions = new List<Action>();
        /// <summary>
        /// TokenActions
        /// </summary>
        public int TokenActions;
        /// <summary>
        /// NonTerminalActions
        /// </summary>
        public int NonTerminalActions;
        /// <summary>
        /// TokenOffset
        /// </summary>
        public int TokenOffset;
        /// <summary>
        /// NonTerminalOffset
        /// </summary>
        public int NonTerminalOffset;
        /// <summary>
        /// Default
        /// </summary>
        public int Default;

        internal class KeyComparer : IComparer<State>
        {
            public int Compare(State x, State y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;
                var v = y.NonTerminalActions - x.NonTerminalActions;
                if (v == 0)
                {
                    v = y.TokenActions - x.TokenActions;
                    if (v == 0)
                        v = y.ID - x.ID;
                }
                Debug.Assert(v != 0);
                return v;
            }
        }

        internal void BuildShifts(Context ctx)
        {
            /* Each configuration becomes complete after it contibutes to a successor state.  Initially, all configurations are incomplete */
            foreach (var config2 in Configs)
                config2.Complete = false;
            /* Loop through all configurations of the state "stp" */
            for (var configIndex = 0; configIndex < Configs.Count; configIndex++)
            {
                var config = Configs[configIndex];
                if (config.Complete)
                    continue;
                if (config.Dot >= config.Rule.RHSymbols.Length)
                    continue;
                ctx.AllConfigs.ListsReset();
                var symbol = config.Rule.RHSymbols[config.Dot];
                /* For every configuration in the state "stp" which has the symbol "sp" following its dot, add the same configuration to the basis set under construction but with the dot shifted one symbol to the right. */
                for (var basisConfigIndex = configIndex; basisConfigIndex < Configs.Count; basisConfigIndex++)
                {
                    var basisConfig = Configs[basisConfigIndex];
                    if (basisConfig.Complete)
                        continue;
                    if (basisConfig.Dot >= basisConfig.Rule.RHSymbols.Length)
                        continue;
                    var scanSymbol = basisConfig.Rule.RHSymbols[basisConfig.Dot];
                    if (scanSymbol != symbol)
                        continue;
                    basisConfig.Complete = true;
                    var newConfig = ctx.AllConfigs.AddBasis(basisConfig.Rule, basisConfig.Dot + 1);
                    newConfig.Basises.Add(basisConfig);
                }
                /* Get a pointer to the state described by the basis configuration set constructed in the preceding loop */
                var newState = ctx.AllStates.GetState(ctx);
                /* The state "newstp" is reached from the state "stp" by a shift action on the symbol "sp" */
                if (symbol.Type == SymbolType.MultiTerminal)
                    for (var i = 0; i < symbol.Children.Length; i++)
                        Actions.Add(new Action
                        {
                            Type = ActionType.Shift,
                            Symbol = symbol.Children[i],
                            State = newState,
                        });
                else
                    Actions.Add(new Action
                    {
                        Type = ActionType.Shift,
                        Symbol = symbol,
                        State = newState,
                    });
            }
        }
    }
}
