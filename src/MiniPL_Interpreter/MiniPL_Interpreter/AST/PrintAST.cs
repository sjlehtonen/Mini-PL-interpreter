using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents the print command
    /// For example: print "hello"
    /// </summary>
    class PrintAST : AST
    {
        public AST expression;
        public PrintAST(Token token, AST expression) : base(token)
        {
            this.expression = expression;
        }
    }
}
