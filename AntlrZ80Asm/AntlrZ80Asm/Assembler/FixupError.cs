using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
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