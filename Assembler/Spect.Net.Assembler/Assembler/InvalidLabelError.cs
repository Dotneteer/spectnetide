using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an error when invalid argument is used
    /// </summary>
    public class InvalidLabelError : SemanticErrorBase
    {
        public InvalidLabelError(SourceLineBase line, string label) : 
            base("Z0040", line, label)
        {
        }
    }
}