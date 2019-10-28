using Antlr4.Runtime;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a parser (syntax) error
    /// </summary>
    public class Z80AsmParserErrorInfo
    {
        /// <summary>
        /// Source line of the error
        /// </summary>
        public int SourceLine { get; set; }

        /// <summary>
        /// Position within the source line
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Problematic token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Exception raised by the parser
        /// </summary>
        public RecognitionException ParserException { get; set; }
    }
}