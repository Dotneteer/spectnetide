using System.Reflection;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface defines the responsibility of a ROM provider
    /// </summary>
    public interface IRomProvider: IVmComponentProvider
    {
        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <param name="asm">
        /// Assembly to check for the resource. If null, the calling assembly
        /// is used
        /// </param>
        /// <returns>Content of the ROM</returns>
        byte[] LoadRom(string romResourceName, Assembly asm = null);
    }
}