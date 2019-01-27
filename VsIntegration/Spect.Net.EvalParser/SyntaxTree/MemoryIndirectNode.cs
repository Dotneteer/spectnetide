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
        /// Memory width specifier
        /// </summary>
        public string WidthSpecifier { get; }

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
            switch (WidthSpecifier)
            {
                case "W":
                    SuggestType(ExpressionValueType.Word);
                    return new ExpressionValue(evalContext.GetMemoryIndirectValue(address).Value 
                        + evalContext.GetMemoryIndirectValue(new ExpressionValue(address.Value + 1)).Value << 8);
                case "DW":
                    SuggestType(ExpressionValueType.DWord);
                    return new ExpressionValue(evalContext.GetMemoryIndirectValue(address).Value
                        + (evalContext.GetMemoryIndirectValue(new ExpressionValue(address.Value + 1)).Value << 8)
                        + (evalContext.GetMemoryIndirectValue(new ExpressionValue(address.Value + 2)).Value << 16)
                        + (evalContext.GetMemoryIndirectValue(new ExpressionValue(address.Value + 3)).Value << 24));
                default:
                    SuggestType(ExpressionValueType.Byte);
                    return evalContext.GetMemoryIndirectValue(address);
            }
        }

        /// <summary>
        /// Initializes with the specified address expression
        /// </summary>
        /// <param name="address">Memory address expression</param>
        /// <param name="widthSpecifier">Width specifier</param>
        public MemoryIndirectNode(ExpressionNode address, string widthSpecifier)
        {
            Address = address;
            WidthSpecifier = widthSpecifier;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() =>
            !string.IsNullOrEmpty(WidthSpecifier)
                ? $"[{Address} @{WidthSpecifier}]" 
                : $"[{Address}]";
    }
}