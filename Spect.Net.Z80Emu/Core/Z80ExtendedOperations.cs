// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides extended operations
    /// (OpCodes used with the 0xED prefix)
    /// </summary>
    public partial class Z80
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
            var opMethod = _extendedOperations[OpCode];
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

                IN_Q_C,   OUT_C_Q,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_XR_A,  // 40..47
                IN_Q_C,   OUT_C_Q,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     LD_XR_A,  // 48..4F
                IN_Q_C,   OUT_C_Q,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     LD_A_XR,  // 50..57
                IN_Q_C,   OUT_C_Q,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     LD_A_XR,  // 58..5F
                IN_Q_C,   OUT_C_Q,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     RRD,      // 60..67
                IN_Q_C,   OUT_C_Q,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     RLD,      // 60..6F
                IN_Q_C,   OUT_C_Q,  SBCHL_QQ, LDNNi_QQ, NEG,      RETN,     IM_N,     null,     // 70..77
                IN_Q_C,   OUT_C_Q,  ADCHL_QQ, LDQQ_NNi, NEG,      RETN,     IM_N,     null,     // 78..7F

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
        /// "IN Q,(C)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on
        /// the top half (A8 through A15) of the address bus at this time. 
        /// Then one byte from the selected port is placed on the data bus 
        /// and written to Register Q in the CPU.
        /// 
        /// S is set if input data is negative; otherwise, it is reset.
        /// Z is set if input data is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity is even; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | Q | 0 | 0 | 0 |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 4 (12)
        /// </remarks>
        private void IN_Q_C()
        {
            Registers.MW = (ushort)(Registers.BC + 1);
            var pval = ReadPort(Registers.BC);
            ClockP4();
            var q = (Reg8Index)((OpCode & 0x38) >> 3);
            if (q != Reg8Index.F)
            {
                Registers[q] = pval;
            }
            Registers.F = (byte)(s_AluLogOpFlags[pval] | (Registers.F & FlagsSetMask.C));
        }

        /// <summary>
        /// "OUT (C),Q" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are placed on the bottom half (A0 
        /// through A7) of the address bus to select the I/O device at one 
        /// of 256 possible ports. The contents of Register B are placed on 
        /// the top half (A8 through A15) of the address bus at this time.
        /// Then the byte contained in register Q is placed on the data bus 
        /// and written to the selected peripheral device.
        /// 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 1 | 0 | 1 | ED
        /// =================================
        /// | 0 | 1 | Q | Q | Q | 0 | 0 | 1 |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 4 (12)
        /// </remarks>
        private void OUT_C_Q()
        {
            Registers.MW = (ushort)(Registers.BC + 1);
            var q = (Reg8Index)((OpCode & 0x38) >> 3);
            ClockP3();
            WritePort(Registers.BC, 
                q != Reg8Index.F ? Registers[q] : (byte)0);
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
            Registers.MW = (ushort)(Registers.HL + 1);
            var cfVal = Registers.CFlag ? 1 : 0;
            var qq = (Reg16Index)((OpCode & 0x30) >> 4);
            var flags = FlagsSetMask.N;
            flags |= (byte)((((Registers.HL & 0x0FFF) - (Registers[qq] & 0x0FFF) - cfVal) >> 8) & FlagsSetMask.H);
            var sbcVal = (uint)((Registers.HL & 0xFFFF) - (Registers[qq] & 0xFFFF) - cfVal);
            if ((sbcVal & 0x10000) != 0)
            {
                flags |= FlagsSetMask.C;
            }
            if ((sbcVal & 0xFFFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            var signedSbc = (short)Registers.HL - (short)Registers[qq] - cfVal;
            if (signedSbc < -0x8000 || signedSbc >= 0x8000)
            {
                flags |= FlagsSetMask.PV;
            }
            Registers.HL = (ushort)sbcVal;
            Registers.F = (byte)(flags | (Registers.H & (FlagsSetMask.S | FlagsSetMask.R3 | FlagsSetMask.R5)));
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
            var l = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var addr = (ushort)(ReadMemory(Registers.PC) << 8 | l);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(addr + 1);
            var regVal =  Registers[(Reg16Index)((OpCode & 0x30) >> 4)];
            WriteMemory(addr, (byte)(regVal & 0xFF));
            ClockP3();
            WriteMemory(Registers.MW, (byte)(regVal >> 8));
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
            var result = -Registers.A;
            var lNibble = -(Registers.A & 0x0F) & 0x10;

            var flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            flags |= FlagsSetMask.N;
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if (Registers.A != 0) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (result >= 0x80 || result <= -0x81) flags |= FlagsSetMask.PV;

            Registers.F = flags;
            Registers.A = (byte)result;
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
        /// T-States: 4, 4, 3, 3 (14)
        /// </remarks>
        private void RETN()
        {
            IFF1 = IFF2;
            ushort addr = ReadMemory(Registers.SP);
            ClockP3();
            Registers.SP++;
            addr += (ushort)(ReadMemory(Registers.SP) * 0x100);
            ClockP3();
            Registers.SP++;
            Registers.PC = addr;
            Registers.MW = addr;
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
            var mode = (byte)((OpCode & 0x18) >> 3);
            if (mode < 2) mode = 1;
            mode--;
            InterruptMode = mode;
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
            if ((OpCode & 0x08) == 0)
            {
                Registers.I = Registers.A;
            }
            else
            {
                Registers.R = Registers.A;
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
            Registers.MW = (ushort)(Registers.HL + 1);
            var cfVal = Registers.CFlag ? 1 : 0;
            var qq = (Reg16Index)((OpCode & 0x30) >> 4);
            var flags = (byte)((((Registers.HL & 0x0FFF) + (Registers[qq] & 0x0FFF) + (Registers.F & FlagsSetMask.C)) >> 8) & FlagsSetMask.H);
            var adcVal = (uint)((Registers.HL & 0xFFFF) + (Registers[qq] & 0xFFFF) + cfVal);
            if ((adcVal & 0x10000) != 0)
            {
                flags |= FlagsSetMask.C;
            }
            if ((adcVal & 0xFFFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            var signedAdc = (short)Registers.HL + (short)Registers[qq] + cfVal;
            if (signedAdc < -0x8000 || signedAdc >= 0x8000)
            {
                flags |= FlagsSetMask.PV;
            }
            Registers.HL = (ushort)adcVal;
            Registers.F = (byte)(flags | (Registers.H & (FlagsSetMask.S | FlagsSetMask.R3 | FlagsSetMask.R5)));
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
            var addrl = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var addr = (ushort)(ReadMemory(Registers.PC) << 8 | addrl);
            ClockP3();
            Registers.PC++;
            Registers.MW = (ushort)(addr + 1);
            var l = ReadMemory(addr);
            ClockP3();
            var h = ReadMemory(Registers.MW);
            ClockP3();
            Registers[(Reg16Index)((OpCode & 0x30) >> 4)] = (ushort)(h << 8 | l);
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
            Registers.A = (OpCode & 0x08) == 0 
                ? Registers.I : Registers.R;
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
            var tmp = ReadMemory(Registers.HL);
            ClockP3();
            Registers.MW = (ushort)(Registers.HL + 1);
            WriteMemory(Registers.HL, (byte)((Registers.A << 4) | (tmp >> 4)));
            ClockP3();
            Registers.A = (byte)((Registers.A & 0xF0) | (tmp & 0x0F));
            Registers.F = (byte)(s_AluLogOpFlags[Registers.A] | (Registers.F & FlagsSetMask.C));
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
            var tmp = ReadMemory(Registers.HL);
            ClockP3();
            Registers.MW = (ushort)(Registers.HL + 1);
            WriteMemory(Registers.HL, (byte)((Registers.A & 0x0F) | (tmp << 4)));
            ClockP3();
            Registers.A = (byte)((Registers.A & 0xF0) | (tmp >> 4));
            Registers.F = (byte)(s_AluLogOpFlags[Registers.A] | (Registers.F & FlagsSetMask.C));
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
            var memVal = ReadMemory(Registers.HL++);
            ClockP3();
            WriteMemory(Registers.DE++, memVal);
            ClockP5();
            memVal += Registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            if (--Registers.BC != 0) Registers.F |= FlagsSetMask.PV;
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
            var memVal = ReadMemory(Registers.HL++);
            var compRes = Registers.A - memVal;
            ClockP3();
            var flags = (byte)(Registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((Registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--Registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            Registers.F = (byte)flags;
            Registers.MW++;
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
            Registers.MW = (ushort)(Registers.BC + 1);
            var val = ReadPort(Registers.BC);
            ClockP1();
            WriteMemory(Registers.HL++, val);
            ClockP3();
            Registers.F = (byte)(s_DecOpFlags[Registers.B] | (Registers.F & FlagsSetMask.C));
            Registers.B--;
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
            Registers.F = s_DecOpFlags[Registers.B];
            Registers.B--;
            ClockP1();
            var val = ReadMemory(Registers.HL++);
            ClockP3();
            WritePort(Registers.BC, val);
            ClockP4();
            Registers.F &= FlagsResetMask.C;
            if (Registers.L == 0) Registers.F |= FlagsSetMask.C;
            Registers.MW = (ushort)(Registers.BC + 1);
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
            var memVal = ReadMemory(Registers.HL--);
            ClockP3();
            WriteMemory(Registers.DE--, memVal);
            ClockP5();
            memVal += Registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            if (--Registers.BC != 0) Registers.F |= FlagsSetMask.PV;
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
            var memVal = ReadMemory(Registers.HL--);
            var compRes = Registers.A - memVal;
            ClockP3();
            var flags = (byte)(Registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((Registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--Registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
            }
            Registers.F = (byte)flags;
            Registers.MW--;
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
            Registers.MW = (ushort)(Registers.BC - 1);
            var val = ReadPort(Registers.BC);
            ClockP1();
            WriteMemory(Registers.HL--, val);
            ClockP3();
            Registers.F = (byte)(s_DecOpFlags[Registers.B] | (Registers.F & FlagsSetMask.C));
            Registers.B--;
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
            Registers.F = s_DecOpFlags[Registers.B];
            Registers.B--;
            ClockP1();
            var val = ReadMemory(Registers.HL--);
            ClockP3();
            WritePort(Registers.BC, val);
            ClockP4();
            Registers.F &= FlagsResetMask.C;
            if (Registers.L == 0xFF) Registers.F |= FlagsSetMask.C;
            Registers.MW = (ushort)(Registers.BC - 1);
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
            var memVal = ReadMemory(Registers.HL++);
            ClockP3();
            WriteMemory(Registers.DE++, memVal);
            ClockP3();
            memVal += Registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            ClockP2();
            if (--Registers.BC == 0)
            {
                return;
            }
            Registers.F |= FlagsSetMask.PV;
            Registers.PC -= 2;
            ClockP5();
            Registers.MW = (ushort)(Registers.PC + 1);
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
            Registers.MW++;
            var memVal = ReadMemory(Registers.HL++);
            var compRes = Registers.A - memVal;
            ClockP3();
            var flags = (byte)(Registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((Registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--Registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    Registers.PC -= 2;
                    ClockP5();
                    Registers.MW = (ushort)(Registers.PC + 1);
                }
            }
            Registers.F = (byte)flags;
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
            Registers.MW = (ushort)(Registers.BC + 1);
            var val = ReadPort(Registers.BC);
            ClockP1();
            WriteMemory(Registers.HL++, val);
            ClockP3();
            Registers.F = (byte)(s_DecOpFlags[Registers.B] | (Registers.F & FlagsSetMask.C));
            Registers.B--;
            ClockP4();
            if (Registers.B != 0)
            {
                Registers.F |= FlagsSetMask.PV;
                Registers.PC -= 2;
                ClockP5();
            }
            else Registers.F &= FlagsResetMask.PV;
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
            Registers.F = s_DecOpFlags[Registers.B];
            Registers.B--;
            ClockP1();
            var val = ReadMemory(Registers.HL++);
            ClockP3();
            WritePort(Registers.BC, val);
            ClockP4();
            if (Registers.B != 0)
            {
                Registers.F |= FlagsSetMask.PV;
                Registers.PC -= 2;
                ClockP5();
            }
            else
            {
                Registers.F &= FlagsResetMask.PV;
            }
            Registers.F &= FlagsResetMask.C;
            if (Registers.L == 0) Registers.F |= FlagsSetMask.C;
            Registers.MW = (ushort)(Registers.BC + 1);
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
            var memVal = ReadMemory(Registers.HL--);
            ClockP3();
            WriteMemory(Registers.DE--, memVal);
            ClockP5();
            memVal += Registers.A;
            memVal = (byte)((memVal & FlagsSetMask.R3) + ((memVal << 4) & FlagsSetMask.R5));
            Registers.F = (byte)((Registers.F & ~(FlagsSetMask.N | FlagsSetMask.H | FlagsSetMask.PV | FlagsSetMask.R3 | FlagsSetMask.R5)) + memVal);
            if (--Registers.BC == 0)
            {
                return;
            }
            Registers.F |= FlagsSetMask.PV;
            Registers.PC -= 2;
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
            Registers.MW--;
            var memVal = ReadMemory(Registers.HL--);
            var compRes = Registers.A - memVal;
            ClockP3();
            var flags = (byte)(Registers.F & FlagsSetMask.C) | FlagsSetMask.N;
            flags |= (byte)(compRes & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S));
            if ((compRes & 0xFF) == 0)
            {
                flags |= FlagsSetMask.Z;
            }
            if ((((Registers.A & 0x0F) - (compRes & 0x0F)) & 0x10) != 0)
            {
                flags |= FlagsSetMask.H;
            }
            ClockP5();
            if (--Registers.BC != 0)
            {
                flags |= FlagsSetMask.PV;
                if ((flags & FlagsSetMask.Z) == 0)
                {
                    Registers.PC -= 2;
                    ClockP5();
                    Registers.MW = (ushort)(Registers.PC + 1);
                }
            }
            Registers.F = (byte)flags;
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
            Registers.MW = (ushort)(Registers.BC - 1);
            var val = ReadPort(Registers.BC);
            ClockP1();
            WriteMemory(Registers.HL--, val);
            ClockP3();
            Registers.F = (byte)(s_DecOpFlags[Registers.B] | (Registers.F & FlagsSetMask.C));
            Registers.B--;
            ClockP4();
            if (Registers.B != 0)
            {
                Registers.F |= FlagsSetMask.PV;
                Registers.PC -= 2;
                ClockP5();
            }
            else
            {
                Registers.F &= FlagsResetMask.PV;
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
            Registers.F = s_DecOpFlags[Registers.B];
            Registers.B--;
            ClockP1();
            var val = ReadMemory(Registers.HL--);
            ClockP3();
            WritePort(Registers.BC, val);
            ClockP4();
            if (Registers.B != 0)
            {
                Registers.F |= FlagsSetMask.PV;
                Registers.PC -= 2;
                ClockP5();
            }
            else Registers.F &= FlagsResetMask.PV;
            Registers.F &= FlagsResetMask.C;
            if (Registers.L == 0xFF) Registers.F |= FlagsSetMask.C;
            Registers.MW = (ushort)(Registers.BC - 1);
        }
    }
}