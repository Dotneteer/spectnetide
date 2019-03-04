using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ORG pragma
    /// </summary>
    public sealed class OrgPragma : ExpressionPragmaBase, ILabelSetter
    {
        public OrgPragma(IZ80AsmVisitorContext visitorContext, IParseTree context) 
            : base(visitorContext, context)
        {
        }
    }
}