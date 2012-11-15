using System;
using System.Collections.Generic;

namespace System.Text.Lalr
{
    internal class StateCollection : Dictionary<List<Config>, State>
    {
        public StateCollection()
            : base(new Config.ListKeyEqualityComparer()) { }

        internal State GetState(Context ctx)
        {
            var basises = ctx.AllConfigs.SliceAllBasises();
            State state;
            if (base.TryGetValue(basises, out state))
            {
                /* Copy all the follow-set propagation links from the state under construction into the preexisting state, then return a pointer to the preexisting state */
                using (var xE = basises.GetEnumerator())
                using (var yE = state.Basises.GetEnumerator())
                    while (xE.MoveNext() && yE.MoveNext())
                    {
                        var x = xE.Current;
                        var y = yE.Current;
                        y.Basises.AddRange(x.Basises);
                        x.Forwards.Clear();
                        x.Basises.Clear();
                    }
                var configs = ctx.AllConfigs.SliceAllItems(false);
                ctx.AllConfigs.Eat(configs);
            }
            else
            {
                /* This really is a new state.  Construct all the details */
                ctx.AllConfigs.Closure(ctx);
                var configs = ctx.AllConfigs.SliceAllItems(true);
                state = new State
                {
                    Basises = basises,
                    Configs = configs,
                    ID = ctx.States++,
                };
                ctx.AllStates.Add(state.Basises, state);
                /* Recursively compute successor states */
                state.BuildShifts(ctx);
            }
            return state;
        }

        public State[] ToStateArray()
        {
            var array = new State[Values.Count];
            Values.CopyTo(array, 0);
            return array;
        }
    }
}
