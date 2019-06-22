using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFH pragma
    /// </summary>
    public sealed class DefhPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The byte vector to define
        /// </summary>
        public ExpressionNode ByteVector { get; set; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefhPragma(IZ80AsmVisitorContext visitorContext, Z80AsmParser.DefhPragmaContext context)
        {
            if (context.expr() != null)
            {
                ByteVector = visitorContext.GetExpression(context.expr());
            }
        }
    }
}