namespace Spect.Net.Z80Tests.ViewModels
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
            SectionType = sectionType;
        }
    }
}