using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Lalr.Emitters
{
    /// <summary>
    /// EmitterActionTable
    /// </summary>
    public class EmitterActionTable
    {
        private static readonly AX.KeyComparer _keyComparer = new AX.KeyComparer();

        /* Each state contains a set of token transaction and a set of nonterminal transactions.  Each of these sets makes an instance of the following structure.  An array of these structures is used to order the creation of entries in the yy_action[] table. */
        private struct AX
        {
            public State State;   /* A pointer to a state */
            public bool Token;           /* True to use tokens.  False for non-terminals */
            public int Actions;         /* Number of actions */
            public int iOrder;          /* Original order of action sets */

            public class KeyComparer : IComparer<AX>
            {
                public int Compare(AX x, AX y)
                {
                    if (object.ReferenceEquals(x, y))
                        return 0;
                    var c = y.Actions - x.Actions;
                    if (c == 0)
                        c = y.iOrder - x.iOrder;
                    //Debug.Assert(c != 0);
                    return c;
                }
            }
        }

        /// <summary>
        /// lookahead_action
        /// </summary>
        public struct lookahead_action
        {
            /// <summary>
            /// Lookahead
            /// </summary>
            public int Lookahead;
            /// <summary>
            /// Action
            /// </summary>
            public int Action;
        }

        /// <summary>
        /// _UsedActions
        /// </summary>
        public int _UsedActions;
        /// <summary>
        /// _AllocatedActions
        /// </summary>
        public int _AllocatedActions;
        /// <summary>
        /// Actions
        /// </summary>
        public lookahead_action[] Actions;
        /// <summary>
        /// Lookaheads
        /// </summary>
        public lookahead_action[] Lookaheads;
        /// <summary>
        /// mnLookahead
        /// </summary>
        public int mnLookahead;
        /// <summary>
        /// mnAction
        /// </summary>
        public int mnAction;
        /// <summary>
        /// mxLookahead
        /// </summary>
        public int mxLookahead;
        /// <summary>
        /// _UsedLookaheads
        /// </summary>
        public int _UsedLookaheads;
        /// <summary>
        /// _AllocatedLookaheads
        /// </summary>
        public int _AllocatedLookaheads;

        /// <summary>
        /// Actions the specified lookahead.
        /// </summary>
        /// <param name="lookahead">The lookahead.</param>
        /// <param name="actionID">The action ID.</param>
        public void Action(int lookahead, int actionID)
        {
            if (_UsedLookaheads >= _AllocatedLookaheads)
            {
                _AllocatedLookaheads += 25;
                Array.Resize(ref Lookaheads, _AllocatedLookaheads);
            }
            if (_UsedLookaheads == 0)
            {
                mxLookahead = lookahead;
                mnLookahead = lookahead;
                mnAction = actionID;
            }
            else
            {
                if (mxLookahead < lookahead)
                    mxLookahead = lookahead;
                if (mnLookahead > lookahead)
                {
                    mnLookahead = lookahead;
                    mnAction = actionID;
                }
            }
            Lookaheads[_UsedLookaheads].Lookahead = lookahead;
            Lookaheads[_UsedLookaheads].Action = actionID;
            _UsedLookaheads++;
        }

        /// <summary>
        /// Gets the number of entries in the yy_action table
        /// </summary>
        public int Size
        {
            get { return _UsedActions; }
        }

        /// <summary>
        /// Gets the value for the N-th entry in yy_action
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetAction(int index)
        {
            return Actions[index].Action;
        }

        /// <summary>
        /// Gets the value for the N-th entry in yy_lookahead.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetLookahead(int index)
        {
            return Actions[index].Lookahead;
        }

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        /// <returns></returns>
        public int Insert()
        {
            Debug.Assert(_UsedLookaheads > 0);

            /* Make sure we have enough space to hold the expanded action table in the worst case.  The worst case occurs if the transaction set must be appended to the current action table */
            var n = mxLookahead + 1;
            if (_UsedActions + n >= _AllocatedActions)
            {
                var oldAlloc = _AllocatedActions;
                _AllocatedActions = _UsedActions + n + _AllocatedActions + 20;
                Array.Resize(ref Actions, _AllocatedActions);
                for (var index = oldAlloc; index < _AllocatedActions; index++)
                {
                    Actions[index].Lookahead = -1;
                    Actions[index].Action = -1;
                }
            }

            /* Scan the existing action table looking for an offset that is a duplicate of the current transaction set.  Fall out of the loop if and when the duplicate is found.
            ** i is the index in aAction[] where mnLookahead is inserted. */
            var i = _UsedActions - 1;
            for (; i >= 0; i--)
                if (Actions[i].Lookahead == mnLookahead)
                {
                    /* All lookaheads and actions in the aLookahead[] transaction must match against the candidate aAction[i] entry. */
                    if (Actions[i].Action != mnAction)
                        continue;
                    var j = 0;
                    for (; j < _UsedLookaheads; j++)
                    {
                        var k = Lookaheads[j].Lookahead - mnLookahead + i;
                        if ((k < 0) || (k >= _UsedActions))
                            break;
                        if (Lookaheads[j].Lookahead != Actions[k].Lookahead)
                            break;
                        if (Lookaheads[j].Action != Actions[k].Action)
                            break;
                    }
                    if (j < _UsedLookaheads)
                        continue;

                    /* No possible lookahead value that is not in the aLookahead[] transaction is allowed to match aAction[i] */
                    var n2 = 0;
                    for (var j2 = 0; j2 < _UsedActions; j2++)
                    {
                        if (Actions[j2].Lookahead < 0)
                            continue;
                        if (Actions[j2].Lookahead == j2 + mnLookahead - i)
                            n2++;
                    }
                    if (n2 == _UsedLookaheads)
                        break;  /* An exact match is found at offset i */
                }

            /* If no existing offsets exactly match the current transaction, find an an empty offset in the aAction[] table in which we can add the aLookahead[] transaction. */
            if (i < 0)
            {
                /* Look for holes in the aAction[] table that fit the current aLookahead[] transaction.  Leave i set to the offset of the hole.
                ** If no holes are found, i is left at p->nAction, which means the transaction will be appended. */
                for (i = 0; i < _AllocatedActions - mxLookahead; i++)
                    if (Actions[i].Lookahead < 0)
                    {
                        var j = 0;
                        for (; j < _UsedLookaheads; j++)
                        {
                            var k = Lookaheads[j].Lookahead - mnLookahead + i;
                            if (k < 0)
                                break;
                            if (Actions[k].Lookahead >= 0)
                                break;
                        }
                        if (j < _UsedLookaheads)
                            continue;
                        for (j = 0; j < _UsedActions; j++)
                            if (Actions[j].Lookahead == j + mnLookahead - i)
                                break;
                        if (j == _UsedActions)
                            break;  /* Fits in empty slots */
                    }
            }
            /* Insert transaction set at index i. */
            for (var j = 0; j < _UsedLookaheads; j++)
            {
                var k = Lookaheads[j].Lookahead - mnLookahead + i;
                Actions[k] = Lookaheads[j];
                if (k >= _UsedActions)
                    _UsedActions = k + 1;
            }
            _UsedLookaheads = 0;

            /* Return the offset that is added to the lookahead in order to get the index into yy_action of the action */
            return i - mnLookahead;
        }

        /* Generate the action table and its associates:
           **
           **  yy_action[]        A single table containing all actions.
           **  yy_lookahead[]     A table containing the lookahead for each entry in
           **                     yy_action.  Used to detect hash collisions.
           **  yy_shift_ofst[]    For each state, the offset into yy_action for
           **                     shifting terminals.
           **  yy_reduce_ofst[]   For each state, the offset into yy_action for
           **                     shifting non-terminals after a reduce.
           **  yy_default[]       Default action for each state.
           */
        /// <summary>
        /// Makes the specified CTX.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="maxTokenOffset">The max token offset.</param>
        /// <param name="minTokenOffset">The min token offset.</param>
        /// <param name="maxNonTerminalOffset">The max non terminal offset.</param>
        /// <param name="minNonTerminalOffset">The min non terminal offset.</param>
        /// <returns></returns>
        public static EmitterActionTable Make(Context ctx, out int maxTokenOffset, out int minTokenOffset, out int maxNonTerminalOffset, out int minNonTerminalOffset)
        {
            /* Compute the actions on all states and count them up */
            var ax = new AX[ctx.States * 2];
            for (var i = 0; i < ctx.States; i++)
            {
                var state = ctx.Sorted[i];
                ax[i * 2] = new AX { State = state, Token = true, Actions = state.TokenActions };
                ax[i * 2 + 1] = new AX { State = state, Token = false, Actions = state.NonTerminalActions };
            }
            maxTokenOffset = minTokenOffset = 0;
            maxNonTerminalOffset = minNonTerminalOffset = 0;
            /* Compute the action table.  In order to try to keep the size of the action table to a minimum, the heuristic of placing the largest action sets first is used. */
            for (var i = 0; i < ctx.States * 2; i++) ax[i].iOrder = i;
            Array.Sort(ax, _keyComparer);
            var actionTable = new EmitterActionTable();
            for (var i = 0; i < ctx.States * 2 && ax[i].Actions > 0; i++)
            {
                var state = ax[i].State;
                if (ax[i].Token)
                {
                    foreach (var action in state.Actions)
                    {
                        if (action.Symbol.ID >= ctx.Terminals) continue;
                        var actionID = action.ComputeID(ctx);
                        if (actionID < 0) continue;
                        actionTable.Action(action.Symbol.ID, actionID);
                    }
                    state.TokenOffset = actionTable.Insert();
                    if (state.TokenOffset < minTokenOffset) minTokenOffset = state.TokenOffset;
                    if (state.TokenOffset > maxTokenOffset) maxTokenOffset = state.TokenOffset;
                }
                else
                {
                    foreach (var action in state.Actions)
                    {
                        if (action.Symbol.ID < ctx.Terminals) continue;
                        if (action.Symbol.ID == ctx.Symbols.Length - 1) continue;
                        var actionID = action.ComputeID(ctx);
                        if (actionID < 0) continue;
                        actionTable.Action(action.Symbol.ID, actionID);
                    }
                    state.NonTerminalOffset = actionTable.Insert();
                    if (state.NonTerminalOffset < minNonTerminalOffset) minNonTerminalOffset = state.NonTerminalOffset;
                    if (state.NonTerminalOffset > maxNonTerminalOffset) maxNonTerminalOffset = state.NonTerminalOffset;
                }
            }
            ax = null;
            return actionTable;
        }
    }
}
