using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spect.Net.SpectrumEmu.FpCalc;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    public partial class Z80Disassembler
    {
        private SpectrumSpecificMode _spectMode;
        private int _seriesCount;

        /// <summary>
        /// Generates bytecode output for the specified memory section
        /// </summary>
        /// <param name="section">Section information</param>
        private void GenerateRst28ByteCodeOutput(MemorySection section)
        {
            _spectMode = SpectrumSpecificMode.Spectrum48Rst28;
            _seriesCount = 0;
            _currentOpCodes = new StringBuilder(16);
            var addr = _offset = section.StartAddress;
            while (addr <= section.EndAddress)
            {
                _currentOpCodes.Clear();
                var opCode = Fetch();
                // ReSharper disable once UnusedVariable
                var item = DisassembleCalculatorEntry((ushort)addr, opCode, out var carryOn);
                _output.AddItem(item);
                addr = _offset;
            }
        }

        /// <summary>
        /// Checks if the disassembler should enter into Spectrum-specific mode after
        /// the specified disassembly item.
        /// </summary>
        /// <param name="item">Item used to check the Spectrum-specific mode</param>
        /// <returns>
        /// True, to move to the Spectrum-specific mode; otherwise, false
        /// </returns>
        private bool ShouldEnterSpectrumSpecificMode(DisassemblyItem item)
        {
            // --- Check for Spectrum 48K RST #08
            if ((DisassemblyFlags & SpectrumSpecificDisassemblyFlags.Spectrum48Rst08) != 0 
                && item?.OpCodes.Trim() == "CF")
            {
                _spectMode = SpectrumSpecificMode.Spectrum48Rst08;
                item.HardComment = "(Report error)";
                return true;
            }

            // --- Check for Spectrum 48K RST #28
            if ((DisassemblyFlags & SpectrumSpecificDisassemblyFlags.Spectrum48Rst28) != 0
                && (item?.OpCodes.Trim() == "EF"            // --- RST #28
                    || item?.OpCodes.Trim() == "CD 5E 33"   // --- CALL 335E
                    || item?.OpCodes.Trim() == "CD 62 33")) // --- CALL 3362
            {
                _spectMode = SpectrumSpecificMode.Spectrum48Rst28;
                _seriesCount = 0;
                item.HardComment = "(Invoke Calculator)";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disassembles the subsequent operation as Spectrum-specific
        /// </summary>
        /// <param name="carryOn">
        /// True, if the disassembler still remains in Spectrum-specific mode;
        /// otherwise, false
        /// </param>
        /// <returns>Disassembled operation</returns>
        private DisassemblyItem DisassembleSpectrumSpecificOperation(out bool carryOn)
        {
            if (_spectMode == SpectrumSpecificMode.None)
            {
                carryOn = false;
                return null;
            }

            DisassemblyItem item = null;
            carryOn = false;

            // --- Handle Spectrum 48 Rst08
            if (_spectMode == SpectrumSpecificMode.Spectrum48Rst08)
            {
                // --- The next byte is the operation code
                var address = (ushort)_offset;
                var errorCode = Fetch();
                _spectMode = SpectrumSpecificMode.None;
                item = new DisassemblyItem(address)
                {
                    OpCodes = $"{errorCode:X2}",
                    Instruction = $".defb #{errorCode:X2}",
                    HardComment = $"(error code: #{errorCode:X2})",
                    LastAddress = (ushort)(_offset - 1)
                };
            }

            // --- Handle Spectrum 48 Rst08
            if (_spectMode == SpectrumSpecificMode.Spectrum48Rst28)
            {
                var address = (ushort)_offset;
                var calcCode = Fetch();
                item = DisassembleCalculatorEntry(address, calcCode, out carryOn);
            }

            if (!carryOn)
            {
                _spectMode = SpectrumSpecificMode.None;
            }
            return item;
        }

        /// <summary>
        /// Disassemble a calculator entry
        /// </summary>
        private DisassemblyItem DisassembleCalculatorEntry(ushort address, byte calcCode, out bool carryOn)
        {
            // --- Create the default disassembly item
            var item = new DisassemblyItem(address)
            {
                LastAddress = (ushort) (_offset - 1),
                Instruction = $".defb #{calcCode:X2}"
            };

            var opCodes = new List<byte> { calcCode };
            carryOn = true;

            // --- If we're in series mode, obtain the subsequent series value
            if (_seriesCount > 0)
            {
                var lenght = (calcCode >> 6) + 1;
                if ((calcCode & 0x3F) == 0) lenght++;
                for (var i = 0; i < lenght; i++)
                {
                    var nextByte = Fetch();
                    opCodes.Add(nextByte);
                }
                item.Instruction = ".defb " + string.Join(", ", opCodes.Select(o => $"#{o:X2}"));
                item.HardComment = $"({FloatNumber.FromCompactBytes(opCodes)})";
                _seriesCount--;
                return item;
            }

            // --- Generate the output according the calculation op code
            switch (calcCode)
            {
                case 0x00:
                case 0x33:
                case 0x35:
                    var jump = Fetch();
                    opCodes.Add(jump);
                    var jumpAddr = (ushort)(_offset - 1 + (sbyte) jump);
                    _output.CreateLabel(jumpAddr, null);
                    item.Instruction = $".defb #{calcCode:X2}, #{jump:X2}";
                    item.HardComment = $"({s_CalcOps[calcCode]}: {GetLabelName(jumpAddr)})";
                    carryOn = calcCode != 0x33;
                    break;

                case 0x34:
                    _seriesCount = 1;
                    item.HardComment = "(stk-data)";
                    break;

                case 0x38:
                    item.HardComment = "(end-calc)";
                    carryOn = false;
                    break;

                case 0x86:
                case 0x88:
                case 0x8C:
                    _seriesCount = calcCode - 0x80;
                    item.HardComment = $"(series-0{calcCode-0x80:X1})";
                    break;

                case 0xA0:
                case 0xA1:
                case 0xA2:
                case 0xA3:
                case 0xA4:
                    var constNo = calcCode - 0xA0;
                    item.HardComment = GetIndexedCalcOp(0x3F, constNo);
                    break;

                case 0xC0:
                case 0xC1:
                case 0xC2:
                case 0xC3:
                case 0xC4:
                case 0xC5:
                    var stNo = calcCode - 0xC0;
                    item.HardComment = GetIndexedCalcOp(0x40, stNo);
                    break;

                case 0xE0:
                case 0xE1:
                case 0xE2:
                case 0xE3:
                case 0xE4:
                case 0xE5:
                    var getNo = calcCode - 0xE0;
                    item.HardComment = GetIndexedCalcOp(0x41, getNo);
                    break;

                default:
                    var comment = s_CalcOps.ContainsKey(calcCode) 
                        ? s_CalcOps[calcCode] 
                        : $"calc code: #{calcCode:X2}";
                    item.HardComment = $"({comment})";
                    break;
            }
            item.OpCodes = string.Join(" ", opCodes.Select(o => $"{o:X2}"));
            return item;
        }

        private string GetIndexedCalcOp(byte opCode, int index)
        {
            if (s_CalcOps.ContainsKey(opCode))
            {
                var values = s_CalcOps[opCode].Split('|');
                if (index >= 0 && values.Length > index)
                {
                    return $"({values[index]})";
                }
            }
            return $"calc code: {opCode}/{index}";
        }

        /// <summary>
        /// Spectrum-specific disassembly mode
        /// </summary>
        private enum SpectrumSpecificMode
        {
            None = 0,
            Spectrum48Rst08,
            Spectrum48Rst28
        }

        /// <summary>
        /// The names of Spectrum 48 RST 28 calculator operations
        /// </summary>
        private static readonly Dictionary<byte, string> s_CalcOps = new Dictionary<byte, string>
        {
            { 0x00, "jump-true" },
            { 0x01, "exchange" },
            { 0x02, "delete" },
            { 0x03, "subtract" },
            { 0x04, "multiply" },
            { 0x05, "division" },
            { 0x06, "to-power" },
            { 0x07, "or" },
            { 0x08, "no-&-no" },
            { 0x09, "no-l-eql" },
            { 0x0A, "no-gr-eq" },
            { 0x0B, "nos-neql" },
            { 0x0C, "no-grtr" },
            { 0x0D, "no-less" },
            { 0x0E, "nos-eql" },
            { 0x0F, "addition" },
            { 0x10, "str-&-no" },
            { 0x11, "str-l-eql" },
            { 0x12, "str-gr-eq" },
            { 0x13, "strs-neql" },
            { 0x14, "str-grtr" },
            { 0x15, "str-less" },
            { 0x16, "strs-eql" },
            { 0x17, "strs-add" },
            { 0x18, "val$" },
            { 0x19, "usr-$" },
            { 0x1A, "read-in" },
            { 0x1B, "negate" },
            { 0x1C, "code" },
            { 0x1D, "val" },
            { 0x1E, "len" },
            { 0x1F, "sin" },
            { 0x20, "cos" },
            { 0x21, "tan" },
            { 0x22, "asn" },
            { 0x23, "acs" },
            { 0x24, "atn" },
            { 0x25, "ln" },
            { 0x26, "exp" },
            { 0x27, "int" },
            { 0x28, "sqr" },
            { 0x29, "sgn" },
            { 0x2A, "abs" },
            { 0x2B, "peek" },
            { 0x2C, "in" },
            { 0x2D, "usr-no" },
            { 0x2E, "str$" },
            { 0x2F, "chr$" },
            { 0x30, "not" },
            { 0x31, "duplicate" },
            { 0x32, "n-mod-m" },
            { 0x33, "jump" },
            { 0x34, "stk-data" },
            { 0x35, "dec-jr-nz" },
            { 0x36, "less-0" },
            { 0x37, "greater-0" },
            { 0x38, "end-calc" },
            { 0x39, "get-argt" },
            { 0x3A, "truncate" },
            { 0x3B, "fp-calc-2" },
            { 0x3C, "e-to-fp" },
            { 0x3D, "re-stack" },
            { 0x3E, "series-06|series-08|series-0C" },
            { 0x3F, "stk-zero|stk-one|stk-half|stk-pi-half|stk-ten" },
            { 0x40, "st-mem-0|st-mem-1|st-mem-2|st-mem-3|st-mem-4|st-mem-5" },
            { 0x41, "get-mem-0|get-mem-1|get-mem-2|get-mem-3|get-mem-4|get-mem-5" }
        };
    }
}