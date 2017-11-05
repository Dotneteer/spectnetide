using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class loads the TZX content from the Code Discovery project
    /// </summary>
    public class ProjectFileTapeContentProvider: VmComponentProviderBase, ITapeContentProvider
    {
        /// <summary>
        /// Tha tape set to load the content from
        /// </summary>
        public string TapeSetName { get; set; }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTapeContent()
        {
            var solution = VsxPackage.GetPackage<SpectNetPackage>().CodeDiscoverySolution;
            var filename = solution?.CurrentProject?.DefaultTapeItem.Filename
                    ?? solution?.CurrentTzxItem?.Filename 
                    ?? solution?.CurrentTapItem?.Filename;
            return filename == null 
                ? null 
                : new BinaryReader(File.OpenRead(filename));
        }
    }
}