// ReSharper disable InconsistentNaming

using System;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    ///     This partion of the class provides standard CPU operations
    ///     for direct (with no prefix) execution
    /// </summary>
    public partial class Z80Cpu
    {
        /// <summary>
        ///     Standard (non-prefixed) operations jump table
        /// </summary>
        private Action[] _standarOperations;

        /// <summary>
        ///     Processes the operations withno op code prefix
        /// </summary>
        private void ProcessStandardOperations()
        {
            var opMethod = _indexMode == OpIndexMode.None
                ? _standarOperations[_opCode]
                : _indexedOperations[_opCode];
            opMethod?.Invoke();
        }

        /// <summary>
        ///     Initializes the standard operation execution tables
        /// </summary>
        private void InitializeNormalOpsExecutionTable()
        {
            _standarOperations = new Action[]
            {
                null, LdBCNN, LdBCiA, IncBC, IncB, DecB, LdBN, Rlca, // 00..07
                ExAF, AddHLBC, LdABCi, DecBC, IncC, DecC, LdCN, Rrca, // 08..0F
                Djnz, LdDENN, LdDEiA, IncDE, IncD, DecD, LdDN, Rla, // 10..17
                JrE, AddHLDE, LdADEi, DecDE, IncE, DecE, LdEN, Rra, // 18..1F
                JrNZ, LdHLNN, LdNNiHL, IncHL, IncH, DecH, LdHN, Daa, // 20..27
                JrZ, AddHLHL, LdHLNNi, DecHL, IncL, DecL, LdLN, Cpl, // 28..2F
                JrNC, LdSPNN, LdNNA, IncSP, IncHLi, DecHLi, LdHLiN, Scf, // 30..37
                JrC, AddHLSP, LdNNiA, DecSP, IncA, DecA, LdAN, Ccf, // 38..3F
                null, LdB_C, LdB_D, LdB_E, LdB_H, LdB_L, LdB_HLi, LdB_A, // 40..47
                LdC_B, null, LdC_D, LdC_E, LdC_H, LdC_L, LdC_HLi, LdC_A, // 48..4F
                LdD_B, LdD_C, null, LdD_E, LdD_H, LdD_L, LdD_HLi, LdD_A, // 50..57
                LdE_B, LdE_C, LdE_D, null, LdE_H, LdE_L, LdE_HLi, LdE_A, // 58..5F
                LdH_B, LdH_C, LdH_D, LdH_E, null, LdH_L, LdH_HLi, LdH_A, // 60..67
                LdL_B, LdL_C, LdL_D, LdL_E, LdL_H, null, LdL_HLi, LdL_A, // 68..6F
                LdHLi_B, LdHLi_C, LdHLi_D, LdHLi_E, LdHLi_H, LdHLi_L, HALT, LdHLi_A, // 70..77
                LdA_B, LdA_C, LdA_D, LdA_E, LdA_H, LdA_L, LdA_HLi, null, // 78..7F
                AddA_B, AddA_C, AddA_D, AddA_E, AddA_H, AddA_L, AddA_HLi, AddA_A, // 80..87
                AdcA_B, AdcA_C, AdcA_D, AdcA_E, AdcA_H, AdcA_L, AdcA_HLi, AdcA_A, // 88..8F
                SubB, SubC, SubD, SubE, SubH, SubL, SubHLi, SubA, // 90..97
                SbcB, SbcC, SbcD, SbcE, SbcH, SbcL, SbcHLi, SbcA, // 98..9F
                AndB, AndC, AndD, AndE, AndH, AndL, AndHLi, AndA, // A0..A7
                XorB, XorC, XorD, XorE, XorH, XorL, XorHLi, XorA, // A8..AF
                OrB, OrC, OrD, OrE, OrH, OrL, OrHLi, OrA, // B0..B7
                CpB, CpC, CpD, CpE, CpH, CpL, CpHLi, CpA, // B8..BF
                RetNZ, PopBC, JpNZ_NN, JpNN, CallNZ, PushBC, AluAN, Rst00, // C0..C7
                RetZ, Ret, JpZ_NN, null, CallZ, CallNN, AluAN, Rst08, // C8..CF
                RetNC, PopDE, JpNC_NN, OutNA, CallNC, PushDE, AluAN, Rst10, // D0..D7
                RetC, Exx, JpC_NN, InAN, CallC, null, AluAN, Rst18, // D8..DF
                RetPO, PopHL, JpPO_NN, ExSPiHL, CallPO, PushHL, AluAN, Rst20, // E0..E7
                RetPE, JpHL, JpPE_NN, ExDEHL, CallPE, null, AluAN, Rst28, // E8..EF
                RetP, PopAF, JpP_NN, Di, CallP, PushAF, AluAN, Rst30, // F0..F7
                RetM, LdSPHL, JpM_NN, Ei, CallM, null, AluAN, Rst38 // F8..FF
            };
        }

        /// <summary>
        ///     "ld bc,NN" operation
        /// </summary>
        /// <remarks>
        ///     The 16-bit integer value is loaded to the BC register pair.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0x01
        ///     =================================
        ///     |             N Low             |
        ///     =================================
        ///     |             N High            |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdBCNN()
        {
            _registers.C = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.B = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "ld (bc),a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the A are loaded to the memory location
        ///     specified by the contents of the register pair BC.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0x02
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdBCiA()
        {
            WriteMemory(_registers.BC, _registers.A);
            _registers.MH = _registers.A;
            ClockP3();
        }

        /// <summary>
        ///     "inc bc" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair BC are incremented.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0x03
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void IncBC()
        {
            _registers.BC++;
            ClockP2();
        }

        /// <summary>
        ///     "inc b" operation
        /// </summary>
        /// <remarks>
        ///     Register B is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0x04
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncB()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.B++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec b" operation
        /// </summary>
        /// <remarks>
        ///     Register B is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0x05
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecB()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.B--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld b,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to B.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0x06
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdBN()
        {
            _registers.B = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "rlca" operation
        /// </summary>
        /// <remarks>
        ///     The contents of  A are rotated left 1 bit position. The
        ///     sign bit (bit 7) is copied to the Carry flag and also
        ///     to bit 0.
        ///     S, Z, P/V are not affected.
        ///     H, N are reset.
        ///     C is data from bit 7 of A.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0x07
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Rlca()
        {
            int rlcaVal = _registers.A;
            rlcaVal <<= 1;
            var cf = (byte) ((rlcaVal & 0x100) != 0 ? FlagsSetMask.C : 0);
            if (cf != 0)
                rlcaVal = (rlcaVal | 0x01) & 0xFF;
            _registers.A = (byte) rlcaVal;
            _registers.F = (byte) (cf | (_registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        ///     "ex af,af'" operation
        /// </summary>
        /// <remarks>
        ///     The 2-byte contents of the register pairs AF and AF' are exchanged.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0x08
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void ExAF()
        {
            _registers.ExchangeAfSet();
        }

        /// <summary>
        ///     "add hl,bc" operation
        /// </summary>
        /// <remarks>
        ///     The contents of BC are added to the contents of HL and
        ///     the result is stored in HL.
        ///     S, Z, P/V are not affected.
        ///     H is set if carry from bit 11; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 15; otherwise, it is reset.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0x09
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLBC()
        {
            _registers.MW = (ushort) (_registers.HL + 1);
            _registers.HL = AluAddHL(_registers.HL, _registers.BC);
            ClockP7();
        }

        /// <summary>
        ///     "ld a,(bc)" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the memory location specified by BC are loaded to A.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0x0A
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdABCi()
        {
            _registers.MW = (ushort) (_registers.BC + 1);
            _registers.A = ReadMemory(_registers.BC);
            ClockP3();
        }

        /// <summary>
        ///     "dec bc" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair BC are decremented.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0x0B
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void DecBC()
        {
            _registers.BC--;
            ClockP2();
        }

        /// <summary>
        ///     "inc c" operation
        /// </summary>
        /// <remarks>
        ///     Register C is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0x0C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncC()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.C++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec c" operation
        /// </summary>
        /// <remarks>
        ///     Register C is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0x0D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecC()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.C--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld c,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to C.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0x0E
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdCN()
        {
            _registers.C = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "rrca" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are rotated right 1 bit position. Bit 0 is
        ///     copied to the Carry flag and also to bit 7.
        ///     S, Z, P/V are not affected.
        ///     H, N are reset.
        ///     C is data from bit 0 of A.
        ///     =================================
        ///     | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0x0F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Rrca()
        {
            int rrcaVal = _registers.A;
            var cf = (byte) ((rrcaVal & 0x01) != 0 ? FlagsSetMask.C : 0);
            if ((rrcaVal & 0x01) != 0)
                rrcaVal = (rrcaVal >> 1) | 0x80;
            else
                rrcaVal >>= 1;
            _registers.A = (byte) rrcaVal;
            _registers.F = (byte) (cf | (_registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        ///     "djnz E" operation
        /// </summary>
        /// <remarks>
        ///     This instruction is similar to the conditional jump
        ///     instructions except that value of B is used to determine
        ///     branching. B is decremented, and if a nonzero value remains,
        ///     the value of displacement E is added to PC. The next
        ///     instruction is fetched from the location designated by
        ///     the new contents of the PC. The jump is measured from the
        ///     address of the instruction op code and contains a range of
        ///     –126 to +129 bytes. The assembler automatically adjusts for
        ///     the twice incremented PC. If the result of decrementing leaves
        ///     B with a zero value, the next instruction executed is taken
        ///     from the location following this instruction.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0x10
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: B!=0: 5, 3, 5 (13)
        ///     B=0:  5, 3 (8)
        /// </remarks>
        private void Djnz()
        {
            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            ClockP1();
            if (--_registers.B == 0) return;

            var oldPc = _registers.PC - 2;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc, 
                    $"djnz {_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "ld de,NN" operation
        /// </summary>
        /// <remarks>
        ///     The 16-bit integer value is loaded to the DE register pair.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0x11
        ///     =================================
        ///     |             N Low             |
        ///     =================================
        ///     |             N High            |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdDENN()
        {
            _registers.E = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.D = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "ld (de),a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the A are loaded to the memory location
        ///     specified by the contents of the register pair DE.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0x12
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdDEiA()
        {
            WriteMemory(_registers.DE, _registers.A);
            _registers.MH = _registers.A;
            ClockP3();
        }

        /// <summary>
        ///     "inc de" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair DE are incremented.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0x13
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void IncDE()
        {
            _registers.DE++;
            ClockP2();
        }

        /// <summary>
        ///     "inc d" operation
        /// </summary>
        /// <remarks>
        ///     Register D is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0x14
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncD()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.D++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec d" operation
        /// </summary>
        /// <remarks>
        ///     Register D is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0x15
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecD()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.D--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld d,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to D.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0x16
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdDN()
        {
            _registers.D = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "rla" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are rotated left 1 bit position through the
        ///     Carry flag. The previous contents of the Carry flag are copied
        ///     to bit 0.
        ///     S, Z, P/V are not affected.
        ///     H, N are reset.
        ///     C is data from bit 7 of A.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x17
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Rla()
        {
            var rlcaVal = _registers.A;
            var newCF = (rlcaVal & 0x80) != 0 ? FlagsSetMask.C : 0;
            rlcaVal <<= 1;
            if (_registers.CFlag)
                rlcaVal |= 0x01;
            _registers.A = rlcaVal;
            _registers.F = (byte) ((byte) newCF | (_registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        ///     "jr e" operation
        /// </summary>
        /// <remarks>
        ///     This instruction provides for unconditional branching
        ///     to other segments of a program. The value of displacement E is
        ///     added to PC and the next instruction is fetched from the location
        ///     designated by the new contents of the PC. This jump is measured
        ///     from the address of the instruction op code and contains a range
        ///     of –126 to +129 bytes. The assembler automatically adjusts for
        ///     the twice incremented PC.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0x18
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: 4, 3, 5 (12)
        /// </remarks>
        private void JrE()
        {
            var oldPc = _registers.PC - 1;

            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jr {_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "add hl,de" operation
        /// </summary>
        /// <remarks>
        ///     The contents of DE are added to the contents of HL and
        ///     the result is stored in HL.
        ///     S, Z, P/V are not affected.
        ///     H is set if carry from bit 11; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 15; otherwise, it is reset.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0x19
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLDE()
        {
            _registers.MW = (ushort) (_registers.HL + 1);
            _registers.HL = AluAddHL(_registers.HL, _registers.DE);
            ClockP7();
        }

        /// <summary>
        ///     "ld a,(de)" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the memory location specified by DE are loaded to A.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0x1A
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdADEi()
        {
            _registers.MW = (ushort) (_registers.DE + 1);
            _registers.A = ReadMemory(_registers.DE);
            ClockP3();
        }

        /// <summary>
        ///     "dec de" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair DE are decremented.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0x1B
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void DecDE()
        {
            _registers.DE--;
            ClockP2();
        }

        /// <summary>
        ///     "inc e" operation
        /// </summary>
        /// <remarks>
        ///     Register E is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0x1C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncE()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.E++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec e" operation
        /// </summary>
        /// <remarks>
        ///     Register E is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0x1D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecE()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.E--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld e,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to E.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0x1E
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdEN()
        {
            _registers.E = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "rra" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are rotated right 1 bit position through the
        ///     Carry flag. The previous contents of the Carry flag are copied
        ///     to bit 7.
        ///     S, Z, P/V are not affected.
        ///     H, N are reset.
        ///     C is data from bit 0 of A.
        ///     =================================
        ///     | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x1F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Rra()
        {
            var rlcaVal = _registers.A;
            var newCF = (rlcaVal & 0x01) != 0 ? FlagsSetMask.C : 0;
            rlcaVal >>= 1;
            if (_registers.CFlag)
                rlcaVal |= 0x80;
            _registers.A = rlcaVal;
            _registers.F = (byte) ((byte) newCF | (_registers.F & FlagsSetMask.SZPV));
        }

        /// <summary>
        ///     "JR NZ,E" operation
        /// </summary>
        /// <remarks>
        ///     This instruction provides for conditional branching to
        ///     other segments of a program depending on the results of a test
        ///     (Z flag is not set). If the test evaluates to *true*, the value of displacement
        ///     E is added to PC and the next instruction is fetched from the
        ///     location designated by the new contents of the PC. The jump is
        ///     measured from the address of the instruction op code and contains
        ///     a range of –126 to +129 bytes. The assembler automatically adjusts
        ///     for the twice incremented PC.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0x20
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: Condition is met: 4, 3, 5 (12)
        ///     Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrNZ()
        {
            var oldPc = _registers.PC - 1;

            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) != 0) return;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jr nz,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "ld hl,NN" operation
        /// </summary>
        /// <remarks>
        ///     The 16-bit integer value is loaded to the HL register pair.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0x21
        ///     =================================
        ///     |             N Low             |
        ///     =================================
        ///     |             N High            |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdHLNN()
        {
            _registers.L = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.H = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "ld (NN),hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the low-order portion of HL (L) are loaded to memory
        ///     address (NN), and the contents of the high-order portion of HL (H)
        ///     are loaded to the next highest memory address(NN + 1).
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0x22
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3, 3, 3 (16)
        /// </remarks>
        private void LdNNiHL()
        {
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort) ((ReadMemory(_registers.PC) << 8) | l);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort) (addr + 1);
            WriteMemory(addr, _registers.L);
            ClockP3();
            WriteMemory(_registers.MW, _registers.H);
            ClockP3();
        }

        /// <summary>
        ///     "inc hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair HL are incremented.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0x23
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void IncHL()
        {
            _registers.HL++;
            ClockP2();
        }

        /// <summary>
        ///     "inc h" operation
        /// </summary>
        /// <remarks>
        ///     Register H is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0x24
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncH()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.H++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec h" operation
        /// </summary>
        /// <remarks>
        ///     Register H is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0x25
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecH()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.H--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld h,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to H.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0x26
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHN()
        {
            _registers.H = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "daa" operation
        /// </summary>
        /// <remarks>
        ///     This instruction conditionally adjusts A for BCD addition
        ///     and subtraction operations. For addition(ADD, ADC, INC) or
        ///     subtraction(SUB, SBC, DEC, NEG), the following table indicates
        ///     the operation being performed:
        ///     ====================================================
        ///     |Oper.|C before|Upper|H before|Lower|Number|C after|
        ///     |     |DAA     |Digit|Daa     |Digit|Added |Daa    |
        ///     ====================================================
        ///     | ADD |   0    | 9-0 |   0    | 0-9 |  00  |   0   |
        ///     |     |   0    | 0-8 |   0    | A-F |  06  |   0   |
        ///     |     |   0    | 0-9 |   1    | 0-3 |  06  |   0   |
        ///     |     |   0    | A-F |   0    | 0-9 |  60  |   1   |
        ///     ----------------------------------------------------
        ///     | ADC |   0    | 9-F |   0    | A-F |  66  |   1   |
        ///     ----------------------------------------------------
        ///     | INC |   0    | A-F |   1    | 0-3 |  66  |   1   |
        ///     |     |   1    | 0-2 |   0    | 0-9 |  60  |   1   |
        ///     |     |   1    | 0-2 |   0    | A-F |  66  |   1   |
        ///     |     |   1    | 0-3 |   1    | 0-3 |  66  |   1   |
        ///     ----------------------------------------------------
        ///     | SUB |   0    | 0-9 |   0    | 0-9 |  00  |   0   |
        ///     ----------------------------------------------------
        ///     | SBC |   0    | 0-8 |   1    | 6-F |  FA  |   0   |
        ///     ----------------------------------------------------
        ///     | DEC |   1    | 7-F |   0    | 0-9 |  A0  |   1   |
        ///     ----------------------------------------------------
        ///     | NEG |   1    | 6-7 |   1    | 6-F |  9A  |   1   |
        ///     ====================================================
        ///     S is set if most-significant bit of the A is 1 after an
        ///     operation; otherwise, it is reset.
        ///     Z is set if A is 0 after an operation; otherwise, it is reset.
        ///     H: see the DAA instruction table.
        ///     P/V is set if A is at even parity after an operation;
        ///     otherwise, it is reset.
        ///     N is not affected.
        ///     C: see the DAA instruction table.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0x27
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Daa()
        {
            var daaIndex = _registers.A + (((_registers.F & 3) + ((_registers.F >> 2) & 4)) << 8);
            _registers.AF = s_DaaResults[daaIndex];
        }

        /// <summary>
        ///     "JR Z,E" operation
        /// </summary>
        /// <remarks>
        ///     This instruction provides for conditional branching to
        ///     other segments of a program depending on the results of a test
        ///     (Z flag is set). If the test evaluates to *true*, the value of displacement
        ///     E is added to PC and the next instruction is fetched from the
        ///     location designated by the new contents of the PC. The jump is
        ///     measured from the address of the instruction op code and contains
        ///     a range of –126 to +129 bytes. The assembler automatically adjusts
        ///     for the twice incremented PC.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0x28
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: Condition is met: 4, 3, 5 (12)
        ///     Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrZ()
        {
            var oldPc = _registers.PC - 1;

            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) == 0) return;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jr z,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "add hl,hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of HL are added to the contents of HL and
        ///     the result is stored in HL.
        ///     S, Z, P/V are not affected.
        ///     H is set if carry from bit 11; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 15; otherwise, it is reset.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 0x29
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLHL()
        {
            _registers.MW = (ushort) (_registers.HL + 1);
            _registers.HL = AluAddHL(_registers.HL, _registers.HL);
            ClockP7();
        }

        /// <summary>
        ///     "ld hl,(NN)" operation
        /// </summary>
        /// <remarks>
        ///     The contents of memory address (NN) are loaded to the
        ///     low-order portion of HL (L), and the contents of the next
        ///     highest memory address (NN + 1) are loaded to the high-order
        ///     portion of HL (H).
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0x2A
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3, 3, 3 (16)
        /// </remarks>
        private void LdHLNNi()
        {
            ushort adr = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            adr += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort) (adr + 1);
            ushort val = ReadMemory(adr);
            ClockP3();
            val += (ushort) (ReadMemory(_registers.MW) << 8);
            ClockP3();
            _registers.HL = val;
        }

        /// <summary>
        ///     "dec hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair HL are decremented.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0x2B
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void DecHL()
        {
            _registers.HL--;
            ClockP2();
        }

        /// <summary>
        ///     "inc l" operation
        /// </summary>
        /// <remarks>
        ///     Register L is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0x2C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncL()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.L++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec l" operation
        /// </summary>
        /// <remarks>
        ///     Register L is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 2 | 0 | 1 | 1 | 0 | 1 | 0x2D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecL()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.L--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld l,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to H.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0x2E
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdLN()
        {
            _registers.L = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "cpl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are inverted (one's complement).
        ///     S, Z, P/V, C are not affected.
        ///     H and N are set.
        ///     =================================
        ///     | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0x2F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Cpl()
        {
            _registers.A ^= 0xFF;
            _registers.F = (byte) ((_registers.F & ~FlagsSetMask.R3R5)
                                  | FlagsSetMask.NH
                                  | FlagsSetMask.H
                                  | (_registers.A & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     "JR NC,E" operation
        /// </summary>
        /// <remarks>
        ///     This instruction provides for conditional branching to
        ///     other segments of a program depending on the results of a test
        ///     (C flag is not set). If the test evaluates to *true*, the value of displacement
        ///     E is added to PC and the next instruction is fetched from the
        ///     location designated by the new contents of the PC. The jump is
        ///     measured from the address of the instruction op code and contains
        ///     a range of –126 to +129 bytes. The assembler automatically adjusts
        ///     for the twice incremented PC.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0x30
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: Condition is met: 4, 3, 5 (12)
        ///     Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrNC()
        {
            var oldPc = _registers.PC - 1;

            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) != 0) return;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jr nc,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "ld sp,NN" operation
        /// </summary>
        /// <remarks>
        ///     The 16-bit integer value is loaded to the SP register pair.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0x31
        ///     =================================
        ///     |             N Low             |
        ///     =================================
        ///     |             N High            |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdSPNN()
        {
            var oldSP = _registers.SP;

            var p = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var s = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.SP = (ushort) ((s << 8) | p);

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 3),
                    $"ld sp,{_registers.SP:X4}H",
                    oldSP,
                    _registers.SP,
                    Tacts
                ));
        }

        /// <summary>
        ///     "ld a,(NN)" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to the memory address specified by
        ///     the operand NN
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0x32
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LdNNA()
        {
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort) ((ReadMemory(_registers.PC) << 8) | l);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort) (((addr + 1) & 0xFF) + (_registers.A << 8));
            WriteMemory(addr, _registers.A);
            _registers.MH = _registers.A;
            ClockP3();
        }

        /// <summary>
        ///     "inc sp" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair SP are incremented.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0x33
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void IncSP()
        {
            _registers.SP++;
            ClockP2();

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 1),
                    "inc sp",
                    (ushort)(_registers.SP - 1),
                    _registers.SP,
                    Tacts
                ));
        }

        /// <summary>
        ///     "inc (hl)" operation
        /// </summary>
        /// <remarks>
        ///     The byte contained in the address specified by the contents HL
        ///     is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if (HL) was 0x7F before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0x34
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void IncHLi()
        {
            var memValue = ReadMemory(_registers.HL);
            ClockP3();
            memValue = AluIncByte(memValue);
            ClockP1();
            WriteMemory(_registers.HL, memValue);
            ClockP3();
        }

        /// <summary>
        ///     "dec (hl)" operation
        /// </summary>
        /// <remarks>
        ///     The byte contained in the address specified by the contents HL
        ///     is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if (HL) was 0x80 before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0x35
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void DecHLi()
        {
            var memValue = ReadMemory(_registers.HL);
            ClockP3();
            memValue = AluDecByte(memValue);
            ClockP1();
            WriteMemory(_registers.HL, memValue);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),N" operation
        /// </summary>
        /// <remarks>
        ///     The N 8-bit value is loaded to the memory address specified by HL.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0x36
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void LdHLiN()
        {
            var val = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            WriteMemory(_registers.HL, val);
            ClockP3();
        }

        /// <summary>
        ///     "scf" operation
        /// </summary>
        /// <remarks>
        ///     The Carry flag in F is set.
        ///     Other flags are not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 0x37
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Scf()
        {
            _registers.F = (byte) ((_registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV))
                                  | (_registers.A & (FlagsSetMask.R5 | FlagsSetMask.R3))
                                  | FlagsSetMask.C);
        }

        /// <summary>
        ///     "JR C,E" operation
        /// </summary>
        /// <remarks>
        ///     This instruction provides for conditional branching to
        ///     other segments of a program depending on the results of a test
        ///     (C flag is set). If the test evaluates to *true*, the value of displacement
        ///     E is added to PC and the next instruction is fetched from the
        ///     location designated by the new contents of the PC. The jump is
        ///     measured from the address of the instruction op code and contains
        ///     a range of –126 to +129 bytes. The assembler automatically adjusts
        ///     for the twice incremented PC.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0x38
        ///     =================================
        ///     |             E-2               |
        ///     =================================
        ///     T-States: Condition is met: 4, 3, 5 (12)
        ///     Condition is not met: 4, 3 (7)
        /// </remarks>
        private void JrC()
        {
            var oldPc = _registers.PC - 1;

            var e = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) == 0) return;
            _registers.MW = _registers.PC = (ushort) (_registers.PC + (sbyte) e);
            ClockP5();

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jr c,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "add hl,sp" operation
        /// </summary>
        /// <remarks>
        ///     The contents of SP are added to the contents of HL and
        ///     the result is stored in HL.
        ///     S, Z, P/V are not affected.
        ///     H is set if carry from bit 11; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 15; otherwise, it is reset.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0x39
        ///     =================================
        ///     T-States: 4, 4, 3 (11)
        /// </remarks>
        private void AddHLSP()
        {
            _registers.MW = (ushort) (_registers.HL + 1);
            _registers.HL = AluAddHL(_registers.HL, _registers.SP);
            ClockP7();
        }

        /// <summary>
        ///     "ld (NN),a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the memory location specified by the operands
        ///     NN are loaded to A.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 0x3A
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3, 3 (13)
        /// </remarks>
        private void LdNNiA()
        {
            ushort adr = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            adr += (ushort) (ReadMemory(_registers.PC) * 0x100);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort) (adr + 1);
            _registers.A = ReadMemory(adr);
            ClockP3();
        }

        /// <summary>
        ///     "dec sp" operation
        /// </summary>
        /// <remarks>
        ///     The contents of register pair SP are decremented.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0x3B
        ///     =================================
        ///     T-States: 4, 2 (6)
        /// </remarks>
        private void DecSP()
        {
            _registers.SP--;
            ClockP2();

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 1),
                    "dec sp",
                    (ushort)(_registers.SP + 1),
                    _registers.SP,
                    Tacts
                ));
        }

        /// <summary>
        ///     "inc a" operation
        /// </summary>
        /// <remarks>
        ///     Register A is incremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if r was 7Fh before operation; otherwise, it is reset.
        ///     N is reset.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0x3C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void IncA()
        {
            _registers.F = (byte) (s_IncOpFlags[_registers.A++] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "dec a" operation
        /// </summary>
        /// <remarks>
        ///     Register A is decremented.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4, otherwise, it is reset.
        ///     P/V is set if m was 80h before operation; otherwise, it is reset.
        ///     N is set.
        ///     C is not affected.
        ///     =================================
        ///     | 0 | 0 | 2 | 0 | 1 | 1 | 0 | 1 | 0x3D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void DecA()
        {
            _registers.F = (byte) (s_DecOpFlags[_registers.A--] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld a,N" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit integer N is loaded to A.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0x3E
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdAN()
        {
            _registers.A = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
        }

        /// <summary>
        ///     "ccf" operation
        /// </summary>
        /// <remarks>
        ///     The Carry flag in F is inverted.
        ///     Other flags are not affected.
        ///     =================================
        ///     | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0x3f
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Ccf()
        {
            _registers.F = (byte) ((_registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV))
                                  | (_registers.A & (FlagsSetMask.R5 | FlagsSetMask.R3))
                                  | ((_registers.F & FlagsSetMask.C) != 0 ? FlagsSetMask.H : FlagsSetMask.C));
        }

        /// <summary>
        ///     "ld b,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0x41
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_C()
        {
            _registers.B = _registers.C;
        }

        /// <summary>
        ///     "ld b,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 0x42
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_D()
        {
            _registers.B = _registers.D;
        }

        /// <summary>
        ///     "ld b,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 0x43
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_E()
        {
            _registers.B = _registers.E;
        }

        /// <summary>
        ///     "ld b,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0x44
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_H()
        {
            _registers.B = _registers.H;
        }

        /// <summary>
        ///     "ld b,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0x45
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_L()
        {
            _registers.B = _registers.L;
        }

        /// <summary>
        ///     "ld (hl),b" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0 | 0x46
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdB_HLi()
        {
            _registers.B = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld b,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to B.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0x47
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdB_A()
        {
            _registers.B = _registers.A;
        }

        /// <summary>
        ///     "ld c,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 0x48
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_B()
        {
            _registers.C = _registers.B;
        }

        /// <summary>
        ///     "ld c,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0x4A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_D()
        {
            _registers.C = _registers.D;
        }

        /// <summary>
        ///     "ld c,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0x4B
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_E()
        {
            _registers.C = _registers.E;
        }

        /// <summary>
        ///     "ld c,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 0x4C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_H()
        {
            _registers.C = _registers.H;
        }

        /// <summary>
        ///     "ld c,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 0x4D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_L()
        {
            _registers.C = _registers.L;
        }

        /// <summary>
        ///     "ld c,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0 | 0x4E
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdC_HLi()
        {
            _registers.C = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld c,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to C.
        ///     =================================
        ///     | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 0x4F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdC_A()
        {
            _registers.C = _registers.A;
        }

        /// <summary>
        ///     "ld d,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0x50
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_B()
        {
            _registers.D = _registers.B;
        }

        /// <summary>
        ///     "ld d,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 0x51
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_C()
        {
            _registers.D = _registers.C;
        }

        /// <summary>
        ///     "ld d,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0x53
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_E()
        {
            _registers.D = _registers.E;
        }

        /// <summary>
        ///     "ld d,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0x54
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_H()
        {
            _registers.D = _registers.H;
        }

        /// <summary>
        ///     "ld d,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0x55
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_L()
        {
            _registers.D = _registers.L;
        }

        /// <summary>
        ///     "ld d,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0 | 0x56
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdD_HLi()
        {
            _registers.D = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld d,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to D.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 0x57
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdD_A()
        {
            _registers.D = _registers.A;
        }

        /// <summary>
        ///     "ld e,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0x58
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_B()
        {
            _registers.E = _registers.B;
        }

        /// <summary>
        ///     "ld e,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0x59
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_C()
        {
            _registers.E = _registers.C;
        }

        /// <summary>
        ///     "ld e,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 0 | 0x5A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_D()
        {
            _registers.E = _registers.D;
        }

        /// <summary>
        ///     "ld e,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 0x5C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_H()
        {
            _registers.E = _registers.H;
        }

        /// <summary>
        ///     "ld e,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 1 | 0x5D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_L()
        {
            _registers.E = _registers.L;
        }

        /// <summary>
        ///     "ld e,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0 | 0x5E
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdE_HLi()
        {
            _registers.E = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld e,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to E.
        ///     =================================
        ///     | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 0x5F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdE_A()
        {
            _registers.E = _registers.A;
        }

        /// <summary>
        ///     "ld h,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0x60
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_B()
        {
            _registers.H = _registers.B;
        }

        /// <summary>
        ///     "ld h,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0x61
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_C()
        {
            _registers.H = _registers.C;
        }

        /// <summary>
        ///     "ld h,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0x62
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_D()
        {
            _registers.H = _registers.D;
        }

        /// <summary>
        ///     "ld h,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 0x63
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_E()
        {
            _registers.H = _registers.E;
        }

        /// <summary>
        ///     "ld h,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0x65
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_L()
        {
            _registers.H = _registers.L;
        }

        /// <summary>
        ///     "ld h,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 0x66
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdH_HLi()
        {
            _registers.H = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld h,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to H.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 0x67
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdH_A()
        {
            _registers.H = _registers.A;
        }

        /// <summary>
        ///     "ld l,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0x68
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_B()
        {
            _registers.L = _registers.B;
        }

        /// <summary>
        ///     "ld l,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0x69
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_C()
        {
            _registers.L = _registers.C;
        }

        /// <summary>
        ///     "ld l,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0x6A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_D()
        {
            _registers.L = _registers.D;
        }

        /// <summary>
        ///     "ld l,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 0x6B
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_E()
        {
            _registers.L = _registers.E;
        }

        /// <summary>
        ///     "ld l,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0x6C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_H()
        {
            _registers.L = _registers.H;
        }

        /// <summary>
        ///     "ld l,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 0x6E
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdL_HLi()
        {
            _registers.L = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     "ld l,a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to L.
        ///     =================================
        ///     | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 0x6F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdL_A()
        {
            _registers.L = _registers.A;
        }

        /// <summary>
        ///     "ld (hl),b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0x70
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_B()
        {
            WriteMemory(_registers.HL, _registers.B);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0x71
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_C()
        {
            WriteMemory(_registers.HL, _registers.C);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0x72
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_D()
        {
            WriteMemory(_registers.HL, _registers.D);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 0x73
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_E()
        {
            WriteMemory(_registers.HL, _registers.E);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0x74
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_H()
        {
            WriteMemory(_registers.HL, _registers.H);
            ClockP3();
        }

        /// <summary>
        ///     "ld (hl),l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 0x75
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_L()
        {
            WriteMemory(_registers.HL, _registers.L);
            ClockP3();
        }

        /// <summary>
        ///     "halt" operation
        /// </summary>
        /// <remarks>
        ///     The HALT instruction suspends CPU operation until a subsequent
        ///     interrupt or reset is received.While in the HALT state,
        ///     the processor executes NOPs to maintain memory refresh logic.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 0x76
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void HALT()
        {
            _stateFlags |= Z80StateFlags.Halted;
            _registers.PC--;
        }

        /// <summary>
        ///     "ld (hl),a" operation
        /// </summary>
        /// <remarks>
        ///     The contents of A are loaded to the memory location specified
        ///     by the contents of HL.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0x77
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdHLi_A()
        {
            WriteMemory(_registers.HL, _registers.A);
            ClockP3();
        }

        /// <summary>
        ///     "ld a,b" operation
        /// </summary>
        /// <remarks>
        ///     The contents of B are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0x78
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_B()
        {
            _registers.A = _registers.B;
        }

        /// <summary>
        ///     "ld a,c" operation
        /// </summary>
        /// <remarks>
        ///     The contents of C are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0x79
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_C()
        {
            _registers.A = _registers.C;
        }

        /// <summary>
        ///     "ld a,d" operation
        /// </summary>
        /// <remarks>
        ///     The contents of D are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0x7A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_D()
        {
            _registers.A = _registers.D;
        }

        /// <summary>
        ///     "ld a,e" operation
        /// </summary>
        /// <remarks>
        ///     The contents of E are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 0x7B
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_E()
        {
            _registers.A = _registers.E;
        }

        /// <summary>
        ///     "ld a,h" operation
        /// </summary>
        /// <remarks>
        ///     The contents of H are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0x7C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_H()
        {
            _registers.A = _registers.H;
        }

        /// <summary>
        ///     "ld a,l" operation
        /// </summary>
        /// <remarks>
        ///     The contents of L are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 0x7D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void LdA_L()
        {
            _registers.A = _registers.L;
        }

        /// <summary>
        ///     "ld a,(hl)" operation
        /// </summary>
        /// <remarks>
        ///     The 8-bit contents of memory location (HL) are loaded to A.
        ///     =================================
        ///     | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0 | 0x7E
        ///     =================================
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void LdA_HLi()
        {
            _registers.A = ReadMemory(_registers.HL);
            ClockP3();
        }

        /// <summary>
        ///     add a,b
        /// </summary>
        /// <remarks>
        ///     The contents of B are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0x80
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_B()
        {
            var src = _registers.B;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,c
        /// </summary>
        /// <remarks>
        ///     The contents of C are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0x81
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_C()
        {
            var src = _registers.C;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,d
        /// </summary>
        /// <remarks>
        ///     The contents of D are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0x82
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_D()
        {
            var src = _registers.D;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,e
        /// </summary>
        /// <remarks>
        ///     The contents of E are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0x83
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_E()
        {
            var src = _registers.E;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,h
        /// </summary>
        /// <remarks>
        ///     The contents of H are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0x84
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_H()
        {
            var src = _registers.H;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,l
        /// </summary>
        /// <remarks>
        ///     The contents of L are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0x85
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_L()
        {
            var src = _registers.L;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,(hl)
        /// </summary>
        /// <remarks>
        ///     The byte at the memory address specified by the contents of HL
        ///     is added to the contents of A, and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0x86
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_HLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     add a,a
        /// </summary>
        /// <remarks>
        ///     The contents of B are added to the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0x87
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AddA_A()
        {
            var src = _registers.A;
            _registers.F = s_AdcFlags[_registers.A * 0x100 + src];
            _registers.A += src;
        }

        /// <summary>
        ///     adc a,b
        /// </summary>
        /// <remarks>
        ///     The contents of B and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0x88
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_B()
        {
            var src = _registers.B;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,c
        /// </summary>
        /// <remarks>
        ///     The contents of C and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0x89
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_C()
        {
            var src = _registers.C;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,d
        /// </summary>
        /// <remarks>
        ///     The contents of D and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0x8A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_D()
        {
            var src = _registers.D;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,e
        /// </summary>
        /// <remarks>
        ///     The contents of E and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0x8B
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_E()
        {
            var src = _registers.E;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,h
        /// </summary>
        /// <remarks>
        ///     The contents of H and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0x8C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_H()
        {
            var src = _registers.H;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,l
        /// </summary>
        /// <remarks>
        ///     The contents of L and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0x8D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_L()
        {
            var src = _registers.L;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,(hl)
        /// </summary>
        /// <remarks>
        ///     The byte at the memory address specified by the contents of HL
        ///     and the C flag is added to the contents of A, and the
        ///     result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0x8E
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_HLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     adc a,a
        /// </summary>
        /// <remarks>
        ///     The contents of A and the C flag are added to the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if carry from bit 3; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is set if carry from bit 7; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0x8F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AdcA_A()
        {
            var src = _registers.A;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_AdcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A += (byte) (src + carry);
        }

        /// <summary>
        ///     sub b
        /// </summary>
        /// <remarks>
        ///     The contents of B are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0x90
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubB()
        {
            var src = _registers.B;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub c
        /// </summary>
        /// <remarks>
        ///     The contents of C are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0x91
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubC()
        {
            var src = _registers.C;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub d
        /// </summary>
        /// <remarks>
        ///     The contents of D are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0x92
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubD()
        {
            var src = _registers.D;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub e
        /// </summary>
        /// <remarks>
        ///     The contents of E are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0x93
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubE()
        {
            var src = _registers.E;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub h
        /// </summary>
        /// <remarks>
        ///     The contents of H are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0x94
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubH()
        {
            var src = _registers.H;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub l
        /// </summary>
        /// <remarks>
        ///     The contents of L are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0x95
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubL()
        {
            var src = _registers.L;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub (hl)
        /// </summary>
        /// <remarks>
        ///     The byte at the memory address specified by the contents of HL
        ///     is subtracted from the contents of A, and the
        ///     result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0x96
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sub a
        /// </summary>
        /// <remarks>
        ///     The contents of A are subtracted from the contents of A, and the result is
        ///     stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x97
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SubA()
        {
            var src = _registers.A;
            _registers.F = s_SbcFlags[_registers.A * 0x100 + src];
            _registers.A -= src;
        }

        /// <summary>
        ///     sbc b
        /// </summary>
        /// <remarks>
        ///     The contents of B and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0x98
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcB()
        {
            var src = _registers.B;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc c
        /// </summary>
        /// <remarks>
        ///     The contents of C and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0x99
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcC()
        {
            var src = _registers.C;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc d
        /// </summary>
        /// <remarks>
        ///     The contents of D and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0x9A
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcD()
        {
            var src = _registers.D;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc e
        /// </summary>
        /// <remarks>
        ///     The contents of E and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0x9B
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcE()
        {
            var src = _registers.E;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc h
        /// </summary>
        /// <remarks>
        ///     The contents of H and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0x9C
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcH()
        {
            var src = _registers.H;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc l
        /// </summary>
        /// <remarks>
        ///     The contents of L and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0x9D
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcL()
        {
            var src = _registers.L;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc (hl)
        /// </summary>
        /// <remarks>
        ///     The byte at the memory address specified by the contents of HL
        ///     and the C flag is subtracted from the contents of A, and the
        ///     result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0x9E
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     sbc a
        /// </summary>
        /// <remarks>
        ///     The contents of A and the C flag are subtracted from the contents of A,
        ///     and the result is stored in A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0x9F
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void SbcA()
        {
            var src = _registers.A;
            var carry = (_registers.F & FlagsSetMask.C) == 0 ? 0 : 1;
            _registers.F = s_SbcFlags[carry * 0x10000 + _registers.A * 0x100 + src];
            _registers.A -= (byte) (src + carry);
        }

        /// <summary>
        ///     and b
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between B and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0xA0
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndB()
        {
            var src = _registers.B;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and c
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between C and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0xA1
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndC()
        {
            var src = _registers.C;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and d
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between D and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0xA2
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndD()
        {
            var src = _registers.D;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and e
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between E and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0xA3
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndE()
        {
            var src = _registers.E;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and h
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between H and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0xA4
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndH()
        {
            var src = _registers.H;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and l
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between L and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0xA5
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndL()
        {
            var src = _registers.L;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and (hl)
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between the byte at the
        ///     memory address specified by the contents of HL and the byte
        ///     contained in A; the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0xA6
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     and A
        /// </summary>
        /// <remarks>
        ///     A logical AND operation is performed between A and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0xA7
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void AndA()
        {
            var src = _registers.A;
            _registers.A &= src;
            _registers.F = (byte) (s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        ///     xor b
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between B and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0xA8
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorB()
        {
            var src = _registers.B;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor c
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between C and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 0xA9
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorC()
        {
            var src = _registers.C;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor d
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between D and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0xAA
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorD()
        {
            var src = _registers.D;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor e
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between E and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0xAB
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorE()
        {
            var src = _registers.E;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor h
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between H and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0xAC
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorH()
        {
            var src = _registers.H;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor l
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between L and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 0xAD
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorL()
        {
            var src = _registers.L;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor (hl)
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between the byte at the
        ///     memory address specified by the contents of HL and the byte
        ///     contained in A; the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0xAE
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     xor a
        /// </summary>
        /// <remarks>
        ///     A logical XOR operation is performed between A and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0xAF
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void XorA()
        {
            var src = _registers.A;
            _registers.A ^= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or b
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between B and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0xB0
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrB()
        {
            var src = _registers.B;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or c
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between C and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0xB1
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrC()
        {
            var src = _registers.C;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or d
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between D and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 0xB2
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrD()
        {
            var src = _registers.D;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or e
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between E and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0xB3
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrE()
        {
            var src = _registers.E;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or h
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between H and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0xB4
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrH()
        {
            var src = _registers.H;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or l
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between L and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 0xB5
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrL()
        {
            var src = _registers.L;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or (hl)
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between the byte at the
        ///     memory address specified by the contents of HL and the byte
        ///     contained in A; the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0xB6
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     or a
        /// </summary>
        /// <remarks>
        ///     A logical OR operation is performed between A and the byte contained in A;
        ///     the result is stored in the Accumulator.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is reset.
        ///     P/V is reset if overflow; otherwise, it is reset.
        ///     N is reset.
        ///     C is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 0xB7
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void OrA()
        {
            var src = _registers.A;
            _registers.A |= src;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        ///     cp b
        /// </summary>
        /// <remarks>
        ///     The contents of B are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0xB8
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpB()
        {
            var src = _registers.B;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp c
        /// </summary>
        /// <remarks>
        ///     The contents of C are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0xB9
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpC()
        {
            var src = _registers.C;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp d
        /// </summary>
        /// <remarks>
        ///     The contents of D are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0xBA
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpD()
        {
            var src = _registers.D;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp e
        /// </summary>
        /// <remarks>
        ///     The contents of E are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0xBB
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpE()
        {
            var src = _registers.E;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp h
        /// </summary>
        /// <remarks>
        ///     The contents of H are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0xBC
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpH()
        {
            var src = _registers.H;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp l
        /// </summary>
        /// <remarks>
        ///     The contents of L are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 0xBD
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpL()
        {
            var src = _registers.L;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp (hl)
        /// </summary>
        /// <remarks>
        ///     The contents of the byte at the memory address specified by
        ///     the contents of HL are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0xBE
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpHLi()
        {
            var src = ReadMemory(_registers.HL);
            ClockP3();
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     cp a
        /// </summary>
        /// <remarks>
        ///     The contents of A are compared with the contents of A.
        ///     If there is a true compare, the Z flag is set. The execution of
        ///     this instruction does not affect A.
        ///     S is set if result is negative; otherwise, it is reset.
        ///     Z is set if result is 0; otherwise, it is reset.
        ///     H is set if borrow from bit 4; otherwise, it is reset.
        ///     P/V is set if overflow; otherwise, it is reset.
        ///     N is set.
        ///     C is set if borrow; otherwise, it is reset.
        ///     =================================
        ///     | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0xBF
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void CpA()
        {
            var src = _registers.A;
            var res = _registers.A * 0x100 + src;
            _registers.F = (byte) ((s_SbcFlags[res]
                                   & FlagsResetMask.R3 & FlagsResetMask.R5)
                                  | (res & FlagsSetMask.R3R5));
        }

        /// <summary>
        ///     "RET NZ" operation
        /// </summary>
        /// <remarks>
        ///     If Z flag is not set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0xC0
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetNZ()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.Z) != 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret nz",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "pop bc" operation
        /// </summary>
        /// <remarks>
        ///     The top two bytes of the external memory last-in, first-out (LIFO)
        ///     stack are popped to register pair BC. SP holds the 16-bit address
        ///     of the current top of the stack. This instruction first loads to
        ///     the low-order portion of RR, the byte at the memory location
        ///     corresponding to the contents of SP; then SP is incremented and
        ///     the contents of the corresponding adjacent memory location are
        ///     loaded to the high-order portion of RR and the SP is now incremented
        ///     again.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0xC1
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopBC()
        {
            var oldSp = _registers.SP;

            ushort val = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.BC = (ushort) ((ReadMemory(_registers.SP) << 8) | val);
            ClockP3();
            _registers.SP++;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "pop bc",
                    oldSp,
                    _registers.BC,
                    Tacts));
        }

        /// <summary>
        ///     "jp nz,NN" operation
        /// </summary>
        /// <remarks>
        ///     If Z flag is not set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0 | 0xC2
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNZ_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) != 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp nz,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "jp NN" operation
        /// </summary>
        /// <remarks>
        ///     Operand NN is loaded to PC. The next instruction is fetched
        ///     from the location designated by the new contents of the PC.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 0xC3
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp {_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "call nz,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag Z is not set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0 | 0xC4
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallNZ()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) != 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call nz,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "push bc" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the register pair BC are pushed to the external
        ///     memory last-in, first-out (LIFO) stack. SP holds the 16-bit
        ///     address of the current top of the Stack. This instruction first
        ///     decrements SP and loads the high-order byte of register pair RR
        ///     to the memory address specified by SP. Then SP is decremented again
        ///     and loads the low-order byte of RR to the memory location
        ///     corresponding to this new address in SP.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0xC5
        ///     =================================
        ///     T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushBC()
        {
            var oldSp = _registers.SP;

            var val = _registers.BC;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte) (val >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (val & 0xFF));
            ClockP3();

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "push bc",
                    oldSp,
                    _registers.BC,
                    Tacts));
        }

        /// <summary>
        ///     "rst 00h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0000H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 1 | 0xC7
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst00()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0000;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 00H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET Z" operation
        /// </summary>
        /// <remarks>
        ///     If Z flag is set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 0xC8
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetZ()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.Z) == 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret z",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "ret" operation
        /// </summary>
        /// <remarks>
        ///     The byte at the memory location specified by the contents of SP
        ///     is moved to the low-order eight bits of PC. SP is now incremented
        ///     and the byte at the memory location specified by the new contents
        ///     of this instruction is fetched from the memory location specified
        ///     by PC.
        ///     This instruction is normally used to return to the main line
        ///     program at the completion of a routine entered by a CALL
        ///     instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 0xC9
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void Ret()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "jp z,NN" operation
        /// </summary>
        /// <remarks>
        ///     If Z flag is set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0xCA
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpZ_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) == 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp z,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "call z,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag Z is set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 0 | 0xCC
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallZ()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.Z) == 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call z,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "call NN" operation
        /// </summary>
        /// <remarks>
        ///     The current contents of PC are pushed onto the top of the
        ///     external memory stack. The operands NN are then loaded to PC to
        ///     point to the address in memory at which the first op code of a
        ///     subroutine is to be fetched. At the end of the subroutine, a RET
        ///     instruction can be used to return to the original program flow by
        ///     popping the top of the stack back to PC. The push is accomplished
        ///     by first decrementing the current contents of SP, loading the
        ///     high-order byte of the PC contents to the memory address now pointed
        ///     to by SP; then decrementing SP again, and loading the low-order
        ///     byte of the PC contents to the top of stack.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 1 | 0 | 1 | 0xCD
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 4, 3, 3 (17)
        /// </remarks>
        private void CallNN()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            ClockP1();

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC & 0xFF));
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call {_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "rst 08h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0008H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 1 | 0xCF
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst08()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0008;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 08H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET NC" operation
        /// </summary>
        /// <remarks>
        ///     If C flag is not set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0xD0
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetNC()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.C) != 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret nc",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "pop de" operation
        /// </summary>
        /// <remarks>
        ///     The top two bytes of the external memory last-in, first-out (LIFO)
        ///     stack are popped to register pair DE. SP holds the 16-bit address
        ///     of the current top of the stack. This instruction first loads to
        ///     the low-order portion of RR, the byte at the memory location
        ///     corresponding to the contents of SP; then SP is incremented and
        ///     the contents of the corresponding adjacent memory location are
        ///     loaded to the high-order portion of RR and the SP is now incremented
        ///     again.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 0xD1
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopDE()
        {
            var oldSp = _registers.SP;

            ushort val = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.DE = (ushort) ((ReadMemory(_registers.SP) << 8) | val);
            ClockP3();
            _registers.SP++;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "pop de",
                    oldSp,
                    _registers.DE,
                    Tacts));
        }

        /// <summary>
        ///     "jp nc,NN" operation
        /// </summary>
        /// <remarks>
        ///     If C flag is not set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 0xD2
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpNC_NN()
        {
            var oldPc = _registers.PC - 1;
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) != 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp nc,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "out (N),a" operation
        /// </summary>
        /// <remarks>
        ///     The operand N is placed on the bottom half (A0 through A7) of
        ///     the address bus to select the I/O device at one of 256 possible
        ///     ports. The contents of A also appear on the top half(A8 through
        ///     A15) of the address bus at this time. Then the byte contained
        ///     in A is placed on the data bus and written to the selected
        ///     peripheral device.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0xD3
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3, 4 (11)
        /// </remarks>
        private void OutNA()
        {
            ClockP3();
            ushort port = ReadMemory(_registers.PC++);
            _registers.MW = (ushort) (((port + 1) & 0xFF) + (_registers.A << 8));
            ClockP3();
            port += (ushort) (_registers.A << 8);

            WritePort(port, _registers.A);
            ClockP1();
        }

        /// <summary>
        ///     "call nc,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag C is not set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0xD4
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallNC()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) != 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call nc,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "push de" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the register pair DE are pushed to the external
        ///     memory last-in, first-out (LIFO) stack. SP holds the 16-bit
        ///     address of the current top of the Stack. This instruction first
        ///     decrements SP and loads the high-order byte of register pair RR
        ///     to the memory address specified by SP. Then SP is decremented again
        ///     and loads the low-order byte of RR to the memory location
        ///     corresponding to this new address in SP.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0xD5
        ///     =================================
        ///     T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushDE()
        {
            var oldSp = _registers.SP;

            var val = _registers.DE;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte) (val >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (val & 0xFF));
            ClockP3();

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "push de",
                    oldSp,
                    _registers.DE,
                    Tacts));
        }

        /// <summary>
        ///     "rst 10h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0010H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 1 | 0xD7
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst10()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0010;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 10H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET C" operation
        /// </summary>
        /// <remarks>
        ///     If C flag is set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0xD8
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetC()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.C) == 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret c",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "exx" operation
        /// </summary>
        /// <remarks>
        ///     Each 2-byte value in register pairs BC, DE, and HL is exchanged
        ///     with the 2-byte value in BC', DE', and HL', respectively.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0xD9
        ///     =================================
        ///     T-States: 4, (4)
        /// </remarks>
        private void Exx()
        {
            _registers.ExchangeRegisterSet();
        }

        /// <summary>
        ///     "jp c,NN" operation
        /// </summary>
        /// <remarks>
        ///     If C flag is not set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 0xDA
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpC_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) == 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp c,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "in a,(N)" operation
        /// </summary>
        /// <remarks>
        ///     The operand N is placed on the bottom half (A0 through A7) of
        ///     the address bus to select the I/O device at one of 256 possible
        ///     ports. The contents of A also appear on the top half (A8 through
        ///     A15) of the address bus at this time. Then one byte from the
        ///     selected port is placed on the data bus and written to A
        ///     in the CPU.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 1 | 0xDB
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     T-States: 4, 3, 4 (11)
        /// </remarks>
        private void InAN()
        {
            ClockP3();
            ushort port = ReadMemory(_registers.PC++);
            ClockP4();
            port += (ushort) (_registers.A << 8);
            _registers.MW = (ushort) ((_registers.A << 8) + port + 1);
            _registers.A = ReadPort(port);
        }

        /// <summary>
        ///     "call c,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag C is set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0xDC
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallC()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.C) == 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call c,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "rst 18h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0018H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 1 | 0xDF
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst18()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0018;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 18H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET PO" operation
        /// </summary>
        /// <remarks>
        ///     If PV flag is not set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0xE0
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetPO()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.PV) != 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret po",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "pop hl" operation
        /// </summary>
        /// <remarks>
        ///     The top two bytes of the external memory last-in, first-out (LIFO)
        ///     stack are popped to register pair HL. SP holds the 16-bit address
        ///     of the current top of the stack. This instruction first loads to
        ///     the low-order portion of RR, the byte at the memory location
        ///     corresponding to the contents of SP; then SP is incremented and
        ///     the contents of the corresponding adjacent memory location are
        ///     loaded to the high-order portion of RR and the SP is now incremented
        ///     again.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 0xE1
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopHL()
        {
            var oldSp = _registers.SP;

            ushort val = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.HL = (ushort) ((ReadMemory(_registers.SP) << 8) | val);
            ClockP3();
            _registers.SP++;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "pop hl",
                    oldSp,
                    _registers.HL,
                    Tacts));
        }

        /// <summary>
        ///     "jp po,NN" operation
        /// </summary>
        /// <remarks>
        ///     If PV flag is not set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0xE2
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpPO_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.PV) != 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp po,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "ex (sp),hl" operation
        /// </summary>
        /// <remarks>
        ///     The low-order byte contained in HL is exchanged with the contents
        ///     of the memory address specified by the contents of SP, and the
        ///     high-order byte of HL is exchanged with the next highest memory
        ///     address (SP+1).
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 0xE3
        ///     =================================
        ///     T-States: 4, 3, 4, 3, 5 (19)
        /// </remarks>
        private void ExSPiHL()
        {
            var tmpSp = _registers.SP;
            _registers.MW = ReadMemory(tmpSp);
            ClockP3();
            WriteMemory(tmpSp, _registers.L);
            ClockP4();
            tmpSp++;
            _registers.MW += (ushort) (ReadMemory(tmpSp) * 0x100);
            ClockP3();
            WriteMemory(tmpSp, _registers.H);
            _registers.HL = _registers.MW;
            ClockP5();

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "ex (sp),hl",
                    _registers.SP,
                    _registers.HL,
                    Tacts));
        }

        /// <summary>
        ///     "call po,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag PV is not set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 0xE4
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallPO()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.PV) != 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call po,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "push hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the register pair HL are pushed to the external
        ///     memory last-in, first-out (LIFO) stack. SP holds the 16-bit
        ///     address of the current top of the Stack. This instruction first
        ///     decrements SP and loads the high-order byte of register pair RR
        ///     to the memory address specified by SP. Then SP is decremented again
        ///     and loads the low-order byte of RR to the memory location
        ///     corresponding to this new address in SP.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 0xE5
        ///     =================================
        ///     T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushHL()
        {
            var oldSp = _registers.SP;

            var val = _registers.HL;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte) (val >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (val & 0xFF));
            ClockP3();

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "push hl",
                    oldSp,
                    _registers.HL,
                    Tacts));
        }

        /// <summary>
        ///     "rst 20h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0020H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 0xE7
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst20()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0020;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 20H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET PE" operation
        /// </summary>
        /// <remarks>
        ///     If PV flag is not set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 0xE8
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetPE()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.PV) == 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret pe",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "jp (hl)" operation
        /// </summary>
        /// <remarks>
        ///     PC is loaded with the contents of HL. The next instruction is
        ///     fetched from the location designated by the new contents of PC.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 0xE9
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void JpHL()
        {
            var oldPc = _registers.PC - 1;

            _registers.PC = _registers.HL;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    "jp (hl)",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "jp pe,NN" operation
        /// </summary>
        /// <remarks>
        ///     If PV flag is set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0 | 0xEA
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpPE_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.PV) == 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp pe,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "ex de,hl" operation
        /// </summary>
        /// <remarks>
        ///     The 2-byte contents of register pairs DE and HL are exchanged.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 1 | 0xEB
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void ExDEHL()
        {
            Registers.Swap(ref _registers.DE, ref _registers.HL);
        }

        /// <summary>
        ///     "call pe,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag PV is set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 0xEC
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallPE()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.PV) == 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call pe,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "rst 28h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0028H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 0xEF
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst28()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0028;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 28H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET P" operation
        /// </summary>
        /// <remarks>
        ///     If S flag is not set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0xF0
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetP()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.S) != 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret p",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "pop af" operation
        /// </summary>
        /// <remarks>
        ///     The top two bytes of the external memory last-in, first-out (LIFO)
        ///     stack are popped to register pair AF. SP holds the 16-bit address
        ///     of the current top of the stack. This instruction first loads to
        ///     the low-order portion of RR, the byte at the memory location
        ///     corresponding to the contents of SP; then SP is incremented and
        ///     the contents of the corresponding adjacent memory location are
        ///     loaded to the high-order portion of RR and the SP is now incremented
        ///     again.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 0xF1
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void PopAF()
        {
            var oldSp = _registers.SP;

            ushort val = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.AF = (ushort) ((ReadMemory(_registers.SP) << 8) | val);
            ClockP3();
            _registers.SP++;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "pop af",
                    oldSp,
                    _registers.AF,
                    Tacts));
        }

        /// <summary>
        ///     "jp p,NN" operation
        /// </summary>
        /// <remarks>
        ///     If S flag is not set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 0xF2
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpP_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.S) != 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp p,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }

        /// <summary>
        ///     "di" operation
        /// </summary>
        /// <remarks>
        ///     Disables the maskable interrupt by resetting the interrupt
        ///     enable flip-flops (IFF1 and IFF2).
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 1 | 0xF3
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Di()
        {
            _iff2 = _iff1 = false;
        }

        /// <summary>
        ///     "call p,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag S is not set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 0xF4
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallP()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.S) != 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call p,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "push af" operation
        /// </summary>
        /// <remarks>
        ///     The contents of the register pair BC are pushed to the external
        ///     memory last-in, first-out (LIFO) stack. SP holds the 16-bit
        ///     address of the current top of the Stack. This instruction first
        ///     decrements SP and loads the high-order byte of register pair RR
        ///     to the memory address specified by SP. Then SP is decremented again
        ///     and loads the low-order byte of RR to the memory location
        ///     corresponding to this new address in SP.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 1 | 0xF5
        ///     =================================
        ///     T-States: 5, 3, 3 (10)
        /// </remarks>
        private void PushAF()
        {
            var oldSp = _registers.SP;

            var val = _registers.AF;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte) (val >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (val & 0xFF));
            ClockP3();

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(_registers.PC - 1),
                    "push af",
                    oldSp,
                    _registers.AF,
                    Tacts));
        }

        /// <summary>
        ///     "rst 30h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0030H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 1 | 0xF7
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst30()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0030;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 30H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "RET M" operation
        /// </summary>
        /// <remarks>
        ///     If S flag is set, the byte at the memory location specified
        ///     by the contents of SP is moved to the low-order 8 bits of PC.
        ///     SP is incremented and the byte at the memory location specified by
        ///     the new contents of the SP are moved to the high-order eight bits of
        ///     PC.The SP is incremented again. The next op code following this
        ///     instruction is fetched from the memory location specified by the PC.
        ///     This instruction is normally used to return to the main line program at
        ///     the completion of a routine entered by a CALL instruction.
        ///     If condition X is false, PC is simply incremented as usual, and the
        ///     program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0xF8
        ///     =================================
        ///     T-States: If X is true: 5, 3, 3 (11)
        ///     If X is false: 5 (5)
        /// </remarks>
        private void RetM()
        {
            var oldSp = _registers.SP;

            ClockP1();
            if ((_registers.F & FlagsSetMask.S) == 0) return;

            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            _registers.MW += (ushort) (ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "ret m",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        ///     "ld sp,hl" operation
        /// </summary>
        /// <remarks>
        ///     The contents of HL are loaded to SP.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 0xF9
        ///     =================================
        ///     T-States: 4 (6)
        /// </remarks>
        private void LdSPHL()
        {
            var oldSP = _registers.SP;

            _registers.SP = _registers.HL;
            ClockP2();

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 1),
                    "ld sp,hl",
                    oldSP,
                    _registers.SP,
                    Tacts
                ));
        }

        /// <summary>
        ///     "jp m,NN" operation
        /// </summary>
        /// <remarks>
        ///     If S flag is set, the instruction loads operand NN
        ///     to PC, and the program continues with the instruction
        ///     beginning at address NN.
        ///     If condition X is false, PC is incremented as usual, and
        ///     the program continues with the next sequential instruction.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 0 | 0xFA
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void JpM_NN()
        {
            var oldPc = _registers.PC - 1;

            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.S) == 0) return;
            _registers.PC = _registers.MW;

            BranchDebugSupport?.RecordBranchEvent(
                new BranchEvent((ushort)oldPc,
                    $"jp m,{_registers.PC:X4}H",
                    _registers.PC,
                    Tacts));
        }


        /// <summary>
        ///     "ei" operation
        /// </summary>
        /// <remarks>
        ///     Sets both interrupt enable flip flops (IFFI and IFF2) to a
        ///     logic 1 value, allowing recognition of any maskable interrupt.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 0 | 1 | 1 | 0xFB
        ///     =================================
        ///     T-States: 4 (4)
        /// </remarks>
        private void Ei()
        {
            _iff2 = _iff1 = _isInterruptBlocked = true;
        }

        /// <summary>
        ///     "call m,NN" operation
        /// </summary>
        /// <remarks>
        ///     If flag S is set, this instruction pushes the current
        ///     contents of PC onto the top of the external memory stack, then
        ///     loads the operands NN to PC to point to the address in memory
        ///     at which the first op code of a subroutine is to be fetched.
        ///     At the end of the subroutine, a RET instruction can be used to
        ///     return to the original program flow by popping the top of the
        ///     stack back to PC. If condition X is false, PC is incremented as
        ///     usual, and the program continues with the next sequential
        ///     instruction. The stack push is accomplished by first decrementing
        ///     the current contents of SP, loading the high-order byte of the PC
        ///     contents to the memory address now pointed to by SP; then
        ///     decrementing SP again, and loading the low-order byte of the PC
        ///     contents to the top of the stack.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0xFC
        ///     =================================
        ///     |           8-bit L             |
        ///     =================================
        ///     |           8-bit H             |
        ///     =================================
        ///     T-States: 4, 3, 3 (10)
        /// </remarks>
        private void CallM()
        {
            _registers.MW = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            _registers.MW += (ushort) (ReadMemory(_registers.PC) << 8);
            ClockP3();
            _registers.PC++;
            if ((_registers.F & FlagsSetMask.S) == 0) return;

            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            ClockP1();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 3),
                    $"call m,{_registers.PC:X4}H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     "rst 38h" operation
        /// </summary>
        /// <remarks>
        ///     The current PC contents are pushed onto the external memory stack,
        ///     and the Page 0 memory location assigned by operand N is loaded to
        ///     PC. Program execution then begins with the op code in the address
        ///     now pointed to by PC. The push is performed by first decrementing
        ///     the contents of SP, loading the high-order byte of PC to the
        ///     memory address now pointed to by SP, decrementing SP again, and
        ///     loading the low-order byte of PC to the address now pointed to by
        ///     SP. The Restart instruction allows for a jump to address 0038H.
        ///     Because all addresses are stored in Page 0 of memory, the high-order
        ///     byte of PC is loaded with 0x00.
        ///     =================================
        ///     | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 0xFF
        ///     =================================
        ///     T-States: 5, 3, 3 (11)
        /// </remarks>
        private void Rst38()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC;

            _registers.SP--;
            ClockP1();

            WriteMemory(_registers.SP, (byte) (_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte) _registers.PC);
            ClockP3();

            _registers.MW = 0x0038;
            _registers.PC = _registers.MW;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)(oldPc - 1),
                    "rst 38H",
                    oldSp,
                    oldPc,
                    Tacts));
        }

        /// <summary>
        ///     Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        ///     operation for A and the 8-bit value specified in N.
        /// </summary>
        /// <remarks>
        ///     The flags are set according to the ALU operation rules.
        ///     =================================
        ///     | 0 | 1 | A | A | A | 1 | 1 | 0 |
        ///     =================================
        ///     |            8-bit              |
        ///     =================================
        ///     A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///     100=AND, 101=XOR, 110=OR, 111=CP
        ///     T-States: 4, 3 (7)
        /// </remarks>
        private void AluAN()
        {
            var val = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;

            _AluAlgorithms[(_opCode & 0x38) >> 3](val, _registers.CFlag);
        }
    }
}