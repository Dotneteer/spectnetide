using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.ProjectWizard
{
    /// <summary>
    /// This view model represents the UI of the Spectrum project wizard
    /// </summary>
    public class SpectrumProjectWizardViewModel: ViewModelBaseWithDesignTimeFix
    {
        private SpectrumRepositoryItemViewModel _selectedItem;

        /// <summary>
        /// The list of available models
        /// </summary>
        public ObservableCollection<SpectrumRepositoryItemViewModel> ProjectModels { get; }

        public SpectrumRepositoryItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        /// <summary>
        /// Initialize the list of models
        /// </summary>
        public SpectrumProjectWizardViewModel()
        {
            ProjectModels = new ObservableCollection<SpectrumRepositoryItemViewModel>();

            ProjectModels.Add(new SpectrumRepositoryItemViewModel
            {
                ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                RevisionKey = SpectrumModels.PAL,
                ModelName = "ZX Spectrum 48K",
                ScreenMode = "PAL",
                CpuMode = "Normal Speed",
                RevisionNo = "1.0"
            });
            ProjectModels.Add(new SpectrumRepositoryItemViewModel
            {
                ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                RevisionKey = SpectrumModels.PAL,
                ModelName = "ZX Spectrum 48K",
                ScreenMode = "PAL",
                CpuMode = "Turbo x2",
                RevisionNo = "1.0"
            });
            ProjectModels.Add(new SpectrumRepositoryItemViewModel
            {
                ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                RevisionKey = SpectrumModels.NTSC,
                ModelName = "ZX Spectrum 48K",
                ScreenMode = "NTSC",
                CpuMode = "Normal Speed",
                RevisionNo = "1.0"
            });
            ProjectModels.Add(new SpectrumRepositoryItemViewModel
            {
                ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                RevisionKey = SpectrumModels.NTSC,
                ModelName = "ZX Spectrum 48K",
                ScreenMode = "NTSC",
                CpuMode = "Turbo x2",
                RevisionNo = "1.0"
            });

            SelectedItem = ProjectModels[0];
        }
    }
}