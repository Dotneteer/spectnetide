namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a Z80 flag value node
    /// </summary>
    public sealed class Z80FlagNode : ExpressionNode
    {
        /// <summary>
        /// Register name
        /// </summary>
        public string Flag { get; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            SuggestType(ExpressionValueType.Byte);
            return evalContext.GetZ80FlagValue(Flag);
        }

        /// <summary>
        /// Initializes with the specified flag
        /// </summary>
        /// <param name="flag">Flag name</param>
        public Z80FlagNode(string flag)
        {
            Flag = flag;
        }
    }
}