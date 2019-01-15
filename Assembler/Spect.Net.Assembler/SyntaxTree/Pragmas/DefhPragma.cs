using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFH pragma
    /// </summary>
    public sealed class DefhPragma : PragmaBase
    {
        /// <summary>
        /// The bytevector to define
        /// </summary>
        public ExpressionNode ByteVector { get; set; }
    }
}