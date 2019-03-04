using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ALIGN pragma
    /// </summary>
    public sealed class AlignPragma : ExpressionPragmaBase
    {
        public AlignPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}