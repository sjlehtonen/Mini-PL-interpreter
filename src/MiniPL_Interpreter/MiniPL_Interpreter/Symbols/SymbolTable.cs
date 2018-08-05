using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter.Symbols
{
    /// <summary>
    /// Represents the symbol table
    /// </summary>
    class SymbolTable
    {
        private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
        public SymbolTable()
        {
            DefineSymbol("int", "int", null);
            DefineSymbol("string", "string", null);
            DefineSymbol("bool", "bool", null);
        }

        /// <summary>
        /// Adds a symbol to the symbol table
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        public void DefineSymbol(string name, string type, object value)
        {
            this.symbols[name] = new Symbol(value, type);
        }

        /// <summary>
        /// Gets a symbol from the table
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Symbol</returns>
        public Symbol LookupSymbol(string name)
        {
            if (!this.symbols.ContainsKey(name)) return null;
            return this.symbols[name];
        }
    }
}
