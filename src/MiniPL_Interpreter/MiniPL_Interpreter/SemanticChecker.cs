using System;
using System.Collections.Generic;
using System.Text;
using MiniPL_Interpreter.Symbols;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// This class will traverse the AST and check for any semantic errors
    /// If semantic errors are found, then those errors are added into the errors list
    /// </summary>
    class SemanticChecker : NodeInterpreter
    {
        public Parser parser { get; private set; }
        public SymbolTable symbolTable { get; private set; }
        private List<Error> errors = new List<Error>();
        public AST ast { get; private set; }
        public SemanticChecker(Parser parser)
        {
            this.parser = parser;
            this.symbolTable = new SymbolTable();
        }

        private int VisitNumericAST(NumericAST ast)
        {
            return ast.value;
        }

        private string VisitStringAST(StringAST ast)
        {
            return ast.value;
        }

        private bool VisitBooleanAST(BooleanAST ast)
        {
            return ast.value;
        }

        /// <summary>
        /// Checks semantic errors of a binary operation
        /// If the types don't match, then add error
        /// </summary>
        /// <param name="ast">AST</param>
        /// <returns>Result</returns>
        private object VisitBinaryOperationAST(BinaryOperationAST ast)
        {
            object component1 = this.VisitNode(ast.left);
            object component2 = this.VisitNode(ast.right);

            if (component1 is string && component2 is string)
            {
                string c1 = component1 as string;
                string c2 = component2 as string;

                if (ast.token.type == TokenType.PLUS) return c1 + c2;
                else if (ast.token.type == TokenType.EQUAL) return c1.Equals(c2);
                else if (ast.token.type == TokenType.LESS_THAN) return c1.Length < c2.Length;
                else errors.Add(new UnsupportedOperationError(ast.token, "string"));
            }
            else if (component1 is int && component2 is int)
            {
                // I have classified the division by zero as a runtime error
                // So in the semantic analysis it is ignored and reported at the runtime
                // This is to make the division of the two phases more clear
                try
                {
                    if (ast.token.type == TokenType.PLUS) return (int)component1 + (int)component2;
                    else if (ast.token.type == TokenType.MINUS) return (int)component1 - (int)component2;
                    else if (ast.token.type == TokenType.MUL) return (int)component1 * (int)component2;
                    else if (ast.token.type == TokenType.DIV) return (int)component1 / (int)component2;
                    else if (ast.token.type == TokenType.EQUAL) return (int)component1 == (int)component2;
                    else if (ast.token.type == TokenType.LESS_THAN) return (int)component1 < (int)component2;
                    else errors.Add(new UnsupportedOperationError(ast.token, "int"));
                }
                catch (DivideByZeroException) { return 0; }
                catch (OverflowException) { return 0; }
            }
            else if (component1 is bool && component2 is bool)
            {
                if (ast.token.type == TokenType.EQUAL) return (bool)component1 == (bool)component2;
                else if (ast.token.type == TokenType.LOGICAL_AND) return (bool)component1 & (bool)component2;
                else if (ast.token.type == TokenType.LESS_THAN)
                {
                    if ((bool)component1 == false && (bool)component2 == true) return true;
                    else return false;
                }
                else errors.Add(new UnsupportedOperationError(ast.token, "bool"));
            }
            else if (component1 is null || component2 is null) { }
            else errors.Add(new Error(ast.token, "Type mismatch"));
            return null;
        }

        /// <summary>
        /// Check semantic errors for unary operations
        /// If operator is not supporter for the selected type, add error
        /// </summary>
        /// <param name="ast">AST</param>
        private object VisitOneOperatorAST(OneOperatorAST ast)
        {
            object component1 = this.VisitNode(ast.expression);
            if (component1 is bool && ast.token.type != TokenType.LOGICAL_NOT) errors.Add(new UnsupportedOperationError(ast.token, "bool"));
            else if (component1 is int && ast.token.type != TokenType.MINUS) errors.Add(new UnsupportedOperationError(ast.token, "int"));
            else if (component1.GetType() != typeof(bool) && ast.token.type == TokenType.LOGICAL_NOT) errors.Add(new UnsupportedOperationError(ast.token, component1.GetType().ToString()));
            else if (component1 is string) errors.Add(new UnsupportedOperationError(ast.token, "string"));

            if (component1 is bool && ast.token.type == TokenType.LOGICAL_NOT) return !(bool)component1;
            else if (component1 is int && ast.token.type == TokenType.MINUS) return -(int)component1;
            return null;
        }

        /// <summary>
        /// Traverses a statementlist
        /// </summary>
        /// <param name="ast">AST</param>
        private void VisitStatementListAST(StatementListAST ast)
        {
            foreach (AST node in ast.children) this.VisitNode(node);
        }

        private void VisitNoOperationAST(NoOperationAST ast)
        {
            return;
        }

        /// <summary>
        /// Checks assign operation for semantic errors
        /// Ensure that the types match, if not, then add error
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="variableName">Variable name</param>
        /// <param name="typeName">Type name</param>
        /// <param name="assignValue">Value to be assignment</param>
        /// <param name="define">Tells whether to add the value to the symbol table</param>
        private void CheckAssignOperation(Token token, string variableName, string typeName, object assignValue, bool define = true)
        {
            if ((assignValue is int && typeName == "int" || assignValue is string && typeName == "string" || assignValue is bool && typeName == "bool") && define) symbolTable.DefineSymbol(variableName, typeName, assignValue);
            else if ((assignValue is int && typeName == "int" || assignValue is string && typeName == "string" || assignValue is bool && typeName == "bool") && !define) { }
            else if (assignValue is null) { }
            else errors.Add(new Error(token, "Type mismatch"));
        }

        /// <summary>
        /// Checks assign operation for semantic errors
        /// </summary>
        /// <param name="ast">AST</param>
        private void VisitAssignAST(AssignAST ast)
        {
            string variableName = (string)ast.left.value;
            if (this.VisitNode(ast.left) != null)
            {
                string typeName = this.symbolTable.LookupSymbol(variableName).type;
                object assignValue = this.VisitNode(ast.right);
                this.CheckAssignOperation(ast.right.token, variableName, typeName, assignValue, false);
            }
        }

        /// <summary>
        /// Check variable declaration for semantic errors
        /// </summary>
        /// <param name="ast">AST</param>
        private void VisitVariableDeclarationAST(VariableDeclarationAST ast)
        {
            string varName = (string)ast.varNode.value;
            string typeName = (string)ast.typeNode.value;

            if (this.symbolTable.LookupSymbol(varName) != null) errors.Add(new Error(ast.varNode.token, "Variable " + varName + " already declared"));
            else
            {
                if (ast.assignedValue == null)
                {
                    if (typeName == "int") this.symbolTable.DefineSymbol(varName, typeName, 0);
                    else if (typeName == "string") this.symbolTable.DefineSymbol(varName, typeName, "");
                    else this.symbolTable.DefineSymbol(varName, typeName, false);
                } 
                else
                {
                    object assignValue = this.VisitNode(ast.assignedValue);
                    this.CheckAssignOperation(ast.assignedValue.token, varName, typeName, assignValue);
                }
            }
        }

        private void VisitTypeAST(TypeAST ast)
        {
            return;
        }

        private void VisitPrintAST(PrintAST ast)
        {
            object a = this.VisitNode(ast.expression);
            if (a != null && a.GetType() != typeof(string) && a.GetType() != typeof(int)) errors.Add(new Error(ast.expression.token, "Unsupported type for print"));
        }

        private object VisitVariableAST(VariableAST ast)
        {
            string variableName = (string)ast.value;
            if (this.symbolTable.LookupSymbol(variableName) == null)
            {
                errors.Add(new Error(ast.token, "Variable not declared"));
                return null;
            }
            return this.symbolTable.LookupSymbol(variableName).value;
        }

        private void VisitReadAST(ReadAST ast)
        {
            string varName = (string)ast.variable.value;
            this.VisitNode(ast.variable);
        }

        /// <summary>
        /// Check assert statement for semantic errors
        /// If expression doesn't evaluate to boolean, then add error
        /// </summary>
        /// <param name="ast">AST</param>
        private void VisitAssertAST(AssertAST ast)
        {
            object res = this.VisitNode(ast.expression);
            if (res == null) errors.Add(new Error(ast.token, "Assert requires an expression that evaluates to boolean"));
            else if (res.GetType() != typeof(bool)) errors.Add( new Error(ast.token, "Assert requires an expression that evaluates to boolean")); 
        }

        /// <summary>
        /// Checks for statement for semantic errors
        /// Expression has to evaluate to integer and control variable has to be integer
        /// If they are not, add error
        /// </summary>
        /// <param name="ast">AST</param>
        private void VisitForAST(ForAST ast)
        {
            object v1 = this.VisitNode(ast.variable);
            object v2 = this.VisitNode(ast.expression1);
            object v3 = this.VisitNode(ast.expression2);

            if (v1 == null || v1.GetType() != typeof(int)) errors.Add(new Error(ast.variable.token, "Invalid variable type for for loop, integer required"));
            if (v2 == null || v2.GetType() != typeof(int)) errors.Add(new Error(ast.expression1.token, "Invalid type for expression in for loop, integer required"));
            if (v3 == null || v3.GetType() != typeof(int)) errors.Add(new Error(ast.expression2.token, "Invalid type for expression in for loop, integer required"));
            this.VisitNode(ast.children);

            foreach(AST child in ast.children.children)
            {
                if (child is VariableDeclarationAST) errors.Add(new Error(child.token, "Variable declaration inside for loop"));
            }

            ForAST tempFor = ast;
            Queue<AssignAST> assignAsts = new Queue<AssignAST>();
            Queue<ForAST> nestedFors = new Queue<ForAST>();

            // "Recursively" go through all the children and their children if they are for loops
            while(tempFor != null)
            {
                foreach (AST node in tempFor.children.children)
                {
                    if (node is AssignAST) assignAsts.Enqueue((AssignAST)node);
                    if (node is ForAST) nestedFors.Enqueue((ForAST)node);
                }

                if (nestedFors.Count != 0) tempFor = nestedFors.Dequeue();
                else tempFor = null;
            }

            while(assignAsts.Count != 0)
            {
                AssignAST tempNode = assignAsts.Dequeue();
                string varName = (string)tempNode.left.value;
                if (varName == (string)ast.variable.value) errors.Add(new Error(tempNode.left.token, "Trying to assign loop control variable inside a loop"));
            }
        }

        /// <summary>
        /// Analyzes the program AST
        /// </summary>
        public void Analyze()
        {
            AST tree = this.parser.Parse();
            this.ast = tree;
            this.VisitNode(tree);
        }

        /// <summary>
        /// Checks if there are errors
        /// </summary>
        /// <returns>True if errors, otherwise false</returns>
        public bool HasErrors()
        {
            if (errors.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Prints all the errors
        /// </summary>
        public void PrintAllErrors()
        {
            errors.ForEach(x => Console.WriteLine(x));
        }
    }
}
