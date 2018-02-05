using Spect.Net.Assembler.Assembler;

namespace Spect.Net.VsPackage.ToolWindows.CompilerOutput
{
    /// <summary>
    /// Respresents fixup record information
    /// </summary>
    public class FixupInfo
    {
        /// <summary>
        /// Type of the fixup
        /// </summary>
        public FixupType Type { get; }

        /// <summary>
        /// Affected code segment
        /// </summary>
        public int SegmentIndex { get; }

        /// <summary>
        /// Offset within the code segment
        /// </summary>
        public ushort Offset { get; }

        /// <summary>
        /// Signs if the fixup is resolved
        /// </summary>
        public bool Resolved { get; set; }

        /// <summary>
        /// Gets the expression text
        /// </summary>
        public string Expression { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FixupInfo(FixupType type, int segmentIndex, ushort offset, bool resolved, string expression)
        {
            Type = type;
            SegmentIndex = segmentIndex;
            Offset = offset;
            Resolved = resolved;
            Expression = expression;
        }
    }
}