namespace Spect.Net.Spectrum.Machine
{
    /// <summary>
    /// This enumeration defines how the spectrum emulation mode
    /// should work
    /// </summary>
    public enum EmulationMode
    {
        /// <summary>
        /// Run the virtual machine until stopped
        /// </summary>
        Continuous,

        /// <summary>
        /// Run the VM while a single Z80 instruction is executed
        /// </summary>
        SingleZ80Instruction,

        /// <summary>
        /// Run the VM until the CPU is halted
        /// </summary>
        UntilHalt,

        /// <summary>
        /// Run the CPU until the current ULA rendering frame ends
        /// by the ULA clock
        /// </summary>
        UntilFrameEnds,

        /// <summary>
        /// Run the CPU until the current ULA rendering frame ends
        /// and wait while the time for the next frame comes.
        /// </summary>
        UntilNextFrame,

        /// <summary>
        /// Run the CPU until the current ULA rendering frame ends
        /// and wait while the time for the next frame comes and
        /// initialize the next cycle
        /// </summary>
        UntilNextFrameCycle
    }
}