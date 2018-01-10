using Antlr4.Runtime;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a register assignment
    /// </summary>
    public class RegisterAssignment : AssignmentNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public RegisterAssignment(ParserRuleContext context) : base(context)
        {
        }

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