namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class describes a disasembly data section
    /// </summary>
    public class DisassemblyDataSection
    {
        /// <summary>
        /// Starting address (inclusive)
        /// </summary>
        public ushort FromAddr { get; }

        /// <summary>
        /// Ending address (inclusive)
        /// </summary>
        public ushort ToAddr { get; }

        public DataSectionType SectionType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DisassemblyDataSection(ushort fromAddr, ushort toAddr, DataSectionType sectionType)
        {
            FromAddr = fromAddr;
            ToAddr = toAddr;

            // --- If we have odd number of bytes, words should be converted to bytes
            SectionType = (toAddr - fromAddr + 1) % 2 == 1 && sectionType == DataSectionType.Word
                ? DataSectionType.Byte
                : sectionType;
        }

        /// <summary>
        /// Tests is this section contains the provided address.
        /// </summary>
        /// <param name="addr">Address to check</param>
        /// <returns>
        /// True, if the address is contained within the section; otherwise, false
        /// </returns>
        public bool ContainsAddress(ushort addr)
        {
            return addr >= FromAddr && addr <= ToAddr;
        }

        /// <summary>
        /// Tests whether this section intersects with the provided range
        /// </summary>
        /// <param name="otherFrom">Start of the other range</param>
        /// <param name="otherTo">End of the other range</param>
        /// <returns>
        /// True, if the range intersects with the section; otherwise, false
        /// </returns>
        public bool Intersects(ushort otherFrom, ushort otherTo)
        {
            return otherFrom >= FromAddr && otherFrom <= ToAddr
                   || otherTo >= FromAddr && otherTo <= ToAddr
                   || FromAddr >= otherFrom && FromAddr <= otherTo
                   || ToAddr >= otherFrom && ToAddr <= otherTo;
        }

        /// <summary>
        /// Tests whether this section intersects with the other section
        /// </summary>
        /// <param name="other">Other section</param>
        /// <returns>
        /// True, if the other section intersects with this one; otherwise, false
        /// </returns>
        public bool Intersects(DisassemblyDataSection other)
        {
            return Intersects(other.FromAddr, other.ToAddr);
        }

        /// <summary>
        /// Tests whether this section contains the specified range
        /// </summary>
        /// <param name="otherFrom">Start of the other range</param>
        /// <param name="otherTo">End of the other range</param>
        /// <returns>
        /// True, if this section entirely contains the range; otherwise, false
        /// </returns>
        public bool ContainsSection(ushort otherFrom, ushort otherTo)
        {
            return otherFrom >= FromAddr && otherFrom <= ToAddr
                   && otherTo >= FromAddr && otherTo <= ToAddr;
        }

        /// <summary>
        /// Tests whether this section contains the specified range
        /// </summary>
        /// <param name="other">Other section</param>
        /// <returns>
        /// True, if this section entirely contains the other one; otherwise, false
        /// </returns>
        public bool ContainsSection(DisassemblyDataSection other)
        {
            return ContainsSection(other.FromAddr, other.ToAddr);
        }
    }
}