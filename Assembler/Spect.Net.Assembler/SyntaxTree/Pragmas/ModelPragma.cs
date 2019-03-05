using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the MODEL pragma
    /// </summary>
    public sealed class ModelPragma : PragmaBase
    {
        /// <summary>
        /// The Spectrum model to use
        /// </summary>
        public string Model { get; set; }

        public ModelPragma(Z80AsmParser.ModelPragmaContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                Model = context.IDENTIFIER().NormalizeToken();
            }
            else if (context.NEXT() != null)
            {
                Model = context.NEXT().NormalizeToken();
            }
        }
    }
}