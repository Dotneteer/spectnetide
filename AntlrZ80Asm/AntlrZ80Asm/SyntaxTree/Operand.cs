using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
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
    /// This enumeration declares the available operand types
    /// </summary>
    public enum OperandType
    {
        /// <summary>
        /// No operand
        /// </summary>
        None = 0,

        /// <summary>
        /// B, C, D, E, H, L, A
        /// </summary>
        Reg8,

        /// <summary>
        /// XH, XL, YH, YL
        /// </summary>
        Reg8Idx,

        /// <summary>
        /// I, R
        /// </summary>
        Reg8Spec,

        /// <summary>
        /// BC, DE, HL, SP
        /// </summary>
        Reg16,

        /// <summary>
        /// IX, IY
        /// </summary>
        Reg16Idx,

        /// <summary>
        /// AF, AF'
        /// </summary>
        Reg16Spec,

        /// <summary>
        /// (BC), (DE), (HL), (SP)
        /// </summary>
        RegIndirect,

        /// <summary>
        /// (expression)
        /// </summary>
        MemIndirect,

        /// <summary>
        /// (C)
        /// </summary>
        CPort,

        /// <summary>
        /// (IX+disp), (IY+disp)
        /// </summary>
        IndexedAddress,

        /// <summary>
        /// Expression
        /// </summary>
        Expr
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