using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.Keyboard
{
    /// <summary>
    /// This class is responsible for handling the key events
    /// </summary>
    public class KeyPressHandler
    {
        private volatile bool _isDown;

        /// <summary>
        /// Get the machine keyboard
        /// </summary>
        public KeyboardEmulator Keyboard => SpectNetPackage.Default.EmulatorViewModel.Machine.Keyboard;

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
            await SetExtendedModeAsync();
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
            await SetExtendedModeAsync();
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
            await SetExtendedModeAsync();
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
            var set = await SetGraphicsModeAsync();
            await WaitKeyReleased(keycontrol.Code);
            if (set)
            {
                await ReleaseGraphicsModeAsync();
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
            await ReleaseGraphicsModeAsync();
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
            var refreshPeriod = Keyboard.RefreshPeriod;
            await Task.Delay(refreshPeriod);
            do
            {
                Keyboard.QueueKeyStroke(2, code, secondaryCode);
                await Task.Delay(refreshPeriod);
                
            } while (_isDown);

            // --- Put the focus back to the ZX Spectrum emulator tool window
            var emuWindow = SpectNetPackage.Default.GetToolWindow<SpectrumEmulator.SpectrumEmulatorToolWindow>();
            if (emuWindow?.IsVisible == true)
            {
                var frame = (IVsWindowFrame)emuWindow.Frame;
                frame.Show();
            }
        }

        /// <summary>
        /// Sets the keyboard in Graphics mode
        /// </summary>
        private async Task<bool> SetGraphicsModeAsync()
        {
            if (!GetModeValue(out var modeVal)) return false;
            if ((modeVal & 0x02) != 0) return false;

            Keyboard.QueueKeyStroke(2, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
            await Task.Delay(Keyboard.RefreshPeriod*2);
            return true;
        }

        /// <summary>
        /// Remove the keyboard from Graphics mode
        /// </summary>
        private async Task ReleaseGraphicsModeAsync()
        {
            await Task.Delay(Keyboard.RefreshPeriod);
            if (!GetModeValue(out var modeVal)) return;
            if ((modeVal & 0x02) == 0) return;

            Keyboard.QueueKeyStroke(2, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
            await Task.Delay(Keyboard.RefreshPeriod*2);
        }

        /// <summary>
        /// Sets the keyboard in Extended mode
        /// </summary>
        private async Task SetExtendedModeAsync()
        {
            if (!GetModeValue(out var modeVal)) return;
            if ((modeVal & 0x01) != 0) return;

            Keyboard.QueueKeyStroke(2, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
            await Task.Delay(Keyboard.RefreshPeriod);
            Keyboard.QueueKeyStroke(2, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
            await Task.Delay(Keyboard.RefreshPeriod);
        }

        /// <summary>
        /// Gets the value of the MODE system variable
        /// </summary>
        /// <param name="modeValue">Value of the system variable</param>
        /// <returns>True, if value obtained; otherwise, false</returns>
        private bool GetModeValue(out ushort? modeValue)
        {
            modeValue = null;
            return SpectNetPackage.Default.EmulatorViewModel.MachineState == VmState.Running 
                && Keyboard.GetModeValue(out modeValue);
        }
    }
}