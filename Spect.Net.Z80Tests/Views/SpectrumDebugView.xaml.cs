using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Z80Tests.ViewModels.SpectrumEmu;

namespace Spect.Net.Z80Tests.Views
{
    /// <summary>
    /// Interaction logic for SpectrumDebugView.xaml
    /// </summary>
    public partial class SpectrumDebugView
    {
        public SpectrumDebugViewModel Vm { get; set; }

        public SpectrumDebugView()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic) return;

            Loaded += (sender, args) =>
            {
                Vm = DataContext as SpectrumDebugViewModel;
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                Messenger.Default.Register<SpectrumVmExecCycleCompletedMessage>(this, OnExecutionCycleCompleted);
            };
        }

        private void OnExecutionCycleCompleted(SpectrumVmExecCycleCompletedMessage obj)
        {
            DisassemblyList.ScrollTo(Vm.SpectrumVm.Cpu.Registers.PC);
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            SpectrumControl.ProcessKeyDown(e.Key);
        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            SpectrumControl.ProcessKeyUp(e.Key);
        }
    }
}
