using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an abstract memory touch expression
    /// </summary>
    public abstract class MemoryTouchNodeBase : ExpressionNode
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="start">Start address</param>
        /// <param name="end">End address</param>
        protected MemoryTouchNodeBase(ParserRuleContext context, ExpressionNode start,
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
        /// The length of the memory section
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
            var startAddr = StartAddress.Evaluate(evalContext);
            if (startAddr.Type == ExpressionValueType.Error)
            {
                EvaluationError = StartAddress.EvaluationError;
                return ExpressionValue.Error;
            }

            var endAddr = startAddr;
            if (EndAddress != null)
            {
                endAddr = EndAddress.Evaluate(evalContext);
                if (endAddr.Type == ExpressionValueType.Error)
                {
                    EvaluationError = EndAddress?.EvaluationError;
                    return ExpressionValue.Error;
                }
            }

            if (checkOnly) return ExpressionValue.NonEvaluated;

            if (startAddr.Type == ExpressionValueType.ByteArray || endAddr.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Code reach operator cannot be applied on a byte array";
                return ExpressionValue.Error;
            }

            return new ExpressionValue(EvalTouch(evalContext, startAddr.AsWord(), endAddr.AsWord()));
        }

        /// <summary>
        /// Evaluates a memory touch expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>True, if all bytes within the section has been touched</returns>
        public abstract byte[] EvalTouch(IExpressionEvaluationContext evalContext, ushort start, ushort end);
    }
}