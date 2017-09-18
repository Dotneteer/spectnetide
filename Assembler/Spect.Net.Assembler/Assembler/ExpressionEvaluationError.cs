using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an error occurred while evaluating an expression
    /// </summary>
    public class ExpressionEvaluationError : SemanticErrorBase
    {
        public ExpressionEvaluationError(string errorCode, SourceLineBase line, params object[] parameters) : 
            base(errorCode, line, parameters)
        {
        }
    }
}