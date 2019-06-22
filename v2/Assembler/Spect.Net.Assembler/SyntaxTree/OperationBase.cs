using Antlr4.Runtime.Tree;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents an instruction
    /// </summary>
    public abstract class OperationBase : SourceLineBase
    {
        /// <summary>
        /// Mnemonic of the instruction (in UPPERCASE)
        /// </summary>
        public string Mnemonic { get; set; }

        // TODO: Eliminate the null default
        protected OperationBase(IParseTree context = null)
        {
            if (context != null)
            {
                Mnemonic = context.GetChild(0).NormalizeToken();
            }
        }
    }
}