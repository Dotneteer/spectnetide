using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the VAR pragma
    /// </summary>
    public sealed class VarPragma : LabelSetterPragmaBase
    {
        /// <summary>
        /// The VAR parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}