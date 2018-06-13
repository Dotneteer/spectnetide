using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the INCLUDEBIN pragma
    /// </summary>
    public sealed class IncludeBinPragma : PragmaBase
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

        public IncludeBinPragma(ExpressionNode fileExpr, ExpressionNode offsetExpr, ExpressionNode lengthExpr)
        {
            FileExpr = fileExpr;
            OffsetExpr = offsetExpr;
            LengthExpr = lengthExpr;
        }
    }
}