using System.Collections.Generic;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// This class represents a data block of the language
    /// </summary>
    public class DataBlock : LanguageBlockBase
    {
        /// <summary>
        /// The 'data' keyword span
        /// </summary>
        public TextSpan DataKeywordSpan { get; set; }

        /// <summary>
        /// The data members in this block
        /// </summary>
        public List<DataMember> DataMembers { get; set; }

        /// <summary>
        /// The 'end' keyword span
        /// </summary>
        public TextSpan EndKeywordSpan { get; set; }
    }
}