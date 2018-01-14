using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a register assignment
    /// </summary>
    public class RegisterAssignmentNode : AssignmentNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="expr">Register value</param>
        public RegisterAssignmentNode(Z80TestParser.RegAssignmentContext context, ExpressionNode expr) : base(context)
        {
            RegisterName = context.registerSpec()?.GetText().ToLower();
            Expr = expr;
        }

        /// <summary>
        /// The register's name
        /// </summary>
        public string RegisterName { get; }

        /// <summary>
        /// Right-side expression
        /// </summary>
        public ExpressionNode Expr { get; }
    }
}