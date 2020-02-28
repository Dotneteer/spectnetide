using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the BANK pragma
    /// </summary>
    public sealed class BankPragma : ExpressionPragmaBase
    {
        /// <summary>
        /// Start offset of the bank
        /// </summary>
        public ExpressionNode StartOffset { get; set; }

        public BankPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
            if (context.ChildCount > 3)
            {
                StartOffset = visitorContext.GetExpression(context.GetChild(3));
            }
        }
    }
}