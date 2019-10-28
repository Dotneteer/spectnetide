using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the FILLB pragma
    /// </summary>
    public class FillbPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The # of bytes to define
        /// </summary>
        public ExpressionNode Count { get; set; }

        /// <summary>
        /// The byte to emit
        /// </summary>
        public ExpressionNode Expression { get; }


        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public FillbPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
        {
            if (context.ChildCount > 1)
            {
                Count = visitorContext.GetExpression(context.GetChild(1));
            }

            if (context.ChildCount > 3)
            {
                Expression = visitorContext.GetExpression(context.GetChild(3));
            }
        }
    }
}