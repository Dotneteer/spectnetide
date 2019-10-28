using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.VsPackage.Machines
{
    /// <summary>
    /// This class implements the tape load provider for a SpectNetIDE project
    /// </summary>
    public class VsIntegratedTapeLoadProvider: VmComponentProviderBase, ITapeLoadProvider
    {
        /// <summary>
        /// Gets a binary reader that provides tape content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from.</returns>
        public BinaryReader GetTapeContent()
        {
            var solution = SpectNetPackage.Default.Solution;
            var filename = solution?.ActiveProject?.DefaultTapeItem?.Filename
                           ?? solution?.ActiveTzxItem?.Filename
                           ?? solution?.ActiveTapItem?.Filename;
            return filename == null
                ? null
                : new BinaryReader(File.OpenRead(filename));
        }
    }
}