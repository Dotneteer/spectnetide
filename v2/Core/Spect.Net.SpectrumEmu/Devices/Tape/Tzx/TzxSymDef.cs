using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This block represents an extremely wide range of data encoding techniques.
    /// </summary>
    /// <remarks>
    /// The basic idea is that each loading component (pilot tone, sync pulses, data) 
    /// is associated to a specific sequence of pulses, where each sequence (wave) can 
    /// contain a different number of pulses from the others. In this way we can have 
    /// a situation where bit 0 is represented with 4 pulses and bit 1 with 8 pulses.
    /// </remarks>
    public class TzxSymDef: ITapeDataSerialization
    {
        /// <summary>
        /// Bit 0 - Bit 1: Starting symbol polarity
        /// </summary>
        /// <remarks>
        /// 00: opposite to the current level (make an edge, as usual) - default
        /// 01: same as the current level(no edge - prolongs the previous pulse)
        /// 10: force low level
        /// 11: force high level
        /// </remarks>
        public byte SymbolFlags;

        /// <summary>
        /// The array of pulse lengths
        /// </summary>
        public ushort[] PulseLengths;

        public TzxSymDef(byte maxPulses)
        {
            PulseLengths = new ushort[maxPulses];
        }

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public void ReadFrom(BinaryReader reader)
        {
            SymbolFlags = reader.ReadByte();
            PulseLengths = TzxDataBlockBase.ReadWords(reader, PulseLengths.Length);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(SymbolFlags);
            TzxDataBlockBase.WriteWords(writer, PulseLengths);
        }
    }
}