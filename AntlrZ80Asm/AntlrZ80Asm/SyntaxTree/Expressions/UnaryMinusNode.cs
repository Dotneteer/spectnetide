namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY - operation
    /// </summary>
    public sealed class UnaryMinusNode : UnaryExpressionNode
    {
        public override ushort Evaluate() => (ushort)-Operand.Evaluate();
    }
}