using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFN pragma
    /// </summary>
    public sealed class DefmnPragma : PragmaBase
    {
        /// <summary>
        /// The message to define
        /// </summary>
        public ExpressionNode Message { get; set; }

        /// <summary>
        /// Should the message be terminated with zero?
        /// </summary>
        public bool NullTerminator { get; set; }
    }
}