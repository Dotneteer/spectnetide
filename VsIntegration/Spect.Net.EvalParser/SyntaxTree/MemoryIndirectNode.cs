namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a memory value node
    /// </summary>
    public sealed class MemoryIndirectNode : ExpressionNode
    {
        /// <summary>
        /// Register name
        /// </summary>
        public ExpressionNode Address { get; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var address = Address.Evaluate(evalContext);
            if (!address.IsValid)
            {
                return ExpressionValue.Error;
            }
            return evalContext.GetMemoryIndirectValue(address);
        }

        /// <summary>
        /// Initializes with the specified address expression
        /// </summary>
        /// <param name="address">Memory address expression</param>
        public MemoryIndirectNode(ExpressionNode address)
        {
            Address = address;
        }
    }
}