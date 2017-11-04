using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.SpectrumControl;
using MachineViewModel = Spect.Net.WpfClient.Machine.MachineViewModel;

namespace Spect.Net.WpfClient
{
    /// <summary>
    /// This class represents the design time version view model of the app
    /// </summary>
    /// <remarks>
    /// The sole aim of this class is to provide a design time help for the
    /// XAML editor to carry out data binding
    /// </remarks>

    public class DesignTimeAppViewModel: EnhancedViewModelBase
    {
        /// <summary>
        /// Hides the constructor from external actors
        /// </summary>
        public DesignTimeAppViewModel()
        {
            MachineViewModel = new MachineViewModel();
        }

        /// <summary>
        /// Contains the view model used by Spectrum control
        /// </summary>
        public MachineViewModel MachineViewModel { get; }
    }
}