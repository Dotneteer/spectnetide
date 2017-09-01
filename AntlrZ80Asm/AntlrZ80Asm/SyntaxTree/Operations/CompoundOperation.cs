using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a compound instruction that contains 
    /// additional arguments additionally to the mnemonic
    /// </summary>
    public sealed class CompoundOperation : OperationBase
    {
        /// <summary>
        /// First operands
        /// </summary>
        public Operand Operand { get; set; }

        /// <summary>
        /// Second operands
        /// </summary>
        public Operand Operand2 { get; set; }

        /// <summary>
        /// Condition (flow control operations)
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Bit index expression (bit manipulation operations)
        /// </summary>
        public ExpressionNode BitIndex { get; set; }
    }
}