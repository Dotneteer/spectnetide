using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This struct describes an inclusion/exclusion rule
    /// between two operand types
    /// </summary>
    public class OperandRule
    {
        public OperandType FirstOp { get; }
        public OperandType SecondOp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public OperandRule(OperandType firstOp, OperandType secondOp = OperandType.None)
        {
            FirstOp = firstOp;
            SecondOp = secondOp;
        }
    }
}