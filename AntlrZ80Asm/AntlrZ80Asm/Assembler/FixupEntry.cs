using AntlrZ80Asm.SyntaxTree;
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
        /// The source line that belongs to the fixup
        /// </summary>
        public SourceLineBase SourceLine { get; }

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

        /// <summary>
        /// Gets the optional label, provided the fixup is FixupType.Equ.
        /// </summary>
        public string Label { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FixupEntry(SourceLineBase sourceLine, FixupType type, int segmentIndex, int offset, ExpressionNode expression, string label = null)
        {
            SourceLine = sourceLine;
            Type = type;
            SegmentIndex = segmentIndex;
            Offset = offset;
            Expression = expression;
            Label = label;
        }
    }
}