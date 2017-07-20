using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Devices.Interrupt
{
    /// <summary>
    /// This device is responsible to raise a maskable interrupt in every screen
    /// rendering frame, according to Spectrum specification
    /// </summary>
    public class InterruptDevice: IInterruptDevice
    {
        /// <summary>
        /// Represents the longest instruction tact count
        /// </summary>
        public const int LONGEST_OP_TACTS = 23;

        /// <summary>
        /// The Z80 CPU that receives th interrupt request
        /// </summary>
        public Z80Cpu Cpu { get; }

        /// <summary>
        /// The ULA tact to raise the interrupt at
        /// </summary>
        public int InterruptTact { get; }

        /// <summary>
        /// Signs that an interrupt has been raised in this frame.
        /// </summary>
        public bool InterruptRaised { get; private set; }

        /// <summary>
        /// Signs that the interrupt signal has been revoked
        /// </summary>
        public bool InterruptRevoked { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public InterruptDevice(Z80Cpu cpu, int interruptTact)
        {
            Cpu = cpu;
            InterruptTact = interruptTact;
            Reset();
        }

        /// <summary>
        /// Resets the device.
        /// </summary>
        /// <remarks>
        /// You should reset the device in each screen frame to raise an
        /// interrupt at the next frame.
        /// </remarks>
        public void Reset()
        {
            InterruptRaised = false;
            InterruptRevoked = false;
        }

        /// <summary>
        /// Generates an interrupt in the current phase, if time has come.
        /// </summary>
        /// <param name="currentTact">Current frame tact</param>
        public void CheckForInterrupt(int currentTact)
        {
            if (InterruptRevoked)
            {
                // --- We fully handled the interrupt in this frame
                return;
            }

            if (currentTact < InterruptTact)
            {
                // --- The interrupt should not be raised yet
                return;
            }

            if (currentTact > InterruptTact + LONGEST_OP_TACTS)
            {
                // --- Let's revoke the INT signal independently whether the CPU
                // --- caught it or not
                InterruptRevoked = true;
                Cpu.StateFlags &= Z80StateFlags.InvInt;
                return;
            }

            if (InterruptRaised)
            {
                // --- The interrupt is raised, not revoked, but the CPU has not handled it yet
                return;
            }

            // --- It's time to raise the interrupt
            InterruptRaised = true;
            Cpu.StateFlags |= Z80StateFlags.Int;
        }

        /// <summary>
        /// Announces that the device should start a new frame
        /// </summary>
        /// <remarks>
        /// Although this device is bound to the ULA frame cycle, it does not need to do
        /// anything when a new frame is started.
        /// </remarks>
        public void StartNewFrame()
        {
            InterruptRaised = false;
            InterruptRevoked = false;
        }

        /// <summary>
        /// Signs that the current frame has been completed
        /// </summary>
        /// <remarks>
        /// Although this device is bound to the ULA frame cycle, it does not need to do
        /// anything when a frame is completed.
        /// </remarks>
        public void SignFrameCompleted()
        {
        }
    }
}