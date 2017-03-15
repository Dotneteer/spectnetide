using System.IO;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// provides TZX tape content
    /// </summary>
    public interface ITzxTapeContentProvider: IVmComponentProvider
    {
        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns></returns>
        BinaryReader GetTzxContent();
    }
}