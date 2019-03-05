using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a LOOP statement
    /// </summary>
    public sealed class UntilStatement : EndStatementBase
    {
        /// <summary>
        /// Until expression
        /// </summary>
        public ExpressionNode Expression { get; }

        public UntilStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.UntilStatementContext context)
        {
            Expression = visitorContext.GetExpression(context.expr());
        }
    }
}