using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFGX pragma
    /// </summary>
    public sealed class DefgxPragma : ExpressionPragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefgxPragma(IZ80AsmVisitorContext visitorContext, IParseTree context)
            : base(visitorContext, context)
        {
        }
    }
}