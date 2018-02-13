using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an address range as an expression
    /// </summary>
    public sealed class AddressRangeNode : ExpressionNode
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="start">Start address</param>
        /// <param name="end">End address</param>
        public AddressRangeNode(ParserRuleContext context, ExpressionNode start, 
            ExpressionNode end) : base(context)
        {
            StartAddress = start;
            EndAddress = end;
        }

        /// <summary>
        /// The start address of the memory section
        /// </summary>
        public ExpressionNode StartAddress { get; }

        /// <summary>
        /// The end address of the memory section
        /// </summary>
        public ExpressionNode EndAddress { get; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext) =>
            StartAddress.ReadyToEvaluate(evalContext)
            && (EndAddress == null || EndAddress.ReadyToEvaluate(evalContext))
            && evalContext.GetMachineContext() != null;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            // --- Test for operand errors
            var addrValue = StartAddress.Evaluate(evalContext);
            if (addrValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = StartAddress.EvaluationError;
                return ExpressionValue.Error;
            }
            var length = EndAddress == null ? new ExpressionValue(1) : EndAddress.Evaluate(evalContext);
            if (length.Type == ExpressionValueType.Error)
            {
                EvaluationError = EndAddress?.EvaluationError;
                return ExpressionValue.Error;
            }

            if (checkOnly) return ExpressionValue.NonEvaluated;

            if (addrValue.Type == ExpressionValueType.ByteArray || length.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Memory address operator cannot be applied on a byte array";
                return ExpressionValue.Error;
            }

            var addr = addrValue.AsNumber();
            return new ExpressionValue(evalContext.GetMachineContext().GetMemorySection((ushort)addr, 
                (ushort)(addr + length.AsNumber() - 1)));
        }
    }
}