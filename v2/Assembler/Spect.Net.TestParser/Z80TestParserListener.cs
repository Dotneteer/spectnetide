using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;

namespace Spect.Net.TestParser
{
    /// <summary>
    /// Implements a modified listener that collect comment information
    /// </summary>
    public class Z80TestParserListener: Z80TestParserBaseListener
    {
        private readonly BufferedTokenStream _tokens;

        public Z80TestParserListener(BufferedTokenStream tokens)
        {
            _tokens = tokens;
            CommentSpans = new SortedList<int, int>();
        }

        /// <summary>
        /// Contains the comments for a particular rule context
        /// </summary>
        public SortedList<int, int> CommentSpans { get; }

        /// <summary>
        /// Exit a parse tree produced by <see cref="Z80TestParser.compileUnit"/>.
        /// <para>The default implementation does nothing.</para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        public override void ExitCompileUnit(Z80TestParser.CompileUnitContext context)
        {
            CommentSpans.Clear();
            foreach (var token in _tokens.GetTokens())
            {
                if (token.Channel == Z80TestLexer.COMMENT)
                {
                    CommentSpans.Add(token.StartIndex, token.StopIndex);
                }
            }
        }

    }
}