using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class defines an operand for operations with compoound addressing scheme
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// The type of addressing used
        /// </summary>
        public AddressingType AddressingType { get; set; }

        /// <summary>
        /// Name of the register (null in case of AddressIndirection and Expression)
        /// </summary>
        public string Register { get; set; }

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
    }

    /// <summary>
    /// This enumeration defines the operand type
    /// </summary>
    public enum AddressingType
    {
        None,
        Register,
        RegisterIndirection,
        Expression,
        AddressIndirection,
        IndexedAddress
    }
}