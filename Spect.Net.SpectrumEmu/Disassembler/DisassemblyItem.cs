using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents the output of a single disassembly item
    /// </summary>
    public class DisassemblyItem
    {
        /// <summary>
        /// The memory address of the disassembled instruction
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Operation codes used for the disassembly
        /// </summary>
        public IList<byte> OpCodes { get; set; }

        /// <summary>
        /// Optional label name
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Z80 assembly instruction
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Optional comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Optional prefix comment
        /// </summary>
        public string PrefixComment { get; set; }

        /// <summary>
        /// Optional target address, if the instruction contains any
        /// </summary>
        public ushort? TargetAddress { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyItem(ushort address)
        {
            Address = address;
            OpCodes = new List<byte>();
            Label = null;
            Instruction = null;
            Comment = null;
            TargetAddress = null;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            var opCodesStr = string.Join(" ", OpCodes.Select(op => $"{op:X2}")).PadRight(12);
            sb.AppendFormat("{0:X4} {1} {2}{3}; {4}", 
                Address, 
                opCodesStr, 
                Label == null ? "    " : Label + ": ",
                Instruction,
                Comment ?? "");
            return sb.ToString();
        }
    }
}