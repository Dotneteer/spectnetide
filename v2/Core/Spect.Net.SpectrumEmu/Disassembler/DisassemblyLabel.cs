using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a label with its references
    /// </summary>
    public class DisassemblyLabel
    {
        /// <summary>
        /// Label address
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// Addresses of instructions that reference this label
        /// </summary>
        public IList<ushort> References { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyLabel(ushort address)
        {
            Address = address;
            References = new List<ushort>();
        }
    }
}