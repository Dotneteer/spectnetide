using System;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

#pragma warning disable 67

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
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        IDeviceState IDevice.GetState() => new InterruptDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

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

            // --- Do not raise the interrupt when the CPU blocks it
            if (_cpu.IsInterruptBlocked) return;

            // --- It's time to raise the interrupt
            InterruptRaised = true;
            _cpu.StateFlags |= Z80StateFlags.Int;
            FrameCount++;
        }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

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

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// State of the interrupt device
        /// </summary>
        public class InterruptDeviceState : IDeviceState
        {
            public bool InterruptRaised { get; set; }
            public bool InterruptRevoked { get; set; }

            public InterruptDeviceState(InterruptDevice device)
            {
                InterruptRaised = device.InterruptRaised;
                InterruptRevoked = device.InterruptRevoked;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is InterruptDevice intr)) return;

                intr.InterruptRaised = InterruptRaised;
                intr.InterruptRevoked = InterruptRevoked;
            }
        }
    }

#pragma warning restore
}