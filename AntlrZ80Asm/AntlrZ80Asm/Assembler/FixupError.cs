namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents a fixup-phase error
    /// </summary>
    public class FixupError : SemanticErrorBase
    {
        public FixupError(string message) : base(-1, -1, null, message)
        {
        }
    }
}