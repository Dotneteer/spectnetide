namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the division operation
    /// </summary>
    public sealed class DivideOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Divides the left operand with the right one
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
        {
            var divider = RightOperand.Evaluate();
            if (divider != 0) return (ushort) (LeftOperand.Evaluate() / divider);

            EvaluationError = "Divide by zero error";
            return 0;
        }
    }
}