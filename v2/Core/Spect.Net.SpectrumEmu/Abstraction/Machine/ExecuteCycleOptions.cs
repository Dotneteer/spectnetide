namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This class provides options for the ExecuteCycle function.
    /// </summary>
    public class ExecuteCycleOptions
    {
        /// <summary>
        /// The emulation mode to use.
        /// </summary>
        public EmulationMode EmulationMode { get; }

        /// <summary>
        /// The debug mode to use.
        /// </summary>
        public DebugStepMode DebugStepMode { get; }

        /// <summary>
        /// Indicates if fast tape mode is allowed.
        /// </summary>
        public bool FastTapeMode { get; }

        /// <summary>
        /// The index of the ROM when a termination point is defined.
        /// </summary>
        public int TerminationRom { get; }

        /// <summary>
        /// The value of the PC register to reach when EmulationMode is
        /// set to UntilExecutionPoint.
        /// </summary>
        public ushort TerminationPoint { get; }

        /// <summary>
        /// Initializes the options.
        /// </summary>
        /// <param name="emulationMode">Execution emulation mode.</param>
        /// <param name="debugStepMode">Debugging execution mode.</param>
        /// <param name="fastTapeMode">Fast tape mode.</param>
        /// <param name="terminationRom">ROM index of the termination point.</param>
        /// <param name="terminationPoint">Termination point to reach.</param>
        public ExecuteCycleOptions(EmulationMode emulationMode = EmulationMode.Continuous,
            DebugStepMode debugStepMode = DebugStepMode.StopAtBreakpoint,
            bool fastTapeMode = false,
            int terminationRom = 0x0000,
            ushort terminationPoint = 0x0000)
        {
            EmulationMode = emulationMode;
            DebugStepMode = debugStepMode;
            FastTapeMode = fastTapeMode;
            TerminationRom = terminationRom;
            TerminationPoint = terminationPoint;
        }
    }

}