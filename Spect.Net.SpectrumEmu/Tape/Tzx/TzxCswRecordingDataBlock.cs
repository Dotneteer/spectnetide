using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxCswRecordingDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Block length (without these four bytes)
        /// </summary>
        public uint BlockLength { get; set; }

        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PauseAfter { get; set; }

        /// <summary>
        /// Sampling rate
        /// </summary>
        public byte[] SamplingRate { get; set; }

        /// <summary>
        /// Compression type
        /// </summary>
        /// <remarks>
        /// 0x01=RLE, 0x02=Z-RLE
        /// </remarks>
        public byte CompressionType { get; set; }

        /// <summary>
        /// Number of stored pulses (after decompression, for validation purposes)
        /// </summary>
        public uint PulseCount { get; set; }

        /// <summary>
        /// CSW data, encoded according to the CSW file format specification
        /// </summary>
        public byte[] Data { get; set; }

        #region Overrides of TzxDataBlockBase

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x18;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            BlockLength = reader.ReadUInt32();
            PauseAfter = reader.ReadUInt16();
            SamplingRate = reader.ReadBytes(3);
            CompressionType = reader.ReadByte();
            PulseCount = reader.ReadUInt32();
            var length = (int)BlockLength - 4 /* PauseAfter*/ - 3 /* SamplingRate */ 
                - 1 /* CompressionType */ - 4 /* PulseCount */;
            Data = reader.ReadBytes(length);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(BlockLength);
            writer.Write(PauseAfter);
            writer.Write(SamplingRate);
            writer.Write(CompressionType);
            writer.Write(PulseCount);
            writer.Write(Data);
        }

        /// <summary>
        /// Override this method to check the content of the block
        /// </summary>
        public override bool IsValid => BlockLength == 4 + 3 + 1 + 4 + Data.Length;

        #endregion
    }
}