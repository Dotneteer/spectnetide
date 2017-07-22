using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This interface describes the behavior of an object that
    /// provides TZX tape content
    /// </summary>
    public interface ITzxSaveProvider: IVmComponentProvider
    {
        /// <summary>
        /// Creates a tape file with the specified name
        /// </summary>
        /// <param name="name">Program name, or null, if default file name should be used</param>
        /// <returns></returns>
        void CreateTapeFile(string name = null);

        /// <summary>
        /// Appends the TZX block to the tape file
        /// </summary>
        /// <param name="block"></param>
        void SaveTzxBlock(ITzxSerialization block);

        /// <summary>
        /// The tape provider can finalize the tape when all 
        /// TZX blocks are written.
        /// </summary>
        /// <param name="name">Optional name to rename the original tape file</param>
        void FinalizeTapeFile(string name = null);
    }
}