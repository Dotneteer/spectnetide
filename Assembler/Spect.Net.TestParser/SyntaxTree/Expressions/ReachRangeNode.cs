namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an address reach range as an expression
    /// </summary>
    public sealed class ReachRangeNode : ExpressionNode
    {
        /// <summary>
        /// The start address of the memory section
        /// </summary>
        public ExpressionNode StartAddress { get; set; }

        /// <summary>
        /// The length of the memory section
        /// </summary>
        public ExpressionNode Length { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext) =>
            StartAddress.ReadyToEvaluate(evalContext)
            && (Length == null || Length.ReadyToEvaluate(evalContext))
            && evalContext.IsMachineAvailable();

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            // --- Test for operand errors
            var addrValue = StartAddress.Evaluate(evalContext);
            if (addrValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = StartAddress.EvaluationError;
                return ExpressionValue.Error;
            }
            var length = Length == null ? new ExpressionValue(1) : Length.Evaluate(evalContext);
            if (length.Type == ExpressionValueType.Error)
            {
                EvaluationError = Length?.EvaluationError;
                return ExpressionValue.Error;
            }

            if (addrValue.Type == ExpressionValueType.ByteArray || length.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Code reach operator cannot be applied on a byte array";
                return ExpressionValue.Error;
            }

            var addr = addrValue.AsWord();
            return new ExpressionValue(evalContext.GetReachSection(addr, 
                (ushort)(addr + length.AsWord() - 1)));
        }
    }
}