using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a type
    /// For example: int
    /// </summary>
    class TypeAST : AST
    {
        public object value { get; private set; }
        public TypeAST(Token token) : base(token)
        {
            this.value = token.value;
        }
    }
}
