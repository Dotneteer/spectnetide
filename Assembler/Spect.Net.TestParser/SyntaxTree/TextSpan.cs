using Antlr4.Runtime;
using Antlr4.Runtime.Tree;


namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This structure represents a text span with an inclusive start and
    /// an exclusive end position.
    /// </summary>
    public struct TextSpan
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TextSpan(int startLine, int startColumn, int endLine, int endColumn)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TextSpan(ParserRuleContext context) : this(
            context.Start.Line,
            context.Start.StartIndex,
            context.Stop.Line,
            context.Stop.Column + context.Stop.StopIndex - context.Stop.StartIndex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TextSpan(IParseTree token): this((CommonToken)((TerminalNodeImpl)token).Symbol)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TextSpan(IToken token)
        {
            EndLine = StartLine = token.Line;
            StartColumn = token.Column;
            EndColumn = token.Column + token.StopIndex - token.StartIndex;
        }

        /// <summary>
        /// The source line number
        /// </summary>
        public int StartLine { get; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int StartColumn { get; }

        /// <summary>
        /// The source line number
        /// </summary>
        public int EndLine { get; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int EndColumn { get; }
    }
}