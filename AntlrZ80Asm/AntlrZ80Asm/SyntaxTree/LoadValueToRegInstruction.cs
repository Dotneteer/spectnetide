using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD 'Reg', 'value' instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld b,#13
    ///     ld (hl),24
    ///     ld bc,#1234
    ///     ld ix,#2345
    /// </remarks>
    public class LoadValueToRegInstruction : OperationBase
    {
        /// <summary>
        /// LD instruction destination
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// LD instruction value to put into a register
        /// </summary>
        public ExpressionNode Expression { get; set; }
    }
}