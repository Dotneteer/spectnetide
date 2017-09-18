using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an issue with a pragma
    /// </summary>
    public class PragmaError : SemanticErrorBase
    {
        public PragmaError(string errorCode, SourceLineBase line, params object[] parameters) :
            base(errorCode, line, parameters)
        {
        }
    }
}