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
                null,     null,     null,     null,     null,     null,     null,     null,     // 20..27
                null,     null,     null,     null,     null,     null,     null,     null,     // 28..2F
                null,     null,     null,     null,     null,     null,     null,     null,     // 30..37
                null,     null,     null,     null,     null,     null,     null,     null,     // 38..3F

                IN_B_C,   OUT_C_B,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_XR_A,  // 40..47
                IN_C_C,   OUT_C_C,  ADCHL_QQ, LDQQ_NNi, NEG,      RETI,     IM_N,     LD_XR_A,  // 48..4F
                IN_D_C,   OUT_C_D,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_A_XR,  // 50..57
                IN_E_C,   OUT_C_E,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     LD_A_XR,  // 58..5F
                IN_H_C,   OUT_C_H,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     RRD,      // 60..67
                IN_L_C,   OUT_C_L,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     RLD,      // 60..6F
                IN_F_C,   OUT_C_0,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     null,     // 70..77
                IN_A_C,   OUT_C_A,  ADCHL_QQ, LDSP_NNi, NEG,      RETN,     IM_N,     null,     // 78..7F

                null,     null,     null,     null,     null,     null,     null,     null,     // 80..87
                null,     null,     null,     null,     null,     null,     null,     null,     // 88..8F
                null,     null,     null,     null,     null,     null,     null,     null,     // 90..97
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
        /// </remarks>
        private void IN_B_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_B()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.B);
            ClockP1();
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
        /// </remarks>
        private void IN_C_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.C);
            ClockP1();
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
        /// </remarks>
        private void IN_D_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_D()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.D);
            ClockP1();
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
        /// </remarks>
        private void IN_E_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_E()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.E);
            ClockP1();
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
        /// </remarks>
        private void IN_H_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_H()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.H);
            ClockP1();
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
        /// </remarks>
        private void IN_L_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_L()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.L);
            ClockP1();
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
        /// </remarks>
        private void IN_F_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_0()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, 0);
            ClockP1();
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
        /// </remarks>
        private void IN_A_C()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var pval = ReadPort(_registers.BC);
            ClockP4();
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
        /// </remarks>
        private void OUT_C_A()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            ClockP3();
            WritePort(_registers.BC, _registers.A);
            ClockP1();
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
        /// </remarks>
        private void SBCHL_QQ()
        {
            _registers.MW = (ushort)(_registers.HL + 1);
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
        /// </remarks>
        private void LDNNi_QQ()
        {
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadMemory(_registers.PC) << 8 | l);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort)(addr + 1);
            var regVal =  _registers[(Reg16Index)((_opCode & 0x30) >> 4)];
            WriteMemory(addr, (byte)(regVal & 0xFF));
            ClockP3();
            WriteMemory(_registers.MW, (byte)(regVal >> 8));
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
            if (result >= 0x80 || result <= -0x81) flags |= FlagsSetMask.PV;

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
            _registers.MW = addr;

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
            _registers.MW = addr;

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
        /// </remarks>
        private void ADCHL_QQ()
        {
            _registers.MW = (ushort)(_registers.HL + 1);
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
        /// </remarks>
        private void LDQQ_NNi()
        {
            var addrl = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadMemory(_registers.PC) << 8 | addrl);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort)(addr + 1);
            var l = ReadMemory(addr);
            ClockP3();
            var h = ReadMemory(_registers.MW);
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
        /// </remarks>
        private void LDSP_NNi()
        {
            var oldSP = _registers.SP;

            var addrl = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadMemory(_registers.PC) << 8 | addrl);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort)(addr + 1);
            var l = ReadMemory(addr);
            ClockP3();
            var h = ReadMemory(_registers.MW);
            ClockP3();
            _registers.SP = (ushort)(h << 8 | l);

            StackDebugSupport?.RecordStackPointerManipulationEvent(
                new StackPointerManipulationEvent((ushort)(_registers.PC - 4),
                    $"ld sp,({addr:X4}H)",
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
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | 0 | R | 1 | 1 | 1 | 1 |
        /// =================================
        /// R: 0=I, 1=R
        /// T-States: 4, 5 (9)
        /// </remarks>
        private void LD_A_XR()
        {
            _registers.A = (_opCode & 0x08) == 0 
                ? _registers.I : _registers.R;
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
        /// </remarks>
        private void RRD()
        {
            var tmp = ReadMemory(_registers.HL);
            ClockP3();
            _registers.MW = (ushort)(_registers.HL + 1);
            WriteMemory(_registers.HL, (byte)((_registers.A << 4) | (tmp >> 4)));
            ClockP3();
            _registers.A = (byte)((_registers.A & 0xF0) | (tmp & 0x0F));
            _registers.F = (byte)(s_AluLogOpFlags[_registers.A] | (_registers.F & FlagsSetMask.C));
            ClockP4();
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
        /// </remarks>
        private void RLD()
        {
            var tmp = ReadMemory(_registers.HL);
            ClockP3();
            _registers.MW = (ushort)(_registers.HL + 1);
            WriteMemory(_registers.HL, (byte)((_registers.A & 0x0F) | (tmp << 4)));
            ClockP3();
            _registers.A = (byte)((_registers.A & 0xF0) | (tmp >> 4));
            _registers.F = (byte)(s_AluLogOpFlags[_registers.A] | (_registers.F & FlagsSetMask.C));
            ClockP4();
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
        /// </remarks>
        private void LDI()
        {
            var memVal = ReadMemory(_registers.HL++);
            ClockP3();
            WriteMemory(_registers.DE++, memVal);
            ClockP5();
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
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
        /// </remarks>
        private void CPI()
        {
            var memVal = ReadMemory(_registers.HL++);
            var compRes = _registers.A - memVal;
            ClockP3();
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.F = (byte)flags;
            _registers.MW++;
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
        /// </remarks>
        private void INI()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var val = ReadPort(_registers.BC);
            ClockP1();
            WriteMemory(_registers.HL++, val);
            ClockP3();
            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            ClockP4();
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
        /// </remarks>
        private void OUTI()
        {
            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;
            ClockP1();
            var val = ReadMemory(_registers.HL++);
            ClockP3();
            WritePort(_registers.BC, val);
            ClockP4();
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0) _registers.F |= FlagsSetMask.C;
            _registers.MW = (ushort)(_registers.BC + 1);
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
        /// </remarks>
        private void LDD()
        {
            var memVal = ReadMemory(_registers.HL--);
            ClockP3();
            WriteMemory(_registers.DE--, memVal);
            ClockP5();
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
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
        /// </remarks>
        private void CPD()
        {
            var memVal = ReadMemory(_registers.HL--);
            var compRes = _registers.A - memVal;
            ClockP3();
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            _registers.F = (byte)flags;
            _registers.MW--;
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
        /// </remarks>
        private void IND()
        {
            _registers.MW = (ushort)(_registers.BC - 1);
            var val = ReadPort(_registers.BC);
            ClockP1();
            WriteMemory(_registers.HL--, val);
            ClockP3();
            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            ClockP4();
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
        /// </remarks>
        private void OUTD()
        {
            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;
            ClockP1();
            var val = ReadMemory(_registers.HL--);
            ClockP3();
            WritePort(_registers.BC, val);
            ClockP4();
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0xFF) _registers.F |= FlagsSetMask.C;
            _registers.MW = (ushort)(_registers.BC - 1);
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
        /// </remarks>
        private void LDIR()
        {
            var memVal = ReadMemory(_registers.HL++);
            ClockP3();
            WriteMemory(_registers.DE++, memVal);
            ClockP3();
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            ClockP2();
            if (--_registers.BC == 0)
            {
                return;
            }
            _registers.F |= FlagsSetMask.PV;
            _registers.PC -= 2;
            ClockP5();
            _registers.MW = (ushort)(_registers.PC + 1);
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
        /// </remarks>
        private void CPIR()
        {
            _registers.MW++;
            var memVal = ReadMemory(_registers.HL++);
            var compRes = _registers.A - memVal;
            ClockP3();
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    _registers.PC -= 2;
                    ClockP5();
                    _registers.MW = (ushort)(_registers.PC + 1);
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
        /// </remarks>
        private void INIR()
        {
            _registers.MW = (ushort)(_registers.BC + 1);
            var val = ReadPort(_registers.BC);
            ClockP1();
            WriteMemory(_registers.HL++, val);
            ClockP3();
            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            ClockP4();
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                ClockP5();
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
        /// </remarks>
        private void OTIR()
        {
            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;
            ClockP1();
            var val = ReadMemory(_registers.HL++);
            ClockP3();
            WritePort(_registers.BC, val);
            ClockP4();
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                ClockP5();
            }
            else
            {
                _registers.F &= FlagsResetMask.PV;
            }
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0) _registers.F |= FlagsSetMask.C;
            _registers.MW = (ushort)(_registers.BC + 1);
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
        /// </remarks>
        private void LDDR()
        {
            var memVal = ReadMemory(_registers.HL--);
            ClockP3();
            WriteMemory(_registers.DE--, memVal);
            ClockP5();
            memVal += _registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            _registers.F = (byte)((_registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            if (--_registers.BC == 0)
            {
                return;
            }
            _registers.F |= FlagsSetMask.PV;
            _registers.PC -= 2;
            ClockP5();
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
        /// </remarks>
        private void CPDR()
        {
            _registers.MW--;
            var memVal = ReadMemory(_registers.HL--);
            var compRes = _registers.A - memVal;
            ClockP3();
            var flags = (byte)(_registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((_registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--_registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    _registers.PC -= 2;
                    ClockP5();
                    _registers.MW = (ushort)(_registers.PC + 1);
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
        /// </remarks>
        private void INDR()
        {
            _registers.MW = (ushort)(_registers.BC - 1);
            var val = ReadPort(_registers.BC);
            ClockP1();
            WriteMemory(_registers.HL--, val);
            ClockP3();
            _registers.F = (byte)(s_DecOpFlags[_registers.B] | (_registers.F & FlagsSetMask.C));
            _registers.B--;
            ClockP4();
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                ClockP5();
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
        /// </remarks>
        private void OTDR()
        {
            _registers.F = s_DecOpFlags[_registers.B];
            _registers.B--;
            ClockP1();
            var val = ReadMemory(_registers.HL--);
            ClockP3();
            WritePort(_registers.BC, val);
            ClockP4();
            if (_registers.B != 0)
            {
                _registers.F |= FlagsSetMask.PV;
                _registers.PC -= 2;
                ClockP5();
            }
            else _registers.F &= FlagsResetMask.PV;
            _registers.F &= FlagsResetMask.C;
            if (_registers.L == 0xFF) _registers.F |= FlagsSetMask.C;
            _registers.MW = (ushort)(_registers.BC - 1);
        }
    }
}