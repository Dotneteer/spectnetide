using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This blocks contains information about the hardware that the programs on this tape use.
    /// </summary>
    public class TzxHwInfo: ITzxSerialization
    {
        /// <summary>
        /// Hardware type
        /// </summary>
        public byte HwType { get; set; }

        /// <summary>
        /// Hardwer Id
        /// </summary>
        public byte HwId { get; set; }

        /// <summary>
        /// Information about the tape
        /// </summary>
        /// <remarks>
        /// 00 - The tape RUNS on this machine or with this hardware,
        ///      but may or may not use the hardware or special features of the machine.
        /// 01 - The tape USES the hardware or special features of the machine,
        ///      such as extra memory or a sound chip.
        /// 02 - The tape RUNS but it DOESN'T use the hardware
        ///      or special features of the machine.
        /// 03 - The tape DOESN'T RUN on this machine or with this hardware.
        /// </remarks>
        public byte TapeInfo;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public void ReadFrom(BinaryReader reader)
        {
            HwType = reader.ReadByte();
            HwId = reader.ReadByte();
            TapeInfo = reader.ReadByte();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(HwType);
            writer.Write(HwId);
            writer.Write(TapeInfo);
        }
    }
}