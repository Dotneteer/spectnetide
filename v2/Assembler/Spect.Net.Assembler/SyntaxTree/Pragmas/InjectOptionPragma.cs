using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the MODEL pragma
    /// </summary>
    public sealed class InjectOptionPragma : PragmaBase
    {
        /// <summary>
        /// The Spectrum model to use
        /// </summary>
        public string Option { get; set; }

        public InjectOptionPragma(Z80AsmParser.InjectOptPragmaContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                Option = context.IDENTIFIER().NormalizeToken();
            }
        }
    }
}
