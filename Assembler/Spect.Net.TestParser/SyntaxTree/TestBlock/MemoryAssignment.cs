using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    public class MemoryAssignment : AssignmentClause
    {
        /// <summary>
        /// The memory address;
        /// </summary>
        public ExpressionNode Address { get; set; }

        /// <summary>
        /// Memory expression value
        /// </summary>
        public ExpressionNode Value { get; set; }

        /// <summary>
        /// Oprional length value
        /// </summary>
        public ExpressionNode Lenght { get; set; }
    }
}