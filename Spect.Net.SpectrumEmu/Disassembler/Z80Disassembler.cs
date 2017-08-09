using System;
using System.Collections.Generic;
using System.Text;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class is is responsible for disassembling Z80 binary code.
    /// </summary>
    /// <remarks>
    /// Another partion, in the Z80DisassemblerTables.cs declares the tables used
    /// for disassembling a project
    /// </remarks>
    public partial class Z80Disassembler
    {
        private DisassemblyOutput _output;
        private int _offset;
        private int _opOffset;
        private IList<byte> _currentOpCodes;
        private byte? _displacement;
        private byte _opCode;
        private int _indexMode;

        /// <summary>
        /// The project to disassemble
        /// </summary>
        public DisassembyAnnotations Annotations { get; }

        /// <summary>
        /// Gets the contents of the memory
        /// </summary>
        public byte[] MemoryContents { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80Disassembler(DisassembyAnnotations annotations, byte[] memoryContents)
        {
            Annotations = annotations;
            MemoryContents = memoryContents;
        }

        /// <summary>
        /// Disassembles the memory from the specified start address with the given endAddress
        /// </summary>
        /// <returns></returns>
        public DisassemblyOutput Disassemble(ushort startAddress = 0x0000, ushort endAddress = 0xFFFF)
        {
            _output = new DisassemblyOutput();
            var refSection = new MemorySection(startAddress, endAddress);

            // --- Let's go through the memory sections
            foreach (var section in Annotations.MemorySections)
            {
                if (!section.Overlaps(refSection)) continue;
                var toDisassemble = section.Intersect(refSection);

                switch (section.SectionType)
                {
                    case MemorySectionType.Disassemble:
                        DisassembleSection(toDisassemble);
                        break;

                    case MemorySectionType.ByteArray:
                        GenerateByteArray(toDisassemble);
                        break;

                    case MemorySectionType.WordArray:
                        GenerateWordArray(toDisassemble);
                        break;

                    case MemorySectionType.Skip:
                        GenerateSkipOutput(toDisassemble);
                        break;
                }
            }
            return _output;
        }

        /// <summary>
        /// Creates disassembler output for the specified section
        /// </summary>
        /// <param name="section">Section information</param>
        private void DisassembleSection(MemorySection section)
        {
            _offset = section.StartAddress;
            var endOffset = section.EndAddress;
            while (_offset <= endOffset)
            {
                var item = DisassembleOperation();
                if (item != null)
                {
                    _output.AddItem(item);
                }
            }
            LabelFixup();
        }

        /// <summary>
        /// Generates byte array output for the specified section
        /// </summary>
        /// <param name="section">Section information</param>
        private void GenerateByteArray(MemorySection section)
        {
            var length = section.EndAddress - section.StartAddress + 1;
            for (var i = 0; i < length; i+= 8)
            {
                var sb = new StringBuilder(200);
                sb.Append(".db ");
                for (var j = 0; j < 8; j++)
                {
                    if (i + j >= length) break;
                    if (j > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("{0:X2}H", MemoryContents[section.StartAddress + i + j]);
                }
                var item = new DisassemblyItem((ushort) (section.StartAddress + i))
                {
                    Instruction = sb.ToString()
                };
                _output.AddItem(item);
            }
        }

        /// <summary>
        /// Generates word array output for the specified section
        /// </summary>
        /// <param name="section">Section information</param>
        private void GenerateWordArray(MemorySection section)
        {
            var length = section.EndAddress - section.StartAddress + 1;
            for (var i = 0; i < length; i += 8)
            {
                if (i + 1 >= length) break;
                var sb = new StringBuilder(200);
                sb.Append(".dw ");
                for (var j = 0; j < 8; j += 2)
                {
                    if (i + j + 1 >= length) break;
                    if (j > 0)
                    {
                        sb.Append(", ");
                    }
                    var value = (ushort)(MemoryContents[section.StartAddress + i + j * 2] +
                                (MemoryContents[section.StartAddress + i + j * 2 + 1] << 8));
                    sb.AppendFormat("{0:X4}H", value);
                }
                var item = new DisassemblyItem((ushort)(section.StartAddress + i))
                {
                    Instruction = sb.ToString()
                };
                _output.AddItem(item);
            }
            if (length % 2 == 1)
            {
                GenerateByteArray(new MemorySection(section.EndAddress, section.EndAddress));
            }
        }

        /// <summary>
        /// Generates skip output for the specified section
        /// </summary>
        /// <param name="section">Section information</param>
        private void GenerateSkipOutput(MemorySection section)
        {
            var item = new DisassemblyItem(section.StartAddress)
            {
                PrefixComment =
                    $"Skip section from {section.StartAddress:X4} to {section.EndAddress:X4}",
                Instruction = $".skip {section.EndAddress - section.StartAddress + 1:X4}H"
            };
            _output.AddItem(item);
        }

        /// <summary>
        /// Disassembles a single instruction
        /// </summary>
        private DisassemblyItem DisassembleOperation()
        {
            _opOffset = _offset;
            _currentOpCodes = new List<byte>();
            _displacement = null;
            _indexMode = 0; // No index
            OperationMapBase decodeInfo;
            var address = (ushort)_offset;

            // --- We should generate a normal instruction disassembly
            _opCode = Fetch();

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
                _indexMode = 1; // IX
                _opCode = Fetch();
                decodeInfo = DisassembleIndexedOperation();
            }
            else if (_opCode == 0xFD)
            {
                _indexMode = 2; // IY
                _opCode = Fetch();
                decodeInfo = DisassembleIndexedOperation();
            }
            else
            {
                decodeInfo = s_StandardInstructions.GetInstruction(_opCode);
            }
            return DecodeInstruction(address, decodeInfo);
        }

        private OperationMapBase DisassembleIndexedOperation()
        {
            if (_opCode != 0xCB)
            {
                var decodeInfo = s_IndexedInstructions.GetInstruction(_opCode);
                if (decodeInfo == null)
                {
                    return s_StandardInstructions.GetInstruction(_opCode);
                }
                if (decodeInfo.InstructionPattern.Contains("#D"))
                {
                    // --- The instruction used displacement, get it
                    _displacement = Fetch();
                }
                return decodeInfo;
            }
            _displacement = Fetch();
            _opCode = Fetch();
            return s_IndexedBitInstructions.GetInstruction(_opCode);
        }

        private byte Fetch()
        {
            var value = MemoryContents[(ushort)_offset];
            _offset++;
            _currentOpCodes.Add(value);
            return value;
        }

        private ushort FetchWord()
        {
            var l = Fetch();
            var h = Fetch();
            return (ushort)(h << 8 | l);
        }

        private DisassemblyItem DecodeInstruction(ushort address, OperationMapBase opInfo)
        {
            var comments = Annotations.GetCommentByAddress(address);
            var disassemblyItem = new DisassemblyItem(address)
            {
                OpCodes = _currentOpCodes,
                Instruction = "nop",
                Comment = comments?.Comment,
                PrefixComment = comments?.PrefixComment
            };
            if (opInfo == null) return disassemblyItem;

            var pragmaCount = 0;
            disassemblyItem.Instruction = opInfo.InstructionPattern;
            do
            {
                var pragmaIndex = disassemblyItem.Instruction.IndexOf("#", StringComparison.Ordinal);
                if (pragmaIndex < 0) break;
                pragmaCount++;
                ProcessPragma(disassemblyItem, pragmaIndex);
            } while (pragmaCount < 4);
            return disassemblyItem;
        }

        private void ProcessPragma(DisassemblyItem disassemblyItem, int pragmaIndex)
        {
            var instruction = disassemblyItem.Instruction;
            if (pragmaIndex >= instruction.Length) return;

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
                    var labelAddr = (ushort) (_opOffset + 2 + (sbyte) distance);
                    _output.CreateLabel(labelAddr, (ushort)_opOffset);
                    replacement = GetLabelNameByAddress(labelAddr);
                    break;
                case 'L':
                    // --- #L: absolute label (16 bit address)
                    var target = FetchWord();
                    disassemblyItem.TargetAddress = target;
                    _output.CreateLabel(target, (ushort)_opOffset);
                    replacement = GetLabelNameByAddress(target);
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
                case 'X':
                    // --- #X: Index register (IX or IY) according to current index mode
                    replacement = _indexMode == 1 ? "ix": "iy";
                    break;
                case 'l':
                    // --- #l: Lowest 8 bit index register (XL or YL) according to current index mode
                    replacement = _indexMode == 1 ? "xl" : "yl";
                    break;
                case 'h':
                    // --- #h: Highest 8 bit index register (XH or YH) according to current index mode
                    replacement = _indexMode == 1 ? "xh" : "yh";
                    break;
                case 'D':
                    // --- #D: Index operation displacement
                    if (_displacement.HasValue)
                    {
                        replacement = (sbyte) _displacement < 0 
                            ? $"-{ByteToString((byte) (0x100 - _displacement.Value))}" 
                            : $"+{ByteToString(_displacement.Value)}";
                    }
                    break;
            }

            disassemblyItem.Instruction = instruction.Substring(0, pragmaIndex)
                          + (replacement ?? "")
                          + instruction.Substring(pragmaIndex + 2);
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
        private void LabelFixup()
        {
            foreach (var labelAddr in _output.Labels.Keys)
            {
                var outputItem = _output[labelAddr];
                if (outputItem != null && outputItem.Label == null)
                {
                    outputItem.Label = GetLabelNameByAddress(labelAddr);
                }
            }
        }

        /// <summary>
        /// Gets a label by its address
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <returns>Label information if found; otherwise, null</returns>
        private string GetLabelNameByAddress(ushort addr)
        {
            return Annotations.CustomLabels.TryGetValue(addr, out CustomLabel disassemblyLabel)
                ? disassemblyLabel.Name
                : $"L{addr:X4}";
        }
    }
}