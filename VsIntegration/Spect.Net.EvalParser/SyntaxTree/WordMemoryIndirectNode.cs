using System.Net.NetworkInformation;

namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a word memory value node
    /// </summary>
    public sealed class WordMemoryIndirectNode : ExpressionNode
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

            var lsb = evalContext.GetMemoryIndirectValue(address);
            if (!lsb.IsValid)
            {
                return ExpressionValue.Error;
            }

            var msb = evalContext.GetMemoryIndirectValue(new ExpressionValue(address.Value + 1));
            SuggestType(ExpressionValueType.Word);
            return !msb.IsValid 
                ? ExpressionValue.Error 
                : new ExpressionValue(lsb.Value + (msb.Value << 8));
        }

        /// <summary>
        /// Initializes with the specified address expression
        /// </summary>
        /// <param name="address">Memory address expression</param>
        public WordMemoryIndirectNode(ExpressionNode address)
        {
            Address = address;
        }
    }
}