namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the current project settings
    /// </summary>
    public class Z80ProjectSettings
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