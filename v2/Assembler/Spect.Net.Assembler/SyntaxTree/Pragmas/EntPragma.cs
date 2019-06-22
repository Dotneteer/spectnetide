using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ENT pragma
    /// </summary>
    public sealed class EntPragma : ExpressionPragmaBase
    {
        public EntPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}