using System;

namespace System.Text.Lalr
{
    /// <summary>
    /// Rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// LHSymbol
        /// </summary>
        public Symbol LHSymbol;
        /// <summary>
        /// LHSymbolAlias
        /// </summary>
        public string LHSymbolAlias;
        internal int LHSymbolStart;
        /// <summary>
        /// RuleLineno
        /// </summary>
        public int RuleLineno;
        /// <summary>
        /// RHSymbols
        /// </summary>
        public Symbol[] RHSymbols;
        /// <summary>
        /// RHSymbolsAlias
        /// </summary>
        public string[] RHSymbolsAlias;
        /// <summary>
        /// Lineno
        /// </summary>
        public int Lineno;
        /// <summary>
        /// Code
        /// </summary>
        public string Code;
        internal Symbol PrecedenceSymbol;
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        internal bool CanReduce;
        internal Rule NextLHSymbol;
        /// <summary>
        /// Next
        /// </summary>
        public Rule Next;
    }
}
