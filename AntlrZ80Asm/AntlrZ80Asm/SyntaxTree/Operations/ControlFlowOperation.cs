using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a control flow operation
    /// </summary>
    public sealed class ControlFlowOperation : OperationBase
    {
        /// <summary>
        /// Optional condition
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Target expression
        /// </summary>
        public ExpressionNode Target { get; set; }

        /// <summary>
        /// Target address register
        /// </summary>
        public string Register { get; set; }
    }
}