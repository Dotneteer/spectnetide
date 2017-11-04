using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.WpfClient.SpectrumControl;

namespace Spect.Net.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = AppViewModel.Default;

            // --- We automatically start the machine when the ZX Spectrum control
            // --- is fully loaded and prepared, but not before
            Messenger.Default.Register(this, (SpectrumControlLoadedMessage msg) =>
            {
                SpectrumControl.Vm.FastTapeMode = false;
                SpectrumControl.Vm.StartVmCommand.Execute(null);
            });

            // --- We need to stop playing sound whenever the app closes
            Application.Current.Exit += (sender, obj) => SpectrumControl.Vm.EarBitFrameProvider.KillSound();
        }
    }
}
