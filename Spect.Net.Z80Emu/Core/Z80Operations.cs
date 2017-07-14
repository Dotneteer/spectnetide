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
                null,     LdBCNN,   LdBCiA,   IncBC,    IncB,     DecB,     LdBN,     Rlca,     // 00..07
                ExAF,     AddHLBC,  LdABCi,   DecBC,    IncC,     DecC,     LdCN,     Rrca,     // 08..0F
                Djnz,     LdDENN,   LdDEiA,   IncDE,    IncD,     DecD,     LdDN,     Rla,      // 10..17
                JrE,      AddHLDE,  LdADEi,   DecDE,    IncE,     DecE,     LdEN,     Rra,      // 18..1F
                JrNZ,     LdHLNN,   LdNNiHL,  IncHL,    IncH,     DecH,     LdHN,     Daa,      // 20..27
                JrZ,      AddHLHL,  LdHLNNi,  DecHL,    IncL,     DecL,     LdLN,     Cpl,      // 28..2F
                JrNC,     LdSPNN,   LdNNA,    IncSP,    IncHLi,   DecHLi,   LdHLiN,   Scf,      // 30..37
                JrC,      AddHLSP,  LdNNiA,   DecSP,    IncA,     DecA,     LdAN,     Ccf,      // 38..3F
                null,     LdB_C,    LdB_D,    LdB_E,    LdB_H,    LdB_L,    LdB_HLi,  LdB_A,    // 40..47
                LdC_B,    null,     LdC_D,    LdC_E,    LdC_H,    LdC_L,    LdC_HLi,  LdC_A,    // 48..4F
                LdD_B,    LdD_C,    null,     LdD_E,    LdD_H,    LdD_L,    LdD_HLi,  LdD_A,    // 50..57
                LdE_B,    LdE_C,    LdE_D,    null,     LdE_H,    LdE_L,    LdE_HLi,  LdE_A,    // 58..5F
                LdH_B,    LdH_C,    LdH_D,    LdH_E,    null,     LdH_L,    LdH_HLi,  LdH_A,    // 60..67
                LdL_B,    LdL_C,    LdL_D,    LdL_E,    LdL_H,    null,     LdL_HLi,  LdL_A,    // 68..6F
                LdHLi_B,  LdHLi_C,  LdHLi_D,  LdHLi_E,  LdHLi_H,  LdHLi_L,  HALT,     LdHLi_A,  // 70..77
                LdA_B,    LdA_C,    LdA_D,    LdA_E,    LdA_H,    LdA_L,    LdA_HLi,  null,     // 78..7F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 80..87
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 88..8F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 90..97
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // 98..9F
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // A0..A7
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // A8..AF
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // B0..B7
                AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAQ,    AluAHLi,  AluAQ,    // B8..BF
                RetNZ,    PopBC,    JpNZ_NN,  JpNN,     CallNZ,   PushBC,   AluAN,    Rst00,    // C0..C7
                RetZ,     Ret,      JpZ_NN,   null,     CallZ,    CallNN,   AluAN,    Rst08,    // C8..CF
                RetNC,    PopDE,    JpNC_NN,  OutNA,    CallNC,   PushDE,   AluAN,    Rst10,    // D0..D7
                RetC,     Exx,      JpC_NN,   InAN,     CallC,    null,     AluAN,    Rst18,    // D8..DF
                RetPO,    PopHL,    JpPO_NN,  ExSPiHL,  CallPO,   PushHL,   AluAN,    Rst20,    // E0..E7
                RetPE,    JpHL,     JpPE_NN,  ExDEHL,   CallPE,   null,     AluAN,    Rst28,    // E8..EF
                RetP,     PopAF,    JpP_NN,   Di,       CallP,    PushAF,   AluAN,    Rst30,    // F0..F7
                RetM,     LdSPHL,   JpM_NN,   Ei,       CallM,    null,     AluAN,    Rst38,    // F8..FF
            };
        }

        /// <summary>
        /// "ld bc,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the BC register pair.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0x01
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdBCNN(byte opCode)
        {
            Registers.C = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.B = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "ld (bc),a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the A are loaded to the memory location 
        /// specified by the contents of the register pair BC.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0x02
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdBCiA(byte opCode)
        {
            WriteMemory(Registers.BC, Registers.A);
            Registers.MH = Registers.A;
            ClockP3();
        }

        /// <summary>
        /// "inc bc" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair BC are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0x03
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void IncBC(byte opCode)
        {
            Registers.BC++;
            ClockP2();
        }

        /// <summary>
        /// "inc b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register B is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0x04
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncB(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.B++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register B is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0x05
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecB(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.B--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld b,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to B.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0x06
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdBN(byte opCode)
        {
            Registers.B = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "rlca" operation
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
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0x07
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
        /// "ex af,af'" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 2-byte contents of the register pairs AF and AF' are exchanged.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0x08
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void ExAF(byte opCode)
        {
            Registers.ExchangeAfSet();
        }

        /// <summary>
        /// "add hl,bc" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of BC are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0x09
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLBC(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers.BC);
            ClockP7();
        }

        /// <summary>
        /// "ld a,(bc)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory location specified by BC are loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0x0A
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdABCi(byte opCode)
        {
            Registers.MW = (ushort)(Registers.BC + 1);
            Registers.A = ReadMemory(Registers.BC);
            ClockP3();
        }

        /// <summary>
        /// "dec bc" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair BC are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0x0B
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DecBC(byte opCode)
        {
            Registers.BC--;
            ClockP2();
        }

        /// <summary>
        /// "inc c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register C is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0x0C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncC(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.C++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register C is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0x0D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecC(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.C--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld c,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to C.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0x0E
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdCN(byte opCode)
        {
            Registers.C = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "rrca" operation
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
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0x0F
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void Rrca(byte opCode)
        {
            int rrcaVal = Registers.A;
            var cf = (byte)((rrcaVal & 0x01) != 0 ? FlagsSetMask.C : 0);
            if ((rrcaVal & 0x01) != 0)
            {
                rrcaVal = (rrcaVal >> 1) | 0x80;
            }
            else
            {
                rrcaVal >>= 1;
            }
            Registers.A = (byte)rrcaVal;
            Registers.F = (byte)(cf | (Registers.F & (FlagsSetMask.SZPV)));
        }

        /// <summary>
        /// "djnz E" operation
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
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0x10
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: B!=0: 5, 3, 5 (13)
        ///           B=0:  5, 3 (8)
        /// </remarks>
        private void Djnz(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            ClockP1();
            if (--Registers.B == 0)
            {
                return;
            }
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "ld de,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the DE register pair.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0x11
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdDENN(byte opCode)
        {
            Registers.E = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.D = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "ld (de),a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the A are loaded to the memory location 
        /// specified by the contents of the register pair DE.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0x12
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdDEiA(byte opCode)
        {
            WriteMemory(Registers.DE, Registers.A);
            Registers.MH = Registers.A;
            ClockP3();
        }

        /// <summary>
        /// "inc de" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair DE are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0x13
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void IncDE(byte opCode)
        {
            Registers.DE++;
            ClockP2();
        }

        /// <summary>
        /// "inc d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register D is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0x14
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncD(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.D++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register D is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0x15
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecD(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.D--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld d,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to D.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0x16
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdDN(byte opCode)
        {
            Registers.D = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "rla" operation
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
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x17
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
            Registers.F = (byte)((byte)newCF | (Registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        /// "jr e" operation
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
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0x18
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: 4, 3, 5 (12)
        /// </remarks>
        private void JrE(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "add hl,de" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of DE are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0x19
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLDE(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers.DE);
            ClockP7();
        }

        /// <summary>
        /// "ld a,(de)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory location specified by DE are loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0x1A
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdADEi(byte opCode)
        {
            Registers.MW = (ushort)(Registers.DE + 1);
            Registers.A = ReadMemory(Registers.DE);
            ClockP3();
        }

        /// <summary>
        /// "dec de" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair DE are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0x1B
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DecDE(byte opCode)
        {
            Registers.DE--;
            ClockP2();
        }

        /// <summary>
        /// "inc e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register E is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0x1C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncE(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.E++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register E is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0x1D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecE(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.E--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld e,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to E.
        /// 
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0x1E
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdEN(byte opCode)
        {
            Registers.E = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "rra" operation
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
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x1F
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
            Registers.F = (byte)((byte)newCF | (Registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        /// "JR NZ,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (Z flag is not set). If the test evaluates to *true*, the value of displacement
        /// E is added to PC and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is 
        /// measured from the address of the instruction op code and contains 
        /// a range of –126 to +129 bytes. The assembler automatically adjusts
        /// for the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0x20
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrNZ(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) != 0) return;
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "ld hl,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the HL register pair.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0x21
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdHLNN(byte opCode)
        {
            Registers.L = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.H = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "ld (NN),hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the low-order portion of HL (L) are loaded to memory
        /// address (NN), and the contents of the high-order portion of HL (H) 
        /// are loaded to the next highest memory address(NN + 1).
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0x22
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3, 3 (16)
        /// </remarks>
        private void LdNNiHL(byte opCode)
        {
            var l = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var addr = (ushort)((ReadMemory(Registers.PC) << 8) | l);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(addr + 1);
            WriteMemory(addr, Registers.L);
            ClockP3();
            WriteMemory(Registers.MW, Registers.H);
            ClockP3();
        }

        /// <summary>
        /// "inc hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair HL are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0x23
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void IncHL(byte opCode)
        {
            Registers.HL++;
            ClockP2();
        }

        /// <summary>
        /// "inc h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register H is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0x24
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncH(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.H++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register H is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0x25
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecH(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.H--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld h,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to H.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0x26
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHN(byte opCode)
        {
            Registers.H = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "daa" operation
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
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0x27
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void Daa(byte opCode)
        {
            var daaIndex = Registers.A + ((Registers.F & 3) + ((Registers.F >> 2) & 4) << 8);
            Registers.AF = s_DAAResults[daaIndex];
        }

        /// <summary>
        /// "JR Z,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (Z flag is set). If the test evaluates to *true*, the value of displacement
        /// E is added to PC and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is 
        /// measured from the address of the instruction op code and contains 
        /// a range of –126 to +129 bytes. The assembler automatically adjusts
        /// for the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0x28
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrZ(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) == 0) return;
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "add hl,hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of HL are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 0x29
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLHL(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers.HL);
            ClockP7();
        }

        /// <summary>
        /// "ld hl,(NN)" operation
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
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0x2A
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
            adr += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(adr + 1);
            ushort val = ReadMemory(adr);
            ClockP3();
            val += (ushort)(ReadMemory(Registers.MW) << 8);
            ClockP3();
            Registers.HL = val;
        }

        /// <summary>
        /// "dec hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair HL are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0x2B
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DecHL(byte opCode)
        {
            Registers.HL--;
            ClockP2();
        }

        /// <summary>
        /// "inc l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register L is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0x2C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncL(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.L++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register L is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 2 | 0 | 1 | 1 | 0 | 1 | 0x2D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecL(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.L--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld l,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to H.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0x2E
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdLN(byte opCode)
        {
            Registers.L = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "cpl" operation
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
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0x2F
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void Cpl(byte opCode)
        {
            Registers.A ^= 0xFF;
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.R3R5)) 
                | FlagsSetMask.NH 
                | FlagsSetMask.H
                | (Registers.A & FlagsSetMask.R3R5));
        }

        /// <summary>
        /// "JR NC,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (C flag is not set). If the test evaluates to *true*, the value of displacement
        /// E is added to PC and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is 
        /// measured from the address of the instruction op code and contains 
        /// a range of –126 to +129 bytes. The assembler automatically adjusts
        /// for the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0x30
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrNC(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) != 0) return;
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "ld sp,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer value is loaded to the SP register pair.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0x31
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdSPNN(byte opCode)
        {
            var p = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var s = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.SP = (ushort)((s << 8) | p);
        }

        /// <summary>
        /// "ld a,(NN)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to the memory address specified by 
        /// the operand NN
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0x32
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LdNNA(byte opCode)
        {
            var l = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var addr = (ushort)(ReadMemory(Registers.PC) << 8 | l);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(((addr + 1) & 0xFF) + (Registers.A << 8));
            WriteMemory(addr, Registers.A);
            Registers.MH = Registers.A;
            ClockP3();
        }

        /// <summary>
        /// "inc sp" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair SP are incremented.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0x33
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void IncSP(byte opCode)
        {
            Registers.SP++;
            ClockP2();
        }

        /// <summary>
        /// "JR C,E" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction provides for conditional branching to 
        /// other segments of a program depending on the results of a test
        /// (C flag is set). If the test evaluates to *true*, the value of displacement
        /// E is added to PC and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is 
        /// measured from the address of the instruction op code and contains 
        /// a range of –126 to +129 bytes. The assembler automatically adjusts
        /// for the twice incremented PC.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0x38
        /// =================================
        /// |             E-2               |
        /// =================================
        /// T-States: Condition is met: 4, 3, 5 (12)
        /// Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrC(byte opCode)
        {
            var e = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) == 0) return;
            Registers.MW = Registers.PC = (ushort)(Registers.PC + (sbyte)e);
            ClockP5();
        }

        /// <summary>
        /// "add hl,sp" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of SP are added to the contents of HL and 
        /// the result is stored in HL.
        /// 
        /// S, Z, P/V are not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0x39
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLSP(byte opCode)
        {
            Registers.MW = (ushort)(Registers.HL + 1);
            Registers.HL = AluAddHL(Registers.HL, Registers.SP);
            ClockP7();
        }

        /// <summary>
        /// "dec sp" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register pair SP are decremented.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0x3B
        /// =================================
        /// T-States: 4, 2 (6)
        /// </remarks>
        private void DecSP(byte opCode)
        {
            Registers.SP--;
            ClockP2();
        }

        /// <summary>
        /// "inc a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register A is incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0x3C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void IncA(byte opCode)
        {
            Registers.F = (byte)(s_IncOpFlags[Registers.A++] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "dec a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Register A is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if m was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 0 | 0 | 2 | 0 | 1 | 1 | 0 | 1 | 0x3D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void DecA(byte opCode)
        {
            Registers.F = (byte)(s_DecOpFlags[Registers.A--] | Registers.F & FlagsSetMask.C);
        }

        /// <summary>
        /// "ld a,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to A.
        /// 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0x3E
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdAN(byte opCode)
        {
            Registers.A = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
        }

        /// <summary>
        /// "ld b,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0x41
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_C(byte opCode)
        {
            Registers.B = Registers.C;
        }

        /// <summary>
        /// "ld b,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 0x42
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_D(byte opCode)
        {
            Registers.B = Registers.D;
        }

        /// <summary>
        /// "ld b,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 0x43
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_E(byte opCode)
        {
            Registers.B = Registers.E;
        }

        /// <summary>
        /// "ld b,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0x44
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_H(byte opCode)
        {
            Registers.B = Registers.H;
        }

        /// <summary>
        /// "ld b,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0x45
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_L(byte opCode)
        {
            Registers.B = Registers.L;
        }

        /// <summary>
        /// "ld (hl),b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0 | 0x46
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdB_HLi(byte opCode)
        {
            Registers.B = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld b,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to B.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0x47
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdB_A(byte opCode)
        {
            Registers.B = Registers.A;
        }

        /// <summary>
        /// "ld c,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 0x48
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_B(byte opCode)
        {
            Registers.C = Registers.B;
        }

        /// <summary>
        /// "ld c,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0x4A
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_D(byte opCode)
        {
            Registers.C = Registers.D;
        }

        /// <summary>
        /// "ld c,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0x4B
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_E(byte opCode)
        {
            Registers.C = Registers.E;
        }

        /// <summary>
        /// "ld c,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 0x4C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_H(byte opCode)
        {
            Registers.C = Registers.H;
        }

        /// <summary>
        /// "ld c,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 0x4D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_L(byte opCode)
        {
            Registers.C = Registers.L;
        }

        /// <summary>
        /// "ld c,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0 | 0x4E
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdC_HLi(byte opCode)
        {
            Registers.C = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld c,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to C.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 0x4F
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdC_A(byte opCode)
        {
            Registers.C = Registers.A;
        }

        /// <summary>
        /// "ld d,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0x50
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_B(byte opCode)
        {
            Registers.D = Registers.B;
        }

        /// <summary>
        /// "ld d,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 0x51
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_C(byte opCode)
        {
            Registers.D = Registers.C;
        }

        /// <summary>
        /// "ld d,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0x53
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_E(byte opCode)
        {
            Registers.D = Registers.E;
        }

        /// <summary>
        /// "ld d,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0x54
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_H(byte opCode)
        {
            Registers.D = Registers.H;
        }

        /// <summary>
        /// "ld d,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0x55
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_L(byte opCode)
        {
            Registers.D = Registers.L;
        }

        /// <summary>
        /// "ld d,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0 | 0x56
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdD_HLi(byte opCode)
        {
            Registers.D = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld d,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to D.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 0x57
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdD_A(byte opCode)
        {
            Registers.D = Registers.A;
        }

        /// <summary>
        /// "ld e,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0x58
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_B(byte opCode)
        {
            Registers.E = Registers.B;
        }

        /// <summary>
        /// "ld e,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0x59
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_C(byte opCode)
        {
            Registers.E = Registers.C;
        }

        /// <summary>
        /// "ld e,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 0 | 0x5A
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_D(byte opCode)
        {
            Registers.E = Registers.D;
        }

        /// <summary>
        /// "ld e,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 0x5C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_H(byte opCode)
        {
            Registers.E = Registers.H;
        }

        /// <summary>
        /// "ld e,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 1 | 0x5D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_L(byte opCode)
        {
            Registers.E = Registers.L;
        }

        /// <summary>
        /// "ld e,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0 | 0x5E
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdE_HLi(byte opCode)
        {
            Registers.E = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld e,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to E.
        /// 
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 0x5F
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdE_A(byte opCode)
        {
            Registers.E = Registers.A;
        }

        /// <summary>
        /// "ld h,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0x60
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_B(byte opCode)
        {
            Registers.H = Registers.B;
        }

        /// <summary>
        /// "ld h,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0x61
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_C(byte opCode)
        {
            Registers.H = Registers.C;
        }

        /// <summary>
        /// "ld h,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0x62
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_D(byte opCode)
        {
            Registers.H = Registers.D;
        }

        /// <summary>
        /// "ld h,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 0x63
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_E(byte opCode)
        {
            Registers.H = Registers.E;
        }

        /// <summary>
        /// "ld h,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0x65
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_L(byte opCode)
        {
            Registers.H = Registers.L;
        }

        /// <summary>
        /// "ld h,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 0x66
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdH_HLi(byte opCode)
        {
            Registers.H = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld h,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to H.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 0x67
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdH_A(byte opCode)
        {
            Registers.H = Registers.A;
        }

        /// <summary>
        /// "ld l,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0x68
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_B(byte opCode)
        {
            Registers.L = Registers.B;
        }

        /// <summary>
        /// "ld l,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0x69
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_C(byte opCode)
        {
            Registers.L = Registers.C;
        }

        /// <summary>
        /// "ld l,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0x6A
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_D(byte opCode)
        {
            Registers.L = Registers.D;
        }

        /// <summary>
        /// "ld l,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 0x6B
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_E(byte opCode)
        {
            Registers.L = Registers.E;
        }

        /// <summary>
        /// "ld l,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0x6C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_H(byte opCode)
        {
            Registers.L = Registers.H;
        }

        /// <summary>
        /// "ld l,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 0x6E
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdL_HLi(byte opCode)
        {
            Registers.L = ReadMemory(Registers.HL);
            ClockP3();
        }

        /// <summary>
        /// "ld l,a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to L.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 0x6F
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdL_A(byte opCode)
        {
            Registers.L = Registers.A;
        }

        /// <summary>
        /// "ld (hl),b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0x70
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_B(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.B);
            ClockP3();
        }

        /// <summary>
        /// "ld (hl),c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0x71
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_C(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.C);
            ClockP3();
        }

        /// <summary>
        /// "ld (hl),d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0x72
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_D(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.D);
            ClockP3();
        }

        /// <summary>
        /// "ld (hl),e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 0x73
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_E(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.E);
            ClockP3();
        }

        /// <summary>
        /// "ld (hl),h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0x74
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_H(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.H);
            ClockP3();
        }

        /// <summary>
        /// "ld (hl),l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 0x75
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_L(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.L);
            ClockP3();
        }

        /// <summary>
        /// "halt" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The HALT instruction suspends CPU operation until a subsequent 
        /// interrupt or reset is received.While in the HALT state, 
        /// the processor executes NOPs to maintain memory refresh logic.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 0x76
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void HALT(byte opCode)
        {
            IsInHaltedState = true;
            Registers.PC--;
        }

        /// <summary>
        /// "ld (hl),a" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of A are loaded to the memory location specified 
        /// by the contents of HL.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0x77
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_A(byte opCode)
        {
            WriteMemory(Registers.HL, Registers.A);
            ClockP3();
        }

        /// <summary>
        /// "ld a,b" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of B are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0x78
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_B(byte opCode)
        {
            Registers.A = Registers.B;
        }

        /// <summary>
        /// "ld a,c" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of C are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0x79
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_C(byte opCode)
        {
            Registers.A = Registers.C;
        }

        /// <summary>
        /// "ld a,d" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of D are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0x7A
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_D(byte opCode)
        {
            Registers.A = Registers.D;
        }

        /// <summary>
        /// "ld a,e" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of E are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 0x7B
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_E(byte opCode)
        {
            Registers.A = Registers.E;
        }

        /// <summary>
        /// "ld a,h" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of H are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0x7C
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_H(byte opCode)
        {
            Registers.A = Registers.H;
        }

        /// <summary>
        /// "ld a,l" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of L are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 0x7D
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void LdA_L(byte opCode)
        {
            Registers.A = Registers.L;
        }

        /// <summary>
        /// "ld a,(hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit contents of memory location (HL) are loaded to A.
        /// 
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0 | 0x7E
        /// =================================
        /// T-States: 4, 3 (7)
        /// </remarks>
        private void LdA_HLi(byte opCode)
        {
            Registers.A = ReadMemory(Registers.HL);
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
            var val = ReadMemory(Registers.PC);
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
        private void Scf(byte opCode)
        {
            Registers.F = (byte)((Registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV)) 
                | (Registers.A & (FlagsSetMask.R5 | FlagsSetMask.R3)) 
                | FlagsSetMask.C);
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
            Registers.F = (byte)((Registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV))
                | (Registers.A & (FlagsSetMask.R5 | FlagsSetMask.R3))
                | ((Registers.F & FlagsSetMask.C) != 0 ? FlagsSetMask.H : FlagsSetMask.C));
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
        /// "RET NZ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If Z flag is not set, the byte at the memory location specified
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
        /// | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0xC0
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetNZ(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.Z) != 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort) (ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "pop bc" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair BC. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0xC1
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopBC(byte opCode)
        {
            ushort val = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.BC = (ushort)(ReadMemory(Registers.SP) << 8 | val);
            ClockP3();
            Registers.SP++;
        }

        /// <summary>
        /// "jp nz,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If Z flag is not set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 0xC2
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNZ_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) != 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "jp NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Operand NN is loaded to PC. The next instruction is fetched 
        /// from the location designated by the new contents of the PC.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 0xC3
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "call nz,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag Z is not set, this instruction pushes the current 
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
        /// | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0xC4
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallNZ(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort) (ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) != 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte) (Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte) Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "push bc" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair BC are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0xC5
        /// =================================
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushBC(byte opCode)
        {
            var val = Registers.BC;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(val >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(val & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// "rst 00h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0000H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 1 | 0xC7
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst00(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0000;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET Z" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If Z flag is set, the byte at the memory location specified
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 0xC8
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetZ(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.Z) == 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "ret" operation
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 0xC9
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void Ret(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "jp z,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If Z flag is set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0xCA
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpZ_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) == 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "call z,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag Z is set, this instruction pushes the current 
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
        /// | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 0xCC
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallZ(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.Z) == 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "call NN" operation
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
        /// | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 0xCD
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
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
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
        /// "rst 08h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0008H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 0xCF
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst08(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0008;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET NC" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If C flag is not set, the byte at the memory location specified
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
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0xD0
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetNC(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.C) != 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "pop de" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair DE. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 0xD1
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopDE(byte opCode)
        {
            ushort val = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.DE = (ushort)(ReadMemory(Registers.SP) << 8 | val);
            ClockP3();
            Registers.SP++;
        }

        /// <summary>
        /// "jp nc,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If C flag is not set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 0xD2
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNC_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) != 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "out (N),a" operation
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
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0xD3
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
        /// "call nc,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag C is not set, this instruction pushes the current 
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
        /// | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0xD4
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallNC(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) != 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "push de" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair DE are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0xD5
        /// =================================
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushDE(byte opCode)
        {
            var val = Registers.DE;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(val >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(val & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// "rst 10h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0010H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 0xD7
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst10(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0010;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET C" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If C flag is set, the byte at the memory location specified
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
        /// | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0xD8
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetC(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.C) == 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "exx" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Each 2-byte value in register pairs BC, DE, and HL is exchanged
        /// with the 2-byte value in BC', DE', and HL', respectively.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0xD9
        /// =================================
        /// T-States: 4, (4)
        /// </remarks>
        private void Exx(byte opCode)
        {
            Registers.ExchangeRegisterSet();
        }

        /// <summary>
        /// "jp c,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If C flag is not set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 0xDA
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpC_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) == 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "in a,(N)" operation
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
        /// | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0xDB
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
        /// "call c,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag C is set, this instruction pushes the current 
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
        /// | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0xDC
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallC(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.C) == 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "rst 18h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0018H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 0xDF
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst18(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0018;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET PO" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If PV flag is not set, the byte at the memory location specified
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
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0xE0
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetPO(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.PV) != 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "pop hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair HL. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0xE1
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopHL(byte opCode)
        {
            ushort val = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.HL = (ushort)(ReadMemory(Registers.SP) << 8 | val);
            ClockP3();
            Registers.SP++;
        }

        /// <summary>
        /// "jp po,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If PV flag is not set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0xE2
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpPO_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.PV) != 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "ex (sp),hl" operation
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
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 0xE3
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
        /// "call po,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag PV is not set, this instruction pushes the current 
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
        /// | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 0xE4
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallPO(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.PV) != 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "push hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair HL are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0xE5
        /// =================================
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushHL(byte opCode)
        {
            var val = Registers.HL;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(val >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(val & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// "rst 20h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0020H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 0xE7
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst20(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0020;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET PE" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If PV flag is not set, the byte at the memory location specified
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
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0xE8
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetPE(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.PV) == 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "jp (hl)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// PC is loaded with the contents of HL. The next instruction is 
        /// fetched from the location designated by the new contents of PC.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0xE9
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void JpHL(byte opCode)
        {
            Registers.PC = Registers.HL;
        }

        /// <summary>
        /// "jp pe,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If PV flag is set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0xEA
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpPE_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.PV) == 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "ex de,hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 2-byte contents of register pairs DE and HL are exchanged.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 0xEB
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void ExDEHL(byte opCode)
        {
            Registers.Swap(ref Registers.DE, ref Registers.HL);
        }

        /// <summary>
        /// "call pe,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag PV is set, this instruction pushes the current 
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
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0xEC
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallPE(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.PV) == 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "rst 28h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0028H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 0xEF
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst28(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0028;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET P" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If S flag is not set, the byte at the memory location specified
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
        /// | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0xF0
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetP(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.S) != 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "pop af" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to register pair AF. SP holds the 16-bit address 
        /// of the current top of the stack. This instruction first loads to 
        /// the low-order portion of RR, the byte at the memory location 
        /// corresponding to the contents of SP; then SP is incremented and 
        /// the contents of the corresponding adjacent memory location are 
        /// loaded to the high-order portion of RR and the SP is now incremented 
        /// again.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0xF1
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopAF(byte opCode)
        {
            ushort val = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.AF = (ushort)(ReadMemory(Registers.SP) << 8 | val);
            ClockP3();
            Registers.SP++;
        }

        /// <summary>
        /// "jp p,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If S flag is not set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0xF2
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpP_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.S) != 0) return;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "di" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Disables the maskable interrupt by resetting the interrupt
        /// enable flip-flops (IFF1 and IFF2).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 0xF3
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void Di(byte opCode)
        {
            IFF2 = IFF1 = false;
        }

        /// <summary>
        /// "call p,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag S is not set, this instruction pushes the current 
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
        /// | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0xF4
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallP(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.S) != 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "push af" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the register pair BC are pushed to the external
        /// memory last-in, first-out (LIFO) stack. SP holds the 16-bit 
        /// address of the current top of the Stack. This instruction first 
        /// decrements SP and loads the high-order byte of register pair RR 
        /// to the memory address specified by SP. Then SP is decremented again
        /// and loads the low-order byte of RR to the memory location 
        /// corresponding to this new address in SP.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 0xF5
        /// =================================
        /// T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushAF(byte opCode)
        {
            var val = Registers.AF;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(val >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(val & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// "rst 30h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0030H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 1 | 0xF7
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst30(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0030;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "RET M" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If S flag is set, the byte at the memory location specified
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
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0xF8
        /// =================================
        /// T-States: If X is true: 5, 3, 3 (11)
        ///           If X is false: 5 (5)
        /// </remarks>
        private void RetM(byte opCode)
        {
            ClockP1();
            if ((Registers.F & FlagsSetMask.S) == 0) return;
            Registers.MW = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "ld sp,hl" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of HL are loaded to SP.
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0xF9
        /// =================================
        /// T-States: 4 (6)
        /// </remarks>
        private void LdSPHL(byte opCode)
        {
            Registers.SP = Registers.HL;
            ClockP2();
        }

        /// <summary>
        /// "jp m,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If S flag is set, the instruction loads operand NN 
        /// to PC, and the program continues with the instruction 
        /// beginning at address NN.
        /// If condition X is false, PC is incremented as usual, and 
        /// the program continues with the next sequential instruction.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0xFA
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpM_NN(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.S) == 0) return;
            Registers.PC = Registers.MW;
        }


        /// <summary>
        /// "ei" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Sets both interrupt enable flip flops (IFFI and IFF2) to a
        /// logic 1 value, allowing recognition of any maskable interrupt.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 0xFB
        /// =================================
        /// T-States: 4 (4)
        /// </remarks>
        private void Ei(byte opCode)
        {
            IFF2 = IFF1 = IsInterruptBlocked = true;
        }
        /// <summary>
        /// "call m,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// If flag S is set, this instruction pushes the current 
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
        /// | 1 | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0xFC
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallM(byte opCode)
        {
            Registers.MW = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC) << 8);
            ClockP3();
            Registers.PC++;
            if ((Registers.F & FlagsSetMask.S) == 0) return;
            ClockP1();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// "rst 38h" operation
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
        /// SP. The Restart instruction allows for a jump to address 0038H.
        /// Because all addresses are stored in Page 0 of memory, the high-order
        /// byte of PC is loaded with 0x00.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 0xFF
        /// =================================
        /// T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst38(byte opCode)
        {
            Registers.SP--;
            ClockP1();

            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)Registers.PC);
            ClockP3();

            Registers.MW = 0x0038;
            Registers.PC = Registers.MW;
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
            var val = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;

            _AluAlgorithms[(opCode & 0x38) >> 3](val, Registers.CFlag);
        }
    }
}