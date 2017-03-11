using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// 
    /// </summary>
    public class TzxHardwareInfoDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Signs that this block is not playable
        /// </summary>
        public override bool IsPlayable => false;

        /// <summary>
        /// Number of machines and hardware types for which info is supplied
        /// </summary>
        public byte HwCount { get; set; }

        /// <summary>
        /// List of machines and hardware
        /// </summary>
        public TzxHwInfo[] HwInfo { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x33;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            HwCount = reader.ReadByte();
            HwInfo = new TzxHwInfo[HwCount];
            for (var i = 0; i < HwCount; i++)
            {
                var hw = new TzxHwInfo();
                hw.ReadFrom(reader);
                HwInfo[i] = hw;
            }
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(HwCount);
            foreach (var hw in HwInfo)
            {
                hw.WriteTo(writer);
            }
        }
    }
}