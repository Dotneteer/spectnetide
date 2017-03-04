using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    /// <summary>
    /// This message signs that the Spectrum VM is ready to
    /// run its execution cycle
    /// </summary>
    public class SpectrumVmPreparedToRunMessage: MessageBase
    {
        /// <summary>
        /// Emulation mode to use
        /// </summary>
        public EmulationMode EmulationMode { get; }

        /// <summary>
        /// Debug step mode to use
        /// </summary>
        public DebugStepMode DebugStepMode { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public SpectrumVmPreparedToRunMessage(EmulationMode emulationMode, DebugStepMode debugStepMode)
        {
            EmulationMode = emulationMode;
            DebugStepMode = debugStepMode;
        }
    }
}