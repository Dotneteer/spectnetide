using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents a fixup that recalculates and replaces
    /// unresolved symbol value at the end of the compilation
    /// </summary>
    public class FixupEntry
    {
        /// <summary>
        /// Type fo the fixup
        /// </summary>
        public FixupType Type { get; }

        /// <summary>
        /// Affected code segment
        /// </summary>
        public int SegmentIndex { get; }

        /// <summary>
        /// Offset within the code segment
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Expression to evaluate
        /// </summary>
        public ExpressionNode Expression { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FixupEntry(FixupType type, int segmentIndex, int offset, ExpressionNode expression)
        {
            Type = type;
            SegmentIndex = segmentIndex;
            Offset = offset;
            Expression = expression;
        }
    }
}