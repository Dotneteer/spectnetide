// ReSharper disable ArrangeAccessorOwnerBody

using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an abstract binary operation
    /// </summary>
    public abstract class BinaryOperationNode : ExpressionNode
    {
        private string _evalError;

        /// <summary>
        /// Left operand
        /// </summary>
        public ExpressionNode LeftOperand { get; set; }

        /// <summary>
        /// Right operand
        /// </summary>
        public ExpressionNode RightOperand { get; set; }

        /// <summary>
        /// Retrieves any error in child operator nodes
        /// </summary>
        public override string EvaluationError
        {
            get { return _evalError ?? LeftOperand.EvaluationError ?? RightOperand.EvaluationError; } 
            set { _evalError = value; } 
        }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext) 
            => LeftOperand.ReadyToEvaluate(evalContext) 
                && RightOperand.ReadyToEvaluate(evalContext);

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            return checkOnly
                ? ExpressionValue.NonEvaluated
                : (EvaluationError == null ? Calculate(evalContext) : ExpressionValue.Error);
        }

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public abstract ExpressionValue Calculate(IExpressionEvaluationContext evalContext);

        protected BinaryOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}