namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class provides options for the ExecuteCycle function.
    /// </summary>
    public class ExecuteCycleOptions
    {
        /// <summary>
        /// The emulation mode to use
        /// </summary>
        public EmulationMode EmulationMode { get; }

        /// <summary>
        /// The debug mode to use
        /// </summary>
        public DebugStepMode DebugStepMode { get; }

        /// <summary>
        /// Indicates if fast tape mode is allowed
        /// </summary>
        public bool FastTapeMode { get; }

        /// <summary>
        /// The index of the ROM when a termination point is defined
        /// </summary>
        public int TerminationRom { get; }

        /// <summary>
        /// The value of the PC register to reach when EmulationMode is
        /// set to UntilExceutionPoint
        /// </summary>
        public ushort TerminationPoint { get; }

        /// <summary>
        /// Signs if the instructions within the maskable interrupt 
        /// routine should be skipped
        /// </summary>
        public bool SkipInterruptRoutine { get; }

        /// <summary>
        /// This flag shows that the virtual machine should run in hidden mode
        /// (no screen, no sound, no delays
        /// </summary>
        public bool HiddenMode { get; }

        /// <summary>
        /// Timeout in CPU tacts
        /// </summary>
        public long TimeoutTacts { get; }

        /// <summary>
        /// Initializes the options
        /// </summary>
        /// <param name="emulationMode">Execution emulation mode</param>
        /// <param name="debugStepMode">Debugging execution mode</param>
        /// <param name="fastTapeMode">Fast tape mode</param>
        /// <param name="terminationRom">ROM index of the termination point</param>
        /// <param name="terminationPoint">Termination point to reach</param>
        /// <param name="skipInterruptRoutine">
        /// Signs if maskable interrupt routine instructions should be skipped
        /// </param>
        /// <param name="hiddenMode">The VM should run in hidden mode</param>
        /// <param name="timeoutTacts">Run time out in CPU tacts</param>
        public ExecuteCycleOptions(EmulationMode emulationMode = EmulationMode.Continuous, 
            DebugStepMode debugStepMode = DebugStepMode.StopAtBreakpoint, 
            bool fastTapeMode = false,
            int terminationRom = 0x0000,
            ushort terminationPoint = 0x0000,
            bool skipInterruptRoutine = false,
            bool hiddenMode = false,
            long timeoutTacts = 0)
        {
            EmulationMode = emulationMode;
            DebugStepMode = debugStepMode;
            FastTapeMode = fastTapeMode;
            TerminationRom = terminationRom;
            TerminationPoint = terminationPoint;
            SkipInterruptRoutine = skipInterruptRoutine;
            HiddenMode = hiddenMode;
            TimeoutTacts = timeoutTacts;
        }
    }
}