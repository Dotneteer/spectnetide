namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFG pragma
    /// </summary>
    public sealed class DefgPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The DEFG pattern value
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefgPragma(string pattern)
        {
            Pattern = pattern;
        }
    }
}