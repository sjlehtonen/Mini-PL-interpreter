using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Lexical error class for errors in the lexical analysis phase
    /// </summary>
    class LexicalError : Error
    {
        public LexicalError(int line, int column, string error) : base(line, column, error) { }
        public override string ToString() { return "LEXICAL ERROR " + this.ErrorText(); }
    }
}
