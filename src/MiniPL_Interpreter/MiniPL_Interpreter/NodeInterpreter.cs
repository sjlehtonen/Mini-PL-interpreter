using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Base class for NodeInterpreter
    /// </summary>
    class NodeInterpreter
    {
        /// <summary>
        /// Method gets the class name of the argument and makes the function name with it
        /// For example if node is ForAST, then the method would be VisitForAST
        /// </summary>
        /// <param name="node">Node to visit</param>
        /// <returns>Possible return value</returns>
        public object VisitNode(AST node)
        {
            string methodName = "Visit" + node.GetType().Name;
            Type type = this.GetType();
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            object[] args = { node };

            try { return method.Invoke(this, args); }
            catch (TargetInvocationException e) { throw e.InnerException; }
        }
    }
}
