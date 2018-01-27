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
            ProjectModels = new ObservableCollection<SpectrumRepositoryItemViewModel>
            {
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrum48.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                    RevisionKey = SpectrumModels.PAL,
                    ModelName = "ZX Spectrum 48K",
                    ScreenMode = "PAL",
                    CpuMode = "Normal Speed",
                    RevisionNo = "1.0"
                },
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrum48.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                    RevisionKey = SpectrumModels.NTSC,
                    ModelName = "ZX Spectrum 48K",
                    ScreenMode = "NTSC",
                    CpuMode = "Normal Speed",
                    RevisionNo = "1.0"
                },
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrum48.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                    RevisionKey = SpectrumModels.PAL_2_X,
                    ModelName = "ZX Spectrum 48K",
                    ScreenMode = "PAL",
                    CpuMode = "Turbo x2",
                    RevisionNo = "1.0"
                },
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrum48.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_48,
                    RevisionKey = SpectrumModels.NTSC_2_X,
                    ModelName = "ZX Spectrum 48K",
                    ScreenMode = "NTSC",
                    CpuMode = "Turbo x2",
                    RevisionNo = "1.0"
                },
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrum128.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_128,
                    RevisionKey = SpectrumModels.PAL,
                    ModelName = "ZX Spectrum 128K",
                    ScreenMode = "PAL",
                    CpuMode = "Normal Speed",
                    RevisionNo = "1.0"
                },
                new SpectrumRepositoryItemViewModel
                {
                    IconPath = "Images/Spectrump3e.ico",
                    ModelKey = SpectrumModels.ZX_SPECTRUM_P3_E,
                    RevisionKey = SpectrumModels.PAL,
                    ModelName = "ZX Spectrum +3E",
                    ScreenMode = "PAL",
                    CpuMode = "Normal Speed",
                    RevisionNo = "0.6 (beta)"
                }
            };

            SelectedItem = ProjectModels[0];
        }
    }
}