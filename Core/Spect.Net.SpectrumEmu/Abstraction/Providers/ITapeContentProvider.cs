using System.IO;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// provides TZX tape content
    /// </summary>
    public interface ITapeContentProvider: IVmComponentProvider
    {
        /// <summary>
        /// Tha tape set to load the content from
        /// </summary>
        string TapeSetName { get; set; }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        BinaryReader GetTapeContent();
    }
}