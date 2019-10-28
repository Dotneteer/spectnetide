namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Represents a single abstract assignment
    /// </summary>
    public abstract class AssignmentPlanBase
    {
    }

    /// <summary>
    /// Represents a flag assignment
    /// </summary>
    public class FlagAssignmentPlan : AssignmentPlanBase
    {
        /// <summary>
        /// The name of the flag
        /// </summary>
        public string FlagName { get; }

        public FlagAssignmentPlan(string flagName)
        {
            FlagName = flagName;
        }
    }

    /// <summary>
    /// Represents a register assignment plan
    /// </summary>
    public class RegisterAssignmentPlan : AssignmentPlanBase
    {
        /// <summary>
        /// The name of the register
        /// </summary>
        public string RegisterName { get; }

        /// <summary>
        /// The value to assign to the register
        /// </summary>
        public ushort Value { get; }

        public RegisterAssignmentPlan(string registerName, ushort value)
        {
            RegisterName = registerName;
            Value = value;
        }
    }

    /// <summary>
    /// Represents a memory assignment plan
    /// </summary>
    public class MemoryAssignmentPlan : AssignmentPlanBase
    {
        /// <summary>
        /// Memory address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Source byte array
        /// </summary>
        public byte[] Value { get; }

        public MemoryAssignmentPlan(ushort address, byte[] value)
        {
            Address = address;
            Value = value;
        }
    }
}