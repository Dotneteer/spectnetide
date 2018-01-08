namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    /// <summary>
    /// Represents a flag assignment
    /// </summary>
    public class FlagAssignment : AssignmentClause
    {
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