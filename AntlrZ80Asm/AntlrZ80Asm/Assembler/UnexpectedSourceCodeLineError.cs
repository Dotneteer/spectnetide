using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents an unexpected source code line error
    /// </summary>
    public class UnexpectedSourceCodeLineError : SemanticErrorBase
    {
        public UnexpectedSourceCodeLineError(SourceLineBase line, string message) : 
            base(line.SourceLine, line.Position, string.Empty, message)
        {
        }
    }
}