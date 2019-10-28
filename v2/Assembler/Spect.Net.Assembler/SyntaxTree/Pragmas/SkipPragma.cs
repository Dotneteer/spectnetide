using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the SKIP pragma
    /// </summary>
    public sealed class SkipPragma : ExpressionPragmaBase
    {
        /// <summary>
        /// The byte to fill the skipped memory
        /// </summary>
        public ExpressionNode Fill { get; set; }

        public SkipPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
            if (context.ChildCount > 3)
            {
                Fill = visitorContext.GetExpression(context.GetChild(3));
            }
        }
    }
}