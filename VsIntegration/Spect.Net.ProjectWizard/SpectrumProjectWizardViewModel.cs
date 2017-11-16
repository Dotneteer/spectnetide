using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.ProjectWizard
{
    /// <summary>
    /// This view model represents the UI of the Spectrum project wizard
    /// </summary>
    public class SpectrumProjectWizardViewModel: EnhancedViewModelBase
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
            if (!IsInDesignMode) return;

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
        }
    }

    /// <summary>
    /// This class represents a spectrum repository item
    /// </summary>
    public class SpectrumRepositoryItemViewModel : EnhancedViewModelBase
    {
        private string _modelName;
        private string _screenMode;
        private string _cpuMode;
        private string _revisionNo;
        private string _modelKey;
        private string _revisionKey;

        /// <summary>
        /// The name of the model to display
        /// </summary>
        public string ModelName
        {
            get => _modelName;
            set => Set(ref _modelName, value);
        }

        /// <summary>
        /// The screen mode text to display
        /// </summary>
        public string ScreenMode
        {
            get => _screenMode;
            set => Set(ref _screenMode, value);
        }

        /// <summary>
        /// The CPU mode to display
        /// </summary>
        public string CpuMode
        {
            get => _cpuMode;
            set => Set(ref _cpuMode, value);
        }

        /// <summary>
        /// The Revision number to display
        /// </summary>
        public string RevisionNo
        {
            get => _revisionNo;
            set => Set(ref _revisionNo, value);
        }

        /// <summary>
        /// The model key to find the item in the SpectrumModels repository
        /// </summary>
        public string ModelKey
        {
            get => _modelKey;
            set => Set(ref _modelKey, value);
        }

        /// <summary>
        /// The revision key to find the item in the SpectrumModels repository
        /// </summary>
        public string RevisionKey
        {
            get => _revisionKey;
            set => Set(ref _revisionKey, value);
        }
    }
}