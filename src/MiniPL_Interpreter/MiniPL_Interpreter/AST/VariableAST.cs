using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a variable
    /// </summary>
    class VariableAST : AST
    {
        public object value { get; private set; }
        public VariableAST(Token operation) : base(operation)
        {
            this.value = operation.value;
        }
    }
}
