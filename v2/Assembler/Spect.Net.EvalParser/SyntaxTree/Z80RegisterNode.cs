namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a Z80 register value node
    /// </summary>
    public sealed class Z80RegisterNode : ExpressionNode
    {
        /// <summary>
        /// Register name
        /// </summary>
        public string Register { get; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var result = evalContext.GetZ80RegisterValue(Register, out var is8Bit);
            SuggestType(is8Bit ? ExpressionValueType.Byte : ExpressionValueType.Word);
            return result;
        }

        /// <summary>
        /// Initializes with the specified register
        /// </summary>
        /// <param name="register">Register name</param>
        public Z80RegisterNode(string register)
        {
            Register = register;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => Register == null ? "" : Register.ToUpper();
    }
}