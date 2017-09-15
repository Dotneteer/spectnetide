using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the EQU pragma
    /// </summary>
    public sealed class EquPragma : PragmaBase
    {
        /// <summary>
        /// The EQU parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}