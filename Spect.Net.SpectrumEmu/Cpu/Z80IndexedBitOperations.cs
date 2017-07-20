using System;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Cpu
{
    public partial class Z80Cpu
    {
        /// <summary>
        /// Indexed bit (0xDDCB or 0xFDCB-prefixed) operations jump table
        /// </summary>
        private Action<ushort>[] _indexedBitOperations;

        /// <summary>
        /// Initializes the indexed bit operation execution tables
        /// </summary>
        private void InitializeIndexedBitOpsExecutionTable()
        {
            _indexedBitOperations = new Action<ushort>[]
            {
                XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC,     XRLC_Q,   // 00..07
                XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC,     XRRC_Q,   // 08..0F
                XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL,      XRL_Q,    // 10..17
                XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR,      XRR_Q,    // 18..1F
                XSLA_Q,   XSLA_Q,   XSLA_Q,   XSLA_Q,   XSLA_Q,   XSLA_Q,   XSLA,     XSLA_Q,   // 20..27
                XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA,     XSRA_R,   // 28..2F
                XSLL_Q,   XSLL_Q,   XSLL_Q,   XSLL_Q,   XSLL_Q,   XSLL_Q,   XSLL,     XSLL_Q,   // 30..37
                XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL,     XSRL_R,   // 38..3F

                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 40..47
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 48..4F
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 50..57
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 58..5F
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 60..67
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 68..6F
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 70..77
                XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    XBITN,    // 78..7F

                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // 80..87
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // 88..8F
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // 90..97
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // 98..9F
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // A0..A7
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // A8..AF
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // B0..B7
                XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,     XRES,    // B8..BF

                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // C0..C7
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // C8..CF
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // D0..D7
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // D8..DF
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // E0..E7
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // E8..EF
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,    // F0..F7
                XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET,     XSET     // F8..FF
            };
        }

        /// <summary>
        /// "RLC (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0. The result is
        /// stored in register Q
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRLC_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int rlcVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlcFlags[rlcVal];
            rlcVal <<= 1;
            if ((rlcVal & 0x100) != 0)
            {
                rlcVal = (rlcVal | 0x01) & 0xFF;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlcVal);
            _registers[q] = (byte)rlcVal;
            ClockP3();
        }

        /// <summary>
        /// "RLC (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRLC(ushort addr)
        {
            int rlcVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlcFlags[rlcVal];
            rlcVal <<= 1;
            if ((rlcVal & 0x100) != 0)
            {
                rlcVal = (rlcVal | 0x01) & 0xFF;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlcVal);
            ClockP3();
        }

        /// <summary>
        /// "RRC (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated right 1 bit position. The 
        /// contents of bit 0 are copied to the Carry flag and also to bit 7. The result is
        /// stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRRC_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int rrcVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlcFlags[rrcVal];
            rrcVal = (byte)((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            ClockP1();
            WriteMemory(addr, (byte)rrcVal);
            _registers[q] = (byte)rrcVal;
            ClockP3();
        }


        /// <summary>
        /// "RRC (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated right 1 bit position. The 
        /// contents of bit 0 are copied to the Carry flag and also to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRRC(ushort addr)
        {
            int rrcVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlcFlags[rrcVal];
            rrcVal = (byte)((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            ClockP1();
            WriteMemory(addr, (byte)rrcVal);
            ClockP3();
        }


        /// <summary>
        /// "RL (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous contents of the
        /// Carry flag are copied to bit 0. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRL_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int rlVal = ReadMemory(addr);
            ClockP3();
            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                rlVal <<= 1;
                rlVal++;
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                rlVal <<= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlVal);
            _registers[q] = (byte)rlVal;
            ClockP3();
        }

        /// <summary>
        /// "RL (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous contents of the
        /// Carry flag are copied to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRL(ushort addr)
        {
            int rlVal = ReadMemory(addr);
            ClockP3();
            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                rlVal <<= 1;
                rlVal++;
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                rlVal <<= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlVal);
            ClockP3();
        }

        /// <summary>
        /// "RR (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated right 1 bit position. The 
        /// contents of bit 0 are copied to the Carry flag, and the previous contents of the
        /// Carry flag are copied to bit 7. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRR_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int rrVal = ReadMemory(addr);
            ClockP3();
            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                rrVal >>= 1;
                rrVal += 0x80;
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                rrVal >>= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rrVal);
            _registers[q] = (byte)rrVal;
            ClockP3();
        }

        /// <summary>
        /// "RR (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are rotated right 1 bit position. The 
        /// contents of bit 0 are copied to the Carry flag, and the previous contents of the
        /// Carry flag are copied to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRR(ushort addr)
        {
            int rrVal = ReadMemory(addr);
            ClockP3();
            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                rrVal >>= 1;
                rrVal += 0x80;
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                rrVal >>= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rrVal);
            ClockP3();
        }

        /// <summary>
        /// "SLA (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 7 
        /// are copied to the Carry flag. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSLA_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int slaVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            slaVal <<= 1;
            ClockP1();
            WriteMemory(addr, (byte)slaVal);
            _registers[q] = (byte)slaVal;
            ClockP3();
        }

        /// <summary>
        /// "SLA (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 7 
        /// are copied to the Carry flag. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSLA(ushort addr)
        {
            int slaVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            slaVal <<= 1;
            ClockP1();
            WriteMemory(addr, (byte)slaVal);
            ClockP3();
        }

        /// <summary>
        /// "SRA (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 0 are 
        /// copied to the Carry flag and the previous contents of bit 7 remain
        /// unchanged. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSRA_R(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int sraVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_SraFlags[sraVal];
            sraVal = (sraVal >> 1) + (sraVal & 0x80);
            ClockP1();
            WriteMemory(addr, (byte)sraVal);
            _registers[q] = (byte)sraVal;
            ClockP3();
        }

        /// <summary>
        /// "SRA (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 0 are 
        /// copied to the Carry flag and the previous contents of bit 7 remain
        /// unchanged.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSRA(ushort addr)
        {
            int sraVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_SraFlags[sraVal];
            sraVal = (sraVal >> 1) + (sraVal & 0x80);
            ClockP1();
            WriteMemory(addr, (byte)sraVal);
            ClockP3();
        }

        /// <summary>
        /// "SLL (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// A logic shift left 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 7 
        /// are copied to the Carry flag and bit 0 is set. The result is 
        /// stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSLL_Q(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int sllVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlCarry1Flags[sllVal];
            sllVal <<= 1;
            sllVal++;
            ClockP1();
            WriteMemory(addr, (byte)sllVal);
            _registers[q] = (byte)sllVal;
            ClockP3();
        }

        /// <summary>
        /// "SLL (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// A logic shift left 1 bit position is performed on the 
        /// contents of the indexed memory address. The contents of bit 7 
        /// are copied to the Carry flag and bit 0 is set.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSLL(ushort addr)
        {
            int sllVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RlCarry1Flags[sllVal];
            sllVal <<= 1;
            sllVal++;
            ClockP1();
            WriteMemory(addr, (byte)sllVal);
            ClockP3();
        }

        /// <summary>
        /// "SRR (IDR + D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are shifted right 1 
        /// bit position. The contents of bit 0 are copied to the Carry flag, 
        /// and bit 7 is reset. The result is stored in register Q.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSRL_R(ushort addr)
        {
            var q = (Reg8Index)(OpCode & 0x07);
            int srlVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RrCarry0Flags[srlVal];
            srlVal >>= 1;
            ClockP1();
            WriteMemory(addr, (byte)srlVal);
            _registers[q] = (byte)srlVal;
            ClockP3();
        }

        /// <summary>
        /// "SRR (IDR + D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// The contents of the indexed memory address are shifted right 1 
        /// bit position. The contents of bit 0 are copied to the Carry flag, 
        /// and bit 7 is reset.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSRL(ushort addr)
        {
            int srlVal = ReadMemory(addr);
            ClockP3();
            _registers.F = s_RrCarry0Flags[srlVal];
            srlVal >>= 1;
            ClockP1();
            WriteMemory(addr, (byte)srlVal);
            ClockP3();
        }

        /// <summary>
        /// "BIT N,(IX+D)" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// This instruction tests bit N in the indexed memory location and 
        /// sets the Z flag accordingly.
        /// 
        /// S Set if N = 7 and tested bit is set.
        /// Z is set if specified bit is 0; otherwise, it is reset.
        /// H is set.
        /// P/V is Set just like ZF flag.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 1 | N | N | N | ? | ? | ? |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4 (20)
        /// </remarks>
        private void XBITN(ushort addr)
        {
            var n = (byte)((OpCode & 0x38) >> 3);
            var srcVal = ReadMemory(addr);
            ClockP4();
            var testVal = srcVal & (1 << n);
            var flags = FlagsSetMask.H
                        | (_registers.F & FlagsSetMask.C)
                        | (srcVal & (FlagsSetMask.R3 | FlagsSetMask.R5));
            if (testVal == 0)
            {
                flags |= FlagsSetMask.Z | FlagsSetMask.PV;
            }
            if (n == 7 && testVal != 0)
            {
                flags |= FlagsSetMask.S;
            }
            _registers.F = (byte)flags;
        }

        /// <summary>
        /// "RES N,(IX+D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// Bit N of the indexed memory location addressed is reset.
        /// The result is autocopied to register Q.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 0 | N | N | N | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XRES(ushort addr)
        {
            var srcVal = ReadMemory(addr);
            var n = (byte)((OpCode & 0x38) >> 3);
            var q = (Reg8Index) (OpCode & 0x07);
            srcVal &= (byte)~(1 << n);
            if (q != Reg8Index.F)
            {
                _registers[q] = srcVal;
            }
            ClockP4();
            WriteMemory(addr, srcVal);
            ClockP3();
        }

        /// <summary>
        /// "SET N,(IX+D),Q" operation
        /// </summary>
        /// <param name="addr">Indexed address</param>
        /// <remarks>
        /// 
        /// Bit N of the indexed memory location addressed is set.
        /// The result is autocopied to register Q.
        /// 
        /// =================================
        /// | 1 | 1 | X | 1 | 1 | 1 | 0 | 1 | DD/FD prefix
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 1 | N | N | N | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void XSET(ushort addr)
        {
            var srcVal = ReadMemory(addr);
            var n = (byte)((OpCode & 0x38) >> 3);
            var q = (Reg8Index)(OpCode & 0x07);
            srcVal |= (byte)(1 << n);
            if (q != Reg8Index.F)
            {
                _registers[q] = srcVal;
            }
            ClockP4();
            WriteMemory(addr, srcVal);
            ClockP3();
        }
    }
}