using System.Text;
using Spect.Net.Assembler.Generated;

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
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
        {
            var result = evalContext.GetSymbolValue(SymbolName, StartFromGlobal).ExprValue != null;
            if (!result)
            {
                AddError(FullSymbolName);
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
            var (exprValue, usageInfo) = evalContext.GetSymbolValue(SymbolName, StartFromGlobal);
            if (exprValue != null)
            {
                if (usageInfo != null)
                {
                    usageInfo.IsUsed = true;
                }
                return exprValue;
            }
            AddError(FullSymbolName);
            return ExpressionValue.NonEvaluated;
        }

        /// <summary>
        /// Gets the full name of the symbol
        /// </summary>
        public string FullSymbolName
        {
            get
            {
                var sb = new StringBuilder();
                if (StartFromGlobal) sb.Append("::");
                sb.Append(SymbolName);
                return sb.ToString();
            }
        }

        public IdentifierNode(Z80AsmParser.SymbolContext context) : base(context)
        {
            StartFromGlobal = context.GetChild(0).GetText() == "::";
            SymbolName = context.IDENTIFIER()?.NormalizeToken();
        }
    }
}