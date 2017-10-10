using System;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// Defines the operations a keyboard provider should implement so that
    /// it can represent the Spectrum VM's keyboard
    /// </summary>
    public interface IKeyboardProvider: IVmComponentProvider
    {
        /// <summary>
        /// Sets the method that can handle the status change of a Spectrum keyboard key
        /// </summary>
        /// <param name="statusHandler">Key status handler method</param>
        /// <remarks>
        /// The first argument of the handler method is the Spectrum key code. The
        /// second argument indicates if the specified key is down (true) or up (false)
        /// </remarks>
        void SetKeyStatusHandler(Action<SpectrumKeyCode, bool> statusHandler);

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
        void Scan(bool allowPhysicalKeyboard);

        /// <summary>
        /// Adds an emulated keypress to the queue of the provider.
        /// </summary>
        /// <param name="keypress">Keystroke information</param>
        /// <remarks>The provider can play back emulated key strokes</remarks>
        void QueueKeyPress(EmulatedKeyStroke keypress);
    }
}