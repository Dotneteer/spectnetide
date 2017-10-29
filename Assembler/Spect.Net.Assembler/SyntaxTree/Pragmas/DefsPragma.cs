using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFS pragma
    /// </summary>
    public sealed class DefsPragma : PragmaBase
    {
        /// <summary>
        /// The bytes to define
        /// </summary>
        public ExpressionNode Expression { get; set; }
    }
}