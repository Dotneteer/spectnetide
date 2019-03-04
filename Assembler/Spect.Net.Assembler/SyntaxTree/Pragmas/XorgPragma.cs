using Antlr4.Runtime.Tree;
namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the XORG pragma
    /// </summary>
    public sealed class XorgPragma : ExpressionPragmaBase
    {
        public XorgPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}