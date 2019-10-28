using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors
{
    /// <summary>
    /// Represents the view model of the Spectrum configuration editor
    /// </summary>
    public class SpectrumConfEditorViewModel : EnhancedViewModelBase
    {
        private string _modelName;
        private string _editionName;
        private SpectrumEdition _configurationData;
        private ScreenConfiguration _extendedScreenConfig;

        /// <summary>
        /// The name of the current Spectrum model
        /// </summary>
        public string ModelName
        {
            get => _modelName;
            set => Set(ref _modelName, value);
        }

        /// <summary>
        /// The name of the current Spectrum edition
        /// </summary>
        public string EditionName
        {
            get => _editionName;
            set => Set(ref _editionName, value);
        }

        /// <summary>
        /// The detailed Spectrum configuration
        /// </summary>
        public SpectrumEdition ConfigurationData
        {
            get => _configurationData;
            set
            {
                if (Set(ref _configurationData, value))
                {
                    _extendedScreenConfig = new ScreenConfiguration(_configurationData.Screen);
                    RaisePropertyChanged(nameof(ExtendedScreenConfig));
                }
            }
        }

        /// <summary>
        /// Extended screen configuration
        /// </summary>
        public ScreenConfiguration ExtendedScreenConfig
        {
            get => _extendedScreenConfig;
            set => Set(ref _extendedScreenConfig, value);
        }
    }
}