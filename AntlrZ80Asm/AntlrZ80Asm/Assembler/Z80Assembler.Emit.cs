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
        private BinarySegment _currentSegment;

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
                        EmitCodeFor(opLine);
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
        /// <param name="opLine"></param>
        private void EmitCodeFor(OperationBase opLine)
        {
            if (opLine is TrivialOperation)
            {
                EmitTrivialOperation(opLine);
                return;
            }
            var exOpLine = opLine as ExchangeOperation;
            if (exOpLine != null)
            {
                EmitExchangeOperation(exOpLine);
                return;
            }
            var stackOpLine = opLine as StackOperation;
            if (stackOpLine != null)
            {
                EmitStackOperation(stackOpLine);
                return;
            }
        }

        /// <summary>
        /// Emits code for stack operations
        /// </summary>
        /// <param name="stackOpLine">Assembly line for stack operation</param>
        private void EmitStackOperation(StackOperation stackOpLine)
        {
            var dict = stackOpLine.Mnemonic == "PUSH"
                ? s_PushOpBytes
                : s_PopOpBytes;
            int code;
            if (dict.TryGetValue(stackOpLine.Register, out code))
            {
                EmitDoubleByte(code);
                return;
            }
            _output.Errors.Add(new UnexpectedSourceCodeLineError(stackOpLine,
                $"Cannot find code for {stackOpLine.Mnemonic} {stackOpLine.Register} operation"));
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

        /// <summary>
        /// Emits a trivial operation
        /// </summary>
        /// <param name="trivialOp">Assembly line for a trival operation</param>
        private void EmitTrivialOperation(OperationBase trivialOp)
        {
            int code;
            if (s_TrivialOpBytes.TryGetValue(trivialOp.Mnemonic, out code))
            {
                EmitDoubleByte(code);
                return;
            }
            _output.Errors.Add(new UnexpectedSourceCodeLineError(trivialOp,
                $"Cannot find code for trivial operation '{trivialOp.Mnemonic}'"));
        }

        /// <summary>
        /// Emits a new byte to the current code segment
        /// </summary>
        /// <param name="data">Data byte to emit</param>
        /// <returns>Current code offset</returns>
        private void EmitByte(byte data)
        {
            if (_currentSegment == null)
            {
                _currentSegment = new BinarySegment
                {
                    StartAddress = _options.DefaultStartAddress ?? 0x8000,
                    Displacement = _options.DefaultDisplacement ?? 0x0000
                };
                _output.Segments.Add(_currentSegment);
            }
            _currentSegment.EmittedCode.Add(data);
        }

        /// <summary>
        /// Emits a series of bytes
        /// </summary>
        private void EmitBytes(params byte[] bytes)
        {
            foreach (var data in bytes) EmitByte(data);
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

    }
}