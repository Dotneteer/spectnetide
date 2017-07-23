using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
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
        /// The current Operation Prefix Mode
        /// </summary>

        Z80Cpu.OpPrefixMode PrefixMode { get; set; }

        /// <summary>
        /// The current Operation Index Mode
        /// </summary>
        Z80Cpu.OpIndexMode IndexMode { get; set; }

        /// <summary>
        /// Block interrupts
        /// </summary>
        void BlockInterrupt();
    }
}