using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ORG pragma
    /// </summary>
    public sealed class OrgPragma : PragmaBase
    {
        /// <summary>
        /// The ORG parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}