namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    /// <summary>
    /// This class represents a snapshot of the PSG state
    /// -- created when a register's value changes.
    /// </summary>
    public class PsgStateSnapshot : PsgState
    {
        /// <summary>
        /// The CPU tact when the register's value was set
        /// </summary>
        public long CpuTact { get; set; }

        /// <summary>
        /// Gets the index of the register changed
        /// </summary>
        public byte ChangedRegisterIndex { get; set; }
    }
}