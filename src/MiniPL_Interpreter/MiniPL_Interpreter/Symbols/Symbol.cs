using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{

    /// <summary>
    /// Represents a symbol inside the symbol table
    /// </summary>
    class Symbol
    {
        public object value { get; private set; }
        public string type { get; private set; }
        public Symbol(object value, string type)
        {
            this.value = value;
            this.type = type;
        }
    }
}
