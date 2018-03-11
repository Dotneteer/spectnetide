namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents a Memory Slot Register (0x50-0x57)
    /// </summary>
    public class MemorySlotRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Initializes a new instance of the feature control register.
        /// </summary>
        /// <param name="id">Memory slot id</param>
        /// <remarks>
        /// Stores the memory page index for the specified slot
        /// </remarks>
        public MemorySlotRegister(byte id) : base(id)
        {
        }
    }
}