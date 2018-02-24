// ReSharper disable InconsistentNaming

using System;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This partion of the class provides extended operations
    /// (OpCodes used with the 0xED prefix)
    /// </summary>
    public partial class Z80Cpu
    {
        /// <summary>
        /// Extended (0xED-prefixed) operations jump table
        /// </summary>
        private Action[] _extendedOperations;

        /// <summary>
        /// Processes the operations with 0xED prefix
        /// </summary>
        private void ProcessEDOperations()
        {
            var opMethod = _extendedOperations[_opCode];
            opMethod?.Invoke();
        }

        /// <summary>
        /// Initializes the extended operation execution tables
        /// </summary>
        private void InitializeExtendedOpsExecutionTable()
        {
            _extendedOperations = new Action[]
            {
                null,     null,     null,     null,     null,     null,     null,     null,     // 00..07
                null,     null,     null,     null,     null,     null,     null,     null,     // 08..0F
                null,     null,     null,     null,     null,     null,     null,     null,     // 10..17
                null,     null,     null,     null,     null,     null,     null,     null,     // 18..1F
                null,     null,     null,     SWAPNIB,  MIRR_A,   LD_HL_SP, MIRR_DE,  TEST_N,   // 20..27
                null,     null,     null,     null,     null,     null,     null,     null,     // 28..2F
                MUL,      ADD_HL_A, ADD_DE_A, ADD_BC_A, null,     null,     null,     INC_DEHL, // 30..37
                DEC_DEHL, ADD_DEHL_A, ADD_DEHL_BC, ADD_DEHL_NN, SUB_DEHL_A, SUB_DEHL_BC, null, null,     // 38..3F

                IN_B_C,   OUT_C_B,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_XR_A,  // 40..47
                IN_C_C,   OUT_C_C,  ADCHL_QQ, LDQQ_NNi, NEG,      RETI,     IM_N,     LD_XR_A,  // 48..4F
                IN_D_C,   OUT_C_D,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_A_XR,  // 50..57
                IN_E_C,   OUT_C_E,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     LD_A_XR,  // 58..5F
                IN_H_C,   OUT_C_H,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     RRD,      // 60..67
                IN_L_C,   OUT_C_L,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     RLD,      // 60..6F
                IN_F_C,   OUT_C_0,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     null,     // 70..77
                IN_A_C,   OUT_C_A,  ADCHL_QQ, LDSP_NNi, NEG,      RETN,     IM_N,     null,     // 78..7F

                null,     null,     null,     null,     null,     null,     null,     null,     // 80..87
                null,     null,     PUSH_NN,  POPX,     null,     null,     null,     null,     // 88..8F
                null,     NEXTREG,  NEXTREG_A,null,     null,     null,     null,     null,     // 90..97
                null,     null,     null,     null,     null,     null,     null,     null,     // 98..9F
                LDI,      CPI,      INI,      OUTI,     null,     null,     null,     null,     // A0..A7
                LDD,      CPD,      IND,      OUTD,     null,     null,     null,     null,     // A8..AF
                LDIR,     CPIR,     INIR,     OTIR,     null,     null,     null,     null,     // B0..B7
                LDDR,     CPDR,     INDR,     OTDR,     null,     null,     null,     null,     // B0..BF

                null,     null,     null,     null,     null,     null,     null,     null,     // C0..C7
                null,     null,     null,     null,     null,     null,     null,     null,     // C8..CF
                null,     null,     null,     null,     null,     null,     null,     null,     // D0..D7
                null,     null,     null,     null,     null,     null,     null,     null,     // D8..DF
                null,     null,     null,     null,     null,     null,     null,     null,     // E0..E7
                null,     null,     null,     null,     null,     null,     null,     null,     // E8..EF
                null,     null,     null,     null,     null,     null,     null,     null,     // F0..F7
                null,     null,     null,     null,     null,     null,     null,     null,     // F8..FF
            };
        }

        /// <summary>
        /// "SWAPNIB" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Swaps the high and low nibbles of the Accummulator
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 23
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SWAPNIB()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.A = (byte) ((_registers.A >> 4)  
                | (_registers.A << 4));
        }

        /// <summary>
        /// "MIRROR A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Mirrors (reverses the order) of bits 
        /// in A.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 24
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void MIRR_A()
        {
            if (!AllowExtendedInstructionSet) return;
            var newA = 0;
            var oldA = _registers.A;
            for (var i = 0; i < 8; i++)
            {
                newA <<= 1;
                newA |= oldA & 0x01;
                oldA >>= 1;
            }
            _registers.A = (byte)newA;
        }

        /// <summary>
        /// "LD HL,SP" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Loads the contents of SP into HL.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 25
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void LD_HL_SP()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.HL = _registers.SP;
        }

        /// <summary>
        /// "MIRROR DE" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Mirrors (reverses the order) of bits 
        /// in DE.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 26
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void MIRR_DE()
        {
            if (!AllowExtendedInstructionSet) return;
            var newDE = 0;
            var oldDE = _registers.DE;
            for (var i = 0; i < 16; i++)
            {
                newDE <<= 1;
                newDE |= oldDE & 0x01;
                oldDE >>= 1;
            }
            _registers.DE = (ushort)newDE;
        }

        /// <summary>
        /// "TEST N" operation
        /// </summary>
        /// <remarks>
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 27
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 1 |
        /// =================================
        /// |               N               |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4,pc+1:3
        /// </remarks>
        private void TEST_N()
        {
            if (!AllowExtendedInstructionSet) return;
            var value = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var result = (byte)(_registers.A & value);
            _registers.F = (byte)(s_AluLogOpFlags[result] | FlagsSetMask.H);
        }

        /// <summary>
        /// "MUL" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Multiplies HL by DE, leaving the high word of the result 
        /// in DE and the low word in HL.
        /// Does not alter any flags.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 30
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void MUL()
        {
            if (!AllowExtendedInstructionSet) return;
            var mul = _registers.HL * _registers.DE;
            _registers.DE = (ushort) mul;
            _registers.HL = (ushort) (mul >> 16);
        }

        /// <summary>
        /// "ADD HL,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds the contents of A to HL.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 31
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void ADD_HL_A()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.HL += _registers.A;
        }

        /// <summary>
        /// "ADD DE,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds the contents of A to DE.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 32
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void ADD_DE_A()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.DE += _registers.A;
        }

        /// <summary>
        /// "ADD BC,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds the contents of A to BC.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 33
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void ADD_BC_A()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.BC += _registers.A;
        }

        /// <summary>
        /// "INC DEHL" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Increments the contents of DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 37
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void INC_DEHL()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = ((_registers.DE << 16) + _registers.HL) + 1;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort) dehl;
        }

        /// <summary>
        /// "DEC DEHL" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Decrements the contents of DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 38
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void DEC_DEHL()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = ((_registers.DE << 16) + _registers.HL) - 1;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "ADD DEHL,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds the contents of A to DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 39
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void ADD_DEHL_A()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = (_registers.DE << 16) + _registers.HL + _registers.A;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "ADD DEHL,BC" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds the contents of BC to DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 3A
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void ADD_DEHL_BC()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = (_registers.DE << 16) + _registers.HL + _registers.BC;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "ADD DEHL,NN" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Adds a 16-bit integer value to the contents of DEHL 
        /// (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 3B
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 1 |
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 4, 3, 3 (14)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3
        /// </remarks>
        private void ADD_DEHL_NN()
        {
            if (!AllowExtendedInstructionSet) return;
            int value = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            value += ReadCodeMemory() << 8;
            ClockP3();
            _registers.PC++;
            var dehl = (_registers.DE << 16) + _registers.HL + value;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "SUB DEHL,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Subtracts the contents of A from DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 3C
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SUB_DEHL_A()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = (_registers.DE << 16) + _registers.HL - _registers.A;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "SUB DEHL,BC" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Subtracts the contents of BC from DEHL (as a 32-bit register).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 3D
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SUB_DEHL_BC()
        {
            if (!AllowExtendedInstructionSet) return;
            var dehl = (_registers.DE << 16) + _registers.HL - _registers.BC;
            _registers.DE = (ushort)(dehl >> 16);
            _registers.HL = (ushort)dehl;
        }

        /// <summary>
        /// "IN B,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register B in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 40
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_B_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.B = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),B" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register B is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 41
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_B()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.B);
        }

        /// <summary>
        /// "IN C,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register C in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 48
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_C_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.C = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),C" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register C is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 49
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.C);
        }

        /// <summary>
        /// "IN D,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register D in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 50
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_D_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.D = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),D" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register D is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 51
        /// =================================
        /// | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_D()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.D);
        }

        /// <summary>
        /// "IN E,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register E in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 58
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_E_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.E = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),E" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register E is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 59
        /// =================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_E()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.E);
        }

        /// <summary>
        /// "IN H,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register H in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 60
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_H_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.H = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),H" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register H is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 61
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_H()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.H);
        }

        /// <summary>
        /// "IN L,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register L in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 68
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_L_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.L = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),L" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register L is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 69
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_L()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.L);
        }

        /// <summary>
        /// "IN (C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 70
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_F_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),0" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// 0 is placed on the data bus and written to the selected 
        /// peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 71
        /// =================================
        /// | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_0()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, 0);
        }

        /// <summary>
        /// "IN A,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register A in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 78
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void IN_A_C()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            _registers.A = pval;
            _registers.F = (byte)(s_AluLogOpFlags[pval] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register A is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 79
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4 (12)
        /// Contention breakdown: pc:4,pc+1:4,I/O
        /// </remarks>
        private void OUT_C_A()
        {
            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            WritePort(_registers.BC, _registers.A);
        }

        /// <summary>
        /// "SBC HL,QQ" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the register pair QQ and the Carry Flag are 
        /// subtracted from the contents of HL, and the result is stored 
        /// in HL.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 12; otherwise, it is reset.
        /// P/V is set if overflow; otherwise, it is reset.
        /// N is set.
        //  C is set if borrow; otherwise, it is reset.

        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | 0 | 0 | 1 | 0 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 4, 3 (15)
        /// Contention breakdown: pc:4,pc+1:11
        /// </remarks>
        private void SBCHL_QQ()
        {
            _registers.WZ = (ushort)(_registers.HL + 1);
            var cfVal = _registers.CFlag ? 1 : 0;
            var qq = (Reg16Index)((_opCode & 0x30) >> 4);
            var flags = FlagsSetMask.N;
            flags |= (byte)((((_registers.HL & 0x0FFF) - (_registers[qq] & 0x0FFF) - cfVal) >> 8) & FlagsSetMask.H);
            var sbcVal = (uint)((_registers.HL & 0xFFFF) - (_registers[qq] & 0xFFFF) - cfVal);
            if ((sbcVal & 0x10000) != 0)
            {
                flags |= FlagsSetMask.C;
            }
            if ((sbcVal & 0xFFFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            var signedSbc = (short)_registers.HL - (short)_registers[qq] - cfVal;
            if (signedSbc < -0x8000 || signedSbc >= 0x8000)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.HL = (ushort)sbcVal;
            _registers.F = (byte)(flags | (_registers.H & (FlagsSetMask.S | FlagsSetMask.R3 | FlagsSetMask.R5)));
            ClockP7();
        }

        /// <summary>
        /// "LD (NN),QQ" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The low-order byte of register pair QQ is loaded to memory address
        /// (NN); the upper byte is loaded to memory address(NN + 1).
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | 0 | 0 | 1 | 1 |
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3,nn:3,nn+1:3
        /// </remarks>
        private void LDNNi_QQ()
        {
            var l = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadCodeMemory() << 8 | l);
            ClockP3();
            _registers.PC++;
            _registers.WZ = (ushort)(addr + 1);
            var regVal =  _registers[(Reg16Index)((_opCode & 0x30) >> 4)];
            WriteMemory(addr, (byte)(regVal & 0xFF));
            ClockP3();
            WriteMemory(_registers.WZ, (byte)(regVal >> 8));
            ClockP3();
        }

        /// <summary>
        /// "NEG" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the Accumulator are negated (two's complement).
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if Accumulator was 80h before operation; otherwise, it is reset.
        /// N is set.
        /// C is set if Accumulator was not 00h before operation; otherwise, it is reset.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void NEG()
        {
            var result = -_registers.A;
            var lNibble = -(_registers.A & 0x0F) & 0x10;

            var flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            flags |= FlagsSetMask.N;
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if (_registers.A != 0) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (_registers.A == 0x80) flags |= FlagsSetMask.PV;

            _registers.F = flags;
            _registers.A = (byte)result;
        }

        /// <summary>
        /// "RETI" operation
        /// </summary>
        /// <remarks>
        /// 
        /// This instruction is used at the end of a maskable interrupt service routine to:
        /// Restore the contents of the Program Counter(analogous to the RET instruction)
        /// Signal an I/O device that the interrupt routine is completed.The RETI instruction also
        /// facilitates the nesting of interrupts, allowing higher priority devices to temporarily
        /// suspend service of lower priority service routines.However, this instruction does not
        /// enable interrupts that were disabled when the interrupt routine was entered. Before 
        /// doing the RETI instruction, the enable interrupt instruction (EI) should be executed to
        /// allow recognition of interrupts after completion of the current service routine.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 4D
        /// =================================
        /// | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4, 3, 3 (14)
        /// Contention breakdown: pc:4,pc+1:4,sp:3,sp+1:3
        /// </remarks>
        private void RETI()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC - 2;

            _iff1 = _iff2;
            ushort addr = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            addr += (ushort)(ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = addr;
            _registers.WZ = addr;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "reti",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        /// "RETN" operation
        /// </summary>
        /// <remarks>
        /// 
        /// This instruction is used at the end of a nonmaskable interrupts 
        /// service routine to restore the contents of PC. The state of IFF2
        /// is copied back to IFF1 so that maskable interrupts are enabled 
        /// immediately following the RETN if they were enabled before the 
        /// nonmaskable interrupt.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 4, 3, 3 (14)
        /// Contention breakdown: pc:4,pc+1:4,sp:3,sp+1:3
        /// </remarks>
        private void RETN()
        {
            var oldSp = _registers.SP;
            var oldPc = _registers.PC - 2;

            _iff1 = _iff2;
            ushort addr = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            addr += (ushort)(ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            _registers.PC = addr;
            _registers.WZ = addr;

            StackDebugSupport?.RecordStackContentManipulationEvent(
                new StackContentManipulationEvent((ushort)oldPc,
                    "retn",
                    oldSp,
                    null,
                    Tacts));
        }

        /// <summary>
        /// "IM N" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Sets Interrupt Mode to N
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | N | N | 1 | 1 | 0 |
        /// =================================
        /// NN: 00=IM 0, 01=N/A 10=IM 1, 11=IM 2
        /// T-States: 4, 4 (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void IM_N()
        {
            var mode = (byte)((_opCode & 0x18) >> 3);
            if (mode < 2) mode = 1;
            mode--;
            _interruptMode = mode;
        }

        /// <summary>
        /// "LD I,A/LD R,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of A are loaded to I or R
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | R | 1 | 1 | 1 | 1 |
        /// =================================
        /// R: 0=I, 1=R
        /// T-States: 4, 5 (9)
        /// Contention breakdown: pc:4,pc+1:5
        /// </remarks>
        private void LD_XR_A()
        {
            if ((_opCode & 0x08) == 0)
            {
                _registers.I = _registers.A;
            }
            else
            {
                _registers.R = _registers.A;
            }
            ClockP1();
        }

        /// <summary>
        /// "ADC HL,QQ" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register pair QQ are added with the Carry flag
        /// to the contents of HL, and the result is stored in HL.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// P/V is set if overflow; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        ///
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | 1 | 0 | 1 | 0 |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 4, 3 (15)
        /// Contention breakdown: pc:4,pc+1:11
        /// </remarks>
        private void ADCHL_QQ()
        {
            _registers.WZ = (ushort)(_registers.HL + 1);
            var cfVal = _registers.CFlag ? 1 : 0;
            var qq = (Reg16Index)((_opCode & 0x30) >> 4);
            var flags = (byte)((((_registers.HL & 0x0FFF) + (_registers[qq] & 0x0FFF) + (_registers.F & FlagsSetMask.C)) >> 8) & FlagsSetMask.H);
            var adcVal = (uint)((_registers.HL & 0xFFFF) + (_registers[qq] & 0xFFFF) + cfVal);
            if ((adcVal & 0x10000) != 0)
            {
                flags |= FlagsSetMask.C;
            }
            if ((adcVal & 0xFFFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            var signedAdc = (short)_registers.HL + (short)_registers[qq] + cfVal;
            if (signedAdc < -0x8000 || signedAdc >= 0x8000)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.HL = (ushort)adcVal;
            _registers.F = (byte)(flags | (_registers.H & (FlagsSetMask.S | FlagsSetMask.R3 | FlagsSetMask.R5)));
            ClockP7();
        }

        /// <summary>
        /// "LD QQ,(NN)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of address (NN) are loaded to the low-order portion 
        /// of register pair QQ, and the contents of (NN + 1) are loaded to 
        /// the high-order portion of QQ.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | 1 | 0 | 1 | 1 |
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=HL, 11=SP
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3,nn:3,nn+1:3
        /// </remarks>
        private void LDQQ_NNi()
        {
            var addrl = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadCodeMemory() << 8 | addrl);
            ClockP3();
            _registers.PC++;
            _registers.WZ = (ushort)(addr + 1);
            var l = ReadMemory(addr);
            ClockP3();
            var h = ReadMemory(_registers.WZ);
            ClockP3();
            _registers[(Reg16Index)((_opCode & 0x30) >> 4)] = (ushort)(h << 8 | l);
        }

        /// <summary>
        /// "LD SP,(NN)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of address (NN) are loaded to the low-order portion 
        /// of register pair QQ, and the contents of (NN + 1) are loaded to 
        /// the high-order portion of QQ.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 7B
        /// =================================
        /// | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 1 |
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3,nn:3,nn+1:3
        /// </remarks>
        private void LDSP_NNi()
        {
            var oldSP = _registers.SP;

            var addrl = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadCodeMemory() << 8 | addrl);
            ClockP3();
            _registers.PC++;
            _registers.WZ = (ushort)(addr + 1);
            var l = ReadMemory(addr);
            ClockP3();
            var h = ReadMemory(_registers.WZ);
            ClockP3();
            _registers.SP = (ushort)(h << 8 | l);

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 4),
                    $"ld sp,(#{addr:X4})",
                    oldSP,
                    _registers.SP,
                    Tacts
                ));
        }

        /// <summary>
        /// "LD A,I/LD A,R" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of I or R are loaded to A
        /// 
        /// S is set if the I Register is negative; otherwise, it is reset.
        /// Z is set if the I Register is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V contains contents of IFF2.
        /// N is reset.
        /// C is not affected.
        /// If an interrupt occurs during execution of this instruction, the Parity flag contains a 0.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | R | 1 | 1 | 1 | 1 |
        /// =================================
        /// R: 0=I, 1=R
        /// T-States: 4, 5 (9)
        /// Contention breakdown: pc:4,pc+1:5
        /// </remarks>
        private void LD_A_XR()
        {
            var source = (_opCode & 0x08) == 0
                ? _registers.I : _registers.R;
            _registers.A = source;
            var flags = _registers.F & FlagsSetMask.C
                        | (source & FlagsSetMask.R3R5)
                        | (IFF2 ? FlagsSetMask.PV : 0)
                        | (source & 0x80)
                        | (source == 0 ? FlagsSetMask.Z : 0);
            _registers.F = (byte) flags;
            ClockP1();
        }

        /// <summary>
        /// "RRD" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the low-order four bits (bits 3, 2, 1, and 0) 
        /// of memory location (HL) are copied to the low-order four bits 
        /// of A. The previous contents of the low-order four bits of A are 
        /// copied to the high-order four bits(7, 6, 5, and 4) of location
        /// (HL); and the previous contents of the high-order four bits of 
        /// (HL) are copied to the low-order four bits of (HL). The contents 
        /// of the high-order bits of A are unaffected.
        /// 
        /// S is set if A is negative after an operation; otherwise, it is reset.
        /// Z is set if A is 0 after an operation; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if the parity of A is even after an operation; otherwise, 
        /// it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4, 4, 3, 4, 3 (18)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×4,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:7,hl(write):3
        /// </remarks>
        private void RRD()
        {
            var tmp = ReadMemory(_registers.HL);
            ClockP3();
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }

            _registers.WZ = (ushort)(_registers.HL + 1);
            WriteMemory(_registers.HL, (byte)((_registers.A << 4) | (tmp >> 4)));
            ClockP3();

            _registers.A = (byte)((_registers.A & 0xF0) | (tmp & 0x0F));
            _registers.F = (byte)(s_AluLogOpFlags[_registers.A] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "RLD" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the low-order four bits (bits 3, 2, 1, and 0) 
        /// of the memory location (HL) are copied to the high-order four 
        /// bits (7, 6, 5, and 4) of that same memory location; the previous 
        /// contents of those high-order four bits are copied to the 
        /// low-order four bits of A; and the previous contents of the 
        /// low-order four bits of A are copied to the low-order four bits 
        /// of memory location(HL). The contents of the high-order bits of A
        /// are unaffected.
        /// 
        /// S is set if A is negative after an operation; otherwise, it is reset.
        /// Z is set if the A is 0 after an operation; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if the parity of A is even after an operation; otherwise, 
        /// it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 1 |
        /// =================================
        /// T-States: 4, 4, 3, 4, 3 (18)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×4,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:7,hl(write):3
        /// </remarks>
        private void RLD()
        {
            var tmp = ReadMemory(_registers.HL);
            ClockP3();

            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }

            _registers.WZ = (ushort)(_registers.HL + 1);
            WriteMemory(_registers.HL, (byte)((_registers.A & 0x0F) | (tmp << 4)));
            ClockP3();

            _registers.A = (byte)((_registers.A & 0xF0) | (tmp >> 4));
            _registers.F = (byte)(s_AluLogOpFlags[_registers.A] | (_registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "PUSH NN" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Pushes the 16-bit value to the stack
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 8A
        /// =================================
        /// | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0 |
        /// =================================
        /// |             N Low             |
        /// =================================
        /// |             N High            |
        /// =================================
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3,sp-1:3,sp-2:3
        /// </remarks>
        private void PUSH_NN()
        {
            if (!AllowExtendedInstructionSet) return;

            int value = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            value += ReadCodeMemory() << 8;
            ClockP3();
            _registers.PC++;
            _registers.SP--;
            WriteMemory(_registers.SP, (byte)(value >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte)value);
            ClockP3();
        }

        /// <summary>
        /// "POPX" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Pops a 16-bit value from the stack without storing it
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 8B
        /// =================================
        /// | 1 | 0 | 0 | 0 | 1 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4 (8)
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void POPX()
        {
            if (!AllowExtendedInstructionSet) return;
            _registers.SP += 2;
        }

        /// <summary>
        /// "NEXTREG reg,val" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Sets the specified 8-bit NEXT register to the 
        /// provided 8-bit value
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 91
        /// =================================
        /// | 1 | 0 | 0 | 1 | 0 | 0 | 0 | 1 |
        /// =================================
        /// |           Register            |
        /// =================================
        /// |            Value              |
        /// =================================
        /// T-States: 4, 4, 3, 3 (14)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3,pc+3:3
        /// </remarks>
        private void NEXTREG()
        {
            if (!AllowExtendedInstructionSet) return;
            var reg = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var val = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            if (_tbblueDevice == null) return;
            _tbblueDevice.SelectTbBlueRegister(reg);
            _tbblueDevice.SetTbBlueValue(val);
        }

        /// <summary>
        /// "NEXTREG reg,A" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Sets the specified 8-bit NEXT register to the 
        /// value of A.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED 92
        /// =================================
        /// | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 0 |
        /// =================================
        /// |           Register            |
        /// =================================
        /// |            Value              |
        /// =================================
        /// T-States: 4, 4, 3 (11)
        /// Contention breakdown: pc:4,pc+1:4,pc+2:3
        /// </remarks>
        private void NEXTREG_A()
        {
            if (!AllowExtendedInstructionSet) return;
            var reg = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            if (_tbblueDevice == null) return;
            _tbblueDevice.SelectTbBlueRegister(reg);
            _tbblueDevice.SetTbBlueValue(_registers.A);
        }

        /// <summary>
        /// "LDI" operation
        /// </summary>
        /// <remarks>
        /// 
        /// A byte of data is transferred from the memory location addressed
        /// by the contents of HL to the memory location addressed by the 
        /// contents of DE. Then both these register pairs are incremented 
        /// and BC is decremented.
        /// 
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,de:3,de:1 ×2
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:3,de:5
        /// </remarks>
        private void LDI()
        {
            var memVal = ReadMemory(_registers.HL++);
            ClockP3();
            WriteMemory(_registers.DE, memVal);
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ClockP3();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
            }
            _registers.DE++;
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) | ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) | memVal);
            if (--_registers.BC != 0) _registers.F |= FlagsSetMask.PV;
        }

        /// <summary>
        /// "CPI" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the memory location addressed by HL is compared
        /// with the contents of A. With a true compare, Z flag is 
        /// set. Then HL is incremented and BC is decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if A is (HL); otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×5
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:8
        /// </remarks>
        private void CPI()
        {
            var memVal = ReadMemory(_registers.HL);
            var compRes = _registers.A - memVal;
            var r3r5 = compRes;
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
                r3r5 = compRes - 1;
            }
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            flags |= (byte)(compRes & FlagsSetMask.S);
            flags |= (byte) ((r3r5 & FlagsSetMask.R3) | ((r3r5 << 4) & FlagsSetMask.R5));

            ClockP3();
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            _registers.HL++;

            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.F = (byte)flags;
            _registers.WZ++;
        }

        /// <summary>
        /// "INI" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. Register B can be used as a byte counter, 
        /// and its contents are placed on the top half (A8 through A15) of 
        /// the address bus at this time. Then one byte from the selected 
        /// port is placed on the data bus and written to the CPU. The 
        /// contents of the HL register pair are then placed on the address 
        /// bus and the input byte is written to the corresponding location 
        /// of memory. Finally, B is decremented and HL is incremented.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,I/O,hl:3
        /// </remarks>
        private void INI()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            // I/O
            _registers.WZ = (ushort)(_registers.BC + 1);
            var val = ReadPort(_registers.BC);

            // hl:3
            WriteMemory(_registers.HL, val);
            ClockP3();

            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            _registers.HL++;
        }

        /// <summary>
        /// "OUTI" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the HL register pair are placed on the address
        /// bus to select a location in memory. The byte contained in this 
        /// memory location is temporarily stored in the CPU. Then, after B 
        /// is decremented, the contents of C are placed on the bottom half
        /// (A0 through A7) of the address bus to select the I/O device at 
        /// one of 256 possible ports. Register B is used as a byte counter, 
        /// and its decremented value is placed on the top half (A8 through 
        /// A15) of the address bus. The byte to be output is placed on the 
        /// data bus and written to a selected peripheral device. Finally, 
        /// the HL is incremented.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 0 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,hl:3,I/O
        /// </remarks>
        private void OUTI()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;

            // hl:3
            var val = ReadMemory(_registers.HL);
            ClockP3();

            // I/O
            WritePort(_registers.BC, val);

            _registers.HL++;
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0) _registers.F |= FlagsSetMask.C;
            _registers.WZ = (ushort)(_registers.BC + 1);
        }

        /// <summary>
        /// "LDD" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Transfers a byte of data from the memory location addressed by
        /// HL to the memory location addressed by DE. Then DE, HL, and BC
        /// is decremented.
        /// 
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,de:3,de:1 ×2
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:3,de:5
        /// </remarks>
        private void LDD()
        {
            var memVal = ReadMemory(_registers.HL--);
            ClockP3();
            WriteMemory(_registers.DE, memVal);
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ClockP3();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
            }
            _registers.DE--;
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) | ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) | memVal);
            if (--_registers.BC != 0) _registers.F |= FlagsSetMask.PV;
        }

        /// <summary>
        /// "CPD" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the memory location addressed by HL is compared
        /// with the contents of A. During the compare operation, the Zero
        /// flag is set or reset. HL and BC are decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if A is (HL); otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×5
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:8
        /// </remarks>
        private void CPD()
        {
            var memVal = ReadMemory(_registers.HL);
            var compRes = _registers.A - memVal;
            var r3r5 = compRes;
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
                r3r5 = compRes - 1;
            }
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            flags |= (byte)(compRes & FlagsSetMask.S);
            flags |= (byte)((r3r5 & FlagsSetMask.R3) | ((r3r5 << 4) & FlagsSetMask.R5));

            ClockP3();
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            _registers.HL--;

            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.F = (byte)flags;
            _registers.WZ--;
        }

        /// <summary>
        /// "IND" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of C are placed on the bottom half (A0 through A7)
        /// of the address bus to select the I/O device at one of 256 
        /// possible ports. Register B is used as a byte counter, and its 
        /// contents are placed on the top half (A8 through A15) of the 
        /// address bus at this time. Then one byte from the selected port 
        /// is placed on the data bus and written to the CPU. The contents 
        /// of HL are placed on the address bus and the input byte is written
        /// to the corresponding location of memory. Finally, B and HLL are 
        /// decremented.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,I/O,hl:3
        /// </remarks>
        private void IND()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.WZ = (ushort)(_registers.BC - 1);

            // I/O
            var val = ReadPort(_registers.BC);

            // hl:3
            WriteMemory(_registers.HL, val);
            ClockP3();

            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            _registers.HL--;
        }

        /// <summary>
        /// "OUTD" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the HL register pair are placed on the address
        /// bus to select a location in memory. The byte contained in this 
        /// memory location is temporarily stored in the CPU. Then, after B 
        /// is decremented, the contents of C are placed on the bottom half
        /// (A0 through A7) of the address bus to select the I/O device at 
        /// one of 256 possible ports. Register B is used as a byte counter, 
        /// and its decremented value is placed on the top half (A8 through 
        /// A15) of the address bus. The byte to be output is placed on the 
        /// data bus and written to a selected peripheral device. Finally, 
        /// the HL is decremented.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,hl:3,I/O
        /// </remarks>
        private void OUTD()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;

            // hl:3
            var val = ReadMemory(_registers.HL);
            ClockP3();

            // I/O
            WritePort(_registers.BC, val);

            _registers.HL--;
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0xFF) _registers.F |= FlagsSetMask.C;
            _registers.WZ = (ushort)(_registers.BC - 1);
        }

        /// <summary>
        /// "LDIR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Transfers a byte of data from the memory location addressed by
        /// HL to the memory location addressed DE. Then HL and DE are 
        /// incremented. BC is decremented. If decrementing allows the BC 
        /// to go to 0, the instruction is terminated. If BC isnot 0, 
        /// the program counter is decremented by two and the instruction 
        /// is repeated. Interrupts are recognized and two refresh cycles 
        /// are executed after each data transfer. When the BC is set to 0
        /// prior to instruction execution, the instruction loops 
        /// through 64 KB.
        /// 
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 4, 3, 5, 5 (21)
        /// BC=0:  4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,de:3,de:1 ×2,[de:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:3,de:5,[5]
        /// </remarks>
        private void LDIR()
        {
            var memVal = ReadMemory(_registers.HL++);
            ClockP3();
            WriteMemory(_registers.DE, memVal);
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.DE);
                ClockP1();
                ReadMemory(_registers.DE);
                ClockP1();
            }
            _registers.DE++;
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) | ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) | memVal);
            if (--_registers.BC == 0)
            {
                return;
            }

            _registers.F |= FlagsSetMask.PV;
            _registers.PC -= 2;
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory((ushort)(_registers.DE - 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE - 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE - 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE - 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE - 1));
                ClockP1();
            }
            _registers.WZ = (ushort)(_registers.PC + 1);
        }

        /// <summary>
        /// "CPIR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the memory location addressed HL is compared with
        /// the contents of A. During a compare operation, the Zero flag is 
        /// set or reset. Then HL is incremented and BC is decremented.
        /// If decrementing causes BC to go to 0 or if A = (HL), the 
        /// instruction is terminated. If BC is not 0 and A is not equal
        /// (HL), the program counter is decremented by two and the 
        /// instruction is repeated. Interrupts are recognized and two 
        /// refresh cycles are executed after each data transfer. If BC is set 
        /// to 0 before instruction execution, the instruction loops through 
        /// 64 KB if no match is found.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if A is (HL); otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 4, 3, 5, 5 (21)
        /// BC=0:  4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×5,[hl:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:8,[5]
        /// </remarks>
        private void CPIR()
        {
            _registers.WZ++;
            var memVal = ReadMemory(_registers.HL);
            var compRes = _registers.A - memVal;
            var r3r5 = compRes;
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
                r3r5 = compRes - 1;
            }
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            flags |= (byte)(compRes & FlagsSetMask.S);
            flags |= (byte)((r3r5 & FlagsSetMask.R3) | ((r3r5 << 4) & FlagsSetMask.R5));

            ClockP3();
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            _registers.HL++;

            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    _registers.PC -= 2;
                    if (UseGateArrayContention)
                    {
                        ClockP5();
                    }
                    else
                    {
                        ReadMemory((ushort)(_registers.HL - 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL - 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL - 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL - 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL - 1));
                        ClockP1();
                    }
                    _registers.WZ = (ushort)(_registers.PC + 1);
                }
            }
            _registers.F = (byte)flags;
        }

        /// <summary>
        /// "INIR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. Register B can be used as a byte counter, 
        /// and its contents are placed on the top half (A8 through A15) of 
        /// the address bus at this time. Then one byte from the selected 
        /// port is placed on the data bus and written to the CPU. The 
        /// contents of the HL register pair are then placed on the address 
        /// bus and the input byte is written to the corresponding location 
        /// of memory. Finally, B is decremented and HL is incremented.
        /// If decrementing causes B to go to 0, the instruction is terminated.
        /// If B is not 0, PC is decremented by two and the instruction
        /// repeated. Interrupts are recognized and two refresh cycles 
        /// execute after each data transfer.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 5, 3, 4, 5 (21)
        /// BC=0:  4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,I/O,hl:3,[hl:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:5,I/O,hl:3,[5]
        /// </remarks>
        private void INIR()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.WZ = (ushort)(_registers.BC + 1);

            // I/O
            var val = ReadPort(_registers.BC);

            // hl:3
            WriteMemory(_registers.HL, val);
            ClockP3();

            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            _registers.HL++;
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                if (UseGateArrayContention)
                {
                    ClockP5();
                }
                else
                {
                    ReadMemory((ushort) (_registers.HL - 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL - 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL - 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL - 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL - 1));
                    ClockP1();
                }
            }
            else _registers.F &= FlagsResetMask.PV;
        }

        /// <summary>
        /// "OTIR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the HL register pair are placed on the address
        /// bus to select a location in memory. The byte contained in this 
        /// memory location is temporarily stored in the CPU. Then, after B 
        /// is decremented, the contents of C are placed on the bottom half
        /// (A0 through A7) of the address bus to select the I/O device at 
        /// one of 256 possible ports. Register B is used as a byte counter, 
        /// and its decremented value is placed on the top half (A8 through 
        /// A15) of the address bus. The byte to be output is placed on the 
        /// data bus and written to a selected peripheral device. Finally, 
        /// the HL is incremented.
        /// If the decremented B Register is not 0, PC is decremented by two 
        /// and the instruction is repeated. If B has gone to 0, the 
        /// instruction is terminated. Interrupts are recognized and two 
        /// refresh cycles are executed after each data transfer.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 0 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 5, 3, 4, 5 (21)
        /// BC=0:  4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,hl:3,I/O,[bc:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:5,hl:3,I/O,[5]
        /// </remarks>
        private void OTIR()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;

            // hl:3
            var val = ReadMemory(_registers.HL++);
            ClockP3();

            // I/O
            WritePort(_registers.BC, val);

            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                if (UseGateArrayContention)
                {
                    ClockP5();
                }
                else
                {
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                }
            }
            else
            {
                _registers.F &= FlagsResetMask.PV;
            }
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0) _registers.F |= FlagsSetMask.C;
            _registers.WZ = (ushort)(_registers.BC + 1);
        }

        /// <summary>
        /// "LDDR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// Transfers a byte of data from the memory location addressed by
        /// HL to the memory location addressed by DE. Then DE, HL, and BC
        /// is decremented.
        /// If decrementing causes BC to go to 0, the instruction is 
        /// terminated. If BC is not 0, PC is decremented by two and the 
        /// instruction is repeated. Interrupts are recognized and two 
        /// refresh cycles execute after each data transfer.
        /// When BC is set to 0, prior to instruction execution, the 
        /// instruction loops through 64 KB.
        ///  
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 0 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 4, 3, 5, 5 (21)
        /// BC=0:  4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,de:3,de:1 ×2,[de:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:3,de:5,[5]
        /// </remarks>
        private void LDDR()
        {
            var memVal = ReadMemory(_registers.HL--);
            ClockP3();
            WriteMemory(_registers.DE, memVal);
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ClockP3();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
                WriteMemory(_registers.DE, memVal);
                ClockP1();
            }
            _registers.DE--;
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) | ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) | memVal);
            if (--_registers.BC == 0)
            {
                return;
            }
            _registers.F |= FlagsSetMask.PV;
            _registers.PC -= 2;
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory((ushort)(_registers.DE + 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE + 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE + 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE + 1));
                ClockP1();
                ReadMemory((ushort)(_registers.DE + 1));
                ClockP1();
            }
            _registers.WZ = (ushort)(_registers.PC + 1);
        }

        /// <summary>
        /// "CPDR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the memory location addressed by HL is compared
        /// with the contents of A. During the compare operation, the Zero
        /// flag is set or reset. HL and BC are decremented.
        /// If BC is not 0 and A = (HL), PC is decremented by two and the 
        /// instruction is repeated. Interrupts are recognized and two 
        /// refresh cycles execute after each data transfer. When the BC is 
        /// set to 0, prior to instruction execution, the instruction loops 
        /// through 64 KB if no match is found.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if A is (HL); otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if BC – 1 is not 0; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 1 | 0 | 0 | 1 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 4, 3, 5, 5 (21)
        /// BC=0:  4, 4, 3, 5 (16)
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1 ×5,[hl:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:8,[5]
        /// </remarks>
        private void CPDR()
        {
            _registers.WZ--;
            var memVal = ReadMemory(_registers.HL);
            var compRes = _registers.A - memVal;
            var r3r5 = compRes;
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
                r3r5 = compRes - 1;
            }
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            flags |= (byte)(compRes & FlagsSetMask.S);
            flags |= (byte)((r3r5 & FlagsSetMask.R3) | ((r3r5 << 4) & FlagsSetMask.R5));

            ClockP3();
            if (UseGateArrayContention)
            {
                ClockP5();
            }
            else
            {
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            _registers.HL--;

            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    _registers.PC -= 2;
                    if (UseGateArrayContention)
                    {
                        ClockP5();
                    }
                    else
                    {
                        ReadMemory((ushort)(_registers.HL + 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL + 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL + 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL + 1));
                        ClockP1();
                        ReadMemory((ushort)(_registers.HL + 1));
                        ClockP1();
                    }
                    _registers.WZ = (ushort)(_registers.PC + 1);
                }
            }
            _registers.F = (byte)flags;
        }

        /// <summary>
        /// "INDR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of C are placed on the bottom half (A0 through A7)
        /// of the address bus to select the I/O device at one of 256 
        /// possible ports. Register B is used as a byte counter, and its 
        /// contents are placed on the top half (A8 through A15) of the 
        /// address bus at this time. Then one byte from the selected port 
        /// is placed on the data bus and written to the CPU. The contents 
        /// of HL are placed on the address bus and the input byte is written
        /// to the corresponding location of memory. Finally, B and HL are 
        /// decremented.
        /// If decrementing causes B to go to 0, the instruction is 
        /// terminated. If B is not 0, PC is decremented by two and the 
        /// instruction repeated. Interrupts are recognized and two refresh 
        /// cycles are executed after each data transfer.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 0 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 5, 3, 4, 5 (21)
        /// BC=0:  4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,I/O,hl:3,[hl:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:5,I/O,hl:3,[5]
        /// </remarks>
        private void INDR()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.WZ = (ushort)(_registers.BC - 1);

            // I/O
            var val = ReadPort(_registers.BC);

            // hl:3
            WriteMemory(_registers.HL, val);
            ClockP3();

            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            _registers.HL--;
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                if (UseGateArrayContention)
                {
                    ClockP5();
                }
                else
                {
                    ReadMemory((ushort)(_registers.HL + 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL + 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL + 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL + 1));
                    ClockP1();
                    ReadMemory((ushort)(_registers.HL + 1));
                    ClockP1();
                }
            }
            else
            {
                _registers.F &= FlagsResetMask.PV;
            }

        }

        /// <summary>
        /// "OTDR" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the HL register pair are placed on the address
        /// bus to select a location in memory. The byte contained in this 
        /// memory location is temporarily stored in the CPU. Then, after B 
        /// is decremented, the contents of C are placed on the bottom half
        /// (A0 through A7) of the address bus to select the I/O device at 
        /// one of 256 possible ports. Register B is used as a byte counter, 
        /// and its decremented value is placed on the top half (A8 through 
        /// A15) of the address bus. The byte to be output is placed on the 
        /// data bus and written to a selected peripheral device. Finally, 
        /// the HL is decremented.
        /// If the decremented B Register is not 0, PC is decremented by 
        /// two and the instruction is repeated. If B has gone to 0, the
        /// instruction is terminated. Interrupts are recognized and two 
        /// refresh cycles are executed after each data transfer.
        /// 
        /// S is unknown.
        /// Z is set if B – 1 = 0; otherwise it is reset.
        /// H is unknown.
        /// P/V is unknown.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 1 |
        /// =================================
        /// T-States: 
        /// BC!=0: 4, 5, 3, 4, 5 (21)
        /// BC=0:  4, 5, 3, 4 (16)
        /// Contention breakdown: pc:4,pc+1:5,hl:3,I/O,[bc:1 ×5]
        /// Gate array contention breakdown: pc:4,pc+1:5,hl:3,I/O,[5]
        /// </remarks>
        private void OTDR()
        {
            // pc+1:5 -> remaining 1
            ClockP1();

            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;

            // hl:3
            var val = ReadMemory(_registers.HL--);
            ClockP3();

            // I/O
            WritePort(_registers.BC, val);

            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                if (UseGateArrayContention)
                {
                    ClockP5();
                }
                else
                {
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                    ReadMemory(_registers.BC);
                    ClockP1();
                }
            }
            else _registers.F &= FlagsResetMask.PV;
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0xFF) _registers.F |= FlagsSetMask.C;
            _registers.WZ = (ushort)(_registers.BC - 1);
        }
    }
}