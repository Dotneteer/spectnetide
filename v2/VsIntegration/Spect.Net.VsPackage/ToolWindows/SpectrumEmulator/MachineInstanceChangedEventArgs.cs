using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// These arguments are used when the machine instance changes
    /// </summary>
    public class MachineInstanceChangedEventArgs
    {
        public MachineInstanceChangedEventArgs(SpectrumMachine oldMachine, SpectrumMachine newMachine)
        {
            OldMachine = oldMachine;
            NewMachine = newMachine;
        }

        /// <summary>
        /// The old ZX Spectrum vm
        /// </summary>
        public SpectrumMachine OldMachine { get; }

        /// <summary>
        /// The new ZX Spectrum vm
        /// </summary>
        public SpectrumMachine NewMachine { get; }
    }
}