using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the RNDSEED pragma
    /// </summary>
    public sealed class RndSeedPragma : ExpressionPragmaBase
    {
        public RndSeedPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}