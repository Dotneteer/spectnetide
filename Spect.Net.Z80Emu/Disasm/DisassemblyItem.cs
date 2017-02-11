using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class represents the output of a single disassembly item
    /// </summary>
    public class DisassemblyItem
    {
        public ushort Address { get; }
        public IList<byte> OpCodes { get; }
        public string Label { get; internal set; }
        public string Instruction { get; internal set; }
        public string Comment { get; internal set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyItem(ushort address, IList<byte> opCodes, string instruction, string comment = null)
        {
            Address = address;
            OpCodes = opCodes;
            Label = null;
            Instruction = instruction;
            Comment = comment;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
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