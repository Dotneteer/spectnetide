using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents macro parameter name as an expression
    /// </summary>
    public sealed class MacroParamNode : ExpressionNode
    {
        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string MacroParamId { get; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
            => evalContext.GetSymbolValue(MacroParamId).ExprValue != null;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var idExpr = evalContext.GetSymbolValue(MacroParamId).ExprValue;
            return idExpr ?? ExpressionValue.NonEvaluated;
        }

        /// <summary>
        /// Initializes the node
        /// </summary>
        public MacroParamNode(Z80AsmParser.MacroParamContext context, Z80AsmVisitor visitor) 
            : base(context)
        {
            visitor.AddMacroParam(context);
            if (context.IDENTIFIER() != null)
            {
                visitor.AddMacroParamName(context.IDENTIFIER().GetText());
            }
            MacroParamId = context.IDENTIFIER()?.NormalizeToken();
        }

        /// <summary>
        /// Indicates if this expression has a macro parameter
        /// </summary>
        public override bool HasMacroParameter => true;
    }
}