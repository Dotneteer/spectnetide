using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the VAR pragma
    /// </summary>
    public sealed class VarPragma : ExpressionPragmaBase, ILabelSetter
    {
        public VarPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}