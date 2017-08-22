namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY + operation
    /// </summary>
    public sealed class UnaryPlusNode: UnaryExpressionNode
    {
        public override ushort Evaluate() => Operand.Evaluate();
    }
}