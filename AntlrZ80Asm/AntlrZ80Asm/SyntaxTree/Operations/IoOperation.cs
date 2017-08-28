using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents an I/O operation
    /// </summary>
    public sealed class IoOperation : OperationBase
    {
        /// <summary>
        /// Port address
        /// </summary>
        public ExpressionNode Port { get; set; }

        /// <summary>
        /// Register
        /// </summary>
        public string Register { get; set; }
    }
}