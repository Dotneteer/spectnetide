using System.IO;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// loads TZX/TAP tape content.
    /// </summary>
    public interface ITapeLoadProvider : IVmComponentProvider
    {
        /// <summary>
        /// Gets a binary reader that provides tape content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from.</returns>
        BinaryReader GetTapeContent();
    }
}