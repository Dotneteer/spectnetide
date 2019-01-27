namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class SymbolNode : ExpressionNode
    {
        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var idExpr = evalContext.GetSymbolValue(SymbolName);
            if (idExpr != ExpressionValue.Error)
            {
                SuggestType(ExpressionValueType.Word);
                return idExpr;
            }
            EvaluationError = $"Symbol '{SymbolName}' cannot be found";
            return ExpressionValue.Error;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => SymbolName == null ? "" : SymbolName.ToUpper();
    }
}