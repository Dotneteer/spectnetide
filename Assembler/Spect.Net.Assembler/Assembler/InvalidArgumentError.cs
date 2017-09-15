using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an error when invalid argument is used
    /// </summary>
    public class InvalidArgumentError : SemanticErrorBase
    {
        public InvalidArgumentError(SourceLineBase line, string message) : 
            base(line.SourceLine, line.Position, "", message)
        {
        }
    }
}