using System.Windows.Controls;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for KeyboardToolWindowControl.xaml
    /// </summary>
    public partial class KeyboardToolWindowControl: ISupportsMvvm<KeyboardToolViewModel>
    {
        private KeyPressHandler _keyPressHandler;

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
            _keyPressHandler = new KeyPressHandler();
        }

        public KeyboardToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                SetupKeys(Row1);
                SetupKeys(Row2);
                SetupKeys(Row3);
                SetupKeys(Row4);
            };

            Unloaded += (s, e) =>
            {
                ReleaseKeys(Row1);
                ReleaseKeys(Row2);
                ReleaseKeys(Row3);
                ReleaseKeys(Row4);
            };
        }

        private void SetupKeys(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is SingleKeyControl key)
                {
                    key.MainKeyClicked += _keyPressHandler.OnMainKeyClicked;
                    key.SymShiftKeyClicked += _keyPressHandler.OnSymShiftKeyClicked;
                    key.ExtKeyClicked += _keyPressHandler.OnExtKeyClicked;
                    key.ExtShiftKeyClicked += _keyPressHandler.OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked += _keyPressHandler.OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked += _keyPressHandler.OnGraphicsControlKeyClicked;
                    key.KeyReleased += _keyPressHandler.OnKeyReleased;
                }
            }
        }

        private void ReleaseKeys(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is SingleKeyControl key)
                {
                    key.MainKeyClicked -= _keyPressHandler.OnMainKeyClicked;
                    key.SymShiftKeyClicked -= _keyPressHandler.OnSymShiftKeyClicked;
                    key.ExtKeyClicked -= _keyPressHandler.OnExtKeyClicked;
                    key.ExtShiftKeyClicked -= _keyPressHandler.OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked -= _keyPressHandler.OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked -= _keyPressHandler.OnGraphicsControlKeyClicked;
                    key.KeyReleased -= _keyPressHandler.OnKeyReleased;
                }
            }
        }

        //private void OnMainKeyClicked(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is SingleKeyControl keycontrol)
        //    {
        //        QueueKeyStroke(5, keycontrol.Code, e.ChangedButton == MouseButton.Left
        //            ? (SpectrumKeyCode?)null
        //            : SpectrumKeyCode.CShift);
        //    }
        //    e.Handled = true;
        //}

        //private void OnSymShiftKeyClicked(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is SingleKeyControl keycontrol)
        //    {
        //        QueueKeyStroke(5, keycontrol.Code, SpectrumKeyCode.SShift);
        //    }
        //    e.Handled = true;
        //}

        //private void OnExtKeyClicked(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is SingleKeyControl keycontrol)
        //    {
        //        if (keycontrol.NumericMode)
        //        {
        //            QueueKeyStroke(5, keycontrol.Code, SpectrumKeyCode.CShift);
        //        }
        //        else
        //        {
        //            QueueKeyStroke(3, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
        //            QueueKeyStroke(5, keycontrol.Code);
        //        }
        //    }
        //    e.Handled = true;
        //}

        //private void OnExtShiftKeyClicked(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is SingleKeyControl keycontrol)
        //    {
        //        QueueKeyStroke(3, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
        //        QueueKeyStroke(5, keycontrol.Code, SpectrumKeyCode.SShift);
        //    }
        //    e.Handled = true;
        //}

        //private void OnNumericControlKeyClicked(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is SingleKeyControl keycontrol)
        //    {
        //        QueueKeyStroke(3, SpectrumKeyCode.SShift, SpectrumKeyCode.CShift);
        //        QueueKeyStroke(5, keycontrol.Code, e.ChangedButton == MouseButton.Left
        //            ? (SpectrumKeyCode?)null
        //            : SpectrumKeyCode.CShift);
        //    }
        //    e.Handled = true;
        //}

        ///// <summary>
        ///// Enques an emulated key stroke
        ///// </summary>
        ///// <param name="time">Time given in framecounts</param>
        ///// <param name="primaryCode">Primary key code</param>
        ///// <param name="secondaryCode">Secondary key code</param>
        //private void QueueKeyStroke(int time, SpectrumKeyCode primaryCode, 
        //    SpectrumKeyCode? secondaryCode = null)
        //{
        //    var spectrumVm = Vm?.MachineViewModel?.SpectrumVm;
        //    if (spectrumVm == null) return;

        //    var currentTact = spectrumVm.Cpu.Tacts;
        //    var lastTact = currentTact + spectrumVm.FrameTacts * time * spectrumVm.ClockMultiplier;

        //    Vm.MachineViewModel.SpectrumVm.KeyboardProvider.QueueKeyPress(
        //        new EmulatedKeyStroke(
        //            currentTact, 
        //            lastTact, 
        //            primaryCode, 
        //            secondaryCode));
        //}
    }
}
