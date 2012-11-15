using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Lalr
{
    /// <summary>
    /// Symbol
    /// </summary>
    public class Symbol
    {
        private static readonly IComparer<Symbol> _keyComparer = new KeyComparer();

        internal class KeyComparer : IComparer<Symbol>
        {
            public int Compare(Symbol x, Symbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;
                var xi = x.ID + (x.Name[0] > 'Z' ? 10000000 : 0);
                var yi = y.ID + (y.Name[0] > 'Z' ? 10000000 : 0);
                Debug.Assert(xi != yi || x.Name == y.Name);
                return xi - yi;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int ID { get; internal set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        public SymbolType Type { get; internal set; }
        internal Rule Rule { get; set; }
        /// <summary>
        /// Gets or sets the fallback.
        /// </summary>
        /// <value>
        /// The fallback.
        /// </value>
        public Symbol Fallback { get; set; }
        internal int Precedence { get; set; }
        internal Association Association { get; set; }
        internal HashSet<int> FirstSet { get; set; }
        internal bool Lambda { get; set; }
        /// <summary>
        /// Gets the uses.
        /// </summary>
        public int Uses { get; private set; }
        /// <summary>
        /// Gets or sets the destructor.
        /// </summary>
        /// <value>
        /// The destructor.
        /// </value>
        public string Destructor { get; set; }
        /// <summary>
        /// Gets or sets the destructor lineno.
        /// </summary>
        /// <value>
        /// The destructor lineno.
        /// </value>
        public int DestructorLineno { get; set; }
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string DataType { get; set; }
        /// <summary>
        /// Gets or sets the data type ID.
        /// </summary>
        /// <value>
        /// The data type ID.
        /// </value>
        public int DataTypeID { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public Symbol[] Children;

        internal Symbol() { }

        internal static Symbol New(Context ctx, string name) { return New(ctx, name, true); }
        internal static Symbol New(Context ctx, string name, bool increaseUses)
        {
            Symbol symbol;
            if (!ctx.AllSymbols.TryGetValue(name, out symbol))
            {
                symbol = new Symbol
                {
                    Name = name,
                    Type = (char.IsUpper(name[0]) ? SymbolType.Terminal : SymbolType.NonTerminal),
                    Precedence = -1,
                    Association = Association.Unknown,
                };
                ctx.AllSymbols.Add(name, symbol);
            }
            if (increaseUses)
                symbol.Uses++;
            return symbol;
        }

        internal bool Equals(Symbol other)
        {
            if (base.Equals(other))
                return true;
            if (Type != SymbolType.MultiTerminal || other.Type != SymbolType.MultiTerminal)
                return false;
            if (Children.Length != other.Children.Length)
                return false;
            for (var index = 0; index < Children.Length; index++)
                if (Children[index] != other.Children[index])
                    return false;
            return true;
        }

        internal static Symbol[] ToSymbolArray(Context ctx, out int terminals)
        {
            Symbol.New(ctx, "{default}");
            var symbols = ctx.AllSymbols.Values.ToArray();
            for (var index = 0; index < symbols.Length; index++) symbols[index].ID = index;
            Array.Sort(symbols, _keyComparer);
            for (var index = 0; index < symbols.Length; index++) symbols[index].ID = index;
            var i = 1;
            for (; char.IsUpper(symbols[i].Name[0]); i++) { }
            terminals = i;
            return symbols;
        }
    }
}
