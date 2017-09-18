using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an error when invalid argument is used
    /// </summary>
    public class RelativeAddressError : SemanticErrorBase
    {
        public RelativeAddressError(SourceLineBase line, int value) : 
            base("Z0022", line, value)
        {
        }
    }
}