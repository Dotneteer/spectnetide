namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFG pragma
    /// </summary>
    public sealed class DefgPragma : PragmaBase
    {
        /// <summary>
        /// The DEFG pattern value
        /// </summary>
        public string Pattern { get; }

        public DefgPragma(string pattern)
        {
            Pattern = pattern;
        }
    }
}