using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    /// <summary>
    /// Represents a register assignment
    /// </summary>
    public class RegisterAssignment : AssignmentClause
    {
        /// <summary>
        /// The register's name
        /// </summary>
        public string RegisterName { get; set; }

        /// <summary>
        /// Right-side expression
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}