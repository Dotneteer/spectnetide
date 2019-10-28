namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// Represents the possible types of an expression value
    /// </summary>
    public enum ExpressionValueType
    {
        Error = 0,
        Bool,
        Integer,
        Real,
        String,
        NonEvaluated
    }
}