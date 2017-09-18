using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a fixup-phase error
    /// </summary>
    public class FixupError : SemanticErrorBase
    {
        public FixupError(string errorCode, SourceLineBase opLine, params object[] parameters) : 
            base(errorCode, opLine, parameters)
        {
        }
    }
}