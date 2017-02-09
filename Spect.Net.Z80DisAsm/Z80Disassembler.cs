using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class is is responsible for disassembling Z80 binary code
    /// </summary>
    public partial class Z80Disassembler
    {
        private ushort _offset;
        private ushort _opOffset;
        private IList<byte> _currentOpCodes;
        private byte? _displacement = null;

        /// <summary>
        /// The project to disassemble
        /// </summary>
        public Z80DisAsmProject Project { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80Disassembler(Z80DisAsmProject project)
        {
            Project = project;
        }

        /// <summary>
        /// Executes the disassembly process
        /// </summary>
        /// <param name="startFrom">The index to start disassembly from</param>
        /// <param name="length">The length of code to disassemble</param>
        /// <returns></returns>
        public Z80DisAsmOutput Disassemble(ushort startFrom = 0, ushort length = 0)
        {
            var output = new Z80DisAsmOutput();
            _offset = startFrom;
            var codeLength = Project.Z80Binary.Length;
            if (length > 0 && length < Project.Z80Binary.Length)
            {
                codeLength = length;
            }

            while (_offset < codeLength)
            {
                var item = DisassembleOperation();
                if (item != null)
                {
                    output.OutputItems.Add(item);
                }
            }
            return output;
        }

        /// <summary>
        /// Disassembles a single instruction
        /// </summary>
        private DisassemblyItem DisassembleOperation()
        {
            _opOffset = _offset;
            _currentOpCodes = new List<byte>();
            _displacement = null;

            AsmInstructionBase decodeInfo;
            var indexMode = 0; // No index
            var address = (ushort)(_offset + Project.StartOffset);
            var opCode = Fetch();

            if (opCode == 0xED)
            {
                decodeInfo = s_ExtendedInstructions.GetInstruction(Fetch());
            }
            else if (opCode == 0xCB)
            {
                decodeInfo = s_BitInstructions.GetInstruction(Fetch());
            }
            else if (opCode == 0xDD)
            {
                indexMode = 1; // IX
                decodeInfo = DisassembleIndexedOperation();
            }
            else if (opCode == 0xFD)
            {
                indexMode = 2; // IY
                decodeInfo = DisassembleIndexedOperation();
            }

            decodeInfo = s_StandardInstructions.GetInstruction(opCode);
            return DecodeInstruction(address, decodeInfo, indexMode, _displacement);
        }

        private AsmInstructionBase DisassembleIndexedOperation()
        {
            var opCode = Fetch();
            if (opCode != 0xCB) return s_IndexedInstructions.GetInstruction(opCode);
            _displacement = Fetch();
            return s_IndexedBitInstructions.GetInstruction(Fetch());
        }

        private byte Fetch()
        {
            var opCode = Project.Z80Binary[_offset++];
            _currentOpCodes.Add(opCode);
            return opCode;
        }

        private DisassemblyItem DecodeInstruction(ushort address, AsmInstructionBase opInfo, int indexMode = 0,
            byte? displacement = null)
        {
            return new DisassemblyItem(address, _currentOpCodes, null, opInfo?.InstructionPattern ?? "<none>");
        }
    }
}