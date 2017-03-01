using GalaSoft.MvvmLight;
using Spect.Net.Z80Tests.ViewModels.Debug;

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
            };
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
