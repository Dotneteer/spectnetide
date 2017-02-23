namespace Spect.Net.Z80Emu.Test.Helpers
{
    /// <summary>
    /// This enum defines the run modes the Z80TestMachine allows
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// Run while the machine is disposed or a break signal arrives.
        /// </summary>
        Normal,

        /// <summary>
        /// Run a single CPU Execution cycle, even if an operation
        /// contains multiple bytes
        /// </summary>
        OneCycle,

        /// <summary>
        /// Pause when the next single instruction is executed.
        /// </summary>
        OneInstruction,

        /// <summary>
        /// Run until a HALT instruction is reached.
        /// </summary>
        UntilHalt,

        /// <summary>
        /// Run until a break signal arrives.
        /// </summary>
        UntilBreak,

        /// <summary>
        /// Run until the whole injected code is executed
        /// </summary>
        UntilEnd
    }
}