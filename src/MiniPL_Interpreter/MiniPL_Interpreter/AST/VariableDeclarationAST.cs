using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents variable declaration
    /// For example: var a : int := 5;
    /// </summary>
    class VariableDeclarationAST : AST
    {
        public VariableAST varNode { get; private set; }
        public TypeAST typeNode { get; private set; }
        public AST assignedValue = null;

        public VariableDeclarationAST(VariableAST varNode, TypeAST typeNode, Token token, AST assignValue = null) : base(token)
        {
            this.assignedValue = assignValue;
            this.varNode = varNode;
            this.typeNode = typeNode;
        }
    }
}
