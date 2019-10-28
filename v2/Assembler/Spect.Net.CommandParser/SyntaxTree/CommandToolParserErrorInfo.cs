using Antlr4.Runtime;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a parser (syntax) error
    /// </summary>
    public class CommandToolParserErrorInfo
    {
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