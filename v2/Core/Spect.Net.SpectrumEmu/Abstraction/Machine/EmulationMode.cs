namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This enumeration defines how the spectrum emulation mode.
    /// should work.
    /// </summary>
    public enum EmulationMode
    {
        /// <summary>
        /// Runs until not cancelled or paused by the user
        /// </summary>
        Continuous,

        /// <summary>
        /// Run the CPU until the current ULA rendering frame ends.
        /// by the ULA clock
        /// </summary>
        UntilRenderFrameEnds,

        /// <summary>
        /// Run the virtual machine in debugger mode.
        /// </summary>
        Debugger,

        /// <summary>
        /// Run the VM until the CPU is halted.
        /// </summary>
        UntilHalt,

        /// <summary>
        /// Run the CPU until a specified value of the PC register is reached.
        /// </summary>
        UntilExecutionPoint
    }
}