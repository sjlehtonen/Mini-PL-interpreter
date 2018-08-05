using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents binary operation
    /// For example: a + b;
    /// Token marks the operation type
    /// </summary>
    class BinaryOperationAST : AST
    {
        public AST left { get; private set; }
        public AST right { get; private set; }

        public BinaryOperationAST(AST left, Token operation, AST right) : base(operation)
        {
            this.left = left;
            this.right = right;
        }
    }
}
