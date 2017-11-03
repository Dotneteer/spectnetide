using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.Z80Programs.Debugging
{
    /// <summary>
    /// This class holds breakpoint information used to debug Z80 Assembler
    /// source code
    /// </summary>
    public class BreakpointInfo: IBreakpointInfo
    {
        /// <summary>
        /// This flag shows that the breakpoint is assigned to source code.
        /// </summary>
        public bool IsCpuBreakpoint => false;

        /// <summary>
        /// File name with full path
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Line number within the file
        /// </summary>
        public int FileLine { get; set; }

        /// <summary>
        /// Type of breakpoint
        /// </summary>
        public BreakpointType Type { get; set; }

        /// <summary>
        /// The value of the condition to check
        /// </summary>
        public string ConditionExpression { get; set; }

        /// <summary>
        /// The last value of the condition. Null means that the condition
        /// has not been evaluated yet.
        /// </summary>
        public ushort? LastConditionValue { get; set; }

        /// <summary>
        /// The hit count condition value if the type of the breakpoint 
        /// is related to hit count
        /// </summary>
        public int HitCountValue { get; set; }

        /// <summary>
        /// The current hit count value
        /// </summary>
        public int CurrentHitCount { get; set; }
    }

    /// <summary>
    /// The type of breakpoint
    /// </summary>
    public enum BreakpointType
    {
        NoCondition = 0,
        ConditionTrue,
        ExpressionChanges,
        HitCountEquals,
        HitCountIsMultipleOf,
        HitCountAtLeast
    }
}