using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Represents a fixup item for a structure
    /// </summary>
    public class StructFixup
    {
        /// <summary>
        /// Offset relative to the start address of the struct
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// The type of the fixup
        /// </summary>
        public FixupType Type { get; }

        /// <summary>
        /// The fixup expression
        /// </summary>
        public ExpressionNode Expression { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public StructFixup(int offset, FixupType type, ExpressionNode expression)
        {
            Offset = offset;
            Type = type;
            Expression = expression;
        }
    }
}