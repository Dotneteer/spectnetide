namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the configuration of a Spectrum project
    /// </summary>
    public class SpectrumProjectConfiguration
    {
        /// <summary>
        /// Command to run during the pre-build event
        /// </summary>
        public string PreBuildCommand { get; set; }

        /// <summary>
        /// Command to run during the post-build event
        /// </summary>
        public string PostBuildCommand { get; set; }

        /// <summary>
        /// Command to run during the pre-build event
        /// </summary>
        public string BuildErrorCommand { get; set; }
    }
}