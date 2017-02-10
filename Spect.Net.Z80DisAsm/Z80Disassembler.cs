using System;
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
        private byte? _displacement;
        private byte _opCode;

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
                    output.AddItem(item);
                }
            }

            LabelFixup(output);
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
            _opCode =Fetch();

            if (_opCode == 0xED)
            {
                _opCode = Fetch();
                decodeInfo = s_ExtendedInstructions.GetInstruction(_opCode);
            }
            else if (_opCode == 0xCB)
            {
                _opCode = Fetch();
                decodeInfo = s_BitInstructions.GetInstruction(_opCode);
            }
            else if (_opCode == 0xDD)
            {
                indexMode = 1; // IX
                _opCode = Fetch();
                decodeInfo = DisassembleIndexedOperation();
            }
            else if (_opCode == 0xFD)
            {
                indexMode = 2; // IY
                _opCode = Fetch();
                decodeInfo = DisassembleIndexedOperation();
            }
            else
            {
                decodeInfo = s_StandardInstructions.GetInstruction(_opCode);
            }
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
            var value = Project.Z80Binary[_offset++];
            _currentOpCodes.Add(value);
            return value;
        }

        private ushort FetchWord()
        {
            var l = Fetch();
            var h = Fetch();
            return (ushort)(h << 8 | l);
        }

        private DisassemblyItem DecodeInstruction(ushort address, AsmInstructionBase opInfo, int indexMode = 0,
            byte? displacement = null)
        {
            if (opInfo != null)
            {
                var instruction = opInfo.InstructionPattern;
                var pragmaIndex = instruction.IndexOf("#", StringComparison.Ordinal);
                if (pragmaIndex >= 0)
                {
                    instruction = ProcessPragma(opInfo, instruction, pragmaIndex);
                    pragmaIndex = instruction.IndexOf("#", StringComparison.Ordinal);
                    if (pragmaIndex >= 0)
                    {
                        instruction = ProcessPragma(opInfo, instruction, pragmaIndex);
                    }
                    return new DisassemblyItem(address, _currentOpCodes, instruction);
                }
            }
            return new DisassemblyItem(address, _currentOpCodes, opInfo?.InstructionPattern ?? "<none>");
        }

        private string ProcessPragma(AsmInstructionBase opInfo, string instruction, int pragmaIndex)
        {
            if (pragmaIndex >= instruction.Length)
            {
                return instruction;
            }

            var pragma = instruction[pragmaIndex + 1];
            var replacement = "";
            switch (pragma)
            {
                case '8':
                    // --- #8: 8-bit value defined on bit 3, 4 and 5 ($00, $10, ..., $38)
                    var val = (byte)(_opCode & 0x38);
                    replacement = ByteToString(val);
                    break;
                case 'b':
                    // --- #b: bit index defined on bit 3, 4 and 5 in bit operations
                    var bit = (byte)((_opCode & 0x38) >> 3);
                    replacement = bit.ToString();
                    break;
                case 'r':
                    // --- #r: relative label (8 bit offset)
                    var distance = Fetch();
                    replacement = GetLabelFor((ushort)(_opOffset + 2 + (sbyte) distance));
                    break;
                case 'L':
                    // --- #L: absolute label (16 bit address)
                    replacement = GetLabelFor(FetchWord());
                    break;
                case 'q':
                    // --- #q: 8-bit registers named on bit 3, 4 and 5 (B, C, ..., (HL), A)
                    var regqIndex = (_opCode & 0x38) >> 3;
                    replacement = s_Q8Regs[regqIndex];
                    break;
                case 's':
                    // --- #q: 8-bit registers named on bit 0, 1 and 2 (B, C, ..., (HL), A)
                    var regsIndex = _opCode & 0x07;
                    replacement = s_Q8Regs[regsIndex];
                    break;
                case 'Q':
                    // --- #Q: 16-bit register pair named on bit 4 and 5 (BC, DE, HL, SP)
                    var regQIndex = (_opCode & 0x30) >> 4;
                    replacement = s_Q16Regs[regQIndex];
                    break;
                case 'R':
                    // --- #Q: 16-bit register pair named on bit 4 and 5 (BC, DE, HL, AF)
                    var regRIndex = (_opCode & 0x30) >> 4;
                    replacement = s_R16Regs[regRIndex];
                    break;
                case 'B':
                    // --- #B: 8-bit value from the code
                    replacement = ByteToString(Fetch());
                    break;
                case 'W':
                    // --- #W: 16-bit word from the code
                    replacement = WordToString(FetchWord());
                    break;
            }

            if (replacement.Length > 0)
            {
                instruction = instruction.Substring(0, pragmaIndex) 
                    + replacement 
                    + instruction.Substring(pragmaIndex + 2);
            }
            return instruction;
        }

        private string GetLabelFor(ushort addr)
        {
            var labels = Project.LabelStore.Labels;
            LabelInfo label;
            if (!labels.TryGetValue(addr, out label))
            {
                label = Project.LabelStore.CreateLabel(addr);
            }
            label.References.Add(_opOffset);
            return label.Name;
        }

        private string ByteToString(byte value)
        {
            return $"${value:X2}";
        }

        private string WordToString(ushort value)
        {
            return $"${value:X4}";
        }

        /// <summary>
        /// Fixes the labels within the disassembly output
        /// </summary>
        /// <param name="output">Disassembly output</param>
        private void LabelFixup(Z80DisAsmOutput output)
        {
            var labels = Project.LabelStore.Labels;
            foreach (var labelAddr in labels.Keys)
            {
                var outputItem = output[labelAddr];
                if (outputItem != null && outputItem.Label == null)
                {
                    outputItem.Label = labels[labelAddr].Name;
                }
            }
        }
    }
}