using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// When this block is encountered, the tape will stop ONLY if 
    /// the machine is an 48K Spectrum.
    /// </summary>
    /// <remarks>
    /// This block is to be used for multiloading games that load one 
    /// level at a time in 48K mode, but load the entire tape at once 
    /// if in 128K mode. This block has no body of its own, but follows 
    /// the extension rule.
    /// </remarks>
    public class TzxStopTheTape48DataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Length of the block without these four bytes (0)
        /// </summary>
        public uint Lenght { get; } = 0;

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x2A;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            reader.ReadUInt32();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Lenght);
        }
    }
}