using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an address reach range as an expression
    /// </summary>
    public sealed class MemoryReadTouchNode : MemoryTouchNodeBase
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="start">Start address</param>
        /// <param name="end">End address</param>
        public MemoryReadTouchNode(ParserRuleContext context, ExpressionNode start,
            ExpressionNode end) : base(context, start, end)
        {
        }

        /// <summary>
        /// Evaluates a memory touch expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>True, if all bytes within the section has been touched</returns>
        public override byte[] EvalTouch(IExpressionEvaluationContext evalContext, ushort start, ushort end) 
            => evalContext.GetMachineContext().GetMemoryReadSection(start, end);
    }
}