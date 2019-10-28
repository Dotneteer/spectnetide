using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Represents a single abstract run time assignment
    /// </summary>
    public abstract class RunTimeAssignmentPlanBase
    {
    }

    /// <summary>
    /// Represents a run time flag assignment
    /// </summary>
    public class RunTimeFlagAssignmentPlan : RunTimeAssignmentPlanBase
    {
        /// <summary>
        /// The name of the flag
        /// </summary>
        public string FlagName { get; }

        public RunTimeFlagAssignmentPlan(string flagName)
        {
            FlagName = flagName;
        }
    }

    /// <summary>
    /// Represents a run time register assignment plan
    /// </summary>
    public class RunTimeRegisterAssignmentPlan : RunTimeAssignmentPlanBase
    {
        /// <summary>
        /// The name of the register
        /// </summary>
        public string RegisterName { get; }

        /// <summary>
        /// The value to assign to the register
        /// </summary>
        public ExpressionNode Value { get; }

        public RunTimeRegisterAssignmentPlan(string registerName, ExpressionNode value)
        {
            RegisterName = registerName;
            Value = value;
        }
    }

    /// <summary>
    /// Represents a run time memory assignment plan
    /// </summary>
    public class RunTimeMemoryAssignmentPlan : RunTimeAssignmentPlanBase
    {
        /// <summary>
        /// Memory address
        /// </summary>
        public ExpressionNode Address { get; }

        /// <summary>
        /// Run time value
        /// </summary>
        public ExpressionNode Value { get; }

        /// <summary>
        /// Run time length
        /// </summary>
        public ExpressionNode Length { get; }

        public RunTimeMemoryAssignmentPlan(ExpressionNode address, ExpressionNode value, ExpressionNode length)
        {
            Address = address;
            Value = value;
            Length = length;
        }
    }
}