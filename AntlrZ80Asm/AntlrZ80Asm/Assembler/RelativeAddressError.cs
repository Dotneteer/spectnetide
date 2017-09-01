using AntlrZ80Asm.SyntaxTree;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents an error when invalid argument is used
    /// </summary>
    public class RelativeAddressError : SemanticErrorBase
    {
        public RelativeAddressError(SourceLineBase line, int value) : 
            base(line.SourceLine, line.Position, "", 
                $"Relative jump should be between -128 and 127. {value} is invalid.")
        {
        }
    }
}