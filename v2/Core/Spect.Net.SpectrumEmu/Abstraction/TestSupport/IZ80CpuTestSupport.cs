using Spect.Net.SpectrumEmu.Abstraction.Cpu;

namespace Spect.Net.SpectrumEmu.Abstraction.TestSupport
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of a Z80 CPU device.
    /// </summary>
    public interface IZ80CpuTestSupport
    {
        /// <summary>
        /// Allows setting the number of tacts
        /// </summary>
        /// <param name="tacts">New value of #of tacts</param>
        void SetTacts(long tacts);

        /// <summary>
        /// Sets the specified interrupt mode
        /// </summary>
        /// <param name="im">IM 0, 1, or 2</param>
        void SetInterruptMode(byte im);

        /// <summary>
        /// Sets the IFF1 and IFF2 flags to the specified value;
        /// </summary>
        /// <param name="value">IFF value</param>
        void SetIffValues(bool value);

        /// <summary>
        /// The current Operation Prefix Mode
        /// </summary>

        OpPrefixMode PrefixMode { get; set; }

        /// <summary>
        /// The current Operation Index Mode
        /// </summary>
        OpIndexMode IndexMode { get; set; }

        /// <summary>
        /// Block interrupts
        /// </summary>
        void BlockInterrupt();

        /// <summary>
        /// Removes the CPU from its halted state
        /// </summary>
        void RemoveFromHaltedState();

        /// <summary>
        /// Gets the current execution flow status
        /// </summary>
        IMemoryStatusArray ExecutionFlowStatus { get; }

        /// <summary>
        /// Gets the current memory read status
        /// </summary>
        IMemoryStatusArray MemoryReadStatus { get; }

        /// <summary>
        /// Gets the current memory write status
        /// </summary>
        IMemoryStatusArray MemoryWriteStatus { get; }
    }
}