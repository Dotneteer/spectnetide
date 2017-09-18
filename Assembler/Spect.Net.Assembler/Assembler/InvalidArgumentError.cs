using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an error when invalid argument is used
    /// </summary>
    public class InvalidArgumentError : SemanticErrorBase
    {
        public InvalidArgumentError(string errorCode, SourceLineBase line, params object[] parameters) : 
            base(errorCode, line, parameters)
        {
        }
    }
}