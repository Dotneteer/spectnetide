using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents Z80 assembly instructions that emit
    /// machine code.
    /// </summary>
    public abstract class EmittingOperationBase : OperationBase
    {
        protected EmittingOperationBase(IParseTree context) : base(context)
        {
        }
    }
}