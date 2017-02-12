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
                null,     LdQQNN,   LdQQiA,   IncQQ,    IncQ,     DecQ,     LdQN,     Rlca,     // 00..07
                ExAF,     AddHLQQ,  LdAQQi,   DecQQ,    IncQ,     DecQ,     LdQN,     Rrca,     // 08..0F
                Djnz,     LdQQNN,   LdQQiA,   IncQQ,    IncQ,     DecQ,     LdQN,     Rla,      // 10..17
                JrE,      AddHLQQ,  LdAQQi,   DecQQ,    IncQ,     DecQ,     LdQN,     Rra,      // 18..1F
                JrXE,     LdQQNN,   LdNNiHL,  IncQQ,    IncQ,     DecQ,     LdQN,     Daa,      // 20..27
                JrXE,     AddHLQQ,  LdHLNNi,  DecQQ,    IncQ,     DecQ,     LdQN,     Cpl,      // 28..2F
                JrXE,     LdQQNN,   LdNNA,    IncQQ,    IncHLi,   DecHLi,   LdHLiN,   Scf,      // 30..37
                JrXE,     AddHLQQ,  LdNNiA,   DecQQ,    IncQ,     DecQ,     LdQN,     Ccf,      // 38..3F
                null,     LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQHLi,   LdQdQs,   // 40..47
                LdQdQs,   null,     LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQHLi,   LdQdQs,   // 48..4F
                LdQdQs,   LdQdQs,   null,     LdQdQs,   LdQdQs,   LdQdQs,   LdQHLi,   LdQdQs,   // 50..57
                LdQdQs,   LdQdQs,   LdQdQs,   null,     LdQdQs,   LdQdQs,   LdQHLi,   LdQdQs,   // 58..5F
                LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   null,     LdQdQs,   LdQHLi,   LdQdQs,   // 60..67
                LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   null,     LdQHLi,   LdQdQs,   // 68..6F
                LdHLiQ,   LdHLiQ,   LdHLiQ,   LdHLiQ,   LdHLiQ,   LdHLiQ,   HALT,     LdHLiQ,   // 70..77
                LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQdQs,   LdQHLi,   null,     // 78..7F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 80..87
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 88..8F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 90..97
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 98..9F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // A0..A7
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // A8..AF
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // B0..B7
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // B8..BF
                RetX,     PopQQ,    JpXNN,    JpNN,     CallXNN,  PushQQ,   AluAN,    RstN,     // C0..C7
                RetX,     Ret,      JpXNN,    null,     CallXNN,  CallNN,   AluAN,    RstN,     // C8..CF
                RetX,     PopQQ,    JpXNN,    OutNA,    CallXNN,  PushQQ,   AluAN,    RstN,     // D0..D7
                RetX,     Exx,      JpXNN,    InAN,     CallXNN,  null,     AluAN,    RstN,     // D8..DF
                RetX,     PopQQ,    JpXNN,    ExSPiHL,  CallXNN,  PushQQ,   AluAN,    RstN,     // E0..E7
                RetX,     JpHL,     JpXNN,    ExDEHL,   CallXNN,  null,     AluAN,    RstN,     // E8..EF
                RetX,     PopQQ,    JpXNN,    Di,       CallXNN,  PushQQ,   AluAN,    RstN,     // F0..F7
                RetX,     LdSPHL,   JpXNN,    Ei,       CallXNN,  null,     AluAN,    RstN,     // F8..FF
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
        private void LdQQNN(byte opCode)
        {
            Registers[Get16BitRegisterIndex(opCode)] = Get16BitFromCode();
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
        private void LdQQiA(byte opCode)
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
        private void IncQQ(byte opCode)
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
        private void IncQ(byte opCode)
        {
            var q = Get8BitRegisterIndex(opCode);
            Registers[q] = AluIncByte(Registers[q]);
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
        private void DecQ(byte opCode)
        {
            var q = Get8BitRegisterIndex(opCode);
            Registers[q] = AluDecByte(Registers[q]);
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
        private void LdQN(byte opCode)
        {
            Registers[Get8BitRegisterIndex(opCode)] = Get8BitFromCode();
        }

        /// <summary>
        /// "Rlca" operation
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
        private void Rlca(byte opCode)
        {
            int rlcaVal = Registers.A;
            rlcaVal <<= 1;
            var cf = (byte)((rlcaVal & 0x100) != 0 ? FlagsSetMask.C : 0);
            if (cf != 0)
            {
                rlcaVal = (rlcaVal | 0x01) & 0xFF;
            }
            Registers.A = (byte)rlcaVal;
            Registers.F = (byte)(cf | (Registers.F & (FlagsSetMask.SZPV)));
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
        private void ExAF(byte opCode)
        {
            Registers.ExchangeAfSet();
        }

        /// <summary>
        /// "ADD HL,QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of QQ are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 1 | 0 | 0 | 1 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLQQ(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers[Get16BitRegisterIndex(opCode)]);
            ClockP7();
        }

        /// <summary>
        /// "LD A,(QQ)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory location specified by QQ are loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 1 | 0 | 1 | 0 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=N/A, 11=N/A
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdAQQi(byte opCode)
        {
            var rrval = Registers[Get16BitRegisterIndex(opCode)];
            Registers.MW = (ushort)(rrval + 1);
            Registers.A = ReadMemory(rrval);
            ClockP3();
        }

        /// <summary>
        /// "DEC QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair QQ are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | Q | Q | 1 | 0 | 1 | 1 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DecQQ(byte opCode)
        {
            Registers[Get16BitRegisterIndex(opCode)]--;
            ClockP2();
        }

        /// <summary>
        /// "Rrca" operation
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
        private void Rrca(byte opCode)
        {
            int rrcaVal = Registers.A;
            var cf = (byte)((rrcaVal & 0x01) != 0 ? FlagsSetMask.C : 0);
            rrcaVal >>= 1;
            if ((rrcaVal & 0x01) != 0)
            {
                rrcaVal = (rrcaVal >> 1) | 0x80;
            }
            Registers.A = (byte)rrcaVal;
            Registers.F = (byte)(cf | (Registers.F & (FlagsSetMask.SZPV)));
        }

        /// <summary>
        /// "Djnz E" operation
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
        private void Djnz(byte opCode)
        {
            var e = Get8BitFromCode();
            ClockP1();
            if (--Registers.B == 0)
            {
                return;
            }
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
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
        private void Rla(byte opCode)
        {
            var rlcaVal = Registers.A;
            var newCF = (rlcaVal & 0x80) != 0 ? FlagsSetMask.C : 0;
            rlcaVal <<= 1;
            if (Registers.CFlag)
            {
                rlcaVal |= 0x01;
            }
            Registers.A = rlcaVal;
            Registers.F = (byte)((byte)newCF | (Registers.F & (FlagsSetMask.SZPV)));
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
        private void JrE(byte opCode)
        {
            var val = Get8BitFromCode();
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)val);
            ClockP5();
        }

        /// <summary>
        /// "Rra" operation
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
        private void Rra(byte opCode)
        {
            var rlcaVal = Registers.A;
            var newCF = (rlcaVal & 0x01) != 0 ? FlagsSetMask.C : 0;
            rlcaVal >>= 1;
            if (Registers.CFlag)
            {
                rlcaVal |= 0x80;
            }
            Registers.A = rlcaVal;
            Registers.F = (byte)((byte)newCF | (Registers.F & (FlagsSetMask.SZPV)));
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
        /// X: 00=NZ, 01=Z, 10=NC, 11=C
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrXE(byte opCode)
        {
            var condition = (opCode & 0x18) >> 3;
            ushort val = Get8BitFromCode();
            TestCondition(condition, () =>
            {
                Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)val);
                ClockP5();
            });
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
        private void LdNNiHL(byte opCode)
        {
            var adr = Get16BitFromCode();
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
        /// |     |DAA     |Digit|Daa     |Digit|Added |Daa    |
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
        private void Daa(byte opCode)
        {
            var daaIndex = Registers.A + ((Registers.F & 3) + ((Registers.F >> 2) & 4) << 8);
            Registers.AF = s_DAAResults[daaIndex];
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
        private void LdHLNNi(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            ushort val = ReadMemory(adr);
            ClockP3();
            val += (ushort)(ReadMemory(Registers.MW) * 0x100);
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
        private void Cpl(byte opCode)
        {
            Registers.A ^= 0xFF;
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.R3R5)) | 
                (FlagsSetMask.NH) | (Registers.A & (FlagsSetMask.R3R5)));
        }

        /// <summary>
        /// "LD A,(NN)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to the memory address specified by 
        /// the operand NN
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LdNNA(byte opCode)
        {
            var adr = Get16BitFromCode();
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
        private void IncHLi(byte opCode)
        {
            var memValue = ReadMemory(Registers.HL);
            ClockP3();
            memValue = AluIncByte(memValue);
            ClockP1();
            WriteMemory(Registers.HL, memValue);
            ClockP3();
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
        private void DecHLi(byte opCode)
        {
            var memValue = ReadMemory(Registers.HL);
            ClockP3();
            memValue = AluDecByte(memValue);
            ClockP1();
            WriteMemory(Registers.HL, memValue);
            ClockP3();
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
        private void LdHLiN(byte opCode)
        {
            var val = Get8BitFromCode();
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
        private void Scf(byte opCode)
        {
            Registers.F = (byte) (Registers.F & FlagsResetMask.C | FlagsSetMask.C);
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
        private void LdNNiA(byte opCode)
        {
            ushort adr = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            adr += (ushort)(ReadMemory(Registers.PC) * 0x100);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            Registers.A = ReadMemory(adr);
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
        private void Ccf(byte opCode)
        {
            Registers.F = (byte)(Registers.F & FlagsResetMask.C);
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
        private void LdQdQs(byte opCode)
        {
            Registers[(Reg8Index)((opCode & 0x38) >> 3)] = 
                Registers[(Reg8Index)(opCode & 0x07)];
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
        private void LdQHLi(byte opCode)
        {
            Registers[(Reg8Index)((opCode & 0x38) >> 3)] = 
                ReadMemory(Registers.HL);
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
        private void LdHLiQ(byte opCode)
        {
            WriteMemory(Registers.HL, Registers[(Reg8Index)(opCode & 0x07)]);
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
        /// ALU operation (W) with A and the register specified by Q.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 0 | 1 | W | W | W | Q | Q | Q | 
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E,
        ///    100=H, 101=L, 110=N/A, 111=A
        /// W: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4 (4)
        /// </remarks>
        private void AluAQ(byte opCode)
        {
            _AluAlgorithms[(opCode & 0x38) >> 3](
                Registers[(Reg8Index)(opCode & 0x07)], 
                Registers.CFlag);
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// ALU operation (W) with A and the byte at the memory address 
        /// specified HL.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 0 | 1 | W | W | W | 1 | 1 | 0 | 
        /// =================================
        /// W: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void AluAHLi(byte opCode)
        {
            var memVal = ReadMemory(Registers.HL);
            ClockP3();
            _AluAlgorithms[(opCode & 0x38) >> 3](memVal, Registers.CFlag);
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
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetX(byte opCode)
        {
            ClockP1();
            TestCondition((opCode & 0x38) >> 3, () =>
            {
                GetMWFromStack();
                Registers.PC = Registers.MW;
            });
        }

        /// <summary>
        /// "POP QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair QQ. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | Q | Q | 0 | 0 | 0 | 1 | 
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=AF
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopQQ(byte opCode)
        {
            var reg = (Reg16Index)((opCode & 0x30) >> 4);
            var val = Get16BitFromStack();
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
        private void JpXNN(byte opCode)
        {
            GetMWFromCode();
            TestCondition((opCode & 0x38) >> 3, () =>
            {
                Registers.PC = Registers.MW;
            });
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
        private void JpNN(byte opCode)
        {
            GetMWFromCode();
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
        private void CallXNN(byte opCode)
        {
            GetMWFromCode();
            TestCondition((opCode & 0x38) >> 3, () =>
            {
                ClockP1();
                Registers.SP--;
                WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
                ClockP3();
                Registers.SP--;
                WriteMemory(Registers.SP, (byte)Registers.PC);
                ClockP3();
                Registers.PC = Registers.MW;
            });
        }

        /// <summary>
        /// "PUSH QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair QQ are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | Q | Q | 0 | 1 | 0 | 1 | 
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=AF
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushQQ(byte opCode)
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
        private void AluAN(byte opCode)
        {
            _AluAlgorithms[(opCode & 0x38) >> 3](
                Get8BitFromCode(), 
                Registers.CFlag);
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
        private void RstN(byte opCode)
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
        private void Ret(byte opCode)
        {
            GetMWFromStack();
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
        private void CallNN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) >> 8);
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
        private void OutNA(byte opCode)
        {
            ClockP3();
            ushort port = ReadMemory(Registers.PC++);
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
        private void Exx(byte opCode)
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
        private void InAN(byte opCode)
        {
            ClockP3();
            ushort port = ReadMemory(Registers.PC++);
            ClockP4();
            port += (ushort)(Registers.A << 8);
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
        private void ExSPiHL(byte opCode)
        {
            var tmpSp = Registers.SP;
            Registers.MW = ReadMemory(tmpSp);
            ClockP3();
            WriteMemory(tmpSp, Registers.L);
            ClockP4();
            tmpSp++;
            Registers.MW += (ushort)(ReadMemory(tmpSp) * 0x100);
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
        private void JpHL(byte opCode)
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
        private void ExDEHL(byte opCode)
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
        private void Di(byte opCode)
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
        private void LdSPHL(byte opCode)
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
        private void Ei(byte opCode)
        {
            IFF2 = IFF1 = INT_BLOCKED = true;
        }
    }
}