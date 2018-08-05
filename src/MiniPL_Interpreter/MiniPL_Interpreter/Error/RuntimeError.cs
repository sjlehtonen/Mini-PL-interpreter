using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Error for errors happening at runtime
    /// For example: Integer overflow and trying to read non-matching type to some value with the read command
    /// </summary>
    class RuntimeError : Error
    {
        public RuntimeError(Token token, string error) : base(token, error) { }
        public override string ToString() { return "RUNTIME ERROR " + this.ErrorText(); }
    }
}
