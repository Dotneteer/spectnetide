using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Machine;

namespace Spect.Net.VsPackage.Debugging
{
    /// <summary>
    /// This class holds breakpoint information used to debug Z80 Assembler
    /// source code
    /// </summary>
    public class BreakpointInfo : IBreakpointInfo
    {
        /// <summary>
        /// This flag shows that the breakpoint is assigned to source code.
        /// </summary>
        public bool IsCpuBreakpoint { get; set; }

        /// <summary>
        /// Type of breakpoint hit condition
        /// </summary>
        public BreakpointHitType HitType { get; set; }

        /// <summary>
        /// Value of the hit condition
        /// </summary>
        public ushort HitConditionValue { get; set; }

        /// <summary>
        /// Value of the filter condition
        /// </summary>
        public string FilterCondition { get; set; }

        /// <summary>
        /// The expression that represents the filter condition
        /// </summary>
        public ExpressionNode FilterExpression { get; set; }

        /// <summary>
        /// File name with full path
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Line number within the file
        /// </summary>
        public int FileLine { get; set; }

        /// <summary>
        /// The current hit count value
        /// </summary>
        public int CurrentHitCount { get; set; }
    }
}
