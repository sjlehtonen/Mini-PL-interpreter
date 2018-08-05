using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// AST that represents a for loop
    /// Contains the statements that are children of the loop, the control variable, and evaluation expressions
    /// </summary>
    class ForAST : AST
    {
        public StatementListAST children { get; private set; }
        public VariableAST variable { get; private set; }
        public AST expression1 { get; private set; }
        public AST expression2 { get; private set; }

        public ForAST(Token token, StatementListAST ast, VariableAST variable, AST expression1, AST expression2) : base(token)
        {
            this.children = ast;
            this.variable = variable;
            this.expression1 = expression1;
            this.expression2 = expression2;
        }
    }
}
