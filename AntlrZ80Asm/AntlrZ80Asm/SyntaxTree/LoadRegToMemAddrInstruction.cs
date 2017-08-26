using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD ('Expression'), 'Reg' instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld (#1234),a
    ///     ld (#1234),e
    ///     ld (#1234),hl
    /// </remarks>
    public class LoadRegToMemAddrInstruction : OperationBase
    {
        /// <summary>
        /// LD instruction destination
        /// </summary>
        public ExpressionNode Destination { get; set; }

        /// <summary>
        /// LD instruction source destination
        /// </summary>
        public string Source { get; set; }
    }
}