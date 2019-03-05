// ReSharper disable ArrangeAccessorOwnerBody

namespace Spect.Net.Assembler.SyntaxTree.Expressions
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
        public ExpressionNode LeftOperand { get; }

        /// <summary>
        /// Right operand
        /// </summary>
        public ExpressionNode RightOperand { get; }

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
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
        {
            // --- Note: We intentionally avoid short-circuit evaluation!
            var leftResult = LeftOperand.ReadyToEvaluate(evalContext);
            var rightResult = RightOperand.ReadyToEvaluate(evalContext);
            return leftResult && rightResult;
        }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            return EvaluationError == null ? Calculate(evalContext) : ExpressionValue.Error;
        }

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public abstract ExpressionValue Calculate(IEvaluationContext evalContext);

        /// <summary>
        /// Indicates if this expression has a macro parameter
        /// </summary>
        public override bool HasMacroParameter
            => (LeftOperand?.HasMacroParameter ?? false)
               || (RightOperand?.HasMacroParameter ?? false);

        protected BinaryOperationNode(object leftOperand, object rightOperand)
        {
            LeftOperand = leftOperand as ExpressionNode;
            RightOperand = rightOperand as ExpressionNode;
        }
    }
}