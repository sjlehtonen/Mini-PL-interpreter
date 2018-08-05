using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents no operation
    /// </summary>
    class NoOperationAST : AST
    {
        public NoOperationAST() : base(null) { }
    }
}
