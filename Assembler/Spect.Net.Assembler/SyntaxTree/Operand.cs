using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class defines an operand for operations with compoound addressing scheme
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// The type of the operand
        /// </summary>
        public OperandType Type { get; set; } = OperandType.None;

        /// <summary>
        /// Name of the register (null in case of AddressIndirection and Expression)
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Span of a register
        /// </summary>
        public TextSpan HighlightSpan { get; set; }

        /// <summary>
        /// The expression to evaluate (Expression, AddressIndirection, 
        /// and IndexedAddress)
        /// </summary>
        public ExpressionNode Expression { get; set; }

        /// <summary>
        /// Displacement sign in case of IndexedAddress, or null, 
        /// if there's no displacement
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Condition value
        /// </summary>
        public string Condition { get; set; }
    }
}