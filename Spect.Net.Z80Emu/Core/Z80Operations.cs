// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides standard CPU operations
    /// for direct (with no prefix) execution
    /// </summary>
    public partial class Z80
    {
        /// <summary>
        /// Standard (non-prefixed) operations jump table
        /// </summary>
        private Action<byte>[] _standarOperations;

        /// <summary>
        /// Processes the operations withno op code prefix
        /// </summary>
        /// <param name="opCode">Operation code</param>
        private void ProcessStandardOperations(byte opCode)
        {
            var opMethod = IndexMode == OpIndexMode.None 
                ? _standarOperations[opCode] 
                : _indexedOperations[opCode];
            opMethod?.Invoke(opCode);
        }

        /// <summary>
        /// Initializes the standard operation execution tables
        /// </summary>
        private void InitializeNormalOpsExecutionTable()
        {
            _standarOperations = new Action<byte>[]
            {
                null,      LD_QQ_NN,  LD_QQi_A,  INC_QQ,    INC_Q,     DEC_Q,     LD_Q_N,    RLCA,      // 00..07
                EX_AF,     ADD_HL_RR, LD_A_RRi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    RRCA,      // 08..0F
                DJNZ,      LD_QQ_NN,  LD_QQi_A,  INC_QQ,    INC_Q,     DEC_Q,     LD_Q_N,    RLA,       // 10..17
                JR_E,      ADD_HL_RR, LD_A_RRi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    RRA,       // 18..1F
                JR_X_E,    LD_QQ_NN,  LD_NNi_HL, INC_QQ,    INC_Q,     DEC_Q,     LD_Q_N,    DAA,       // 20..27
                JR_X_E,    ADD_HL_RR, LD_HL_NNi, DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    CPL,       // 28..2F
                JR_X_E,    LD_QQ_NN,  LD_NN_A,   INC_QQ,    INC_HLi,   DEC_HLi,   LD_HLi_N,  SCF,       // 30..37
                JR_X_E,    ADD_HL_RR, LD_A_NNi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    CCF,       // 38..3F

                null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 40..47
                LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 48..4F
                LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 50..57
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 58..5F
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 60..67
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_R_HLi,   LD_Rd_Rs, // 68..6F
                LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  HALT,       LD_HLi_R, // 70..77
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   null,     // 78..7F

                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 80..87
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 88..8F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 90..97
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 98..9F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // A0..A7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // A8..AF
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // B0..B7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // B8..BF

                RET_X,     POP_RR,    JP_X_NN,   JP_NN,     CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // C0..C7
                RET_X,     RET,       JP_X_NN,   null,      CALL_X_NN, CALL_NN,   ALU_A_N,    RST_N,    // C8..CF
                RET_X,     POP_RR,    JP_X_NN,   OUT_NN_A,  CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // D0..D7
                RET_X,     EXX,       JP_X_NN,   IN_A_NN,   CALL_X_NN, null,      ALU_A_N,    RST_N,    // D8..DF
                RET_X,     POP_RR,    JP_X_NN,   EX_SPi_HL, CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // E0..E7
                RET_X,     JP_HL,     JP_X_NN,   EX_DE_HL,  CALL_X_NN, null,      ALU_A_N,    RST_N,    // E8..EF
                RET_X,     POP_RR,    JP_X_NN,   DI,        CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // F0..F7
                RET_X,     LD_SP_HL,  JP_X_NN,   EI,        CALL_X_NN, null,      ALU_A_N,    RST_N,    // F8..FF
            };
        }

        /// <summary>
        /// "LD QQ,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the QQ register pair.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 0 | 0 | 0 | 1 | 
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LD_QQ_NN(byte opCode)
        {
            var qq = Get16BitRegisterIndex(opCode);
            var nn = Get16BitFromCode();
            Registers[qq] = nn;
        }

        /// <summary>
        /// "LD (QQ),A" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the A are loaded to the memory location 
        /// specified by the contents of the register pair QQ.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 0 | 0 | 1 | 0 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=N/A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_QQi_A(byte opCode)
        {
            WriteMemory(Registers[Get16BitRegisterIndex(opCode)], Registers.A);
            Registers.MH = Registers.A;
            ClockP3();
        }

        /// <summary>
        /// "INC QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair QQ are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 0 | 0 | 1 | 1 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void INC_QQ(byte opCode)
        {
            Registers[Get16BitRegisterIndex(opCode)]++;
            ClockP2();
        }

        /// <summary>
        /// "INC Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register Q is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | Q | 1 | 0 | 0 |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void INC_Q(byte opCode)
        {
            var q = Get8BitRegisterIndex(opCode);
            var val = Registers[q]++;
            Registers.F = (byte) (IncOpFlags[val] | Registers.F & (byte) FlagsSetMask.C);
        }

        /// <summary>
        /// "DEC Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register Q is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | Q | 1 | 0 | 1 |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void DEC_Q(byte opCode)
        {
            var q = Get8BitRegisterIndex(opCode);
            var val = Registers[q]--;
            Registers.F = (byte)(DecOpFlags[val] | Registers.F & (byte)FlagsSetMask.C);
        }

        /// <summary>
        /// "LD Q,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer is loaded to Q.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | Q | 1 | 1 | 0 |
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_Q_N(byte opCode)
        {
            var q = Get8BitRegisterIndex(opCode);
            Registers[q] = Get8BitFromCode();
        }

        /// <summary>
        /// "RLCA" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of  A are rotated left 1 bit position. The 
        /// sign bit (bit 7) is copied to the Carry flag and also 
        /// to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void RLCA(byte opCode)
        {
            int rlcaVal = Registers.A;
            rlcaVal <<= 1;
            var cf = (byte)((rlcaVal & 0x100) != 0 ? FlagsSetMask.C : 0);
            if (cf != 0)
            {
                rlcaVal = (rlcaVal | 0x01) & 0xFF;
            }
            Registers.A = (byte)rlcaVal;
            Registers.F = (byte)(cf | (Registers.F & (byte)(FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV)));
        }

        /// <summary>
        /// "EX AF,AF'" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 2-byte contents of the register pairs AF and AF' are exchanged.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void EX_AF(byte opCode)
        {
            Registers.ExchangeAfSet();
        }

        /// <summary>
        /// "ADD HL,RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of RR are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 1 | 0 | 0 | 1 |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void ADD_HL_RR(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers[Get16BitRegisterIndex(opCode)]);
            ClockP7();
        }

        /// <summary>
        /// "LD A,(RR)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory location specified by RR are loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 1 | 0 | 1 | 0 |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=N/A, 11=N/A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_A_RRi(byte opCode)
        {
            var rrval = Registers[(Reg16Index)((opCode & 0x30) >> 4)];
            Registers.MW = (ushort)(rrval + 1);
            Registers.A = ReadMemory(rrval, false);
            ClockP3();
        }

        /// <summary>
        /// "DEC RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair RR are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 1 | 0 | 1 | 1 |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DEC_RR(byte opCode)
        {
            var rr = (Reg16Index)((opCode & 0x30) >> 4);
            Registers[rr] -= 1;
            ClockP2();
        }

        /// <summary>
        /// "RRCA" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are rotated right 1 bit position. Bit 0 is 
        /// copied to the Carry flag and also to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void RRCA(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DJNZ E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction is similar to the conditional jump
        /// instructions except that value of B is used to determine 
        /// branching. B is decremented, and if a nonzero value remains,
        /// the value of displacement E is added to PC. The next 
        /// instruction is fetched from the location designated by 
        /// the new contents of the PC. The jump is measured from the 
        /// address of the instruction op code and contains a range of 
        /// –126 to +129 bytes. The assembler automatically adjusts for
        /// the twice incremented PC. If the result of decrementing leaves 
        /// B with a zero value, the next instruction executed is taken 
        /// from the location following this instruction.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 |
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: B!=0: 5, 3, 5 (13)
        ///           B=0:  5, 3 (8)
        /// </remarks>
        private void DJNZ(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "RLA" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are rotated left 1 bit position through the
        /// Carry flag. The previous contents of the Carry flag are copied
        ///  to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void RLA(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "JR E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for unconditional branching 
        /// to other segments of a program. The value of displacement E is 
        /// added to PC and the next instruction is fetched from the location 
        /// designated by the new contents of the PC. This jump is measured
        /// from the address of the instruction op code and contains a range 
        /// of –126 to +129 bytes. The assembler automatically adjusts for 
        /// the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 |
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: 4, 3, 5 (12)
        /// </remarks>
        private void JR_E(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "RRA" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are rotated right 1 bit position through the
        /// Carry flag. The previous contents of the Carry flag are copied 
        /// to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void RRA(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "JR X,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (X). If the test evaluates to *true*, the value of displacement
        /// E is added to PC and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is 
        /// measured from the address of the instruction op code and contains 
        /// a range of –126 to +129 bytes. The assembler automatically adjusts
        /// for the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | X | X | 0 | 0 | 0 | 
        /// =================================
        /// |             E-2               |
        /// =================================
        /// X: 00=Z, 01=NZ, 10=NC, 11=C
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JR_X_E(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (NN),HL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the low-order portion of HL (L) are loaded to memory
        /// address (NN), and the contents of the high-order portion of HL (H) 
        /// are loaded to the next highest memory address(NN + 1).
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3, 3 (16)
        /// </remarks>
        private void LD_NNi_HL(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            WriteMemory(adr, Registers.L);
            ClockP3();
            WriteMemory(Registers.MW, Registers.H);
            ClockP3();
        }

        /// <summary>
        /// "DAA" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction conditionally adjusts A for BCD addition 
        /// and subtraction operations. For addition(ADD, ADC, INC) or 
        /// subtraction(SUB, SBC, DEC, NEG), the following table indicates 
        /// the operation being performed:
        /// 
        /// ====================================================
        /// |Oper.|C before|Upper|H before|Lower|Number|C after|
        /// |     |DAA     |Digit|DAA     |Digit|Added |DAA    |
        /// ====================================================
        /// | ADD |   0    | 9-0 |   0    | 0-9 |  00  |   0   |
        /// |     |   0    | 0-8 |   0    | A-F |  06  |   0   |
        /// |     |   0    | 0-9 |   1    | 0-3 |  06  |   0   |
        /// |     |   0    | A-F |   0    | 0-9 |  60  |   1   |
        /// ---------------------------------------------------- 
        /// | ADC |   0    | 9-F |   0    | A-F |  66  |   1   |
        /// ---------------------------------------------------- 
        /// | INC |   0    | A-F |   1    | 0-3 |  66  |   1   |
        /// |     |   1    | 0-2 |   0    | 0-9 |  60  |   1   |
        /// |     |   1    | 0-2 |   0    | A-F |  66  |   1   |
        /// |     |   1    | 0-3 |   1    | 0-3 |  66  |   1   |
        /// ---------------------------------------------------- 
        /// | SUB |   0    | 0-9 |   0    | 0-9 |  00  |   0   |
        /// ---------------------------------------------------- 
        /// | SBC |   0    | 0-8 |   1    | 6-F |  FA  |   0   |
        /// ---------------------------------------------------- 
        /// | DEC |   1    | 7-F |   0    | 0-9 |  A0  |   1   |
        /// ---------------------------------------------------- 
        /// | NEG |   1    | 6-7 |   1    | 6-F |  9A  |   1   |
        /// ====================================================
        ///
        /// S is set if most-significant bit of the A is 1 after an 
        /// operation; otherwise, it is reset.
        /// Z is set if A is 0 after an operation; otherwise, it is reset.
        /// H: see the DAA instruction table.
        /// P/V is set if A is at even parity after an operation; 
        /// otherwise, it is reset.
        /// N is not affected.
        /// C: see the DAA instruction table.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DAA(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD HL,(NN)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of memory address (NN) are loaded to the 
        /// low-order portion of HL (L), and the contents of the next 
        /// highest memory address (NN + 1) are loaded to the high-order
        /// portion of HL (H).
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3, 3 (16)
        /// </remarks>
        private void LD_HL_NNi(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            ushort val = ReadMemory(adr, false);
            ClockP3();
            val += (ushort)(ReadMemory(Registers.MW, false) * 0x100);
            ClockP3();
            Registers.HL = val;
        }

        /// <summary>
        /// "CPL " operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are inverted (one's complement).
        /// 
        /// S, Z, P/V, C are not affected.
        /// H and N are set.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void CPL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (NN),A" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to the memory address specified by 
        /// the operand NN
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LD_NN_A(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(((adr + 1) & 0xFF) + (Registers.A << 8));
            WriteMemory(adr, Registers.A);
            Registers.MH = Registers.A;
            ClockP3();
        }

        /// <summary>
        /// "INC (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The byte contained in the address specified by the contents HL
        /// is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if (HL) was 0x7F before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void INC_HLi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The byte contained in the address specified by the contents HL
        /// is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if (HL) was 0x80 before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void DEC_HLi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (HL),N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The N 8-bit value is loaded to the memory address specified by HL.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LD_HLi_N(byte opCode)
        {
            var val = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            WriteMemory(Registers.HL, val);
            ClockP3();
        }

        /// <summary>
        /// "SCF" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The Carry flag in F is set.
        /// 
        /// Other flags are not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void SCF(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (NN),A" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory location specified by the operands
        /// NN are loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LD_A_NNi(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            Registers.A = ReadMemory(adr, false);
            ClockP3();
        }

        /// <summary>
        /// "SCF" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The Carry flag in F is inverted.
        /// 
        /// Other flags are not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void CCF(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD Rd,Rs" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of any register Rs are loaded to any other 
        /// register Rd.
        /// 
        /// =================================
        /// | 0 | 1 | d | d | d | s | s | s | 
        /// =================================
        /// s, d: 000=B, 001=C, 010=D, 011=E
        ///       100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void LD_Rd_Rs(byte opCode)
        {
            var regD = (Reg8Index)(opCode & 0x07);
            var regS = (Reg8Index)((opCode & 0x38) >> 3);
            Registers[regS] = Registers[regD];
        }

        /// <summary>
        /// "LD R,(HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to 
        /// register R.
        /// 
        /// =================================
        /// | 0 | 1 | R | R | R | 1 | 1 | 0 | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_R_HLi(byte opCode)
        {
            var reg = (Reg8Index)((opCode & 0x38) >> 3);
            Registers[reg] = ReadMemory(Registers.HL, false);
            ClockP3();
        }

        /// <summary>
        /// "LD (HL),R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of R are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | R | R | R | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_HLi_R(byte opCode)
        {
            var reg = (Reg8Index)(opCode & 0x07);
            WriteMemory(Registers.HL, Registers[reg]);
            ClockP3();
        }

        /// <summary>
        /// "HALT" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The HALT instruction suspends CPU operation until a subsequent 
        /// interrupt or reset is received.While in the HALT state, 
        /// the processor executes NOPs to maintain memory refresh logic.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void HALT(byte opCode)
        {
            HALTED = true;
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and the register specified by R.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 0 | 1 | A | A | A | R | R | R | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E,
        ///    100=H, 101=L, 110=N/A, 111=A
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4 (4)
        /// </remarks>
        private void ALU_A_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and the byte at the memory address specified HL.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 0 | 1 | A | A | A | 1 | 1 | 0 | 
        /// =================================
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void ALU_A_HLi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "RET X" operation, where X is a condition
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If condition X is true, the byte at the memory location specified
        /// by the contents of SP is moved to the low-order 8 bits of PC.
        /// SP is incremented and the byte at the memory location specified by 
        /// the new contents of the SP are moved to the high-order eight bits of 
        /// PC.The SP is incremented again. The next op code following this 
        /// instruction is fetched from the memory location specified by the PC.
        /// This instruction is normally used to return to the main line program at
        /// the completion of a routine entered by a CALL instruction. 
        /// If condition X is false, PC is simply incremented as usual, and the 
        /// program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | X | X | X | 0 | 0 | 0 | 
        /// =================================
        /// X: 000=NZ, 001=Z, 010=NC, 011=C,
        ///    100=PO, 101=PE, 110=P, 111=M 
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RET_X(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "POP RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair RR. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | R | R | 0 | 0 | 0 | 1 | 
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=AF
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void POP_RR(byte opCode)
        {
            var reg = (Reg16Index)((opCode & 0x30) >> 4);
            ushort val = ReadMemory(Registers.SP, false);
            ClockP3();
            Registers.SP++;
            val |= (ushort)(ReadMemory(Registers.SP, false) << 8);
            ClockP3();
            Registers.SP++;
            if (reg == Reg16Index.SP)
            {
                Registers.AF = val;
            }
            else
            {
                Registers[reg] = val;
            }
        }

        /// <summary>
        /// "JP X,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If condition X is true, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | X | X | X | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// X: 000=NZ, 001=Z, 010=NC, 011=C,
        ///    100=PO, 101=PE, 110=P, 111=M 
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JP_X_NN(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "JP NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Operand NN is loaded to PC. The next instruction is fetched 
        /// from the location designated by the new contents of the PC.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JP_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC, false) >> 8);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "CALL X,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If condition X is true, this instruction pushes the current 
        /// contents of PC onto the top of the external memory stack, then 
        /// loads the operands NN to PC to point to the address in memory 
        /// at which the first op code of a subroutine is to be fetched. 
        /// At the end of the subroutine, a RET instruction can be used to 
        /// return to the original program flow by popping the top of the 
        /// stack back to PC. If condition X is false, PC is incremented as 
        /// usual, and the program continues with the next sequential 
        /// instruction. The stack push is accomplished by first decrementing 
        /// the current contents of SP, loading the high-order byte of the PC 
        /// contents to the memory address now pointed to by SP; then 
        /// decrementing SP again, and loading the low-order byte of the PC 
        /// contents to the top of the stack.
        /// 
        /// =================================
        /// | 1 | 1 | X | X | X | 1 | 0 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// X: 000=NZ, 001=Z, 010=NC, 011=C,
        ///    100=PO, 101=PE, 110=P, 111=M 
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CALL_X_NN(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "PUSH RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair RR are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | R | R | 0 | 1 | 0 | 1 | 
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=AF
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PUSH_RR(byte opCode)
        {
            var reg = (Reg16Index)((opCode & 0x30) >> 4);
            var val = reg == Reg16Index.SP 
                ? Registers.AF
                : Registers[reg];
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(val >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(val & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and the 8-bit value specified in N.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 0 | 1 | A | A | A | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void ALU_A_N(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "RST N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The current PC contents are pushed onto the external memory stack,
        /// and the Page 0 memory location assigned by operand N is loaded to 
        /// PC. Program execution then begins with the op code in the address 
        /// now pointed to by PC. The push is performed by first decrementing 
        /// the contents of SP, loading the high-order byte of PC to the 
        /// memory address now pointed to by SP, decrementing SP again, and 
        /// loading the low-order byte of PC to the address now pointed to by 
        /// SP. The Restart instruction allows for a jump to one of eight 
        /// addresses according to N (0x08*N).
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | N | N | N | 1 | 1 | 1 | 
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void RST_N(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = (ushort)(opCode & 0x38);
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The byte at the memory location specified by the contents of SP
        /// is moved to the low-order eight bits of PC. SP is now incremented
        /// and the byte at the memory location specified by the new contents 
        /// of this instruction is fetched from the memory location specified 
        /// by PC.
        /// This instruction is normally used to return to the main line 
        /// program at the completion of a routine entered by a CALL 
        /// instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void RET(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.SP, false);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP, false) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "CALL NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The current contents of PC are pushed onto the top of the
        /// external memory stack. The operands NN are then loaded to PC to 
        /// point to the address in memory at which the first op code of a 
        /// subroutine is to be fetched. At the end of the subroutine, a RET 
        /// instruction can be used to return to the original program flow by 
        /// popping the top of the stack back to PC. The push is accomplished 
        /// by first decrementing the current contents of SP, loading the 
        /// high-order byte of the PC contents to the memory address now pointed
        /// to by SP; then decrementing SP again, and loading the low-order 
        /// byte of the PC contents to the top of stack.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 4, 3, 3 (17)
        /// </remarks>
        private void CALL_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC, false) >> 8);
            ClockP3();
            Registers.PC++;

            ClockP1();
            Registers.SP--;

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC & 0xFF));
            ClockP3();

            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "OUT (N),A" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The operand N is placed on the bottom half (A0 through A7) of
        /// the address bus to select the I/O device at one of 256 possible
        /// ports. The contents of A also appear on the top half(A8 through
        /// A15) of the address bus at this time. Then the byte contained 
        /// in A is placed on the data bus and written to the selected 
        /// peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3, 4 (11)
        /// </remarks>
        private void OUT_NN_A(byte opCode)
        {
            ClockP3();
            ushort port = ReadMemory(Registers.PC++, false);
            // TODO: Check port + 1
            Registers.MW = (ushort)(((port + 1) & 0xFF) + (Registers.A << 8));
            ClockP3();
            port += (ushort)(Registers.A << 8);

            WritePort(port, Registers.A);
            ClockP1();
        }

        /// <summary>
        /// "EXX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Each 2-byte value in register pairs BC, DE, and HL is exchanged
        /// with the 2-byte value in BC', DE', and HL', respectively.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4, (4)
        /// </remarks>
        private void EXX(byte opCode)
        {
            Registers.ExchangeRegisterSet();
        }

        /// <summary>
        /// "IN A,(N)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The operand N is placed on the bottom half (A0 through A7) of 
        /// the address bus to select the I/O device at one of 256 possible 
        /// ports. The contents of A also appear on the top half (A8 through 
        /// A15) of the address bus at this time. Then one byte from the
        /// selected port is placed on the data bus and written to A 
        /// in the CPU.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3, 4 (11)
        /// </remarks>
        private void IN_A_NN(byte opCode)
        {
            ClockP3();
            ushort port = ReadMemory(Registers.PC++, false);
            ClockP4();
            port += (ushort)(Registers.A << 8);
            // TODO: Check port + 1
            Registers.MW = (ushort)((Registers.A << 8) + port + 1);
            Registers.A = ReadPort(port);
        }

        /// <summary>
        /// "EX (SP),HL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The low-order byte contained in HL is exchanged with the contents
        /// of the memory address specified by the contents of SP, and the 
        /// high-order byte of HL is exchanged with the next highest memory 
        /// address (SP+1).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4, 3, 4, 3, 5 (19)
        /// </remarks>
        private void EX_SPi_HL(byte opCode)
        {
            var tmpSp = Registers.SP;
            Registers.MW = ReadMemory(tmpSp, false);
            ClockP3();
            WriteMemory(tmpSp, Registers.L);
            ClockP4();
            tmpSp++;
            Registers.MW += (ushort)(ReadMemory(tmpSp, false) * 0x100);
            ClockP3();
            WriteMemory(tmpSp, Registers.H);
            Registers.HL = Registers.MW;
            ClockP5();
        }

        /// <summary>
        /// "JP (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// PC is loaded with the contents of HL. The next instruction is 
        /// fetched from the location designated by the new contents of PC.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void JP_HL(byte opCode)
        {
            Registers.PC = Registers.HL;
        }

        /// <summary>
        /// "EX DE,HL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 2-byte contents of register pairs DE and HL are exchanged.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void EX_DE_HL(byte opCode)
        {
            Registers.Swap(ref Registers.DE, ref Registers.HL);
        }

        /// <summary>
        /// "DI" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Disables the maskable interrupt by resetting the interrupt
        /// enable flip-flops (IFF1 and IFF2).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DI(byte opCode)
        {
            IFF2 = IFF1 = false;
        }

        /// <summary>
        /// "LD SP,HL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of HL are loaded to SP.
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4 (6)
        /// </remarks>
        private void LD_SP_HL(byte opCode)
        {
            Registers.SP = Registers.HL;
            ClockP2();
        }

        /// <summary>
        /// "EI" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Sets both interrupt enable flip flops (IFFI and IFF2) to a
        /// logic 1 value, allowing recognition of any maskable interrupt.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void EI(byte opCode)
        {
            IFF2 = IFF1 = INT_BLOCKED = true;
        }
    }
}