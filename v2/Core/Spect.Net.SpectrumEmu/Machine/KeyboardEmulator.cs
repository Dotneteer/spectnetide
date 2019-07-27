using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents the keyboard of a Spectrum machine
    /// </summary>
    public sealed class KeyboardEmulator
    {
        private readonly ISpectrumVm _spectrumVm;
        public KeyboardEmulator(ISpectrumVm spectrumVm)
        {
            _spectrumVm = spectrumVm;
        }

        /// <summary>
        /// Gets the length of frame refresh period in milliseconds
        /// </summary>
        public int RefreshPeriod => (int)(1000.0 * _spectrumVm.FrameTacts /
            _spectrumVm.BaseClockFrequency / _spectrumVm.ClockMultiplier);

        /// <summary>
        /// Enques an emulated key stroke
        /// </summary>
        /// <param name="time">Time given in framecounts</param>
        /// <param name="primaryCode">Primary key code</param>
        /// <param name="secondaryCode">Secondary key code</param>
        public void QueueKeyStroke(int time, SpectrumKeyCode primaryCode,
            SpectrumKeyCode? secondaryCode = null)
        {
            if (_spectrumVm == null) return;

            var currentTact = _spectrumVm.Cpu.Tacts;
            var lastTact = currentTact + _spectrumVm.FrameTacts * time * _spectrumVm.ClockMultiplier;

            _spectrumVm.KeyboardProvider.QueueKeyPress(
                new EmulatedKeyStroke(
                    currentTact,
                    lastTact,
                    primaryCode,
                    secondaryCode));
        }

        /// <summary>
        /// Gets the value of the MODE system variable
        /// </summary>
        /// <param name="modeValue">Value of the system variable</param>
        /// <returns>True, if value obtained; otherwise, false</returns>
        public bool GetModeValue(out ushort? modeValue)
        {
            modeValue = null;
            var memory = _spectrumVm.MemoryDevice.CloneMemory();
            var mode = SystemVariables.Get("MODE")?.Address;
            if (mode == null) return false;
            modeValue = memory[(ushort)mode];
            return true;
        }
    }
}