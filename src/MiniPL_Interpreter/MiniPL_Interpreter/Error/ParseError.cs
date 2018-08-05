using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Error for errors happening at the parser
    /// </summary>
    class ParseError : Error
    {
        public ParseError(Token token, string error) : base(token, error) { }
        public override string ToString() { return "PARSE ERROR " + this.ErrorText(); }
    }
}
