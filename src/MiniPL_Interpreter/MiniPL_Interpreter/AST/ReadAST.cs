using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents the read command
    /// For example: read a;
    /// </summary>
    class ReadAST : AST
    {
        public VariableAST variable { get; private set; }
        public ReadAST(Token token, VariableAST variable) : base(token)
        {
            this.variable = variable;
        }
    }
}
