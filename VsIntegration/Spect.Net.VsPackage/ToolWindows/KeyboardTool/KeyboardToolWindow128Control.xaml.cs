using System;
using System.Windows.Controls;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for KeyboardToolWindow128Control.xaml
    /// </summary>
    public partial class KeyboardToolWindow128Control : 
        ISupportsMvvm<KeyboardToolViewModel>, 
        IDisposable
    {
        private KeyPressHandler _keyPressHandler;
        private readonly object _locker = new object();
        private Action _releaseAction;

        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public KeyboardToolViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(KeyboardToolViewModel vm)
        {
            DataContext = Vm = vm;
            _keyPressHandler?.Dispose();
            _keyPressHandler = new KeyPressHandler(vm.MachineViewModel.SpectrumVm);
        }

        public KeyboardToolWindow128Control()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                SetupKeys(Row1);
                SetupKeys(Row2);
                SetupKeys(Row3);
                SetupKeys(Row4);
                SetupKeys(Row5);
                EnterKey.MainKeyClicked += OnMainKeyClicked;
                EnterKey.KeyReleased += OnKeyReleased;
            };

            Unloaded += (s, e) =>
            {
                ReleaseKeys(Row1);
                ReleaseKeys(Row2);
                ReleaseKeys(Row3);
                ReleaseKeys(Row4);
                ReleaseKeys(Row5);
                EnterKey.MainKeyClicked -= OnMainKeyClicked;
                EnterKey.KeyReleased -= OnKeyReleased;
            };
        }

        private void SetupKeys(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Single128KeyControl key)
                {
                    key.MainKeyClicked += OnMainKeyClicked;
                    key.SymShiftKeyClicked += OnSymShiftKeyClicked;
                    key.ExtKeyClicked += OnExtKeyClicked;
                    key.ExtShiftKeyClicked += OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked += OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked += OnGraphicsControlKeyClicked;
                    key.KeyReleased += OnKeyReleased;
                }
                else if (child is Wide128KeyControl wideKey)
                {
                    wideKey.MainKeyClicked += OnMainKeyClicked;
                    wideKey.KeyReleased += OnKeyReleased;
                }
            }
        }

        private void ReleaseKeys(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Single128KeyControl key)
                {
                    key.MainKeyClicked -= OnMainKeyClicked;
                    key.SymShiftKeyClicked -= OnSymShiftKeyClicked;
                    key.ExtKeyClicked -= OnExtKeyClicked;
                    key.ExtShiftKeyClicked -= OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked -= OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked -= OnGraphicsControlKeyClicked;
                    key.KeyReleased -= OnKeyReleased;
                }
                else if (child is Wide128KeyControl wideKey)
                {
                    wideKey.MainKeyClicked -= OnMainKeyClicked;
                    wideKey.KeyReleased -= OnKeyReleased;
                }
            }
        }

        private void OnMainKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            e.Handled = true;
            lock (_locker) _releaseAction = null;
            _keyPressHandler.KeyDown(keycontrol.Code,
                keycontrol.SecondaryCode
                ?? (e.ChangedButton == MouseButton.Left
                    ? (SpectrumKeyCode?) null
                    : SpectrumKeyCode.CShift));
        }

        private void OnSymShiftKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            e.Handled = true;
            _keyPressHandler.KeyDown(keycontrol.Code, SpectrumKeyCode.SShift);
        }

        private async void OnExtKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            e.Handled = true;
            if (keycontrol.NumericMode)
            {
                _keyPressHandler.KeyDown(keycontrol.Code, SpectrumKeyCode.CShift);
            }
            else
            {
                _keyPressHandler.KeyDown(SpectrumKeyCode.CShift, SpectrumKeyCode.SShift);
                await _keyPressHandler.Delay();
                _keyPressHandler.KeyDown(keycontrol.Code);
            }
        }

        private async void OnExtShiftKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            e.Handled = true;
            _keyPressHandler.KeyDown(SpectrumKeyCode.CShift, SpectrumKeyCode.SShift);
            await _keyPressHandler.Delay();
            _keyPressHandler.KeyDown(keycontrol.Code, SpectrumKeyCode.SShift);
        }

        private async void OnNumericControlKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            e.Handled = true;
            QueueKeyStroke(2, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
            await _keyPressHandler.Delay();
            QueueKeyStroke(2, keycontrol.Code, e.ChangedButton == MouseButton.Left
                ? (SpectrumKeyCode?)null
                : SpectrumKeyCode.CShift);
        }

        private async void OnGraphicsControlKeyClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;
            e.Handled = true;
            QueueKeyStroke(2, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
            await _keyPressHandler.Delay();
            QueueKeyStroke(2, keycontrol.Code);
            await _keyPressHandler.Delay();
            QueueKeyStroke(2, SpectrumKeyCode.N9, SpectrumKeyCode.CShift);
        }

        private void OnKeyReleased(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IKeyCodeProvider keycontrol)) return;

            _keyPressHandler.KeyUp(keycontrol.Code, keycontrol.SecondaryCode);
            e.Handled = true;
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
            var spectrumVm = Vm?.MachineViewModel?.SpectrumVm;
            if (spectrumVm == null) return;

            var currentTact = spectrumVm.Cpu.Tacts;
            var lastTact = currentTact + spectrumVm.FrameTacts * time * spectrumVm.ClockMultiplier;

            Vm.MachineViewModel.SpectrumVm.KeyboardProvider.QueueKeyPress(
                new EmulatedKeyStroke(
                    currentTact,
                    lastTact,
                    primaryCode,
                    secondaryCode));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _keyPressHandler?.Dispose();
            Vm?.Dispose();
        }
    }
}
