using System.Collections.Generic;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Operations;

// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable UsePatternMatching

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler
    {
        public BinarySegment CurrentSegment { get; private set; }

        /// <summary>
        /// Gets the current assembly address (represented by the "$" sign
        /// in the assembly language)
        /// </summary>
        /// <returns></returns>
        public ushort GetCurrentAssemblyAddress()
        {
            EnsureCodeSegment();
            return (ushort)(CurrentSegment.StartAddress 
                + CurrentSegment.Displacement 
                + CurrentSegment.EmittedCode.Count);
        }

        /// <summary>
        /// Emits the code after processing the directives
        /// </summary>
        /// <returns></returns>
        private bool EmitCode()
        {
            // --- Initialize code emission
            _output.Segments.Clear();

            foreach (var asmLine in PreprocessedLines)
            {
                var pragmaLine = asmLine as PragmaBase;
                if (pragmaLine != null)
                {
                    ApplyPragma(pragmaLine);
                }
                else
                {
                    var opLine = asmLine as OperationBase;
                    if (opLine != null)
                    {
                        EmitAssemblyOperationCode(opLine);
                    }
                    else
                    {
                        _output.Errors.Add(new UnexpectedSourceCodeLineError(asmLine,
                            $"A pragma or an operation line was expected, but a {asmLine.GetType()} line received"));
                    }
                }
            }
            return _output.ErrorCount == 0;
        }

        private void ApplyPragma(PragmaBase pragmaLine)
        {
        }

        /// <summary>
        /// Emits code for the specified operation
        /// </summary>
        /// <param name="opLine">Operation to emit the code for</param>
        private void EmitAssemblyOperationCode(OperationBase opLine)
        {
            // --- Handle the trivial operations (with simple mnemonics, like
            // --- nop, ldir, scf, etc.
            if (opLine is TrivialOperation)
            {
                EmitOperationWithLookup(s_TrivialOpBytes, opLine.Mnemonic, opLine);
                return;
            }

            // --- Handle push and pop operations with lookup
            var stackOpLine = opLine as StackOperation;
            if (stackOpLine != null)
            {
                EmitOperationWithLookup(
                    stackOpLine.Mnemonic == "PUSH"
                        ? s_PushOpBytes
                        : s_PopOpBytes, 
                    stackOpLine.Register, 
                    stackOpLine);
                return;
            }

            // --- Handle exchange operations (like 'ex de,hl', etc)
            var exOpLine = opLine as ExchangeOperation;
            if (exOpLine != null)
            {
                EmitExchangeOperation(exOpLine);
                return;
            }

        }

        /// <summary>
        /// Emits code for exchange operations
        /// </summary>
        /// <param name="exOpLine">Assembly line for an exchange operation</param>
        private void EmitExchangeOperation(ExchangeOperation exOpLine)
        {
            if (exOpLine.Destination == "AF") EmitByte(0x08); // ex af,af'
            else if (exOpLine.Destination == "DE") EmitByte(0xEB); // ex de,hl
            else
            {
                if (exOpLine.Source == "HL") EmitByte(0xE3); // ex (sp),hl
                else if (exOpLine.Source == "IX") EmitBytes(0xDD, 0xE3); // ex (sp),ix
                else EmitBytes(0xFD, 0xE3); // ex (sp),iy
            }
        }

        #region Emit helper methods

        /// <summary>
        /// Emits a new byte to the current code segment
        /// </summary>
        /// <param name="data">Data byte to emit</param>
        /// <returns>Current code offset</returns>
        public void EmitByte(byte data)
        {
            EnsureCodeSegment();
            CurrentSegment.EmittedCode.Add(data);
        }

        /// <summary>
        /// Emits an operation using a lookup table
        /// </summary>
        /// <param name="table">Lookup table</param>
        /// <param name="key">Operation key</param>
        /// <param name="operation">Assembly line that represents the operation</param>
        private void EmitOperationWithLookup(IReadOnlyDictionary<string, int> table, string key,
            OperationBase operation)
        {
            int code;
            if (table.TryGetValue(key, out code))
            {
                EmitDoubleByte(code);
                return;
            }
            _output.Errors.Add(new UnexpectedSourceCodeLineError(operation,
                $"Cannot find code for key {key} in operation '{operation.Mnemonic}'"));
        }

        /// <summary>
        /// Ensures that there's a code segment by the time the code is emitted
        /// </summary>
        private void EnsureCodeSegment()
        {
            if (CurrentSegment == null)
            {
                CurrentSegment = new BinarySegment
                {
                    StartAddress = _options?.DefaultStartAddress ?? 0x8000,
                    Displacement = _options?.DefaultDisplacement ?? 0x0000
                };
                _output.Segments.Add(CurrentSegment);
            }
        }

        /// <summary>
        /// Emits a double byte passed as an integer
        /// </summary>
        private void EmitDoubleByte(int doubleByte)
        {
            var low = (byte) (doubleByte & 0xFF);
            var high = (byte) ((doubleByte >> 8) & 0xFF);
            if (high != 0)
            {
                EmitByte(high);
            }
            EmitByte(low);
        }

        /// <summary>
        /// Emits a series of bytes
        /// </summary>
        private void EmitBytes(params byte[] bytes)
        {
            foreach (var data in bytes) EmitByte(data);
        }

        #endregion

        #region Operation lookup tables

        /// <summary>
        /// Z80 binary operation codes for trivial operations
        /// </summary>
        private static readonly Dictionary<string, int> s_TrivialOpBytes = new Dictionary<string, int>
        {
            {"NOP", 0x00},
            {"RLCA", 0x07},
            {"RRCA", 0x0F},
            {"RLA", 0x17},
            {"RRA", 0x1F},
            {"DAA", 0x27},
            {"CPL", 0x2F},
            {"SCF", 0x37},
            {"CCF", 0x3F},
            {"RET", 0xC9},
            {"EXX", 0xD9},
            {"DI", 0xF3},
            {"EI", 0xFB},
            {"NEG", 0xED44},
            {"RETN", 0xED45},
            {"RETI", 0xED4D},
            {"RRD", 0xED67},
            {"RLD", 0xED6F},
            {"LDI", 0xEDA0},
            {"CPI", 0xEDA1},
            {"INI", 0xEDA2},
            {"OUTI", 0xEDA3},
            {"LDD", 0xEDA8},
            {"CPD", 0xEDA9},
            {"IND", 0xEDAA},
            {"OUTD", 0xEDAB},
            {"LDIR", 0xEDB0},
            {"CPIR", 0xEDB1},
            {"INIR", 0xEDB2},
            {"OTIR", 0xEDB3},
            {"LDDR", 0xEDB8},
            {"CPDR", 0xEDB9},
            {"INDR", 0xEDBA},
            {"OTDR", 0xEDBB}
        };

        /// <summary>
        /// Z80 PUSH operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_PushOpBytes = new Dictionary<string, int>
        {
            {"AF", 0xF5},
            {"BC", 0xC5},
            {"DE", 0xD5},
            {"HL", 0xE5},
            {"IX", 0xDDE5},
            {"IY", 0xFDE5}
        };

        /// <summary>
        /// Z80 POP operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_PopOpBytes = new Dictionary<string, int>
        {
            {"AF", 0xF1},
            {"BC", 0xC1},
            {"DE", 0xD1},
            {"HL", 0xE1},
            {"IX", 0xDDE1},
            {"IY", 0xFDE1}
        };

        /// <summary>
        /// Z80 INC operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_IncOpBytes = new Dictionary<string, int>
        {
            {"A", 0x3C},
            {"B", 0x04},
            {"C", 0x0C},
            {"D", 0x14},
            {"E", 0x1C},
            {"H", 0x24},
            {"L", 0x2C},
            {"(HL)", 0x34},
            {"XL", 0xDD2C},
            {"XH", 0xDD24},
            {"YL", 0xFD2C},
            {"YH", 0xFD24},
            {"BC", 0x03},
            {"DE", 0x13},
            {"HL", 0x23},
            {"SP", 0x33},
            {"IX", 0xDD23},
            {"IY", 0xFD23},
        };

        /// <summary>
        /// Z80 DEC operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_DecOpBytes = new Dictionary<string, int>
        {
            {"A", 0x3D},
            {"B", 0x05},
            {"C", 0x0D},
            {"D", 0x15},
            {"E", 0x1D},
            {"H", 0x25},
            {"L", 0x2D},
            {"(HL)", 0x35},
            {"XL", 0xDD2D},
            {"XH", 0xDD25},
            {"YL", 0xFD2D},
            {"YH", 0xFD25},
            {"BC", 0x0B},
            {"DE", 0x1B},
            {"HL", 0x2B},
            {"SP", 0x3B},
            {"IX", 0xDD2B},
            {"IY", 0xFD2B},
        };

        #endregion
    }
}
