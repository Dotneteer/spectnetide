using System.IO;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// provides TZX tape content
    /// </summary>
    public interface ITzxTapeContentProvider
    {
        /// <summary>
        /// Resets the tape content
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns></returns>
        BinaryReader GetTzxContent();
    }
}