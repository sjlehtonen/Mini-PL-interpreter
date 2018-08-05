using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents the assert(..) method
    /// </summary>
    class AssertAST : AST
    {
        public AST expression { get; private set; }
        public AssertAST(Token token, AST expression) : base(token)
        {
            this.expression = expression;
        }
    }
}
