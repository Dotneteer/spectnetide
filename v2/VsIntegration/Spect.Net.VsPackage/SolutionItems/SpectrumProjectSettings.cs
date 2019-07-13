namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents the settings stored with a ZX Spectrum project
    /// </summary>
    public class SpectrumProjectSettings
    {
        /// <summary>
        /// The default tape file that is loaded in the Spectrum vm
        /// </summary>
        public string DefaultTapeFile { get; set; }

        /// <summary>
        /// The default annotation file to use
        /// </summary>
        public string DefaultAnnotationFile { get; set; }

        /// <summary>
        /// The default code file to start
        /// </summary>
        public string DefaultCodeFile { get; set; }
    }
}