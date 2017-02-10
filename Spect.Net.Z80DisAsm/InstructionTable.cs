using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class represents a table of instruction deconding information.
    /// </summary>
    public class InstructionTable
    {
        private readonly Dictionary<byte, SimpleInstruction> _simpleInstructions;
        private readonly List<MaskedInstruction> _maskedInstructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public InstructionTable(IList<AsmInstructionBase> instructions)
        {
            _simpleInstructions = instructions
                .Where(instr => instr is SimpleInstruction)
                .Cast<SimpleInstruction>()
                .ToDictionary(instr => instr.OpCode);
            _maskedInstructions = instructions
                .Where(instr => instr is MaskedInstruction)
                .Cast<MaskedInstruction>().ToList();
        }

        /// <summary>
        /// Gets the instruction decoding information for the 
        /// specified <paramref name="opCode"/>
        /// </summary>
        /// <param name="opCode">
        /// Instruction information, if found; otherwise, null.
        /// </param>
        /// <returns></returns>
        public AsmInstructionBase GetInstruction(byte opCode)
        {
            SimpleInstruction simple;

            _simpleInstructions.TryGetValue(opCode, out simple);
            var masked = _maskedInstructions.FirstOrDefault(mi => (opCode & mi.Mask) == mi.OpCode);
            var result = simple != null ? (AsmInstructionBase) simple : masked;
            return result?.InstructionPattern == null ? null : result;
        }
    }
}