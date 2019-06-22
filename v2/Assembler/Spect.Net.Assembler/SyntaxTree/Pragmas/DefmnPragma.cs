using Antlr4.Runtime.Tree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFN pragma
    /// </summary>
    public sealed class DefmnPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The message to define
        /// </summary>
        public ExpressionNode Message { get; }

        /// <summary>
        /// Should the message be terminated with zero?
        /// </summary>
        public bool NullTerminator { get; }

        /// <summary>
        /// Should the message have the last byte's bit 7 set?
        /// </summary>
        public bool Bit7Terminator { get; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefmnPragma(IZ80AsmVisitorContext visitorContext, IParseTree context, 
            bool nullTerminator, bool bit7Terminator)
        {
            if (context.ChildCount > 1)
            {
                Message = visitorContext.GetExpression(context.GetChild(1));
            }
            NullTerminator = nullTerminator;
            Bit7Terminator = bit7Terminator;
        }
    }
}