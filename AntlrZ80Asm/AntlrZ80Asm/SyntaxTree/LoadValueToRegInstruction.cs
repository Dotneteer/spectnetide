using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a load instruction
    /// </summary>
    public class LoadValueToRegInstruction : Instruction
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