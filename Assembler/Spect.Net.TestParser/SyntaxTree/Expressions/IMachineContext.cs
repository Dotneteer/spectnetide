namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// Represents the context of a virtual machine
    /// </summary>
    public interface IMachineContext
    {
        /// <summary>
        /// Signs if this is a compile time context
        /// </summary>
        bool IsCompileTimeContext { get; }

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        ushort GetRegisterValue(string regName);

        /// <summary>
        /// Gets the value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Register name</param>
        /// <returns>
        /// The flags's current value
        /// </returns>
        bool GetFlagValue(string flagName);

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        byte[] GetMemorySection(ushort start, ushort end);

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        byte[] GetReachSection(ushort start, ushort end);
    }
}