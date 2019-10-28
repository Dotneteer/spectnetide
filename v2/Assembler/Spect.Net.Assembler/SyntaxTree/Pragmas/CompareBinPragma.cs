using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the INCLUDEBIN pragma
    /// </summary>
    public sealed class CompareBinPragma : PragmaBase
    {
        /// <summary>
        /// The filename expression
        /// </summary>
        public ExpressionNode FileExpr { get; }

        /// <summary>
        /// The start offset expression
        /// </summary>
        public ExpressionNode OffsetExpr { get; }

        /// <summary>
        /// The length expression
        /// </summary>
        public ExpressionNode LengthExpr { get; }

        public CompareBinPragma(IZ80AsmVisitorContext visitorContext, Z80AsmParser.CompareBinPragmaContext context)
        {
            if (context.expr() == null) return;

            if (context.expr().Length > 0)
            {
                FileExpr = visitorContext.GetExpression(context.expr()[0]);
            }
            if (context.expr().Length > 1)
            {
                OffsetExpr = visitorContext.GetExpression(context.expr()[1]);
            }
            if (context.expr().Length > 2)
            {
                LengthExpr = visitorContext.GetExpression(context.expr()[2]);
            }
        }
    }
}