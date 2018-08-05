using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a statement list
    /// For example: the lines inside a for loop are a statement list
    /// </summary>
    class StatementListAST : AST
    {
        public List<AST> children = new List<AST>();
        public StatementListAST() : base(null)
        {

        }
    }
}
