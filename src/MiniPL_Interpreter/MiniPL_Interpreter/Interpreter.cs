using System;
using System.Collections.Generic;
using System.Text;
using MiniPL_Interpreter.Symbols;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Class that is responsible for traversin the AST and executing the program
    /// </summary>
    class Interpreter : NodeInterpreter
    {
        private Parser parser;
        private SymbolTable symbolTable = new SymbolTable();
        private AST ast = null;
        public Interpreter(Parser parser)
        {
            this.parser = parser;
        }

        public Interpreter(SemanticChecker semanticChecker)
        {
            this.parser = semanticChecker.parser;
            this.ast = semanticChecker.ast;
            this.symbolTable = semanticChecker.symbolTable;
        }

        /// <summary>
        /// Traverses NumericAST
        /// </summary>
        /// <param name="ast">NumericAST</param>
        /// <returns>Int value</returns>
        private int VisitNumericAST(NumericAST ast)
        {
            return ast.value;
        }

        /// <summary>
        /// Traverses StringAST
        /// </summary>
        /// <param name="ast">StringAST</param>
        /// <returns>String value</returns>
        private string VisitStringAST(StringAST ast)
        {
            return ast.value;
        }

        /// <summary>
        /// Traverses BooleanAST
        /// </summary>
        /// <param name="ast">BooleanAST</param>
        /// <returns>Boolean value</returns>
        private bool VisitBooleanAST(BooleanAST ast)
        {
            return ast.value;
        }

        /// <summary>
        /// Traverses BinaryOperationAST
        /// </summary>
        /// <param name="ast"></param>
        /// <returns>BinaryOperationAST</returns>
        private object VisitBinaryOperationAST(BinaryOperationAST ast)
        {
            object component1 = this.VisitNode(ast.left);
            object component2 = this.VisitNode(ast.right);

            if (component1 is string && component2 is string)
            {
                string c1 = component1 as string;
                string c2 = component2 as string;

                if (ast.token.type == TokenType.EQUAL) return c1.Equals(c2);
                else if (ast.token.type == TokenType.LESS_THAN) return c1.Length < c2.Length;
                else return c1 + c2;
            }
            else if (component1 is bool && component2 is bool)
            {
                if (ast.token.type == TokenType.EQUAL) return (bool)component1 == (bool)component2;
                else if (ast.token.type == TokenType.LOGICAL_AND) return (bool)component1 & (bool)component2;
                else // less than
                {
                    if ((bool)component1 == false && (bool)component2 == true) return true;
                    else return false;
                }
            }
            else
            {
                try
                {
                    if (ast.token.type == TokenType.PLUS) return (int)component1 + (int)component2;
                    else if (ast.token.type == TokenType.MINUS) return (int)component1 - (int)component2;
                    else if (ast.token.type == TokenType.MUL) return (int)component1 * (int)component2;
                    else if (ast.token.type == TokenType.EQUAL) return (int)component1 == (int)component2;
                    else if (ast.token.type == TokenType.LESS_THAN) return (int)component1 < (int)component2;
                    else return (int)component1 / (int)component2;
                }
                catch (DivideByZeroException) { throw new RuntimeError(ast.token, "Attempted to divide by zero").Exception(); }
                catch (OverflowException) { throw new RuntimeError(ast.token, "Integer overflow").Exception(); }
            }
        }

        /// <summary>
        /// Traverses the for loop AST
        /// </summary>
        /// <param name="ast">ForAST</param>
        private void VisitForAST(ForAST ast)
        {
            int startVal = (int)this.VisitNode(ast.expression1);
            int endVal = (int)this.VisitNode(ast.expression2);
            for (int i = startVal; i <= endVal; i++)
            {
                symbolTable.DefineSymbol((string)ast.variable.value, "int", i);
                this.VisitNode(ast.children);
            }
        }

        /// <summary>
        /// Traverses unary operation AST
        /// </summary>
        /// <param name="ast"></param>
        /// <returns>OneOperatorAST</returns>
        private object VisitOneOperatorAST(OneOperatorAST ast)
        {
            object component1 = this.VisitNode(ast.expression);
            if (component1 is bool)
            {
                return !(bool)component1;
            }
            else
            {
                if (ast.token.type == TokenType.MINUS) return -(int)this.VisitNode(ast.expression);
                return +(int)this.VisitNode(ast.expression);
            }
            
        }

        /// <summary>
        /// Traverses a statement list AST
        /// </summary>
        /// <param name="ast">StatementListAST</param>
        private void VisitStatementListAST(StatementListAST ast)
        {
            foreach (AST node in ast.children)
                this.VisitNode(node);
        }

        /// <summary>
        /// Traverses No operation AST
        /// </summary>
        /// <param name="ast">NoOperationAST</param>
        private void VisitNoOperationAST(NoOperationAST ast)
        {
            return;
        }

        /// <summary>
        /// Traverses assign AST
        /// </summary>
        /// <param name="ast">AssignAST</param>
        private void VisitAssignAST(AssignAST ast)
        {
            string variableName = (string)ast.left.value;
            object right = this.VisitNode(ast.right);
            string type = this.symbolTable.LookupSymbol(variableName).type;
            this.symbolTable.DefineSymbol(variableName, type, right);
        }

        /// <summary>
        /// Traverses variable declaration AST
        /// </summary>
        /// <param name="ast">VariableDeclarationAST</param>
        private void VisitVariableDeclarationAST(VariableDeclarationAST ast)
        {
            string varName = (string)ast.varNode.value;
            string typeName = (string)ast.typeNode.value;
            if (ast.assignedValue == null)
            {
                if (typeName == "string") symbolTable.DefineSymbol(varName, typeName, "");
                else if (typeName == "int") symbolTable.DefineSymbol(varName, typeName, 0);
                else symbolTable.DefineSymbol(varName, typeName, false);
            }
            else symbolTable.DefineSymbol(varName, typeName, this.VisitNode(ast.assignedValue));
        }

        /// <summary>
        /// Traverses read AST
        /// </summary>
        /// <param name="ast">ReadAST</param>
        private void VisitReadAST(ReadAST ast)
        {
            char[] whitespace = { '\t', '\r', '\n', ' ', '\v', '\f' };
            string type = symbolTable.LookupSymbol((string)ast.variable.value).type;
            string input = Console.ReadLine();

            // Remove all whitespaces
            string[] split = input.Split(whitespace, 2, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0) input = "";
            else input = split[0];

            if (type == "int")
            {
                try
                {
                    int num = int.Parse(input);
                    symbolTable.DefineSymbol((string)ast.variable.value, type, num);
                }
                catch (FormatException)
                {
                    throw new RuntimeError(ast.variable.token, "Tried to assign non-integer value to integer").Exception();
                }
                catch (OverflowException)
                {
                    throw new RuntimeError(ast.variable.token, "Integer overflow").Exception();
                }
            }
            else
            {
                symbolTable.DefineSymbol((string)ast.variable.value, type, input);
            }
        }

        /// <summary>
        /// Traverses assert AST
        /// </summary>
        /// <param name="ast">AssertAST</param>
        private void VisitAssertAST(AssertAST ast)
        {
            bool res = (bool)VisitNode(ast.expression);
            if (!res) Console.WriteLine("Assertion failed");
        }

        /// <summary>
        /// Traverses type AST
        /// </summary>
        /// <param name="ast">TypeAST</param>
        private void VisitTypeAST(TypeAST ast)
        {
            return;
        }

        /// <summary>
        /// Traverses print AST
        /// </summary>
        /// <param name="ast">PrintAST</param>
        private void VisitPrintAST(PrintAST ast)
        {
            object value = this.VisitNode(ast.expression);
            Console.Write(value);
        }

        /// <summary>
        /// Traverses variable AST
        /// </summary>
        /// <param name="ast">VariableAST</param>
        /// <returns>Symbol value</returns>
        private object VisitVariableAST(VariableAST ast)
        {
            string variableName = (string)ast.value;
            return this.symbolTable.LookupSymbol(variableName).value;
        }

        /// <summary>
        /// Starts interpreting
        /// </summary>
        public void Interpret()
        {
            if (ast == null) ast = this.parser.Parse();
            this.VisitNode(ast);
        }
    }

        
}
