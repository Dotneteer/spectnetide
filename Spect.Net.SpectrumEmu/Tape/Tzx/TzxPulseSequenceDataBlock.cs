using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
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
        public ushort[] PulseLenghts { get; set; }

        #region Overrides of TzxDataBlockBase

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
            var lengths = reader.ReadBytes(2*PulseCount);
            for (var i = 0; i < PulseCount; i++)
            {
                PulseLenghts[i] = (ushort) (lengths[i*2] + lengths[i*2 + 1] << 8);
            }
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PulseCount);
            for (var i = 0; i < PulseCount; i++)
            {
                writer.Write(PulseLenghts[i]);
            }
        }

        /// <summary>
        /// Override this method to check the content of the block
        /// </summary>
        public override bool IsValid => PulseCount == PulseLenghts.Length;

        #endregion
    }
}