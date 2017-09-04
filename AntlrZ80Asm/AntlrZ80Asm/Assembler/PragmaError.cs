using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents an issue with a pragma
    /// </summary>
    public class PragmaError : SemanticErrorBase
    {
        public PragmaError(SourceLineBase line, string message) : 
            base(line.SourceLine, line.Position, "", message)
        {
        }
    }
}