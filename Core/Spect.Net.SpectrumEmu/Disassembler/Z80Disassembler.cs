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
        private static readonly Dictionary<Type, object> s_Providers = new Dictionary<Type, object>();

        private DisassemblyOutput _output;
        private int _offset;
        private int _opOffset;
        private StringBuilder _currentOpCodes;
        private byte? _displacement;
        private byte _opCode;
        private int _indexMode;
        private bool _overflow;

        #region Disassembly providers

        /// <summary>
        /// Resets the list of providers
        /// </summary>
        public static void ResetProviders()
        {
            s_Providers.Clear();
        }

        /// <summary>
        /// Adds a provider instance with the specified type
        /// </summary>
        /// <param name="instance">Provider instance</param>
        public static void SetProvider<TProv>(object instance)
            where TProv: class
        {
            s_Providers[typeof(TProv)] = instance;
        }

        /// <summary>
        /// Gets the disassembly provider with the specified type
        /// </summary>
        /// <typeparam name="TProv">Provider type</typeparam>
        /// <returns>Provider instance, if found; otherwise, null</returns>
        public static TProv GetProvider<TProv>()
            where TProv : class
        {
            return s_Providers.TryGetValue(typeof(TProv), out var provider) 
                ? provider as TProv 
                : null;
        }

        #endregion

        /// <summary>
        /// Gets the contents of the memory
        /// </summary>
        public byte[] MemoryContents { get; }

        /// <summary>
        /// Memory sections used by the disassembler
        /// </summary>
        public IEnumerable<MemorySection> MemorySections { get; }

        /// <summary>
        /// The ZX Spectrum specific disassembly flags for each bank
        /// </summary>
        public Dictionary<int, SpectrumSpecificDisassemblyFlags> DisassemblyFlags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="memorySections">Memory map for disassembly</param>
        /// <param name="memoryContents">The contents of the memory to disassemble</param>
        /// <param name="disasmFlags">Optional flags to be used with the disassembly</param>
        public Z80Disassembler(IEnumerable<MemorySection> memorySections, byte[] memoryContents,
            Dictionary<int, SpectrumSpecificDisassemblyFlags> disasmFlags = null)
        {
            MemorySections = memorySections;
            MemoryContents = memoryContents;
            DisassemblyFlags = disasmFlags ?? new Dictionary<int, SpectrumSpecificDisassemblyFlags>();
        }

        /// <summary>
        /// Disassembles the memory from the specified start address with the given endAddress
        /// </summary>
        /// <param name="startAddress">The start address of the disassembly</param>
        /// <param name="endAddress">The end address of the disassembly</param>
        /// <returns>
        /// The disassembly output
        /// </returns>
        public DisassemblyOutput Disassemble(ushort startAddress = 0x0000, ushort endAddress = 0xFFFF)
        {
            _output = new DisassemblyOutput();
            if (endAddress > MemoryContents.Length)
            {
                endAddress = (ushort)(MemoryContents.Length - 1);
            }
            var refSection = new MemorySection(startAddress, endAddress);

            // --- Let's go through the memory sections
            foreach (var section in MemorySections)
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

                    case MemorySectionType.Rst28Calculator:
                        GenerateRst28ByteCodeOutput(toDisassemble);
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
            _overflow = false;
            var endOffset = section.EndAddress;
            var isSpectrumSpecific = false;
            while (_offset <= endOffset && !_overflow)
            {
                if (isSpectrumSpecific)
                {
                    var spectItem = DisassembleSpectrumSpecificOperation(out isSpectrumSpecific);
                    if (spectItem != null)
                    {
                        _output.AddItem(spectItem);
                    }
                }
                else
                {
                    // --- Disassemble the current item
                    var item = DisassembleOperation();
                    if (item != null)
                    {
                        _output.AddItem(item);
                        isSpectrumSpecific = ShouldEnterSpectrumSpecificMode(item);
                    }
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
                sb.Append(".defb ");
                for (var j = 0; j < 8; j++)
                {
                    if (i + j >= length) break;
                    if (j > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("#{0:X2}", MemoryContents[section.StartAddress + i + j]);
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
                sb.Append(".defw ");
                for (var j = 0; j < 8; j += 2)
                {
                    if (i + j + 1 >= length) break;
                    if (j > 0)
                    {
                        sb.Append(", ");
                    }
                    var value = (ushort)(MemoryContents[section.StartAddress + i + j * 2] +
                                (MemoryContents[section.StartAddress + i + j * 2 + 1] << 8));
                    sb.AppendFormat("#{0:X4}", value);
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
            _currentOpCodes = new StringBuilder(16);
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
                if (decodeInfo.InstructionPattern.Contains("^D"))
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
            if (_offset >= MemoryContents.Length)
            {
                _offset = 0;
                _overflow = true;
            }
            var value = MemoryContents[(ushort)_offset];
            _offset++;
            _currentOpCodes.AppendFormat("{0:X2} ", value);
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
            // --- By default, unknown codes are NOP operations
            var disassemblyItem = new DisassemblyItem(address)
            {
                OpCodes = _currentOpCodes.ToString(),
                Instruction = "nop",
                LastAddress = (ushort)(_offset - 1)
            };
            if (opInfo == null) return disassemblyItem;

            // --- We have a real operation, it's time to decode it
            var pragmaCount = 0;
            disassemblyItem.Instruction = opInfo.InstructionPattern;
            do
            {
                var pragmaIndex = disassemblyItem.Instruction.IndexOf("^", StringComparison.Ordinal);
                if (pragmaIndex < 0) break;
                pragmaCount++;
                ProcessPragma(disassemblyItem, pragmaIndex);
            } while (pragmaCount < 4);

            // --- We've fully processed the instruction
            disassemblyItem.OpCodes = _currentOpCodes.ToString();
            disassemblyItem.LastAddress = (ushort)(_offset - 1);
            return disassemblyItem;
        }

        private void ProcessPragma(DisassemblyItem disassemblyItem, int pragmaIndex)
        {
            var instruction = disassemblyItem.Instruction;
            if (pragmaIndex >= instruction.Length) return;

            var pragma = instruction[pragmaIndex + 1];
            var replacement = "";
            var symbolPresent = false;
            var symbolValue = (ushort) 0x000;
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
                    replacement = GetLabelName(labelAddr);
                    symbolPresent = true;
                    disassemblyItem.HasLabelSymbol = true;
                    symbolValue = labelAddr;
                    break;
                case 'L':
                    // --- #L: absolute label (16 bit address)
                    var target = FetchWord();
                    disassemblyItem.TargetAddress = target;
                    _output.CreateLabel(target, (ushort)_opOffset);
                    replacement = GetLabelName(target);
                    symbolPresent = true;
                    disassemblyItem.HasLabelSymbol = true;
                    symbolValue = target;
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
                    var value = Fetch();
                    replacement = ByteToString(value);
                    symbolPresent = true;
                    symbolValue = value;
                    break;
                case 'W':
                    // --- #W: 16-bit word from the code
                    var word = FetchWord();
                    replacement = WordToString(word);
                    symbolPresent = true;
                    symbolValue = word;
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

            if (symbolPresent && !string.IsNullOrEmpty(replacement))
            {
                disassemblyItem.TokenPosition = pragmaIndex;
                disassemblyItem.TokenLength = replacement.Length;
                disassemblyItem.HasSymbol = true;
                disassemblyItem.SymbolValue = symbolValue;
            }
            disassemblyItem.Instruction = instruction.Substring(0, pragmaIndex)
                          + (replacement ?? "")
                          + instruction.Substring(pragmaIndex + 2);
        }

        /// <summary>
        /// Fixes the labels within the disassembly output
        /// </summary>
        private void LabelFixup()
        {
            foreach (var labelAddr in _output.Labels.Keys)
            {
                var outputItem = _output[labelAddr];
                if (outputItem != null)
                {
                    outputItem.HasLabel = true;
                }
            }
        }

        /// <summary>
        /// Converts a byte value to a hexadecimal string
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Hexadecimal representation</returns>
        public static string ByteToString(byte value)
        {
            return $"#{value:X2}";
        }

        /// <summary>
        /// Converts a 16-bit value to a hexadecimal string
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Hexadecimal representation</returns>
        public static string WordToString(ushort value)
        {
            return $"#{value:X4}";
        }

        /// <summary>
        /// Gets a label by its address
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <returns>Label name to display</returns>
        public static string GetLabelName(ushort addr)
        {
            return $"L{addr:X4}";
        }
    }
}