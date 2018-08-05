using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents an unary operation
    /// For example: !b
    /// </summary>
    class OneOperatorAST : AST
    {
        public AST expression { get; private set; }
        public OneOperatorAST(Token operation, AST expression) : base(operation)
        {
            this.expression = expression;
        }
    }
}
