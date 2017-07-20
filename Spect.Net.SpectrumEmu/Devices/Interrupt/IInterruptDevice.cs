using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Devices.Interrupt
{
    /// <summary>
    /// This class defines the responsibilities of the device that generates
    /// the interrupt in the ULA chip.
    /// </summary>
    public interface IInterruptDevice : IUlaFrameBoundDevice
    {
        /// <summary>
        /// The Z80 CPU that receives the interrupt request
        /// </summary>
        Z80Cpu Cpu { get; }

        /// <summary>
        /// The ULA tact to raise the interrupt at
        /// </summary>
        int InterruptTact { get; }

        /// <summary>
        /// Signs that an interrupt has been raised in this frame.
        /// </summary>
        bool InterruptRaised { get; }

        /// <summary>
        /// Signs that the interrupt signal has been revoked
        /// </summary>
        bool InterruptRevoked { get; }

        /// <summary>
        /// Generates an interrupt in the current phase, if time has come.
        /// </summary>
        /// <param name="currentTact">Current frame tact</param>
        void CheckForInterrupt(int currentTact);
    }
}