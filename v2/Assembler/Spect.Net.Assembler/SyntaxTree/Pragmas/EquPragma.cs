using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the EQU pragma
    /// </summary>
    public sealed class EquPragma : ExpressionPragmaBase, ILabelSetter
    {
        public EquPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}