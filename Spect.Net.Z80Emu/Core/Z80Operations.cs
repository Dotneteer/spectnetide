using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides standard CPU operations
    /// for direct (with no prefix) execution
    /// </summary>
    public partial class Z80
    {
        private Action<byte>[] _standarOperations;

        /// <summary>
        /// Processes the operations with 0xED prefix
        /// </summary>
        /// <param name="opCode">Operation code</param>
        private void ProcessStandardOperations(byte opCode)
        {
            Action<byte> opMethod = null;
            if (IndexMode == OpIndexMode.None)
            {
                opMethod = _standarOperations[opCode];
            }
            else
            {
                // TODO: Get IX/IY indexed operation action
                // --- Indexed operations
            }
            opMethod?.Invoke(opCode);
        }

        /// <summary>
        /// Initializes the standard operation execution tables
        /// </summary>
        private void InitializeNormalExecutionTables()
        {
            _standarOperations = new Action<byte>[]
            {
                null,      LD_RR_16,  LD_RR_A,   INC_RR,    INC_R,     DEC_R,     LD_R_8,    RLCA,      // 00..07
                EX_AF,     ADD_HL_RR, LD_A_RRi,  DEC_RR,    INC_R,     DEC_R,     LD_R_8,    RRCA,      // 08..0F
                DJNZ,      LD_RR_16,  LD_RR_A,   INC_RR,    INC_R,     DEC_R,     LD_R_8,    RLA,       // 10..17
                JR_E,      ADD_HL_RR, LD_A_RRi,  DEC_RR,    INC_R,     DEC_R,     LD_R_8,    RRA,       // 18..1F
                JR_XX_E,   LD_RR_16,  LD_NN_HL,  INC_RR,    INC_R,     DEC_R,     LD_R_8,    DAA,       // 20..27
                JR_XX_E,   ADD_HL_RR, LD_HL_NNi, DEC_RR,    INC_R,     DEC_R,     LD_R_8,    CPL,       // 28..2F
                JR_XX_E,   LD_RR_16,  LD_NN_A,   INC_RR,    INC_HLi,   DEC_HLi,   LD_HLi_N,  SCF,       // 30..37
                JR_XX_E,   ADD_HL_RR, LD_A_NNi,  DEC_RR,    INC_R,     DEC_R,     LD_R_8,    CCF,       // 38..3F

                null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 40..47
                LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 48..4F
                LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 50..57
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 58..5F
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 60..67
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_R_HLi,   LD_Rd_Rs, // 68..6F
                LD_HLi_R, LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,   LD_HLi_R, HALT,     LD_HLi_R, // 70..77
                LD_Rd_Rs,  LD_Rd_Rs,   LD_Rd_Rs,   LD_Rd_Rs,   LD_Rd_Rs,    LD_Rd_Rs,  LD_R_HLi,  null,    // 78..7F

                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // 80..87
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // 88..8F
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // 90..97
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // 98..9F
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // A0..A7
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // A8..AF
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // B0..B7
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,   // B8..BF

                RETX,    POPRR,    JPXNN,    JPNNNN,   CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,  // C0..C7
                RETX,    RET,      JPXNN,    null,     CALLXNNNN, CALLNNNN,ALUAN,    RSTNN,  // C8..CF
                RETX,    POPRR,    JPXNN,    OUT_NN_A, CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,  // D0..D7
                RETX,    EXX,      JPXNN,    INA_NN_,  CALLXNNNN, null,    ALUAN,    RSTNN,  // D8..DF
                RETX,    POPRR,    JPXNN,    EX_SP_HL, CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,  // E0..E7
                RETX,    JP_HL_,   JPXNN,    EXDEHL,   CALLXNNNN, null,    ALUAN,    RSTNN,  // E8..EF
                RETX,    POPRR,    JPXNN,    DI,       CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,  // F0..F7
                RETX,    LDSPHL,   JPXNN,    EI,       CALLXNNNN, null,    ALUAN,    RSTNN,  // F8..FF
            };
        }

        /// <summary>
        /// "LD RR,nnnn" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the RR register pair.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 0 | 0 | 0 | 1 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LD_RR_16(byte opCode)
        {
            var rr = (Reg16Index)((opCode & 0x30) >> 4);
            var l = ReadMemory(Registers.PC, false);
            Clock(3);
            Registers.PC++;
            var h = ReadMemory(Registers.PC, false);
            Clock(3);
            Registers.PC++;
            Registers[rr] = (ushort)(l | h << 8);
        }

        /// <summary>
        /// "LD (RR),A" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the A are loaded to the memory location 
        /// specified by the contents of the register pair RR
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 0 | 0 | 1 | 0 |
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=N/A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_RR_A(byte opCode)
        {
            WriteMemory(Registers[(Reg16Index)((opCode & 0x30) >> 4)], Registers.A);
            Registers.MH = Registers.A;
            Clock(3);
        }

        /// <summary>
        /// "INC RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair RR are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | 0 | 0 | 1 | 1 |
        /// =================================
        /// RR: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void INC_RR(byte opCode)
        {
            var rr = (Reg16Index)((opCode & 0x30) >> 4);
            Registers[rr] += 1;
            Clock(2);
        }

        /// <summary>
        /// "INC R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register R is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | R | 1 | 0 | 0 |
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void INC_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register R is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | R | 1 | 0 | 1 |
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void DEC_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD R,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer is loaded to R.
        /// 
        /// =================================
        /// | 0 | 0 | R | R | R | 1 | 1 | 0 |
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LD_R_8(byte opCode)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            Clock(3);
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
            Clock(2);
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
        /// "JR XX,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (XX). If the test evaluates to *true*, the value of displacement
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
        /// XX: 00=Z, 01=NZ, 10=NC, 11=C
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JR_XX_E(byte opCode)
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
        private void LD_NN_HL(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC, false);
            Clock(3);
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            Clock(3);
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            WriteMemory(adr, Registers.L);
            Clock(3);
            WriteMemory(Registers.MW, Registers.H);
            Clock(3);
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
            Clock(3);
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            Clock(3);
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            ushort val = ReadMemory(adr, false);
            Clock(3);
            val += (ushort)(ReadMemory(Registers.MW, false) * 0x100);
            Clock(3);
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
            Clock(3);
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            Clock(3);
            Registers.PC++;
            Registers.MW = (ushort)(((adr + 1) & 0xFF) + (Registers.A << 8));
            WriteMemory(adr, Registers.A);
            Registers.MH = Registers.A;
            Clock(3);
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
            Clock(3);
            Registers.PC++;
            WriteMemory(Registers.HL, val);
            Clock(3);
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
            Clock(3);
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC, false) * 0x100);
            Clock(3);
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            Registers.A = ReadMemory(adr, false);
            Clock(3);
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
            Clock(3);
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
            Clock(3);
        }

        private void EI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LDSPHL(byte obj)
        {
            throw new NotImplementedException();
        }

        private void DI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void EXDEHL(byte obj)
        {
            throw new NotImplementedException();
        }

        private void JP_HL_(byte obj)
        {
            throw new NotImplementedException();
        }

        private void EX_SP_HL(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INA_NN_(byte obj)
        {
            throw new NotImplementedException();
        }

        private void EXX(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OUT_NN_A(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CALLNNNN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RET(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RSTNN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void ALUAN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void PUSHRR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CALLXNNNN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void JPNNNN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void JPXNN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void POPRR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RETX(byte obj)
        {
            throw new NotImplementedException();
        }

        private void ALUA_HL_(byte obj)
        {
            throw new NotImplementedException();
        }

        private void ALUAR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void HALT(byte obj)
        {
            throw new NotImplementedException();
        }
    }
}