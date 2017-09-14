using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxPulseSequenceDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Pause after this block
        /// </summary>
        public byte PulseCount { get; set; }

        /// <summary>
        /// Lenght of block data
        /// </summary>
        public ushort[] PulseLengths { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x13;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            PulseCount = reader.ReadByte();
            PulseLengths = ReadWords(reader, PulseCount);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PulseCount);
            WriteWords(writer, PulseLengths);
        }

        /// <summary>
        /// Override this method to check the content of the block
        /// </summary>
        public override bool IsValid => PulseCount == PulseLengths.Length;
    }
}