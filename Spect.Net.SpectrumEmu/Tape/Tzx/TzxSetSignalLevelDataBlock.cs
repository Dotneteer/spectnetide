using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block sets the current signal level to the specified value (high or low).
    /// </summary>
    public class TzxSetSignalLevelDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Length of the block without these four bytes
        /// </summary>
        public uint Lenght { get; } = 1;

        /// <summary>
        /// Signal level (0=low, 1=high)
        /// </summary>
        public byte SignalLevel { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x2B;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            reader.ReadUInt32();
            SignalLevel = reader.ReadByte();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Lenght);
            writer.Write(SignalLevel);
        }
    }
}