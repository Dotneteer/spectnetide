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
        /// The last address that belongs to the operation
        /// </summary>
        public ushort LastAddress { get; set; }

        /// <summary>
        /// Operation codes used for the disassembly
        /// </summary>
        public string OpCodes { get; set; }

        /// <summary>
        /// Indicates that the disassembly instruction has an associated label
        /// </summary>
        public bool HasLabel { get; set; }

        /// <summary>
        /// The Z80 assembly instruction
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Disassembler-generated comment
        /// </summary>
        public string HardComment { get; set; }

        /// <summary>
        /// Optional target address, if the instruction contains any
        /// </summary>
        public ushort TargetAddress { get; set; }

        /// <summary>
        /// The start position of token to replace
        /// </summary>
        public int TokenPosition { get; set; }

        /// <summary>
        /// The lenght of token to replace
        /// </summary>
        public int TokenLength { get; set; }

        /// <summary>
        /// Signs that this item has a symbol that can be associated with a literal
        /// </summary>
        public bool HasSymbol { get; set; }

        /// <summary>
        /// The symbol value
        /// </summary>
        public ushort SymbolValue { get; set; }

        /// <summary>
        /// Indicates if this item has a label symbol
        /// </summary>
        public bool HasLabelSymbol { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyItem(ushort address)
        {
            Address = address;
            LastAddress = address;
            OpCodes = string.Empty;
            Instruction = null;
            TargetAddress = 0;
            HardComment = null;
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
            sb.AppendFormat("{0:X4} {1} {2}{3}", 
                Address, 
                OpCodes, 
                HasLabel ? $"L{Address:X4}:" : "",
                Instruction);
            return sb.ToString();
        }
    }
}