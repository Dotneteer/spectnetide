using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Devices.Interrupt
{
    /// <summary>
    /// This device is responsible to raise a maskable interrupt in every screen
    /// rendering frame, according to Spectrum specification
    /// </summary>
    public class InterruptDevice: IInterruptDevice
    {
        private IZ80Cpu _cpu;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _cpu = hostVm.Cpu;
            Reset();
        }

        /// <summary>
        /// Represents the longest instruction tact count
        /// </summary>
        public const int LONGEST_OP_TACTS = 23;

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
        public InterruptDevice(int interruptTact)
        {
            InterruptTact = interruptTact;
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
                _cpu.StateFlags &= Z80StateFlags.InvInt;
                return;
            }

            if (InterruptRaised)
            {
                // --- The interrupt is raised, not revoked, but the CPU has not handled it yet
                return;
            }

            // --- It's time to raise the interrupt
            InterruptRaised = true;
            _cpu.StateFlags |= Z80StateFlags.Int;
            FrameCount++;
        }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            InterruptRaised = false;
            InterruptRevoked = false;
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
        }
    }
}