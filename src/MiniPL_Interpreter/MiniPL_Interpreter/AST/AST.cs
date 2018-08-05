using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Abstract base class for AST
    /// </summary>
    abstract class AST
    {
        public Token token { get; private set; }
        public AST(Token op)
        {
            this.token = op;
        }
    }
}
