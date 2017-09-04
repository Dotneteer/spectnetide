using System;
using System.Collections.Generic;
using System.Linq;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Expressions;
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
        /// <summary>
        /// The current output segment of the emitted code
        /// </summary>
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

        /// <summary>
        /// Applies a pragma in the assembly source code
        /// </summary>
        /// <param name="pragmaLine">
        /// Assembly line that represents a pragma
        /// </param>
        private void ApplyPragma(PragmaBase pragmaLine)
        {
            // TODO: Implement pragma handling
        }

        /// <summary>
        /// Emits code for the specified operation
        /// </summary>
        /// <param name="opLine">Operation to emit the code for</param>
        private void EmitAssemblyOperationCode(OperationBase opLine)
        {
            // --- Handle the trivial operations (with simple mnemonics, like
            // --- nop, ldir, scf, etc.
            var trivOpLine = opLine as TrivialOperation;
            if (trivOpLine != null)
            {
                EmitTrivialOperation(trivOpLine);
                return;
            }

            // --- Handle compound operations
            var compoundOpLine = opLine as CompoundOperation;
            if (compoundOpLine != null)
            {
                EmitCompoundOperation(compoundOpLine);
                return;
            }

            // --- Any other case means an internal error
            _output.Errors.Add(new UnexpectedSourceCodeLineError(opLine,
                $"Unexpected operation type '{opLine.GetType().FullName}'"));
        }

        /// <summary>
        /// Emits code for trivial operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line that denotes a trivial Z80 operation.
        /// </param>
        private void EmitTrivialOperation(OperationBase opLine)
        {
            EmitOperationWithLookup(s_TrivialOpBytes, opLine.Mnemonic, opLine);
        }

        /// <summary>
        /// Emits a compound operation
        /// </summary>
        /// <param name="compoundOpLine">
        /// Assembly line that denotes a compound Z80 operation.
        /// </param>
        private void EmitCompoundOperation(CompoundOperation compoundOpLine)
        {
            CompoundOperationDescriptor rules;
            if (!_compoundOpTable.TryGetValue(compoundOpLine.Mnemonic, out rules))
            {
                _output.Errors.Add(new UnexpectedSourceCodeLineError(compoundOpLine,
                    $"No processing rules defined for '{compoundOpLine.Mnemonic}' operation"));
                return;
            }

            // --- Get the operand types
            var op1Type = compoundOpLine.Operand?.Type ?? OperandType.None;
            var op2Type = compoundOpLine.Operand2?.Type ?? OperandType.None;

            var isProcessable = true;

            // --- Check inclusive rules
            if (rules.Allow != null)
            {
                isProcessable = rules.Allow.Any(r => r.FirstOp == op1Type && r.SecondOp == op2Type);
            }

            // --- We applied operands according to rules
            if (isProcessable)
            {
                rules.ProcessAction(this, compoundOpLine);
                return;
            }

            // --- This operations is invalid. Report it with the proper message.
            var message = $"The '{compoundOpLine.Mnemonic}' operation with the specified operands is invalid.";
            _output.Errors.Add(new InvalidArgumentError(compoundOpLine, message));
        }

        /// <summary>
        /// LD operations
        /// </summary>
        private static void ProcessLd(Z80Assembler asm, CompoundOperation op)
        {
            // --- Destination is an 8-bit register
            if (op.Operand.Type == OperandType.Reg8)
            {
                var destReg = op.Operand.Register;
                var destRegIdx = s_Reg8Order.IndexOf(destReg);
                var sourceReg = op.Operand2.Register;

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // ld '8bitreg','8bitReg'
                    asm.EmitByte((byte) (0x40 + (destRegIdx << 3) + s_Reg8Order.IndexOf(sourceReg)));
                    return;
                }

                if (op.Operand2.Type == OperandType.RegIndirect)
                {
                    if (sourceReg == "(BC)")
                    {
                        if (destReg == "A")
                        {
                            // ld a,(bc)
                            asm.EmitByte(0x0A);
                            return;
                        }
                    }
                    else if (sourceReg == "(DE)")
                    {
                        if (destReg == "A")
                        {
                            // ld a,(de)
                            asm.EmitByte(0x1A);
                            return;
                        }
                    }
                    else if (sourceReg == "(HL)")
                    {
                        // ld '8bitreg',(hl)
                        asm.EmitByte((byte) (0x46 + (destRegIdx << 3)));
                        return;
                    }
                    asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Spec)
                {
                    // ld a,i and ld a,r
                    if (op.Operand.Register != "A")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    asm.EmitDoubleByte(sourceReg == "R" ? 0xED5F : 0xED57);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    // ld reg,'xh|xl|yh|yl'
                    // --- Destination must be one of the indexed 8-bit registers
                    if (destRegIdx >= 4 && destRegIdx <= 6)
                    {
                        // --- Deny invalid destination: h, l, (hl)
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    var opCode = sourceReg.StartsWith("X") ? 0xDD44 : 0xFD44;
                    asm.EmitDoubleByte(opCode + (destRegIdx << 3) + (sourceReg.EndsWith("H") ? 0 : 1));
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld reg,expr
                    asm.EmitByte((byte)(0x06 + (destRegIdx << 3)));
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld a,(expr)
                    if (destReg != "A")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    asm.EmitByte(0x3A);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.IndexedAddress)
                {
                    // --- ld '8-bit-reg', '(idxreg+disp)' operation
                    var opCode = (byte)(0x46 + (destRegIdx << 3));
                    asm.EmitIndexedOperation(op.Operand2, opCode);
                    return;
                }
            }

            // --- Destination is an 8-bit index register
            if (op.Operand.Type == OperandType.Reg8Idx)
            {
                var destReg = op.Operand.Register;
                var sourceReg = op.Operand2.Register;
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // ld 'xh|xl|yh|yl', reg
                    var sourceRegIdx = s_Reg8Order.IndexOf(sourceReg);

                    // --- Destination must be one of the indexed 8-bit registers
                    if (sourceRegIdx >= 4 && sourceRegIdx <= 6)
                    {
                        // --- Deny invalid destination: h, l, (hl)
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    var opBytes = destReg.StartsWith("X") ? 0xDD60 : 0xFD60;
                    asm.EmitDoubleByte(opBytes + (destReg.EndsWith("H") ? 0 : 8) + sourceRegIdx);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    // ld 'xh|xl|yh|yl', 'xh|xl|yh|yl'
                    if (sourceReg[0] != destReg[0])
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }

                    var xopBytes = destReg.StartsWith("X") ? 0xDD64 : 0xFD64;
                    asm.EmitDoubleByte(xopBytes + (destReg.EndsWith("H") ? 0 : 8)
                                       + (sourceReg.EndsWith("H") ? 0 : 1));
                    return;
                }

                // ld 'xh|xl|yh|yl',expr
                var opCode = destReg.StartsWith("X") ? 0xDD26 : 0xFD26;
                opCode += destReg.EndsWith("H") ? 0 : 8;
                asm.EmitDoubleByte(opCode);
                asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                return;
            }

            // --- Destination is I or A
            if (op.Operand.Type == OperandType.Reg8Spec)
            {
                // ld i,a and ld r,a
                if (op.Operand2.Register != "A")
                {
                    asm.ReportInvalidLoadOp(op, op.Operand.Register, op.Operand2.Register);
                    return;
                }
                asm.EmitDoubleByte(op.Operand.Register == "R" ? 0xED4F : 0xED47);
                return;
            }

            // --- Destination is memory through a 16-bit register
            if (op.Operand.Type == OperandType.RegIndirect)
            {
                var destReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    var sourceReg = op.Operand2.Register;
                    if (destReg == "(BC)")
                    {
                        if (sourceReg == "A")
                        {
                            // ld (bc),a
                            asm.EmitByte(0x02);
                            return;
                        }
                    }
                    else if (destReg == "(DE)")
                    {
                        if (sourceReg == "A")
                        {
                            // ld (de),a
                            asm.EmitByte(0x12);
                            return;
                        }
                    }
                    else if (destReg == "(HL)")
                    {
                        // ld (hl),'8BitReg'
                        asm.EmitByte((byte)(0x70 + s_Reg8Order.IndexOf(sourceReg)));
                        return;
                    }
                    asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    if (op.Operand.Register != "(HL)")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, "<expression>");
                        return;
                    }
                    // ld (hl),expr
                    asm.EmitByte(0x36);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                return;
            }

            // --- Destination is a memory address
            if (op.Operand.Type == OperandType.MemIndirect)
            {
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    if (op.Operand2.Register != "A")
                    {
                        asm.ReportInvalidLoadOp(op, "(<expression>)", op.Operand2.Register);
                        return;
                    }
                    asm.EmitByte(0x32);
                }
                else if (op.Operand2.Type == OperandType.Reg16)
                {
                    // ld (expr),reg16
                    var sourceReg = op.Operand2.Register;
                    var opCode = 0x22;
                    if (sourceReg == "BC")
                    {
                        opCode = 0xED43;
                    }
                    else if (sourceReg == "DE")
                    {
                        opCode = 0xED53;
                    }
                    else if (sourceReg == "SP")
                    {
                        opCode = 0xED73;
                    }
                    asm.EmitDoubleByte(opCode);
                }
                else if (op.Operand2.Type == OperandType.Reg16Idx)
                {
                    asm.EmitDoubleByte(op.Operand2.Register == "IX" ? 0xDD22 : 0xFD22);
                }
                asm.EmitExpression(op, op.Operand.Expression, FixupType.Bit16);
                return;
            }

            // --- Destination is a 16-bit register
            if (op.Operand.Type == OperandType.Reg16)
            {
                var destReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld reg16,(expr)
                    var opCode = 0x2A;
                    if (destReg == "BC")
                    {
                        opCode = 0xED4B;
                    }
                    else if (destReg == "DE")
                    {
                        opCode = 0xED5B;
                    }
                    else if (destReg == "SP")
                    {
                        opCode = 0xED7B;
                    }
                    asm.EmitDoubleByte(opCode);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld reg16,expr
                    var sourceRegIdx = s_Reg16Order.IndexOf(op.Operand.Register);
                    asm.EmitByte((byte)(0x01 + (sourceRegIdx << 4)));
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                // --- From now on, the destination can be only SP

                if (op.Operand.Register != "SP")
                {
                    asm.ReportInvalidLoadOp(op, op.Operand.Register, op.Operand2.Register);
                    return;
                }

                var spCode = 0xF9;
                if (op.Operand2.Register == "IX")
                {
                    spCode = 0xDDF9;
                }
                else if (op.Operand2.Register == "IY")
                {
                    spCode = 0xFDF9;
                }
                asm.EmitDoubleByte(spCode);
                return;
            }

            // --- Destination is a 16-bit index register
            if (op.Operand.Type == OperandType.Reg16Idx)
            {
                var sourceReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld 'ix|iy',(expr)
                    asm.EmitDoubleByte(sourceReg == "IX" ? 0xDD2A : 0xFD2A);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld 'ix|iy',expr
                    asm.EmitDoubleByte(op.Operand.Register == "IX" ? 0xDD21 : 0xFD21);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }
                return;
            }

            // --- Destination is an indexed memory address
            if (op.Operand.Type == OperandType.IndexedAddress)
            {

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // --- ld '(idxreg+disp)','8bitReg'
                    var opCode = (byte)(0x70 + s_Reg8Order.IndexOf(op.Operand2.Register));
                    asm.EmitIndexedOperation(op.Operand, opCode);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // --- ld '(idxreg+disp)','expr'
                    asm.EmitIndexedOperation(op.Operand, 0x36);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                }
            }
        }

        /// <summary>
        /// BIT, SET, RES operations
        /// </summary>
        private static void ProcessBit(Z80Assembler asm, CompoundOperation op)
        {
            byte opByte;
            switch (op.Mnemonic)
            {
                case "BIT":
                    opByte = 0x40;
                    break;
                case "RES":
                    opByte = 0x80;
                    break;
                default:
                    opByte = 0xC0;
                    break;
            }

            // --- Check the bit index
            var bitIndex = asm.EvalImmediate(op, op.BitIndex);
            if (bitIndex == null)
            {
                return;
            }
            if (bitIndex < 0 || bitIndex > 7)
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    $"Bit index should be between 0 and 7. '{bitIndex}' is invalid."));
                return;
            }

            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                if (op.Operand2 == null)
                {
                    if (op.Mnemonic != "BIT") opByte |= 0x06;
                }
                else if (op.Operand2.Type == OperandType.Reg8)
                {
                    if (op.Mnemonic == "BIT")
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            "'bit' operation with indexed displacement cannot have a register operand"));
                        return;
                    }
                    opByte |= (byte)s_Reg8Order.IndexOf(op.Operand2.Register);
                }
                asm.EmitIndexedBitOperation(op.Operand.Register, op.Operand.Sign, op.Operand.Expression, 
                    (byte)(opByte + (bitIndex << 3)));
                return;
            }

            if (op.Operand.Type == OperandType.Reg8)
            {
                opByte |= (byte)s_Reg8Order.IndexOf(op.Operand.Register);
            }
            else if (op.Operand.Type == OperandType.RegIndirect)
            {
                if (op.Operand.Register != "(HL)")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        $"'{op.Mnemonic}' operation can have {op.Operand.Register} as its operand"));
                    return;
                }
                opByte |= 0x06;
            }
            asm.EmitBytes(0xCB, (byte)(opByte + (bitIndex << 3)));
        }

        /// <summary>
        /// Shift and rotate operations
        /// </summary>
        private static void ProcessShiftRotate(Z80Assembler asm, CompoundOperation op)
        {
            var sOpByte = (byte)(8 * s_ShiftOpOrder.IndexOf(op.Mnemonic));
            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                if (op.Operand2 == null)
                {
                    sOpByte |= 0x06;
                }
                else if (op.Operand2.Type == OperandType.Reg8)
                {
                    sOpByte |= (byte)s_Reg8Order.IndexOf(op.Operand2.Register);
                }
                asm.EmitIndexedBitOperation(op.Operand.Register, op.Operand.Sign, op.Operand.Expression, sOpByte);
                return;
            }

            if (op.Operand.Type == OperandType.Reg8)
            {
                sOpByte |= (byte) s_Reg8Order.IndexOf(op.Operand.Register);
            }
            else if (op.Operand.Type == OperandType.RegIndirect)
            {
                if (op.Operand.Register != "(HL)")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        $"'{op.Mnemonic}' operation can have {op.Operand.Register} as its operand"));
                    return;
                }
                sOpByte |= 0x06;
            }
            asm.EmitBytes(0xCB, sOpByte);
        }

        /// <summary>
        /// OUT operations
        /// </summary>
        private static void ProcessOut(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.MemIndirect)
            {
                if (op.Operand2.Register != "A")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        "'in (port),reg' operation can use only 'a' as its register operand"));
                    return;
                }

                asm.EmitByte(0xD3);
                asm.EmitExpression(op, op.Operand.Expression, FixupType.Bit8);
                return;
            }

            if (op.Operand.Type == OperandType.CPort)
            {
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    asm.EmitOperationWithLookup(s_OutOpBytes, op.Operand2.Register, op);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    var value = asm.EvalImmediate(op, op.Operand2.Expression);
                    if (value == null || value.Value != 0)
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            "Output value can only be 0."));
                        return;
                    }

                    // --- out (c),0
                    asm.EmitDoubleByte(0xED71);
                }
            }
        }

        /// <summary>
        /// IN operations
        /// </summary>
        private static void ProcessIn(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    if (op.Operand.Register != "A")
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            "'in reg,(port)' operation can use only 'a' as its register operand"));
                        return;
                    }

                    // --- in a,(port)
                    asm.EmitByte(0xDB);
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }
            }

            if (op.Operand.Type == OperandType.CPort)
            {
                // --- in (c)
                asm.EmitDoubleByte(0xED70);
                return;
            }

            // --- in reg,(c)
            asm.EmitOperationWithLookup(s_InOpBytes, op.Operand.Register, op);
        }

        /// <summary>
        /// ALU operations: SUB, AND, XOR, OR, CP
        /// </summary>
        private static void ProcessAlu2(Z80Assembler asm, CompoundOperation op)
        {
            var aluIdx = (byte)s_AluOpOrder.IndexOf(op.Mnemonic);
            if (op.Operand.Type == OperandType.Reg8)
            {
                var regIdx = s_Reg8Order.IndexOf(op.Operand.Register);
                asm.EmitByte((byte)(0x80 + (aluIdx << 3) + regIdx));
                return;
            }

            if (op.Operand.Type == OperandType.RegIndirect)
            {
                if (op.Operand.Register != "(HL)")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        $"'{op.Mnemonic}' cannot have {op.Operand.Register} as its operand"));
                    return;
                }
                asm.EmitByte((byte)(0x86 + (aluIdx << 3)));
                return;
            }

            if (op.Operand.Type == OperandType.Reg8Idx)
            {
                asm.EmitByte((byte)(op.Operand.Register.StartsWith("X") ? 0xDD : 0xFD));
                asm.EmitByte((byte)(0x80 + (aluIdx << 3) + (op.Operand.Register.EndsWith("H") ? 4 : 5)));
                return;
            }

            if (op.Operand.Type == OperandType.Expr)
            {
                asm.EmitByte((byte)(0xC6 + (aluIdx << 3)));
                asm.EmitExpression(op, op.Operand.Expression, FixupType.Bit8);
                return;
            }

            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                var opByte = (byte)(0x86 + (aluIdx << 3));
                asm.EmitIndexedOperation(op.Operand, opByte);
            }
        }

        /// <summary>
        /// ALU operations: ADD, ADC, SBC
        /// </summary>
        private static void ProcessAlu1(Z80Assembler asm, CompoundOperation op)
        {
            var aluIdx = (byte)s_AluOpOrder.IndexOf(op.Mnemonic);
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register != "A")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        $"The first 8-bit argument of '{op.Mnemonic}' can only be 'a'"));
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    var regIdx = s_Reg8Order.IndexOf(op.Operand2.Register);
                    asm.EmitByte((byte) (0x80 + (aluIdx << 3) + regIdx));
                    return;
                }

                if (op.Operand2.Type == OperandType.RegIndirect)
                {
                    if (op.Operand2.Register != "(HL)")
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            $"'{op.Mnemonic} a,{op.Operand2.Register}' has an invalid second operand"));
                        return;
                    }
                    asm.EmitByte((byte)(0x86 + (aluIdx << 3)));
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    asm.EmitByte((byte)(op.Operand2.Register.StartsWith("X") ? 0xDD : 0xFD));
                    asm.EmitByte((byte)(0x80 + (aluIdx << 3) + (op.Operand2.Register.EndsWith("H") ? 4 : 5)));
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    asm.EmitByte((byte)(0xC6 + (aluIdx << 3)));
                    asm.EmitExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                if (op.Operand2.Type == OperandType.IndexedAddress)
                {
                    var opByte = (byte)(0x86 + (aluIdx << 3));
                    asm.EmitIndexedOperation(op.Operand2, opByte);
                    return;
                }
            }

            if (op.Operand.Type == OperandType.Reg16)
            {
                if (op.Operand.Type == OperandType.Reg16)
                {
                    if (op.Operand.Register != "HL")
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            $"'{op.Mnemonic}' cannot have {op.Operand.Register} as its first operand."));
                        return;
                    }

                    // --- 16-bit register ALU operations
                    int opCodeBase;
                    switch (op.Mnemonic)
                    {
                        case "ADD":
                            opCodeBase = 0x09;
                            break;
                        case "ADC":
                            opCodeBase = 0xED4A;
                            break;
                        default:
                            opCodeBase = 0xED42;
                            break;
                    }
                    asm.EmitDoubleByte(opCodeBase + (s_Reg16Order.IndexOf(op.Operand2.Register) << 4));
                    return;
                }
            }

            if (op.Operand.Type == OperandType.Reg16Idx)
            {
                var opCode = op.Operand.Register == "IX" ? 0xDD09 : 0xFD09;
                if (op.Operand2.Type == OperandType.Reg16)
                {
                    if (op.Operand2.Register == "HL")
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            $"'{op.Mnemonic} {op.Operand.Register},...' cannot have {op.Operand2.Register} as its second operand."));
                        return;
                    }
                    asm.EmitDoubleByte(opCode + (s_Reg16Order.IndexOf(op.Operand2.Register) << 4));
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg16Idx)
                {
                    if (op.Operand.Register != op.Operand2.Register)
                    {
                        asm._output.Errors.Add(new InvalidArgumentError(op,
                            $"'{op.Mnemonic} {op.Operand.Register},...' cannot have {op.Operand2.Register} as its second operand."));
                        return;
                    }
                    asm.EmitDoubleByte(opCode + 0x20);
                }
            }
        }

        /// <summary>
        /// INC/DEC operation
        /// </summary>
        private static void ProcessIncDec(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.RegIndirect && op.Operand.Register != "(HL)")
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    $"The 'inc ' operation cannot be used with {op.Operand.Register}"));
                return;
            }
            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                var opByte = op.Mnemonic == "INC" ? (byte)0x34 : (byte)0x35;
                asm.EmitIndexedOperation(op.Operand, opByte);
            }
            else
            {
                asm.EmitOperationWithLookup(
                    op.Mnemonic == "INC" ? s_IncOpBytes : s_DecOpBytes,
                    op.Operand.Register, op);
            }
        }

        /// <summary>
        /// EX operation
        /// </summary>
        private static void ProcessEx(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Register == "AF")
            {
                if (op.Operand2.Register != "AF'")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        "The 'ex af' operation should use af' as the second argument"));
                    return;
                }
                asm.EmitByte(0x08);

            }
            else if (op.Operand.Register == "DE")
            {
                if (op.Operand2.Register != "HL")
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        "The 'ex de' operation should use hl as the second argument"));
                    return;
                }
                asm.EmitByte(0xEB);
            }
            else if (op.Operand.Register != "(SP)")
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    "The 'ex' operation should use af, de, or (sp) as its first argument"));
            }
            else
            {
                if (op.Operand2.Register == "HL")
                {
                    asm.EmitByte(0xE3);
                }
                else if (op.Operand2.Register == "IX")
                {
                    asm.EmitBytes(0xDD, 0xE3);
                }
                else if (op.Operand2.Register == "IY")
                {
                    asm.EmitBytes(0xFD, 0xE3); // ex (sp),iy
                }
                else
                {
                    asm._output.Errors.Add(new InvalidArgumentError(op,
                        "The 'ex (sp)' operation should use hl, ix, or iy as its second argument"));
                }
            }

        }

        /// <summary>
        /// RET operation
        /// </summary>
        private static void ProcessRet(Z80Assembler asm, CompoundOperation op)
        {
            var opCode = 0xC9;
            var condIndex = s_ConditionOrder.IndexOf(op.Condition);
            if (condIndex >= 0)
            {
                opCode = 0xC0 + condIndex * 8;
            }
            asm.EmitByte((byte)opCode);
        }

        /// <summary>
        /// CALL operation
        /// </summary>
        private static void ProcessCall(Z80Assembler asm, CompoundOperation op)
        {
            var opCode = 0xCD;
            var condIndex = s_ConditionOrder.IndexOf(op.Condition);
            if (condIndex >= 0)
            {
                opCode = 0xC4 + condIndex * 8;
            }
            asm.EmitByte((byte)opCode);
            asm.EmitExpression(op, op.Operand.Expression, FixupType.Bit16);
        }

        /// <summary>
        /// JP operation
        /// </summary>
        private static void ProcessJp(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Expr)
            {
                // --- Jump to a direct address
                var opCode = 0xC3;
                var condIndex = s_ConditionOrder.IndexOf(op.Condition);
                if (condIndex >= 0)
                {
                    opCode = 0xC2 + condIndex * 8;
                }
                asm.EmitByte((byte)opCode);
                asm.EmitExpression(op, op.Operand.Expression, FixupType.Bit16);
                return;
            }

            if (op.Operand.Type == OperandType.RegIndirect && op.Operand.Register != "(HL)"
                || op.Operand.Type == OperandType.IndexedAddress && op.Operand.Sign != null)
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    "'jp' can be used only with (hl), (ix), or (iy), but no other forms of indirection."));
                return;
            }

            if (op.Condition != null)
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    "'jp' with an indirect target cannot be used with conditions."));
            }

            // --- Jump to a register address
            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                if (op.Operand.Register == "IX") asm.EmitByte(0xDD);
                else if (op.Operand.Register == "IY") asm.EmitByte(0xFD);
            }
            asm.EmitByte(0xE9);
        }

        /// <summary>
        /// JR operation
        /// </summary>
        private static void ProcessJr(Z80Assembler asm, CompoundOperation op)
        {
            var opCode = 0x18;
            var condIndex = s_ConditionOrder.IndexOf(op.Condition);
            if (condIndex >= 0)
            {
                opCode = 0x20 + condIndex * 8;
            }
            asm.EmitJumpRelativeOp(op, op.Operand.Expression, opCode);
        }

        /// <summary>
        /// DJNZ operation
        /// </summary>
        private static void ProcessDjnz(Z80Assembler asm, CompoundOperation op)
        {
            asm.EmitJumpRelativeOp(op, op.Operand.Expression, 0x10);
        }

        /// <summary>
        /// RST operation
        /// </summary>
        private static void ProcessRst(Z80Assembler asm, CompoundOperation op)
        {
            var value = asm.EvalImmediate(op, op.Operand.Expression);
            if (value == null) return;
            if (value > 0x38 || value % 8 != 0)
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    "'rst' can be used only with #00, #08, #10, #18, #20, #28, #30, or #38 arguments. "
                    + $"{value:X4} is invalid."));
                return;
            }
            asm.EmitByte((byte)(0xC7 + value));
        }

        /// <summary>
        /// Process PUSH and POP operations
        /// </summary>
        private static void ProcessStackOp(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Register == "AF'")
            {
                asm._output.Errors.Add(new InvalidArgumentError(op, "POP AF' is invalid."));
                return;
            }
            asm.EmitOperationWithLookup(
                op.Mnemonic == "PUSH" ? s_PushOpBytes : s_PopOpBytes,
                op.Operand.Register, op);

        }

        /// <summary>
        /// Process the IM operation
        /// </summary>
        private static void ProcessImOp(Z80Assembler asm, CompoundOperation op)
        {
            var mode = asm.EvalImmediate(op, op.Operand.Expression);
            if (mode == null) return;

            if (mode < 0 || mode > 2)
            {
                asm._output.Errors.Add(new InvalidArgumentError(op,
                    $"Interrupt mode can only be 0, 1, or 2. '{mode}' is invalid."));
                return;
            }

            var opCodes = new[] { 0xED46, 0xED56, 0xED5E };
            asm.EmitDoubleByte(opCodes[mode.Value]);
        }

        /// <summary>
        /// Reports that the specified source and destination means invalid LD operation
        /// </summary>
        /// <param name="opLine">Assembly line for the load operation</param>
        /// <param name="dest">Load destination</param>
        /// <param name="source">Load source</param>
        private void ReportInvalidLoadOp(SourceLineBase opLine, string dest, string source)
        {
            _output.Errors.Add(new InvalidArgumentError(opLine,
                $"'load {dest},{source}' is an invalid operation."));
        }

        /// <summary>
        /// Emits a jump relative operation
        /// </summary>
        /// <param name="opLine">Control flow operation line</param>
        /// <param name="target">Target expression</param>
        /// <param name="opCode">Operation code</param>
        private void EmitJumpRelativeOp(SourceLineBase opLine, ExpressionNode target, int opCode)
        {
            var value = Eval(target);
            if (target.EvaluationError != null) return;
            var dist = 0;
            if (value == null)
            {
                RecordFixup(FixupType.Jr, target);
            }
            else
            {
                // --- Check for Relative address
                dist = value.Value - (GetCurrentAssemblyAddress() + 2);
                if (dist < -128 || dist > 127)
                {
                    _output.Errors.Add(new RelativeAddressError(opLine, dist));
                    return;
                }
            }
            EmitBytes((byte)opCode, (byte)dist);
        }

        /// <summary>
        /// Emits an indexed operation with the specified operand and operation code
        /// </summary>
        /// <param name="operand">Operand with indexed address</param>
        /// <param name="opCode">Operation code</param>
        private void EmitIndexedOperation(Operand operand, byte opCode)
        {
            byte idxByte, disp;
            var done = GetIndexBytes(operand, out idxByte, out disp);
            EmitBytes(idxByte, opCode);
            if (!done)
            {
                RecordFixup(FixupType.Bit8, operand.Expression);
            }
            EmitByte(disp);
        }

        /// <summary>
        /// Gets the index byte and displacement byte from an indexxed address
        /// </summary>
        /// <param name="operand">Operand with indexed address type</param>
        /// <param name="idxByte">Index byte (0xDD for IX, 0xFD for IY)</param>
        /// <param name="disp">Displacement byte</param>
        /// <returns>
        /// True, if displacement has been resolved; 
        /// false if it can be resolved only during fixup phase
        /// </returns>
        private bool GetIndexBytes(Operand operand, out byte idxByte, out byte disp)
        {
            idxByte = operand.Register == "IX" ? (byte)0xDD : (byte)0xFD;
            disp = 0x00;
            if (operand.Sign == null) return true;

            var dispValue = Eval(operand.Expression);
            if (dispValue == null) return false;
            disp = operand.Sign == "-" 
                ? (byte) -dispValue.Value 
                : (byte) dispValue;
            return true;
        }

        /// <summary>
        /// Emits an indexed operation with the specified operand and operation code
        /// </summary>
        /// <param name="register">Index register</param>
        /// <param name="sign">Displacement sign</param>
        /// <param name="expr">Displacement expression</param>
        /// <param name="opCode">Operation code</param>
        private void EmitIndexedBitOperation(string register, string sign, ExpressionNode expr, byte opCode)
        {
            byte idxByte, disp;
            var done = GetIndexBytes(register, sign, expr, out idxByte, out disp);
            EmitBytes(idxByte, 0xCB, opCode);
            if (!done)
            {
                RecordFixup(FixupType.Bit8, expr);
            }
            EmitByte(disp);
        }

        /// <summary>
        /// Gets the index byte and displacement byte from an indexxed address
        /// </summary>
        /// <param name="register">Index register</param>
        /// <param name="sign">Displacement sign</param>
        /// <param name="expr">Displacement expression</param>
        /// <param name="idxByte">Index byte (0xDD for IX, 0xFD for IY)</param>
        /// <param name="disp">Displacement byte</param>
        /// <returns>
        /// True, if displacement has been resolved; 
        /// false if it can be resolved only during fixup phase
        /// </returns>
        private bool GetIndexBytes(string register, string sign, ExpressionNode expr, out byte idxByte, out byte disp)
        {
            idxByte = register == "IX" ? (byte)0xDD : (byte)0xFD;
            disp = 0x00;
            if (sign == null) return true;

            var dispValue = Eval(expr);
            if (dispValue == null) return false;
            disp = sign == "-"
                ? (byte)-dispValue.Value
                : (byte)dispValue;
            return true;
        }

        /// <summary>
        /// Evaluates the expression and emits bytes accordingly. If the expression
        /// cannot be resolved, creates a fixup.
        /// </summary>
        /// <param name="opLine">Assembly line</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <param name="type">Expression/Fixup type</param>
        /// <returns></returns>
        private void EmitExpression(SourceLineBase opLine, ExpressionNode expr, FixupType type)
        {
            var value = Eval(expr);
            if (value == null)
            {
                if (expr.EvaluationError != null)
                {
                    _output.Errors.Add(new ExpressionEvaluationError(opLine.SourceLine, opLine.Position,
                        "", expr.EvaluationError));
                    return;
                }
                RecordFixup(type, expr);
            }
            var fixupValue = value ?? 0;
            EmitByte((byte)fixupValue);
            if (type == FixupType.Bit16)
            {
                EmitByte((byte)(fixupValue >> 8));
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

        #region Operation rules

        /// <summary>
        /// Represents the rules for the LD operations
        /// </summary>
        private static readonly List<OperandRule> s_LoadRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Spec),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Reg8, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8Idx, OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8Idx, OperandType.Expr),
                new OperandRule(OperandType.Reg8Spec, OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect, OperandType.Expr),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg8),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg16),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16, OperandType.Expr),
                new OperandRule(OperandType.Reg16, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.Reg16, OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16Idx, OperandType.Expr),
                new OperandRule(OperandType.Reg16Idx, OperandType.MemIndirect),
                new OperandRule(OperandType.IndexedAddress, OperandType.Reg8),
                new OperandRule(OperandType.IndexedAddress, OperandType.Expr)
            };

        /// <summary>
        /// Represents a single expression rule
        /// </summary>
        private static readonly List<OperandRule> s_SingleExprRule =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr)
            };

        /// <summary>
        /// Stack operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_StackOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16Spec)
            };

        /// <summary>
        /// Represents a rule set for increment and decrement operations
        /// </summary>
        private static readonly List<OperandRule> s_IncDecOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress)
            };

        /// <summary>
        /// Alu operation rule set (ADD, ADC, SBC)
        /// </summary>
        private static readonly List<OperandRule> s_RsDoubleArgAluRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16Idx),
            };

        /// <summary>
        /// Alu operation rule set (SUB, AND, XOR, OR, CP)
        /// </summary>
        private static readonly List<OperandRule> s_SingleArgAluRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
                new OperandRule(OperandType.Expr),
            };

        /// <summary>
        /// Shift and rotate operations rule set
        /// </summary>
        private static readonly List<OperandRule> s_BitManipRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
                new OperandRule(OperandType.IndexedAddress, OperandType.Reg8),
            };

        /// <summary>
        /// JP operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_JpOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
            };

        /// <summary>
        /// RET operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_RetOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr),
                new OperandRule(OperandType.None)
            };

        /// <summary>
        /// EX operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_ExOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg16Spec, OperandType.Reg16Spec),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg16),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg16Idx)
            };

        /// <summary>
        /// IN operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_InOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg8, OperandType.CPort),
                new OperandRule(OperandType.CPort)
            };

        /// <summary>
        /// OUT operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_OutOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.MemIndirect, OperandType.Reg8),
                new OperandRule(OperandType.CPort, OperandType.Reg8),
                new OperandRule(OperandType.CPort, OperandType.Expr)
            };

        /// <summary>
        /// The table that contains the first level processing rules
        /// </summary>
        private readonly Dictionary<string, CompoundOperationDescriptor> _compoundOpTable =
            new Dictionary<string, CompoundOperationDescriptor>(StringComparer.OrdinalIgnoreCase)
            {
                { "ADC", new CompoundOperationDescriptor(s_RsDoubleArgAluRules, ProcessAlu1) },
                { "ADD", new CompoundOperationDescriptor(s_RsDoubleArgAluRules, ProcessAlu1) },
                { "AND", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "BIT", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "CALL", new CompoundOperationDescriptor(s_SingleExprRule, ProcessCall) },
                { "CP", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "DEC", new CompoundOperationDescriptor(s_IncDecOpRules, ProcessIncDec) },
                { "DJNZ", new CompoundOperationDescriptor(s_SingleExprRule, ProcessDjnz) },
                { "EX", new CompoundOperationDescriptor(s_ExOpRules, ProcessEx) },
                { "IM", new CompoundOperationDescriptor(s_SingleExprRule, ProcessImOp) },
                { "IN", new CompoundOperationDescriptor(s_InOpRules, ProcessIn) },
                { "INC", new CompoundOperationDescriptor(s_IncDecOpRules, ProcessIncDec) },
                { "JP", new CompoundOperationDescriptor(s_JpOpRules, ProcessJp) },
                { "JR", new CompoundOperationDescriptor(s_SingleExprRule, ProcessJr) },
                { "LD", new CompoundOperationDescriptor(s_LoadRules, ProcessLd) },
                { "OR", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "OUT", new CompoundOperationDescriptor(s_OutOpRules, ProcessOut) },
                { "POP", new CompoundOperationDescriptor(s_StackOpRules, ProcessStackOp) },
                { "PUSH", new CompoundOperationDescriptor(s_StackOpRules, ProcessStackOp) },
                { "RES", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "RET", new CompoundOperationDescriptor(s_RetOpRules, ProcessRet) },
                { "RL", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "RLC", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "RR", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "RRC", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "RST", new CompoundOperationDescriptor(s_SingleExprRule, ProcessRst) },
                { "SBC", new CompoundOperationDescriptor(s_RsDoubleArgAluRules, ProcessAlu1) },
                { "SET", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "SLA", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "SLL", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "SRA", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "SRL", new CompoundOperationDescriptor(s_BitManipRules, ProcessShiftRotate) },
                { "SUB", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "XOR", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
            };

        #endregion

        #region Operation lookup tables

        /// <summary>
        /// The order of Z80 shift and rotation operations
        /// </summary>
        private static readonly List<string> s_ShiftOpOrder = new List<string>
        {
            "RLC",
            "RRC",
            "RL",
            "RR",
            "SLA",
            "SRA",
            "SLL",
            "SRL"
        };

        /// <summary>
        /// The order of Z80 ALU operations
        /// </summary>
        private static readonly List<string> s_AluOpOrder = new List<string>
        {
            "ADD",
            "ADC",
            "SUB",
            "SBC",
            "AND",
            "XOR",
            "OR",
            "CP"
        };

        /// <summary>
        /// The index order of 8-bit registers in Z80 operations
        /// </summary>
        private static readonly List<string> s_Reg8Order = new List<string>
        {
            "B",
            "C",
            "D",
            "E",
            "H",
            "L",
            "(HL)",
            "A"
        };

        /// <summary>
        /// The index order of 16-bit registers in Z80 operations
        /// </summary>
        private static readonly List<string> s_Reg16Order = new List<string>
        {
            "BC",
            "DE",
            "HL",
            "SP"
        };

        /// <summary>
        /// The order of Z80 conditions operations
        /// </summary>
        private static readonly List<string> s_ConditionOrder = new List<string>
        {
            "NZ",
            "Z",
            "NC",
            "C",
            "PO",
            "PE",
            "P",
            "M"
        };

        /// <summary>
        /// Z80 binary operation codes for trivial operations
        /// </summary>
        private static readonly Dictionary<string, int> s_TrivialOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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
                {"HALT", 0x76},
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
        private static readonly Dictionary<string, int> s_PushOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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
        private static readonly Dictionary<string, int> s_PopOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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
        private static readonly Dictionary<string, int> s_IncOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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
        private static readonly Dictionary<string, int> s_DecOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
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

        /// <summary>
        /// Z80 IN operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_InOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0xED78},
                {"B", 0xED40},
                {"C", 0xED48},
                {"D", 0xED50},
                {"E", 0xED58},
                {"H", 0xED60},
                {"L", 0xED68},
            };

        /// <summary>
        /// Z80 OUT operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_OutOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0xED79},
                {"B", 0xED41},
                {"C", 0xED49},
                {"D", 0xED51},
                {"E", 0xED59},
                {"H", 0xED61},
                {"L", 0xED69},
            };

        #endregion
    }
}
