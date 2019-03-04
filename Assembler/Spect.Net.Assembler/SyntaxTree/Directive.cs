using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a preprocessor directive
    /// </summary>
    public sealed class Directive : OperationBase
    {
        /// <summary>
        /// Optional identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Optional expression
        /// </summary>
        public ExpressionNode Expr { get; set; }

        public Directive(IZ80AsmVisitorContext visitorContext, IParseTree context): base(context)
        {
            if (context.ChildCount > 1)
            {
                Identifier = context.GetChild(1).NormalizeToken();
            }
            if (context.GetChild(1) is Z80AsmParser.ExprContext)
            {
                Expr = visitorContext.GetExpression(context.GetChild(1));
            }
        }
    }
}