using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents an indexed address with replacement
    /// </summary>
    public class IndexedAddress
    {
        /// <summary>
        /// Index register
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Displacement sign
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Displacement expression
        /// </summary>
        public ExpressionNode Displacement { get; set; }
    }
}