using System;
using System.Collections.Generic;
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
        // --- This method calls back the IKeyboardDevice of the Spectrum VM
        // --- whenever the state of a key changes
        private Action<SpectrumKeyCode, bool> _statusHandler;

        // --- Stores the key strokes to emulate
        private readonly Queue<EmulatedKeyStroke> _emulatedKeyStrokes =
            new Queue<EmulatedKeyStroke>();

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
            lock (_emulatedKeyStrokes)
            {
                _emulatedKeyStrokes.Clear();
            }
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
            _statusHandler = statusHandler;
        }

        /// <summary>
        /// Initiate scanning the entire keyboard
        /// </summary>
        /// <param name="allowPhysicalKeyboard">
        /// Indicates if scanning the physical keyboard is allowed
        /// </param>
        /// <remarks>
        /// This method never scans the physical keyboard
        /// </remarks>
        public void Scan(bool allowPhysicalKeyboard)
        {
            // --- Intentionally left blank
        }

        /// <summary>
        /// Emulates queued key strokes as if those were pressed by the user
        /// </summary>
        /// <returns>
        /// True, if any key stroke has been emulated; otherwise, false
        /// </returns>
        public bool EmulateKeyStroke()
        {
            // --- Exit, if Spectrum virtual machine is not available
            var spectrumVm = HostVm;
            if (spectrumVm == null) return false;

            var currentTact = spectrumVm.Cpu.Tacts;

            // --- Exit, if no keystroke to emulate
            lock (_emulatedKeyStrokes)
            {
                if (_emulatedKeyStrokes.Count == 0) return false;
            }

            // --- Check the next keystroke
            EmulatedKeyStroke keyStroke;
            lock (_emulatedKeyStrokes)
            {
                keyStroke = _emulatedKeyStrokes.Peek();
            }

            // --- Time has not come
            if (keyStroke.StartTact > currentTact) return false;

            if (keyStroke.EndTact < currentTact)
            {
                // --- End emulation of this very keystroke
                _statusHandler?.Invoke(keyStroke.PrimaryCode, false);
                if (keyStroke.SecondaryCode.HasValue)
                {
                    _statusHandler?.Invoke(keyStroke.SecondaryCode.Value, false);
                }
                lock (_emulatedKeyStrokes)
                {
                    _emulatedKeyStrokes.Dequeue();
                }

                // --- We emulated the release
                return true;
            }

            // --- Emulate this very keystroke, and leave it in the queue
            _statusHandler?.Invoke(keyStroke.PrimaryCode, true);
            if (keyStroke.SecondaryCode.HasValue)
            {
                _statusHandler?.Invoke(keyStroke.SecondaryCode.Value, true);
            }
            return true;
        }

        /// <summary>
        /// Adds an emulated keypress to the queue of the provider.
        /// </summary>
        /// <param name="keypress">Keystroke information</param>
        /// <remarks>The provider can play back emulated key strokes</remarks>
        public void QueueKeyPress(EmulatedKeyStroke keypress)
        {
            lock (_emulatedKeyStrokes)
            {
                if (_emulatedKeyStrokes.Count == 0)
                {
                    _emulatedKeyStrokes.Enqueue(keypress);
                    return;
                }

                var last = _emulatedKeyStrokes.Peek();
                if (last.PrimaryCode == keypress.PrimaryCode
                    && last.SecondaryCode == keypress.SecondaryCode)
                {
                    // --- The same key has been clicked
                    if (keypress.StartTact >= last.StartTact && keypress.StartTact <= last.EndTact)
                    {
                        // --- Old and new click ranges overlap, lengthen the old click
                        last.EndTact = keypress.EndTact;
                        return;
                    }
                }
                _emulatedKeyStrokes.Enqueue(keypress);
            }
        }
    }
}