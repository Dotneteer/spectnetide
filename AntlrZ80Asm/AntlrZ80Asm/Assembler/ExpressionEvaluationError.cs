namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents an error occurred while evaluating an expression
    /// </summary>
    public class ExpressionEvaluationError : SemanticErrorBase
    {
        public ExpressionEvaluationError(int line, int position, string problematic, string message) : 
            base(line, position, problematic, message)
        {
        }
    }
}