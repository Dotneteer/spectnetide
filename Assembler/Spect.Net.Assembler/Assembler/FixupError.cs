using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a fixup-phase error
    /// </summary>
    public class FixupError : SemanticErrorBase
    {
        public FixupError(SourceLineBase opLine, string message) : base(opLine.SourceLine, -1, null, message)
        {
        }
    }
}