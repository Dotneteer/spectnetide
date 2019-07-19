using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// saves tape contents.
    /// </summary>
    public interface ITapeSaveProvider : IVmComponentProvider
    {
        /// <summary>
        /// Creates a tape file with the specified name.
        /// </summary>
        /// <returns></returns>
        void CreateTapeFile();

        /// <summary>
        /// This method sets the name of the file according to the 
        /// Spectrum SAVE HEADER information.
        /// </summary>
        /// <param name="name"></param>
        void SetName(string name);

        /// <summary>
        /// Appends the tape block to the tape file.
        /// </summary>
        /// <param name="block">Tape block</param>
        void SaveTapeBlock(ITapeDataSerialization block);
    }
}