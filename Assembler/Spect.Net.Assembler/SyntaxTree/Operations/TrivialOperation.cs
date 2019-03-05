using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a trivial instruction that contains a single mnemonic
    /// without any additional parameter
    /// </summary>
    public class TrivialOperation : EmittingOperationBase
    {
        public TrivialOperation(IParseTree context) : base(context)
        {
        }
    }
}