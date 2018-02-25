namespace Spect.Net.Assembler.SyntaxTree
{
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
}