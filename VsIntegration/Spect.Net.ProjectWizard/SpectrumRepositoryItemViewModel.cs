using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.ProjectWizard
{
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