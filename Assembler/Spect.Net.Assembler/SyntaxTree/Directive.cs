using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a preprocessor directive
    /// </summary>
    public sealed class Directive : OperationBase
    {
        /// <summary>
        /// Optional identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Optional expression
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}