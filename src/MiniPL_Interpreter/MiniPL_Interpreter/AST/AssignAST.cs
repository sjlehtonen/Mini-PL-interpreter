using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents an assignment operation
    /// For example: a := 5;
    /// </summary>
    class AssignAST : AST
    {
        public VariableAST left { get; private set; }
        public AST right { get; private set; }
        public AssignAST(VariableAST left, Token operation, AST right) : base(operation)
        {
            this.left = left;
            this.right = right;
        }
    }
}
