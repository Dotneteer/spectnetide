using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class loads the TZX content from the Code Discovery project
    /// </summary>
    public class ProjectFileTzxLoadContentProvider: ITzxLoadContentProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Tha tape set to load the content from
        /// </summary>
        public string TapeSetName { get; set; }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTzxContent()
        {
            var tzx = VsxPackage.GetPackage<SpectNetPackage>().CurrentWorkspace?.TzxItem;
            return tzx == null 
                ? null 
                : new BinaryReader(File.OpenRead(tzx.Filename));
        }
    }
}