using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Defines the serialization operations of a TZX record
    /// </summary>
    public interface ITzxSerialization
    {
        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        void ReadFrom(BinaryReader reader);

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        void WriteTo(BinaryWriter writer);
    }
}