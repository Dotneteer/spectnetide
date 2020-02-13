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
        public string PreBuild { get; set; }

        /// <summary>
        /// Command to run during the post-build event
        /// </summary>
        public string PostBuild { get; set; }

        /// <summary>
        /// Command to run during the pre-build event
        /// </summary>
        public string BuildError { get; set; }
    }
}