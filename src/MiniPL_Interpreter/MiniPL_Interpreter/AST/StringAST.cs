using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a string value
    /// </summary>
    class StringAST : AST
    {
        public string value { get; private set; }
        public StringAST(Token token) : base(token)
        {
            this.value = (string)token.value;
        }
    }
}
