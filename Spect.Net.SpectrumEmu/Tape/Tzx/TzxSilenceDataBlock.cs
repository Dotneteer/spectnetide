using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Pause (silence) or 'Stop the Tape' block
    /// </summary>
    public class TzxSilenceDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Duration of silence
        /// </summary>
        /// <remarks>
        /// This will make a silence (low amplitude level (0)) for a given time 
        /// in milliseconds. If the value is 0 then the emulator or utility should 
        /// (in effect) STOP THE TAPE, i.e. should not continue loading until 
        /// the user or emulator requests it.
        /// </remarks>
        public ushort Duration { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x20;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Duration = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Duration);
        }
    }
}