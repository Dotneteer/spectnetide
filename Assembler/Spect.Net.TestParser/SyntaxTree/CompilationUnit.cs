using System.Collections.Generic;

namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a compilation unit
    /// </summary>
    public class CompilationUnit: ClauseBase
    {
        /// <summary>
        /// The language blocks
        /// </summary>
        public List<LanguageBlockBase> LanguageBlocks { get; }

        public CompilationUnit()
        {
            LanguageBlocks = new List<LanguageBlockBase>();
        }
    }
}