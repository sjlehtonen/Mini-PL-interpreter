using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// The class that performs the lexical analysis
    /// </summary>
    class LexAnalyzer
    {
        private int pos = 0;
        private int lineNumber = 1;
        private int columnNumber = 1;
        private char currentChar;
        private string text;
        private bool endOfInput = false;

        private Dictionary<string, Token> reservedWords = new Dictionary<string, Token>();
        public LexAnalyzer(string text)
        {
            if (text.Length == 0) throw new Exception("ERROR: The program source file is empty");
            this.InitializeReservedWords();
            this.text = text;
            this.currentChar = text[pos];
        }

        /// <summary>
        /// Add reserved words to the dictionary
        /// </summary>
        private void InitializeReservedWords()
        {
            this.reservedWords.Add("var", new Token(TokenType.VAR, "var"));
            this.reservedWords.Add("int", new Token(TokenType.INTEGER_TYPE, "int"));
            this.reservedWords.Add("bool", new Token(TokenType.BOOL_TYPE, "bool"));
            this.reservedWords.Add("string", new Token(TokenType.STRING_TYPE, "string"));
            this.reservedWords.Add("print", new Token(TokenType.PRINT, "print"));
            this.reservedWords.Add("read", new Token(TokenType.READ, "read"));
            this.reservedWords.Add("assert", new Token(TokenType.ASSERT, "assert"));
            this.reservedWords.Add("for", new Token(TokenType.FOR, "for"));
            this.reservedWords.Add("in", new Token(TokenType.IN, "in"));
            this.reservedWords.Add("do", new Token(TokenType.DO, "do"));
            this.reservedWords.Add("end", new Token(TokenType.END, "end"));
        }

        /// <summary>
        /// Moves the pointer forward in the text
        /// </summary>
        private void MoveForward()
        {
            this.pos++;
            this.columnNumber++;

            if (this.pos <= this.text.Length - 1) this.currentChar = this.text[pos];
            else endOfInput = true;
        }

        /// <summary>
        /// Creates an integer from the text
        /// </summary>
        /// <returns>Integer</returns>
        public int Integer()
        {
            string res = "";
            while (!endOfInput && Char.IsDigit(currentChar))
            {
                res += this.currentChar;
                this.MoveForward();
            }
            try
            {
                int val = int.Parse(res);
                return val;
            }
            catch (OverflowException)
            {
                throw new LexicalError(lineNumber, columnNumber, "Integer overflow").Exception();
            }
        }

        /// <summary>
        /// Skips spaces
        /// </summary>
        private void SkipSpace()
        {
            while (!endOfInput && Char.IsWhiteSpace(currentChar))
            {
                if (this.currentChar == '\n')
                {
                    lineNumber++;
                    columnNumber = 1;
                }
                this.MoveForward();
            }
        }

        /// <summary>
        /// Skips multiline comments
        /// </summary>
        private void SkipMultilineComment()
        {
            int nestedLevel = 1;
            while (!endOfInput  && nestedLevel > 0)
            {
                if (this.currentChar == '*' && this.Peek() == '/')
                {
                    nestedLevel--;
                    this.MoveForward();
                    this.MoveForward();
                    continue;
                }
                if (this.currentChar == '/' && this.Peek() == '*')
                {
                    nestedLevel++;
                    this.MoveForward();
                    this.MoveForward();
                    continue;
                }

                if (this.currentChar == '\n')
                {
                    lineNumber++;
                    columnNumber = 1;
                }
                this.MoveForward();
            }
            if (endOfInput && nestedLevel > 0) throw new LexicalError(lineNumber, columnNumber, "Unclosed comment").Exception();
        }

        /// <summary>
        /// Skips a one line comment
        /// </summary>
        private void SkipOneLineComment()
        {
            while (!endOfInput && this.currentChar != '\n')
                this.MoveForward();
            if (this.currentChar == '\n')
            {
                lineNumber++;
                columnNumber = 1;
            }
            this.MoveForward();
        }

        /// <summary>
        /// Throws a lexer error
        /// </summary>
        private void Error()
        {
            throw new LexicalError(lineNumber, columnNumber, "Invalid character for token").Exception();
        }

        /// <summary>
        /// Peeks one character forward
        /// </summary>
        /// <returns>Char at pos+1</returns>
        public char Peek()
        {
            int pos = this.pos + 1;
            if (pos > this.text.Length - 1) return ' ';
            return this.text[pos];
        }

        /// <summary>
        /// Checks whether a string is a reserved keyword or identifier
        /// </summary>
        /// <returns>Token</returns>
        private Token ReservedOrIdentifier()
        {
            StringBuilder builder = new StringBuilder();
            while (!this.endOfInput && (Char.IsLetterOrDigit(this.currentChar) || this.currentChar == '_'))
            {
                builder.Append(this.currentChar);
                this.MoveForward();
            }

            if (reservedWords.ContainsKey(builder.ToString()))
            {
                Token reservedWordToken = reservedWords[builder.ToString()];
                reservedWordToken.lineNumber = this.lineNumber;
                reservedWordToken.columnNumber = this.columnNumber;
                return reservedWordToken;
            }
            return new Token(TokenType.IDTOKEN, builder.ToString(), this.lineNumber, this.columnNumber);
        }

        /// <summary>
        /// Creates a string token
        /// </summary>
        /// <returns>Token</returns>
        private Token CreateStringToken()
        {
            StringBuilder builder = new StringBuilder();
            while (this.currentChar != '"')
            {
                if (endOfInput) throw new LexicalError(lineNumber, columnNumber, "Unclosed string").Exception();
                if (this.currentChar == '\\')
                {
                    if (this.Peek() == '"') builder.Append('"');
                    else if (this.Peek() == 'n') builder.Append('\n');
                    else if (this.Peek() == '\\') builder.Append('\\');
                    else if (this.Peek() == 't') builder.Append('\t');
                    else if (this.Peek() == 'r') builder.Append('\r');
                    else if (this.Peek() == 'v') builder.Append('\v');
                    else throw new LexicalError(lineNumber, columnNumber, "Invalid character in string after escape character").Exception();

                    this.MoveForward();
                    this.MoveForward();
                    continue;
                } 

                if (this.currentChar != '\n' && this.currentChar != '\r') builder.Append(this.currentChar);
                this.MoveForward();
            }
            this.MoveForward();
            return new Token(TokenType.STRING, builder.ToString(), this.lineNumber, this.columnNumber);
        }

        /// <summary>
        /// Returns the next token
        /// The parser calls this method
        /// </summary>
        /// <returns>Token</returns>
        public Token NextToken()
        {
            while (!this.endOfInput)
            {
                if (Char.IsWhiteSpace(currentChar))
                {
                    this.SkipSpace();
                    continue;
                }

                if (currentChar == '/' && this.Peek() == '/')
                {
                    this.MoveForward();
                    this.MoveForward();
                    this.SkipOneLineComment();
                    continue;
                }

                if (currentChar == '/' && this.Peek() == '*')
                {
                    this.MoveForward();
                    this.MoveForward();
                    this.SkipMultilineComment();
                    continue;
                }

                if (currentChar == '*' && this.Peek() == '/')
                {
                    throw new LexicalError(lineNumber, columnNumber, "Unexpected multiline comment end").Exception();
                }

                if (currentChar == '"')
                {
                    this.MoveForward();
                    return CreateStringToken();
                }

                if (Char.IsDigit(currentChar)) return new Token(TokenType.INTEGER, this.Integer());
                if (currentChar == '+')
                {
                    this.MoveForward();
                    return new Token(TokenType.PLUS, '+', this.lineNumber, this.columnNumber);
                }
                if (currentChar == '-')
                {
                    this.MoveForward();
                    return new Token(TokenType.MINUS, '-', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '=')
                {
                    this.MoveForward();
                    return new Token(TokenType.EQUAL, '=', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '<')
                {
                    this.MoveForward();
                    return new Token(TokenType.LESS_THAN, '<', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '&')
                {
                    this.MoveForward();
                    return new Token(TokenType.LOGICAL_AND, '&', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '!')
                {
                    this.MoveForward();
                    return new Token(TokenType.LOGICAL_NOT, '!', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '*')
                {
                    this.MoveForward();
                    return new Token(TokenType.MUL, '*', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '/')
                {
                    this.MoveForward();
                    return new Token(TokenType.DIV, '/', this.lineNumber, this.columnNumber);
                }

                if (currentChar == '(')
                {
                    this.MoveForward();
                    return new Token(TokenType.PARENLEFT, '(', this.lineNumber, this.columnNumber);
                }

                if (currentChar == ')')
                {
                    this.MoveForward();
                    return new Token(TokenType.PARENRIGHT, ')', this.lineNumber, this.columnNumber);
                }

                if (Char.IsLetter(this.currentChar) || this.currentChar == '_')
                    return this.ReservedOrIdentifier();

                if (this.currentChar == ':' && this.Peek() == '=')
                {
                    this.MoveForward();
                    this.MoveForward();
                    return new Token(TokenType.ASSIGN, ":=", this.lineNumber, this.columnNumber);
                }

                if (this.currentChar == '.' && this.Peek() == '.')
                {
                    this.MoveForward();
                    this.MoveForward();
                    return new Token(TokenType.TWO_CONCECUTIVE_DOTS, "..", this.lineNumber, this.columnNumber);
                }

                if (this.currentChar == ':' && this.Peek() == ' ')
                {
                    this.MoveForward();
                    return new Token(TokenType.TWODOT, ':', this.lineNumber, this.columnNumber);
                }

                if (this.currentChar == ';')
                {
                    this.MoveForward();
                    return new Token(TokenType.SEMICOLON, ';', this.lineNumber, this.columnNumber);
                }

                this.Error();
            }
            return new Token(TokenType.EOF, null, this.lineNumber, this.columnNumber);
        }
    }
}
