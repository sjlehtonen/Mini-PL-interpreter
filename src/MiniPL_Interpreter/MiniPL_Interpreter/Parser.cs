using System;
using System.Collections.Generic;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// The class responsible for parsing the program and ensuring that the program is syntactically correct
    /// This class builds the AST
    /// </summary>
    class Parser
    {
        private LexAnalyzer lexAnalyzer;
        private Token currentToken;

        public Parser(LexAnalyzer lex)
        {
            this.lexAnalyzer = lex;
            this.currentToken = this.lexAnalyzer.NextToken();
        }

        /// <summary>
        /// Throws a parser error
        /// </summary>
        private void Error()
        {
            throw new ParseError(this.currentToken, "Invalid token sequence").Exception();
        }

        /// <summary>
        /// Consumes a token and gets the next one from the lexical analyzer
        /// </summary>
        /// <param name="type">Token type</param>
        public void ConsumeToken(TokenType type)
        {
            if (this.currentToken.type == type) this.currentToken = this.lexAnalyzer.NextToken();
            else Error();
        }

        /// <summary>
        /// The start of the program
        /// </summary>
        /// <returns>Program AST</returns>
        private AST Program()
        {
            StatementListAST list = new StatementListAST();
            List<AST> nodes = this.StatementList();
            foreach (AST node in nodes) list.children.Add(node);
            return list;
        }

        /// <summary>
        /// Returns a factor (integer, string, boolean, unaryast)
        /// </summary>
        /// <returns>Factor AST</returns>
        private AST Factor()
        {
            Token token = this.currentToken;
            if (token.type == TokenType.INTEGER)
            {
                this.ConsumeToken(TokenType.INTEGER);
                return new NumericAST(token);
            }
            else if (token.type == TokenType.STRING)
            {
                this.ConsumeToken(TokenType.STRING);
                return new StringAST(token);
            }
            else if (token.type == TokenType.BOOL)
            {
                this.ConsumeToken(TokenType.BOOL);
                return new BooleanAST(token);
            }
            else if (token.type == TokenType.PARENLEFT)
            {
                this.ConsumeToken(TokenType.PARENLEFT);
                AST ast = this.Expression();
                this.ConsumeToken(TokenType.PARENRIGHT);
                return ast;
            }
            else if (token.type == TokenType.PLUS)
            {
                this.ConsumeToken(TokenType.PLUS);
                AST ast = new OneOperatorAST(token, this.Factor());
                return ast;
            }
            else if (token.type == TokenType.MINUS)
            {
                this.ConsumeToken(TokenType.MINUS);
                AST ast = new OneOperatorAST(token, this.Factor());
                return ast;
            }
            else if (token.type == TokenType.LOGICAL_NOT)
            {
                this.ConsumeToken(TokenType.LOGICAL_NOT);
                AST ast = new OneOperatorAST(token, this.Factor());
                return ast;
            }
            else
            {
                AST ast = this.Variable();
                return ast;
            }
        }

        /// <summary>
        /// Returns a binary operation AST if operation is multiply or divide, if not, then return factor
        /// </summary>
        /// <returns>AST</returns>
        private AST Term()
        {
            AST node = this.Factor();
            while (this.currentToken.type == TokenType.MUL || this.currentToken.type == TokenType.DIV)
            {
                Token token = this.currentToken;
                if (token.type == TokenType.MUL) this.ConsumeToken(TokenType.MUL);
                else if (token.type == TokenType.DIV) this.ConsumeToken(TokenType.DIV);
                node = new BinaryOperationAST(node, token, this.Factor());
            }
            return node;
        }

        /// <summary>
        /// Gets expression AST if operation is plus, minus, equal, less_than or logical_and
        /// If not, then return term
        /// </summary>
        /// <returns></returns>
        private AST Expression()
        {
            AST node = this.Term();
            while (this.currentToken.type == TokenType.PLUS || this.currentToken.type == TokenType.MINUS || this.currentToken.type == TokenType.EQUAL || this.currentToken.type == TokenType.LESS_THAN || this.currentToken.type == TokenType.LOGICAL_AND)
            {
                Token token = this.currentToken;
                if (token.type == TokenType.PLUS) this.ConsumeToken(TokenType.PLUS);
                else if (token.type == TokenType.MINUS) this.ConsumeToken(TokenType.MINUS);
                else if (token.type == TokenType.EQUAL) this.ConsumeToken(TokenType.EQUAL);
                else if (token.type == TokenType.LESS_THAN) this.ConsumeToken(TokenType.LESS_THAN);
                else if (token.type == TokenType.LOGICAL_AND) this.ConsumeToken(TokenType.LOGICAL_AND);
                node = new BinaryOperationAST(node, token, this.Term());
            }
            return node;
        }

        /// <summary>
        /// Gets an assignment AST
        /// </summary>
        /// <returns>AssignmentAST</returns>
        private AST Assignment()
        {
            VariableAST left = this.Variable() as VariableAST;
            Token token = this.currentToken;
            this.ConsumeToken(token.type);
            AST right = this.Expression();
            AssignAST ast = new AssignAST(left, this.currentToken, right);
            return ast;
        }

        /// <summary>
        /// Gets a StatementList
        /// </summary>
        /// <returns>List of statements</returns>
        private List<AST> StatementList()
        {
            AST node = this.Statement();
            List<AST> statements = new List<AST>();
            statements.Add(node);
            while (this.currentToken.type == TokenType.SEMICOLON)
            {
                this.ConsumeToken(TokenType.SEMICOLON);
                statements.Add(this.Statement());
            }
            if (this.currentToken.type == TokenType.IDTOKEN)
                this.Error();
            return statements;
        }


        /// <summary>
        /// Gets a VariableDeclarationAST
        /// </summary>
        /// <returns>VariableDeclarationAST</returns>
        private AST VariableDeclaration()
        {
            this.ConsumeToken(TokenType.VAR);
            VariableAST variable = new VariableAST(this.currentToken);
            this.ConsumeToken(TokenType.IDTOKEN);
            this.ConsumeToken(TokenType.TWODOT);
            TypeAST type = (TypeAST)this.TypeSpec();
            if (this.currentToken.type == TokenType.SEMICOLON)
            {
                return new VariableDeclarationAST(variable, type, this.currentToken);
            }
            else
            {
                this.ConsumeToken(TokenType.ASSIGN);
                return new VariableDeclarationAST(variable, type, this.currentToken, this.Expression());
            }

        }

        /// <summary>
        /// Gets a TypeAST
        /// </summary>
        /// <returns>TypeAST</returns>
        private AST TypeSpec()
        {
            Token token = this.currentToken;

            if (this.currentToken.type == TokenType.INTEGER_TYPE) this.ConsumeToken(TokenType.INTEGER_TYPE);
            else if (this.currentToken.type == TokenType.STRING_TYPE) this.ConsumeToken(TokenType.STRING_TYPE);
            else this.ConsumeToken(TokenType.BOOL_TYPE);

            return new TypeAST(token);
        }

        /// <summary>
        /// Gets a PrintAST
        /// </summary>
        /// <returns>PrintAST</returns>
        private AST Print()
        {
            Token token = this.currentToken;
            this.ConsumeToken(TokenType.PRINT);
            AST expressionAst = this.Expression();
            return new PrintAST(token, expressionAst);
        }

        /// <summary>
        /// Gets a ReadAST
        /// </summary>
        /// <returns>ReadAST</returns>
        private AST Read()
        {
            Token token = this.currentToken;
            this.ConsumeToken(TokenType.READ);
            VariableAST variable = this.Variable() as VariableAST;
            return new ReadAST(token, variable);

        }

        /// <summary>
        /// Gets a AssertAST
        /// </summary>
        /// <returns>AssertAST</returns>
        private AST Assert()
        {
            Token token = this.currentToken;
            this.ConsumeToken(TokenType.ASSERT);
            this.ConsumeToken(TokenType.PARENLEFT);
            AST expressionAst = this.Expression();
            this.ConsumeToken(TokenType.PARENRIGHT);
            return new AssertAST(token, expressionAst);
        }

        /// <summary>
        /// Gets a ForAST
        /// </summary>
        /// <returns>ForAST</returns>
        private AST For()
        {
            Token token = this.currentToken;
            this.ConsumeToken(TokenType.FOR);
            VariableAST variable = this.Variable() as VariableAST;
            this.ConsumeToken(TokenType.IN);
            AST expressionAST = this.Expression();
            this.ConsumeToken(TokenType.TWO_CONCECUTIVE_DOTS);
            AST expressionAST2 = this.Expression();
            this.ConsumeToken(TokenType.DO);

            StatementListAST ast = new StatementListAST();
            this.StatementList().ForEach(x => ast.children.Add(x));
            this.ConsumeToken(TokenType.END);
            this.ConsumeToken(TokenType.FOR);
            return new ForAST(token, ast, variable, expressionAST, expressionAST2);
        }

        /// <summary>
        /// Gets a StatementAST
        /// </summary>
        /// <returns>StatementAST</returns>
        private AST Statement()
        {
            if (this.currentToken.type == TokenType.IDTOKEN) return this.Assignment();
            else if (this.currentToken.type == TokenType.VAR) return this.VariableDeclaration();
            else if (this.currentToken.type == TokenType.PRINT) return this.Print();
            else if (this.currentToken.type == TokenType.READ) return this.Read();
            else if (this.currentToken.type == TokenType.ASSERT) return this.Assert();
            else if (this.currentToken.type == TokenType.FOR) return this.For();
            else return this.EmptyNode();
        }

        /// <summary>
        /// Gets a VariableAST
        /// </summary>
        /// <returns>VariableAST</returns>
        private AST Variable()
        {
            VariableAST ast = new VariableAST(this.currentToken);
            this.ConsumeToken(TokenType.IDTOKEN);
            return ast;
        }

        /// <summary>
        /// Gets a NoOperationAST
        /// </summary>
        /// <returns>NoOperationAST</returns>
        private AST EmptyNode()
        {
            return new NoOperationAST();
        }

        /// <summary>
        /// Parses the program and constructs the AST
        /// </summary>
        /// <returns>AST</returns>
        public AST Parse()
        {
            AST node = this.Program();
            if (this.currentToken.type != TokenType.EOF) this.Error();
            return node;
        }
    }
}
