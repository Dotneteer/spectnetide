using Spect.Net.SpectrumEmu.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// provides TZX tape content
    /// </summary>
    public interface ISaveToTapeProvider: IVmComponentProvider
    {
        /// <summary>
        /// Creates a tape file with the specified name
        /// </summary>
        /// <returns></returns>
        void CreateTapeFile();

        /// <summary>
        /// This method sets the name of the file according to the 
        /// Spectrum SAVE HEADER information
        /// </summary>
        /// <param name="name"></param>
        void SetName(string name);

        /// <summary>
        /// Appends the TZX block to the tape file
        /// </summary>
        /// <param name="block"></param>
        void SaveTapeBlock(ITapeDataSerialization block);

        /// <summary>
        /// The tape provider can finalize the tape when all 
        /// TZX blocks are written.
        /// </summary>
        void FinalizeTapeFile();
    }
}