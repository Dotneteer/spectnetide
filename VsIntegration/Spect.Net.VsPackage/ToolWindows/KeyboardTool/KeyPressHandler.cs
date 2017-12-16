using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Timer = System.Threading.Timer;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// This class is responsible for handling the key events
    /// </summary>
    public class KeyPressHandler : IDisposable
    {
        private readonly int _timerPeriod;
        private readonly int _refreshPeriod;
        private bool _isDown;
        private SpectrumKeyCode _lastCode;
        private SpectrumKeyCode? _lastSecondaryCode;
        private readonly Timer _keyTimer;

        /// <summary>
        /// The hosting Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="spectrumVm">The hosting virtual machine</param>
        public KeyPressHandler(ISpectrumVm spectrumVm)
        {
            SpectrumVm = spectrumVm;
            _isDown = false;
            _refreshPeriod = (int)(1000.0*spectrumVm.FrameTacts / spectrumVm.BaseClockFrequency / spectrumVm.ClockMultiplier);
            _timerPeriod = _refreshPeriod - 5;
            _keyTimer = new Timer(OnKeyTimer, null, -1, _timerPeriod);
        }

        /// <summary>
        /// Handles the event of pressing down a key
        /// </summary>
        /// <param name="code">Primary key code</param>
        /// <param name="secondaryCode">Optional secondary key code</param>
        public void KeyDown(SpectrumKeyCode code, SpectrumKeyCode? secondaryCode = null)
        {
            // --- Release the old key when a new key press arrives
            if (_isDown)
            {
                KeyUp(code, secondaryCode);
            }
            _isDown = true;
            _lastCode = code;
            _lastSecondaryCode = secondaryCode;
            _keyTimer.Change(0, _timerPeriod);
        }

        /// <summary>
        /// Handles the event of releasing a key
        /// </summary>
        /// <param name="code">Primary key code</param>
        /// <param name="secondaryCode">Optional secondary key code</param>
        /// <param name="releaseAction">Optional release action</param>
        public void KeyUp(SpectrumKeyCode code, SpectrumKeyCode? secondaryCode, Action releaseAction = null)
        {
            // --- Stop the timer and release the last key
            _keyTimer.Change(-1, _timerPeriod);
            _isDown = false;
            if (code == _lastCode && secondaryCode == _lastSecondaryCode)
            {
                // --- The previously pressed key has been released
                releaseAction?.Invoke();
            }
        }

        /// <summary>
        /// Delays for two and half refresh period
        /// </summary>
        public async Task Delay()
        {
            await Task.Delay(2 * _refreshPeriod + _refreshPeriod / 2);
        }

        /// <summary>
        /// Enques an emulated key stroke
        /// </summary>
        /// <param name="time">Time given in framecounts</param>
        /// <param name="primaryCode">Primary key code</param>
        /// <param name="secondaryCode">Secondary key code</param>
        public void QueueKeyStroke(int time, SpectrumKeyCode primaryCode,
            SpectrumKeyCode? secondaryCode = null)
        {
            if (SpectrumVm == null) return;

            var currentTact = SpectrumVm.Cpu.Tacts;
            var lastTact = currentTact + SpectrumVm.FrameTacts * time * SpectrumVm.ClockMultiplier;

            SpectrumVm.KeyboardProvider.QueueKeyPress(
                new EmulatedKeyStroke(
                    currentTact,
                    lastTact,
                    primaryCode,
                    secondaryCode));
        }

        /// <summary>
        /// Queue a key press, provided there is a key pressed down
        /// </summary>
        /// <param name="state"></param>
        private void OnKeyTimer(object state)
        {
            if (!_isDown) return;
            QueueKeyStroke(2, _lastCode, _lastSecondaryCode);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _keyTimer?.Dispose();
        }
    }
}