using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Token types
    /// </summary>
    enum TokenType {
        INTEGER = 0,
        PLUS,
        MINUS,
        MUL,
        DIV,
        PARENLEFT,
        PARENRIGHT,
        EOF,
        ASSIGN,
        SEMICOLON,
        IDTOKEN,
        TWODOT,
        VAR,
        INTEGER_TYPE,
        STRING_TYPE,
        BOOL_TYPE,
        STRING,
        BOOL,
        PRINT,
        READ,
        ASSERT,
        EQUAL,
        FOR,
        IN,
        DO,
        END,
        TWO_CONCECUTIVE_DOTS,
        LESS_THAN,
        LOGICAL_AND,
        LOGICAL_NOT
    }

    /// <summary>
    /// A class that represents a token
    /// Includes type, value, line number and column number
    /// </summary>
    class Token
    {
        public TokenType type { get; private set; }
        public object value { get; private set; }

        public int lineNumber { get; set; }
        public int columnNumber { get; set; }

        public Token(TokenType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public Token(TokenType type, object value, int line, int column)
        {
            this.type = type;
            this.value = value;
            this.lineNumber = line;
            this.columnNumber = column;
        }

        /// <summary>
        /// Gets the name of the type of the token
        /// </summary>
        /// <returns>Token type as string</returns>
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
