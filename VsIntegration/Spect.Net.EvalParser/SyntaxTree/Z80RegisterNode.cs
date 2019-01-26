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
            => evalContext.GetZ80RegisterValue(Register);

        /// <summary>
        /// Initializes with the specified register
        /// </summary>
        /// <param name="register">Register name</param>
        public Z80RegisterNode(string register)
        {
            Register = register;
        }
    }
}