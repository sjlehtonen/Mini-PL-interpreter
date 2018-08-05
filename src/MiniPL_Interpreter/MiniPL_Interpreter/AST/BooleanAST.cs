using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a boolean value
    /// </summary>
    class BooleanAST : AST
    {
        public bool value { get; private set; }
        public BooleanAST(Token token) : base(token)
        {
            this.value = (bool)token.value;
        }
    }
}
