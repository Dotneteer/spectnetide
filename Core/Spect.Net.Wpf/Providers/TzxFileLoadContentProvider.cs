using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This provider reads TZX files from the file system
    /// </summary>
    public class TzxFileLoadContentProvider: ITapeContentProvider
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
        public BinaryReader GetTapeContent()
        {
            return new BinaryReader(File.OpenRead(TapeSetName));
        }
    }
}