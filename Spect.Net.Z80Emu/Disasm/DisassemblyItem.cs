using System;
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
        /// <summary>
        /// The memory address of the disassembled instruction
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Operation codes used for the disassembly
        /// </summary>
        public IList<byte> OpCodes { get; internal set; }

        /// <summary>
        /// Optional label name
        /// </summary>
        public string Label { get; internal set; }

        /// <summary>
        /// The Z80 assembly instruction
        /// </summary>
        public string Instruction { get; internal set; }

        /// <summary>
        /// Optional comment
        /// </summary>
        public string Comment { get; internal set; }

        /// <summary>
        /// Optional target address, if the instruction contains any
        /// </summary>
        public ushort? TargetAddress { get; internal set; }

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

        public string AddressFormatted => $"{Address:X4}";
        public string OpCodesFormatted => string.Join(" ", OpCodes.Select(op => $"{op:X2}")).PadRight(12);
        public string LabelFormatted => Label == null ? string.Empty : Label + ":";
        public string CommentFormatted => Comment == null ? string.Empty : "; " + Comment;

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