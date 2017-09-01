using System;
using System.Collections.Generic;
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
            if (opLine is TrivialOperation)
            {
                EmitTrivialOperation(opLine);
                return;
            }

            // --- Handle push and pop operations with lookup
            var stackOpLine = opLine as StackOperation;
            if (stackOpLine != null)
            {
                EmitStackOperation(stackOpLine);
                return;
            }

            // --- Handle exchange operations (like 'ex de,hl', etc)
            var exOpLine = opLine as ExchangeOperation;
            if (exOpLine != null)
            {
                EmitExchangeOperation(exOpLine);
                return;
            }

            // --- Handle increment/decrement operations
            var incDecOpLine = opLine as IncDecOperation;
            if (incDecOpLine != null)
            {
                EmitIncDecOperation(incDecOpLine);
                return;
            }

            // --- Handle load operation
            var ldOpLine = opLine as LoadOperation;
            if (ldOpLine != null)
            {
                EmitLoadOperation(ldOpLine);
                return;
            }

            // --- Handle ALU operations
            var aluOpLine = opLine as AluOperation;
            if (aluOpLine != null)
            {
                EmitAluOperation(aluOpLine);
                return;
            }

            // --- Handle control flow operations
            var cfOpLine = opLine as ControlFlowOperation;
            if (cfOpLine != null)
            {
                EmitControlFlowOperation(cfOpLine);
                return;
            }

            // --- Handle I/O operations
            var ioOpLine = opLine as IoOperation;
            if (ioOpLine != null)
            {
                EmitIoOperation(ioOpLine);
                return;
            }

            // --- Handle IM operations
            var imOpLine = opLine as InterruptModeOperation;
            if (imOpLine != null)
            {
                EmitInterruptModeOperation(imOpLine);
                return;
            }

            // --- Handle bit operations
            var bitOpLine = opLine as BitOperation;
            if (bitOpLine != null)
            {
                EmitBitOperation(bitOpLine);
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
        /// Assembly line for Trivial operation
        /// </param>
        private void EmitTrivialOperation(OperationBase opLine)
        {
            EmitOperationWithLookup(s_TrivialOpBytes, opLine.Mnemonic, opLine);
        }

        /// <summary>
        /// Emits code for trivial operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for a trivial operation
        /// </param>
        private void EmitStackOperation(StackOperation opLine)
        {
            EmitOperationWithLookup(
                opLine.Mnemonic == "PUSH" ? s_PushOpBytes : s_PopOpBytes,
                opLine.Register, opLine);
        }

        /// <summary>
        /// Emits code for exchange operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for an exchange operation
        /// </param>
        private void EmitExchangeOperation(ExchangeOperation opLine)
        {
            if (opLine.Destination == "AF") EmitByte(0x08); // ex af,af'
            else if (opLine.Destination == "DE") EmitByte(0xEB); // ex de,hl
            else
            {
                if (opLine.Source == "HL") EmitByte(0xE3); // ex (sp),hl
                else if (opLine.Source == "IX") EmitBytes(0xDD, 0xE3); // ex (sp),ix
                else EmitBytes(0xFD, 0xE3); // ex (sp),iy
            }
        }

        /// <summary>
        /// Emits code for increment/decrement operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for increment/decrement operation
        /// </param>
        private void EmitIncDecOperation(IncDecOperation opLine)
        {
            if (opLine.Operand.AddressingType == AddressingType.Register)
            {
                EmitOperationWithLookup(
                    opLine.Mnemonic == "INC" ? s_IncOpBytes : s_DecOpBytes,
                    opLine.Operand.Register, opLine);
                return;
            }

            if (opLine.Operand.AddressingType == AddressingType.IndexedAddress)
            {
                var opByte = opLine.Mnemonic == "INC" ? (byte) 0x34 : (byte) 0x35; 
                EmitIndexedOperation(opLine, opLine.Operand, opByte);
                return;
            }

            _output.Errors.Add(new UnexpectedSourceCodeLineError(opLine,
                $"Unexpected {opLine.Mnemonic} operation with addressing type '{opLine.Operand.AddressingType}'"));
        }

        /// <summary>
        /// Emits code for load operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for load operation
        /// </param>
        private void EmitLoadOperation(LoadOperation opLine)
        {
            if (opLine.DestinationOperand.AddressingType == AddressingType.Register)
            {
                // --- Destination is a register
                var destReg = opLine.DestinationOperand.Register;
                var destRegIdx = s_Reg8Order.IndexOf(destReg);

                if (opLine.SourceOperand.AddressingType == AddressingType.Register)
                {
                    // --- Source is a register
                    var sourceReg = opLine.SourceOperand.Register;
                    var sourceRegIdx = s_Reg8Order.IndexOf(sourceReg);

                    if (destRegIdx >= 0)
                    {
                        // --- Destination: standard 8-bit register
                        if (sourceRegIdx >= 0)
                        {
                            if (destRegIdx == 6 && sourceRegIdx == 6)
                            {
                                ReportInvalidLoadOp(opLine, "(hl)", "(hl)");
                                return;
                            }
                            // --- Source: standard 8-bit register
                            EmitByte((byte) (0x40 + (destRegIdx << 3) + sourceRegIdx));
                            return;
                        }

                        if (sourceReg.StartsWith("X") || sourceReg.StartsWith("Y"))
                        {
                            // --- Source must be one of the indexed 8-bit registers
                            if (destRegIdx >= 4 && destRegIdx <= 6)
                            {
                                // --- Deny invalid destination: h, l, (hl)
                                ReportInvalidLoadOp(opLine, destReg, sourceReg);
                                return;
                            }

                            var opCode = sourceReg.StartsWith("X") ? 0xDD44 : 0xFD44;
                            EmitDoubleByte(opCode + (destRegIdx << 3) + (sourceReg.EndsWith("H") ? 0 : 1));
                            return;
                        }
                    }

                    if (destReg.StartsWith("X") || destReg.StartsWith("Y"))
                    {
                        // ld 'xh|xl|yh|yl', reg
                        if (sourceRegIdx >= 0)
                        {
                            // --- Source: standard 8-bit register
                            if (sourceRegIdx >= 4 && sourceRegIdx <= 6)
                            {
                                ReportInvalidLoadOp(opLine, destReg, sourceReg);
                                return;
                            }

                            // --- ld 'xh|xl|yh|yl', 'b|c|d|e|a'
                            var opBytes = destReg.StartsWith("X") ? 0xDD60 : 0xFD60;
                            EmitDoubleByte(opBytes + (destReg.EndsWith("H") ? 0 : 8) + sourceRegIdx);
                            return;
                        }

                        if (sourceReg[0] != destReg[0])
                        {
                            ReportInvalidLoadOp(opLine, destReg, sourceReg);
                            return;
                        }

                        // ld 'xh|xl|yh|yl', 'xh|xl|yh|yl'
                        var xopBytes = destReg.StartsWith("X") ? 0xDD64 : 0xFD64;
                        EmitDoubleByte(xopBytes + (destReg.EndsWith("H") ? 0 : 8) 
                            + (sourceReg.EndsWith("H") ? 0 : 1));
                        return;
                    }

                    // --- Spect 8-bit load operations
                    if (destReg == "I" && sourceReg == "A")
                    {
                        // --- ld i,a
                        EmitBytes(0xED, 0x47);
                        return;
                    }
                    if (destReg == "R" && sourceReg == "A")
                    {
                        // --- ld r,a
                        EmitBytes(0xED, 0x4F);
                        return;
                    }
                    if (destReg == "A" && sourceReg == "I")
                    {
                        // --- ld a,i
                        EmitBytes(0xED, 0x57);
                        return;
                    }
                    if (destReg == "A" && sourceReg == "R")
                    {
                        // --- ld a,r
                        EmitBytes(0xED, 0x5F);
                        return;
                    }

                    // --- ld sp,'hl|ix|iy' operations
                    if (destReg == "SP")
                    {
                        switch (sourceReg)
                        {
                            case "HL": EmitByte(0xF9);
                                break;
                            case "IX":
                                EmitDoubleByte(0xDDF9);
                                break;
                            case "IY":
                                EmitDoubleByte(0xFDF9);
                                break;
                            default:
                                ReportInvalidLoadOp(opLine, destReg, sourceReg);
                                break;
                        }
                        return;
                    }
                }

                if (opLine.SourceOperand.AddressingType == AddressingType.Expression)
                {
                    // --- The destination is a register, the source is an expression
                    if (destRegIdx >= 0)
                    {
                        // --- Standard 8-bit register
                        EmitByte((byte)(0x06 + (destRegIdx << 3)));
                        EmitExpression(opLine, opLine.SourceOperand.Expression, FixupType.Bit8);
                        return;
                    }

                    // --- Get the opcode according to the destination register
                    int opCode = 0x00;
                    var fixupType = FixupType.Bit8;
                    switch (destReg)
                    {
                        case "XH":
                            opCode = 0xDD26;
                            break;
                        case "XL":
                            opCode = 0xDD2E;
                            break;
                        case "YH":
                            opCode = 0xFD26;
                            break;
                        case "YL":
                            opCode = 0xFD2E;
                            break;
                        case "BC":
                            opCode = 0x01;
                            fixupType = FixupType.Bit16;
                            break;
                        case "DE":
                            opCode = 0x11;
                            fixupType = FixupType.Bit16;
                            break;
                        case "HL":
                            opCode = 0x21;
                            fixupType = FixupType.Bit16;
                            break;
                        case "SP":
                            opCode = 0x31;
                            fixupType = FixupType.Bit16;
                            break;
                        case "IX":
                            opCode = 0xDD21;
                            fixupType = FixupType.Bit16;
                            break;
                        case "IY":
                            opCode = 0xFD21;
                            fixupType = FixupType.Bit16;
                            break;
                        default:
                            ReportInvalidLoadOp(opLine, destReg, "<expression>");
                            break;
                    }
                    EmitDoubleByte(opCode);
                    EmitExpression(opLine, opLine.SourceOperand.Expression, fixupType);
                    return;
                }

                if (opLine.SourceOperand.AddressingType == AddressingType.RegisterIndirection)
                {
                    // --- The source is a register indirection
                    // --- ld a,(bc) or ld a,(de) -- we handled ld a,(hl) as an 8-bit-reg-to-8-bit-reg ld op
                    EmitByte(opLine.SourceOperand.Register == "BC" ? (byte)0x0A : (byte)0x1A);
                    return;
                }

                if (opLine.SourceOperand.AddressingType == AddressingType.IndexedAddress)
                {
                    // --- ld '8-bit-reg', '(idxreg+disp)' operation
                    var opCode = (byte)(0x46 + (destRegIdx << 3));
                    EmitIndexedOperation(opLine, opLine.SourceOperand, opCode);
                    return;
                }

                if (opLine.SourceOperand.AddressingType == AddressingType.AddressIndirection)
                {
                    // --- ld 'reg',(address) operation
                    int opCode;
                    switch (opLine.DestinationOperand.Register)
                    {
                        case "A":
                            opCode = 0x3A;
                            break;
                        case "BC":
                            opCode = 0xED4B;
                            break;
                        case "DE":
                            opCode = 0xED5B;
                            break;
                        case "HL":
                            opCode = 0x2A;
                            break;
                        case "SP":
                            opCode = 0xED7B;
                            break;
                        case "IX":
                            opCode = 0xDD2A;
                            break;
                        case "IY":
                            opCode = 0xFD2A;
                            break;
                        default:
                            ReportInvalidLoadOp(opLine, destReg, "(<expression>)");
                            return;
                    }
                    EmitDoubleByte(opCode);
                    EmitExpression(opLine, opLine.SourceOperand.Expression, FixupType.Bit16);
                    return;
                }
            }

            if (opLine.DestinationOperand.AddressingType == AddressingType.RegisterIndirection)
            {
                // --- ld (bc),a and ld (de),a
                EmitByte(opLine.DestinationOperand.Register == "BC" ? (byte) 0x02 : (byte) 0x12);
                return;
            }

            if (opLine.DestinationOperand.AddressingType == AddressingType.AddressIndirection)
            {
                // --- ld (address),'reg' operation
                int opCode;
                switch (opLine.SourceOperand.Register)
                {
                    case "A":
                        opCode = 0x32;
                        break;
                    case "BC":
                        opCode = 0xED43;
                        break;
                    case "DE":
                        opCode = 0xED53;
                        break;
                    case "HL":
                        opCode = 0x22;
                        break;
                    case "SP":
                        opCode = 0xED73;
                        break;
                    case "IX":
                        opCode = 0xDD22;
                        break;
                    case "IY":
                        opCode = 0xFD22;
                        break;
                    default:
                        ReportInvalidLoadOp(opLine, "(<expression>)", opLine.SourceOperand.Register);
                        return;
                }
                EmitDoubleByte(opCode);
                EmitExpression(opLine, opLine.DestinationOperand.Expression, FixupType.Bit16);
                return;
            }

            if (opLine.DestinationOperand.AddressingType == AddressingType.IndexedAddress)
            {
                if (opLine.SourceOperand.AddressingType == AddressingType.Register)
                {
                    // --- ld '(idxreg+disp)','8bitReg'
                    var opCode = (byte)(0x70 + s_Reg8Order.IndexOf(opLine.SourceOperand.Register));
                    EmitIndexedOperation(opLine, opLine.DestinationOperand, opCode);
                    return;
                }
                if (opLine.SourceOperand.AddressingType == AddressingType.Expression)
                {
                    // --- ld '(idxreg+disp)','expr'
                    EmitIndexedOperation(opLine, opLine.DestinationOperand, 0x36);
                    EmitExpression(opLine, opLine.SourceOperand.Expression, FixupType.Bit8);
                    return;
                }

                ReportInvalidLoadOp(opLine, "(ix+disp)", $"<{opLine.SourceOperand.AddressingType}>");
                return;
            }

            // --- Just for the sake of safety
            ReportInvalidLoadOp(opLine, $"<{opLine.DestinationOperand.AddressingType}>", 
                $"<{opLine.SourceOperand.AddressingType}>");
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
        /// Emits code for ALU operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for ALU operation
        /// </param>
        private void EmitAluOperation(AluOperation opLine)
        {
            var aluIdx = (byte)s_AluOpOrder.IndexOf(opLine.Mnemonic);

            if (opLine.Register != null)
            {
                // --- 16-bit register ALU operations
                if (opLine.Register == "HL")
                {
                    int opCodeBase;
                    switch (opLine.Mnemonic)
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
                    if (opLine.Operand.AddressingType == AddressingType.Register)
                    {
                        // --- Operand is normal 16-bit register
                        EmitDoubleByte(opCodeBase + (s_Reg16Order.IndexOf(opLine.Operand.Register) << 4));
                        return;
                    }
                }
                else
                {
                    // --- 16-bit index register ALU operations
                    var prefix = opLine.Register == "IX" ? 0xDD09 : 0xFD09;
                    if (opLine.Operand.Register.StartsWith("I"))
                    {
                        // --- Both operands are index registers
                        if (opLine.Register != opLine.Operand.Register)
                        {
                            _output.Errors.Add(new InvalidArgumentError(opLine,
                                $"'add {opLine.Register},{opLine.Operand.Register}' is an invalid operation."));
                            return;
                        }
                        EmitDoubleByte(prefix + 0x20);
                    }
                    else
                    {
                        // --- Second operand is normal 16-bit register
                        EmitDoubleByte(prefix + (s_Reg16Order.IndexOf(opLine.Operand.Register) << 4));
                    }
                    return;
                }
            }

            if (opLine.Operand.AddressingType == AddressingType.Register)
            {
                var regIdx = s_Reg8Order.IndexOf(opLine.Operand.Register);
                if (regIdx >= 0)
                {
                    // --- Standard 8-bit register
                    EmitByte((byte)(0x80 + (aluIdx << 3) + regIdx));
                }
                else
                {
                    // --- Indexed 8-bit register
                    EmitByte((byte)(opLine.Operand.Register.StartsWith("X") ? 0xDD : 0xFD));
                    EmitByte((byte)(0x80 + (aluIdx << 3) + (opLine.Operand.Register.EndsWith("H") ? 4 : 5)));
                }
                return;
            }

            if (opLine.Operand.AddressingType == AddressingType.Expression)
            {
                // --- ALU operation with expression
                EmitByte((byte)(0xC6 + (aluIdx << 3)));
                EmitExpression(opLine, opLine.Operand.Expression, FixupType.Bit8);
                return;
            }

            if (opLine.Operand.AddressingType == AddressingType.IndexedAddress)
            {
                // --- ALU operation with indexed address
                var opByte = (byte)(0x86 + (aluIdx << 3));
                EmitIndexedOperation(opLine, opLine.Operand, opByte);
            }
        }

        /// <summary>
        /// Emits code for control flow operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for control flow operation
        /// </param>
        private void EmitControlFlowOperation(ControlFlowOperation opLine)
        {
            if (opLine.Mnemonic == "DJNZ")
            {
                EmitJumpRelativeOp(opLine, opLine.Target, 0x10);
                return;
            }

            if (opLine.Mnemonic == "JR")
            {
                var opCode = 0x18;
                var condIndex = s_ConditionOrder.IndexOf(opLine.Condition);
                if (condIndex >= 0)
                {
                    opCode = 0x20 + condIndex * 8;
                }
                EmitJumpRelativeOp(opLine, opLine.Target, opCode);
                return;
            }

            if (opLine.Mnemonic == "JP")
            {
                if (opLine.Target != null)
                {
                    // --- Jump to a direct address
                    var opCode = 0xC3;
                    var condIndex = s_ConditionOrder.IndexOf(opLine.Condition);
                    if (condIndex >= 0)
                    {
                        opCode = 0xC2 + condIndex * 8;
                    }
                    EmitByte((byte)opCode);
                    EmitExpression(opLine, opLine.Target, FixupType.Bit16);
                    return;
                }

                // --- Jump to a register address
                if (opLine.Register.EndsWith("X")) EmitByte(0xDD);
                else if (opLine.Register.EndsWith("Y")) EmitByte(0xFD);
                EmitByte(0xE9);
                return;
            }

            if (opLine.Mnemonic == "CALL")
            {
                var opCode = 0xCD;
                var condIndex = s_ConditionOrder.IndexOf(opLine.Condition);
                if (condIndex >= 0)
                {
                    opCode = 0xC4 + condIndex * 8;
                }
                EmitByte((byte) opCode);
                EmitExpression(opLine, opLine.Target, FixupType.Bit16);
                return;
            }

            if (opLine.Mnemonic == "RET")
            {
                var opCode = 0xC9;
                var condIndex = s_ConditionOrder.IndexOf(opLine.Condition);
                if (condIndex >= 0)
                {
                    opCode = 0xC0 + condIndex * 8;
                }
                EmitByte((byte)opCode);
                return;
            }

            if (opLine.Mnemonic == "RST")
            {
                var value = EvalImmediate(opLine, opLine.Target);
                if (value == null) return;
                if (value > 0x38 || value % 8 != 0)
                {
                    _output.Errors.Add(new InvalidArgumentError(opLine,
                        "'rst' can be used only with #00, #08, #10, #18, #20, #28, #30, or #38 arguments. "
                        + $"{value:X4} is invalid."));
                    return;
                }
                EmitByte((byte)(0xC7 + value));
            }
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
        /// Emits code for I/O operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for I/O operation
        /// </param>
        private void EmitIoOperation(IoOperation opLine)
        {
            if (opLine.Mnemonic == "IN")
            {
                // --- IN operations
                if (opLine.Port != null)
                {
                    // --- in a,(port)
                    EmitByte(0xDB);
                    EmitExpression(opLine, opLine.Port, FixupType.Bit8);
                    return;
                }

                if (opLine.Register == null)
                {
                    // --- in (c)
                    EmitDoubleByte(0xED70);
                    return;
                }

                // --- in reg,(c)
                EmitOperationWithLookup(s_InOpBytes, opLine.Register, opLine);
                return;
            }

            // --- OUT operations
            if (opLine.Port != null)
            {
                // --- out (port),a
                EmitByte(0xD3);
                EmitExpression(opLine, opLine.Port, FixupType.Bit8);
                return;
            }

            if (opLine.Register == null)
            {
                if (opLine.Value != 0)
                {
                    _output.Errors.Add(new InvalidArgumentError(opLine,
                        $"Output value can only be 0. '{opLine.Value}' is invalid."));
                    return;
                }
                // --- out (c),0
                EmitDoubleByte(0xED71);
                return;
            }

            // --- out (c),reg
            EmitOperationWithLookup(s_OutOpBytes, opLine.Register, opLine);
        }

        /// <summary>
        /// Emits code for interrupt mode operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for an interrupt mode operation
        /// </param>
        private void EmitInterruptModeOperation(InterruptModeOperation opLine)
        {
            int mode;
            if (!int.TryParse(opLine.Mode, out mode))
            {
                _output.Errors.Add(new UnexpectedSourceCodeLineError(opLine,
                    $"Unexpected interrup mode '{opLine.Mode}' string"));
                return;
            }

            if (mode < 0 || mode > 2)
            {
                _output.Errors.Add(new InvalidArgumentError(opLine,
                    $"Interrupt mode can only be 0, 1, or 2. '{opLine.Mode}' is invalid."));
                return;
            }

            var opCodes = new[] {0xED46, 0xED56, 0xED5E};
            EmitDoubleByte(opCodes[mode]);
        }

        /// <summary>
        /// Emits code for bit operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line for a bit operation
        /// </param>
        private void EmitBitOperation(BitOperation opLine)
        {
            switch (opLine.Mnemonic)
            {
                case "BIT":
                case "RES":
                case "SET":
                    // --- Base operation code
                    byte opByte;
                    switch (opLine.Mnemonic)
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
                    var bitIndex = Eval(opLine.BitIndex);
                    if (bitIndex == null)
                    {
                        _output.Errors.Add(new InvalidArgumentError(opLine,
                            "Bit index cannot be evaluated. It may contain a symbol that cannot be resolved right now."));
                        return;
                    }
                    if (bitIndex < 0 || bitIndex > 7)
                    {
                        _output.Errors.Add(new InvalidArgumentError(opLine,
                            $"Bit index should be between 0 and 7. '{bitIndex}' is invalid."));
                        return;
                    }

                    // --- Obtain the register index, provided the register has been specified
                    var regIdx = opLine.Register == null 
                        ? (opLine.Mnemonic == "BIT" ? 0x00 : 0x06)
                        : s_Reg8Order.IndexOf(opLine.Register);
                    if (regIdx < 0)
                    {
                        _output.Errors.Add(new UnexpectedSourceCodeLineError(opLine,
                            $"Register '{opLine.Register}' is unexpected in this context."));
                        return;
                    }

                    // --- Calculate the operation code
                    opByte |= (byte)((bitIndex << 3) | regIdx);

                    if (opLine.IndexRegister == null)
                    {
                        // --- Standard BIT/RES/SET operations
                        EmitBytes(0xCB, opByte);
                    }
                    else
                    {
                        // --- Indexed BIT/RES/SET operations
                        EmitIndexedBitOperation(opLine.IndexRegister, opLine.Sign, opLine.Displacement, opByte);
                    }
                    break;
                case "RLC":
                case "RRC":
                case "RL":
                case "RR":
                case "SLA":
                case "SRA":
                case "SLL":
                case "SRL":
                    var sOpByte = (byte)(8 * s_ShiftOpOrder.IndexOf(opLine.Mnemonic));
                    var sRegIdx = opLine.Register == null
                        ? 0x06
                        : s_Reg8Order.IndexOf(opLine.Register);
                    sOpByte |= (byte)sRegIdx;
                    if (opLine.IndexRegister == null)
                    {
                        // --- Standard shift/rotate operations
                        EmitBytes(0xCB, sOpByte);
                    }
                    else
                    {
                        // --- Indexed shift/rotate operations
                        EmitIndexedBitOperation(opLine.IndexRegister, opLine.Sign, opLine.Displacement, sOpByte);
                    }
                    break;
            }
        }

        /// <summary>
        /// Emits an indexed operation with the specified operand and operation code
        /// </summary>
        /// <param name="opLine">Assembly line for the operation</param>
        /// <param name="operand">Operand with indexed address</param>
        /// <param name="opCode">Operation code</param>
        private void EmitIndexedOperation(SourceLineBase opLine, Operand operand, byte opCode)
        {
            if (operand.AddressingType != AddressingType.IndexedAddress)
            {
                _output.Errors.Add(new UnexpectedSourceCodeLineError(opLine,
                    $"Unexpected addressing type '{operand.AddressingType}'"));
                return;
            }
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
