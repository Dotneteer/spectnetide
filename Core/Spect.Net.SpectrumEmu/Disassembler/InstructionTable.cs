using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents a table of instruction deconding information.
    /// </summary>
    public class InstructionTable
    {
        private readonly Dictionary<byte, SingleOperationMap> _simpleInstructions;
        private readonly List<MaskedOperationMap> _maskedInstructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public InstructionTable(IList<OperationMapBase> instructions)
        {
            _simpleInstructions = instructions
                .Where(instr => instr is SingleOperationMap)
                .Cast<SingleOperationMap>()
                .ToDictionary(instr => instr.OpCode);
            _maskedInstructions = instructions
                .Where(instr => instr is MaskedOperationMap)
                .Cast<MaskedOperationMap>().ToList();
        }

        /// <summary>
        /// Gets the instruction decoding information for the 
        /// specified <paramref name="opCode"/>
        /// </summary>
        /// <param name="opCode">
        /// Instruction information, if found; otherwise, null.
        /// </param>
        /// <returns></returns>
        public OperationMapBase GetInstruction(byte opCode)
        {
            _simpleInstructions.TryGetValue(opCode, out var simple);
            var masked = _maskedInstructions.FirstOrDefault(mi => (opCode & mi.Mask) == mi.OpCode);
            var result = simple != null ? (OperationMapBase) simple : masked;
            return result?.InstructionPattern == null ? null : result;
        }
    }
}