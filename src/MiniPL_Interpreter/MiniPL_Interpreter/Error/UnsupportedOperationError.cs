using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Error for operations that aren't supported for certain types of values
    /// </summary>
    class UnsupportedOperationError : Error
    {
        public UnsupportedOperationError(Token token, string type) : base(token, "Unsupported operation for type " + type) { }
    }
}
