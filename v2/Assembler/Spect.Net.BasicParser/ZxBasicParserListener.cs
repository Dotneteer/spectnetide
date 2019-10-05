using Antlr4.Runtime;
using Spect.Net.BasicParser.Generated;
using System.Collections.Generic;

namespace Spect.Net.BasicParser
{
    /// <summary>
    /// Represents a listener that processes the ZX BASIC grammar
    /// </summary>
    public class ZxBasicParserListener: ZxBasicBaseListener
    {
        private readonly BufferedTokenStream _tokens;

        public ZxBasicParserListener(BufferedTokenStream tokens)
        {
            _tokens = tokens;
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
        public override void ExitCompileUnit(ZxBasicParser.CompileUnitContext context)
        {
            CommentSpans.Clear();
            foreach (var token in _tokens.GetTokens())
            {
                if (token.Channel == 0)
                {
                    CommentSpans.Add(token.StartIndex, token.StopIndex);
                }
            }
        }

    }
}
