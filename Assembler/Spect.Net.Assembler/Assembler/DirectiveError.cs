using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a preprocessor error
    /// </summary>
    public class DirectiveError : SemanticErrorBase
    {
        public DirectiveError(string errorCode, OperationBase directive, params object[] parameters) : 
            base(errorCode, directive, parameters)
        {
        }
    }
}