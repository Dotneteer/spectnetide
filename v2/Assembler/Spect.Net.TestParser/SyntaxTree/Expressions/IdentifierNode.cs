using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class IdentifierNode : ExpressionNode
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public IdentifierNode(ParserRuleContext context) : base(context)
        {
            SymbolName = context.GetText();
        }

        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string SymbolName { get; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext)
            => evalContext.GetSymbolValue(SymbolName) != null;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            return evalContext.GetSymbolValue(SymbolName) ?? ExpressionValue.Error;
        }
    }
}