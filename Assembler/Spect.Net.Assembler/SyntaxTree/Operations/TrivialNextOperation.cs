using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a trivial Spectrum Next instruction that contains a 
    /// single mnemonic without any additional parameter
    /// </summary>
    public class TrivialNextOperation : TrivialOperation
    {
        public TrivialNextOperation(IParseTree context) : base(context)
        {
        }
    }
}