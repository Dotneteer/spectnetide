using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an unexpected source code line error
    /// </summary>
    public class UnexpectedSourceCodeLineError : SemanticErrorBase
    {
        public UnexpectedSourceCodeLineError(string errorCode, SourceLineBase line, params object[] parameters) : 
            base(errorCode, line, parameters)
        {
        }
    }
}