using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFM pragma
    /// </summary>
    public sealed class DefmPragma : PragmaBase
    {
        /// <summary>
        /// The message to define
        /// </summary>
        public ExpressionNode Message { get; set; }
    }
}