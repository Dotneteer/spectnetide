using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    public abstract class ExpressionPragmaBase : PragmaBase
    {
        /// <summary>
        /// The expression associated with the pragma
        /// </summary>
        public ExpressionNode Expression { get; }

        protected ExpressionPragmaBase(IZ80AsmVisitorContext visitorContext, IParseTree context)
        {
            if (context.ChildCount > 1)
            {
                Expression = visitorContext.GetExpression(context.GetChild(1));
            }
        }
    }
}