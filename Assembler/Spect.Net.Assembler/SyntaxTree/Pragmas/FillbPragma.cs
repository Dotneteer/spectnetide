using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the FILLB pragma
    /// </summary>
    public sealed class FillbPragma : PragmaBase
    {
        /// <summary>
        /// The bytes to define
        /// </summary>
        public ExpressionNode Count { get; set; }

        /// <summary>
        /// The bytes to define
        /// </summary>
        public ExpressionNode Expression { get; set; }
    }
}