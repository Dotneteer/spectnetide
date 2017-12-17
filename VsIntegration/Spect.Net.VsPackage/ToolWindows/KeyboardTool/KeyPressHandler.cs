using System.Threading.Tasks;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// This class is responsible for handling the key events
    /// </summary>
    public class KeyPressHandler
    {
        public int RefreshPeriod => (int)(1000.0 * SpectrumVm.FrameTacts /
            SpectrumVm.BaseClockFrequency / SpectrumVm.ClockMultiplier);
        private volatile bool _isDown;

        /// <summary>
        /// The hosting Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm => SpectNetPackage.Default.MachineViewModel.SpectrumVm;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public KeyPressHandler()
        {
            _isDown = false;
        }

        /// <summary>
        /// Handles the event when the main key has been clicked
        /// </summary>
        public async void OnMainKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await WaitKeyReleased(keycontrol.Code,
                keycontrol.SecondaryCode
                ?? (e.ChangedButton == MouseButton.Left
                    ? (SpectrumKeyCode?) null
                    : SpectrumKeyCode.CShift));
        }

        /// <summary>
        /// Handle the event when the shift key has been clicked
        /// </summary>
        public async void OnSymShiftKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await WaitKeyReleased(keycontrol.Code, SpectrumKeyCode.SShift);
        }

        /// <summary>
        /// Handle the event when the extended key has been clicked
        /// </summary>
        public async void OnExtKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await SetExtendedMode();
            await WaitKeyReleased(keycontrol.Code);
        }

        /// <summary>
        /// Handle the event when the extended shift key has been clicked
        /// </summary>
        public async void OnExtShiftKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await SetExtendedMode();
            await WaitKeyReleased(keycontrol.Code, SpectrumKeyCode.SShift);
        }

        /// <summary>
        /// Handle the event when a numeric control key has been clicked
        /// </summary>
        public async void OnNumericControlKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await SetExtendedMode();
            await WaitKeyReleased(keycontrol.Code, e.ChangedButton == MouseButton.Left
                ? (SpectrumKeyCode?)null
                : SpectrumKeyCode.CShift);
        }

        /// <summary>
        /// Handle the event when the graphics key has been clicked
        /// </summary>
        public async void OnGraphicsControlKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            var set = await SetGraphicsMode();
            await WaitKeyReleased(keycontrol.Code);
            if (set)
            {
                await ReleaseGraphicsMode();
            }
        }

        /// <summary>
        /// Handle the event when the graphics key has been clicked
        /// </summary>
        public async void OnNumericShiftKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol) || _isDown) return;

            e.Handled = true;
            _isDown = true;
            await ReleaseGraphicsMode();
            await WaitKeyReleased(keycontrol.Code, SpectrumKeyCode.SShift);
        }

        /// <summary>
        /// Handle the event when the key has been released
        /// </summary>
        public void OnKeyReleased(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            e.Handled = true;
        }

        /// <summary>
        /// Waits while the currently pressed key is released
        /// </summary>
        /// <param name="code">Primary key code</param>
        /// <param name="secondaryCode">Secondary key code</param>
        private async Task WaitKeyReleased(SpectrumKeyCode code, SpectrumKeyCode? secondaryCode = null)
        {
            await Task.Delay(RefreshPeriod);
            do
            {
                QueueKeyStroke(2, code, secondaryCode);
                await Task.Delay(RefreshPeriod);
                
            } while (_isDown);
        }

        /// <summary>
        /// Enques an emulated key stroke
        /// </summary>
        /// <param name="time">Time given in framecounts</param>
        /// <param name="primaryCode">Primary key code</param>
        /// <param name="secondaryCode">Secondary key code</param>
        private void QueueKeyStroke(int time, SpectrumKeyCode primaryCode,
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
        /// Sets the keyboard in Graphics mode
        /// </summary>
        private async Task<bool> SetGraphicsMode()
        {
            if (!GetModeValue(out var modeVal)) return false;
            if ((modeVal & 0x02) != 0) return false;

            QueueKeyStroke(1, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
            await Task.Delay(RefreshPeriod*2);
            return true;
        }

        /// <summary>
        /// Remove the keyboard from Graphics mode
        /// </summary>
        private async Task<bool> ReleaseGraphicsMode()
        {
            await Task.Delay(RefreshPeriod);
            if (!GetModeValue(out var modeVal)) return false;
            if ((modeVal & 0x02) == 0) return false;

            QueueKeyStroke(1, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
            await Task.Delay(RefreshPeriod*2);
            return true;
        }

        /// <summary>
        /// Sets the keyboard in Exteded mode
        /// </summary>
        private async Task SetExtendedMode()
        {
            if (!GetModeValue(out var modeVal)) return;
            if ((modeVal & 0x01) != 0) return;

            QueueKeyStroke(2, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
            await Task.Delay(RefreshPeriod);
            QueueKeyStroke(2, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
            await Task.Delay(RefreshPeriod);
        }

        /// <summary>
        /// Gets the value of the MODE system variable
        /// </summary>
        /// <param name="modeValue">Value of the system variable</param>
        /// <returns>True, if value obtained; otherwise, false</returns>
        private bool GetModeValue(out ushort? modeValue)
        {
            modeValue = null;
            if (SpectNetPackage.Default.MachineViewModel.VmState != VmState.Running) return false;
            var memory = SpectrumVm.MemoryDevice.CloneMemory();
            var mode = SystemVariables.Get("MODE")?.Address;
            if (mode == null) return false;
            modeValue = memory[(ushort)mode];
            return true;
        }
    }
}