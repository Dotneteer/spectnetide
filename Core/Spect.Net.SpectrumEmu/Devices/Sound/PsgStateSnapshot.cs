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

        /// <summary>
        /// Creates a new instance of theis class based on the specified values
        /// </summary>
        /// <param name="lastState">Last PSG state</param>
        /// <param name="tact">CPU tacts</param>
        /// <param name="index">Last register index</param>
        /// <param name="value">Register value</param>
        public PsgStateSnapshot(PsgState lastState, long tact, byte index, byte value)
        {
            CpuTact = tact;
            ChangedRegisterIndex = index;
            for (var i = 0; i <= 0x0F; i++)
            {
                this[i] = lastState[i];
            }
            this[index] = value;
        }
    }
}