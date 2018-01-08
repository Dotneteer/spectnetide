namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a block of the Z80 Test Language
    /// </summary>
    public abstract class LanguageBlockBase : ClauseBase
    {
        /// <summary>
        /// The index of the source file this language block belongs to
        /// </summary>
        public int FileIndex { get; set; }
    }
}