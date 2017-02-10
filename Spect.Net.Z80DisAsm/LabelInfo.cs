using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class describes a label with its references
    /// </summary>
    public class LabelInfo
    {
        /// <summary>
        /// Label name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Label address
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// Addresses of instructions that reference this label
        /// </summary>
        public IList<ushort> References { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LabelInfo(string name, ushort address)
        {
            Name = name;
            Address = address;
            References = new List<ushort>();
        }
    }
}