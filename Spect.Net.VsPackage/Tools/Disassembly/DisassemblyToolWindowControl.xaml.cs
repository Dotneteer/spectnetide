using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl
    {
        public DisassemblyViewModel Vm { get; }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new DisassemblyViewModel();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
            };
            PreviewKeyDown += (s, e) => DisassemblyList.HandleListViewKeyEvents(e);
        }

        /// <summary>
        /// Whenever the state of the Spectrum virtual machine changes,
        /// we refrehs the memory dump
        /// </summary>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (Vm.VmStopped)
                {
                    Vm.Clear();
                }
                else
                {
                    Vm.Disassemble();
                }
            });
        }
    }
}
