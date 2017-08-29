using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a bit operation
    /// </summary>
    public class BitOperation : OperationBase
    {
        /// <summary>
        /// Bit index for BIT/SET/RES
        /// </summary>
        public ExpressionNode BitIndex { get; set; }

        /// <summary>
        /// Associated register
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Name of the index register
        /// </summary>
        public string IndexRegister { get; set; }

        /// <summary>
        /// The expression to evaluate (Expression, AddressIndirection, 
        /// and IndexedAddress)
        /// </summary>
        public ExpressionNode Displacement { get; set; }

        /// <summary>
        /// Displacement sign in case of IndexedAddress, or null, 
        /// if there's no displacement
        /// </summary>
        public string Sign { get; set; }
    }
}