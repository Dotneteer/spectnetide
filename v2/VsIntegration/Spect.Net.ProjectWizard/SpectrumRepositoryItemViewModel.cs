namespace Spect.Net.ProjectWizard
{
    /// <summary>
    /// This class represents a spectrum repository item
    /// </summary>
    public class SpectrumRepositoryItemViewModel
    {
        /// <summary>
        /// The path of the model icon
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// The name of the model to display
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// The screen mode text to display
        /// </summary>
        public string ScreenMode { get; set; }

        /// <summary>
        /// The CPU mode to display
        /// </summary>
        public string CpuMode { get; set; }

        /// <summary>
        /// The Revision number to display
        /// </summary>
        public string RevisionNo { get; set; }

        /// <summary>
        /// The model key to find the item in the SpectrumModels repository
        /// </summary>
        public string ModelKey { get; set; }

        /// <summary>
        /// The revision key to find the item in the SpectrumModels repository
        /// </summary>
        public string RevisionKey { get; set; }
    }
}