using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD 'reg', ('Expression') instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld a,(#1234)
    ///     ld e,(#1234)
    ///     ld bc,(#1234)
    /// </remarks>
    public class LoadRegFromMemAddrInstruction : Instruction
    {
        /// <summary>
        /// LD instruction destination
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// LD instruction source destination
        /// </summary>
        public ExpressionNode Source { get; set; }
    }
}