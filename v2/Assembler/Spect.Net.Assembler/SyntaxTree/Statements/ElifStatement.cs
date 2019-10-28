using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents an ELIF statement
    /// </summary>
    public sealed class ElifStatement : StatementBase
    {
        /// <summary>
        /// ELIF expression
        /// </summary>
        public ExpressionNode Expression { get; }

        public ElifStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.ExprContext context)
        {
            Expression = visitorContext.GetExpression(context);
        }
    }
}