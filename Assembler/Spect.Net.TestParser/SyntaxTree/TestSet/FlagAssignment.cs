using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a flag assignment
    /// </summary>
    public class FlagAssignment : AssignmentNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public FlagAssignment(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// Should the flag value be negated?
        /// </summary>
        public bool Negate { get; set; }

        /// <summary>
        /// The flag's name
        /// </summary>
        public string FlagName { get; set; }
    }
}