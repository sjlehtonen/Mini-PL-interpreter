using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a numeric value
    /// </summary>
    class NumericAST : AST
    {
        public int value { get; private set; }
        public NumericAST(Token token) : base(token)
        {
            this.value = (int)token.value;
        }
    }
}
