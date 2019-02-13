using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class IdentifierNode : ExpressionNode
    {
        /// <summary>
        /// Signs if identifier evaluation should start at the global scope.
        /// </summary>
        public bool StartFromGlobal { get; set; }

        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Symbol names within scopes
        /// </summary>
        public List<string> ScopeSymbolNames { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
        {
            var result = evalContext.GetSymbolValue(SymbolName) != null;
            if (!result)
            {
                AddError(SymbolName);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var idExpr = evalContext.GetSymbolValue(SymbolName);
            if (idExpr != null)
            {
                return idExpr;
            }
            AddError(SymbolName);
            return ExpressionValue.NonEvaluated;
        }
    }
}