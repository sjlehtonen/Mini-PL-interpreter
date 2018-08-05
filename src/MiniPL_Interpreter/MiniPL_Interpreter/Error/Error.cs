using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// A class that representes an error
    /// Includes line number, column number and error message
    /// </summary>
    class Error
    {
        private int lineNumber;
        private int columnNumber;
        private string errorMessage;

        public Error(int lineNumber, int columnNumber, string errorMessage)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
            this.errorMessage = errorMessage;
        }

        public Error(Token token, string errorMessage)
        {
            this.lineNumber = token.lineNumber;
            this.columnNumber = token.columnNumber;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets an exception object from the error
        /// </summary>
        /// <returns>Exception</returns>
        public Exception Exception()
        {
            return new Exception(this.ToString());
        }

        protected string ErrorText()
        {
            return "[Line " + lineNumber + ", Column " + columnNumber + "] " + errorMessage;
        }

        public override string ToString()
        {
            return "ERROR [Line " + lineNumber + ", Column " + columnNumber + "] " + errorMessage;
        }

    }
}
