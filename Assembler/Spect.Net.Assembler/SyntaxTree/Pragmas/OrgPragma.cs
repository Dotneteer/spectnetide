using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ORG pragma
    /// </summary>
    public sealed class OrgPragma : LabelSetterPragmaBase
    {
        /// <summary>
        /// The ORG parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}