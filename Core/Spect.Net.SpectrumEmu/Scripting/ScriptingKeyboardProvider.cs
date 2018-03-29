using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This is the default keyboard provider used by the scripting engine
    /// </summary>
    public class ScriptingKeyboardProvider : IKeyboardProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Sets the method that can handle the status change of a Spectrum keyboard key
        /// </summary>
        /// <param name="statusHandler">Key status handler method</param>
        /// <remarks>
        /// The first argument of the handler method is the Spectrum key code. The
        /// second argument indicates if the specified key is down (true) or up (false)
        /// </remarks>
        public void SetKeyStatusHandler(Action<SpectrumKeyCode, bool> statusHandler)
        {
        }

        /// <summary>
        /// Initiate scanning the entire keyboard
        /// </summary>
        /// <param name="allowPhysicalKeyboard">
        /// Indicates if scanning the physical keyboard is allowed
        /// </param>
        /// <remarks>
        /// If the physical keyboard is not allowed, the device can use other
        /// ways to emulate the virtual machine's keyboard
        /// </remarks>
        public void Scan(bool allowPhysicalKeyboard)
        {
        }

        /// <summary>
        /// Emulates queued key strokes as if those were pressed by the user
        /// </summary>
        /// <returns>
        /// True, if any key stroke has been emulated; otherwise, false
        /// </returns>
        public bool EmulateKeyStroke()
        {
            return false;
        }

        /// <summary>
        /// Adds an emulated keypress to the queue of the provider.
        /// </summary>
        /// <param name="keypress">Keystroke information</param>
        /// <remarks>The provider can play back emulated key strokes</remarks>
        public void QueueKeyPress(EmulatedKeyStroke keypress)
        {
        }
    }
}