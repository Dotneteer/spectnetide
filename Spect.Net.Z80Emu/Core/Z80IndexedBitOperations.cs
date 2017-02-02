using System;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Core
{
    public partial class Z80
    {
        /// <summary>
        /// Indexed bit (0xDDCB or 0xFDCB-prefixed) operations jump table
        /// </summary>
        private Action<byte, ushort>[] _indexedBitOperations;

        /// <summary>
        /// Initializes the indexed bit operation execution tables
        /// </summary>
        private void InitializeIndexedBitOpsExecutionTable()
        {
            _indexedBitOperations = new Action<byte, ushort>[]
            {
                XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC_Q,   XRLC,     XRLC_Q,   // 00..07
                XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC_Q,   XRRC,     XRRC_Q,   // 08..0F
                XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL_Q,    XRL,      XRL_Q,    // 10..17
                XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR_Q,    XRR,      XRR_Q,    // 18..1F
                XSLA_R,   XSLA_R,   XSLA_R,   XSLA_R,   XSLA_R,   XSLA_R,   XSLA,     XSLA_R,   // 20..27
                XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA_R,   XSRA,     XSRA_R,   // 28..2F
                XSLL_R,   XSLL_R,   XSLL_R,   XSLL_R,   XSLL_R,   XSLL_R,   XSLL,     XSLL_R,   // 30..37
                XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL_R,   XSRL,     XSRL_R,   // 38..3F

                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 40..47
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 48..4F
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 50..57
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 58..5F
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 60..67
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 68..6F
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 70..77
                XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN_R,  XBITN,    XBITN_R,  // 70..7F

                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // 80..87
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // 88..8F
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // 90..97
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // 98..9F
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // A0..A7
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // A8..AF
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // B0..B7
                XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES_R,   XRES,     XRES_R,   // B8..BF

                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // C0..C7
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // C8..CF
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // D0..D7
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // D8..DF
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // E0..E7
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // E8..EF
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // F0..F7
                XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET_R,   XSET,     XSET_R,   // F8..FF
            };
        }

        /// <summary>
        /// "RLC (IDR + D),Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 7 of Q.
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
        private void XRLC_Q(byte opCode, ushort addr)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rlcVal = ReadMemory(addr, false);
            ClockP3();
            Registers.F = s_RlcFlags[rlcVal];
            rlcVal <<= 1;
            if ((rlcVal & 0x100) != 0)
            {
                rlcVal = (rlcVal | 0x01) & 0xFF;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlcVal);
            Registers[q] = (byte)rlcVal;
            ClockP3();
        }

        /// <summary>
        /// "RLC (IDR + D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        private void XRLC(byte opCode, ushort addr)
        {
            int rlcVal = ReadMemory(addr, false);
            ClockP3();
            Registers.F = s_RlcFlags[rlcVal];
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
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 0 of Q.
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
        private void XRRC_Q(byte opCode, ushort addr)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rrcVal = ReadMemory(addr, false);
            ClockP3();
            Registers.F = s_RlcFlags[rrcVal];
            rrcVal = (byte)((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            ClockP1();
            WriteMemory(addr, (byte)rrcVal);
            Registers[q] = (byte)rrcVal;
            ClockP3();
        }


        /// <summary>
        /// "RRC (IDR + D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        private void XRRC(byte opCode, ushort addr)
        {
            int rrcVal = ReadMemory(addr, false);
            ClockP3();
            Registers.F = s_RlcFlags[rrcVal];
            rrcVal = (byte)((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            ClockP1();
            WriteMemory(addr, (byte)rrcVal);
            ClockP3();
        }


        /// <summary>
        /// "RL (IDR + D),Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 7 of Q.
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
        private void XRL_Q(byte opCode, ushort addr)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rlVal = ReadMemory(addr, false);
            ClockP3();
            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                rlVal <<= 1;
                rlVal++;
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                rlVal <<= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlVal);
            Registers[q] = (byte)rlVal;
            ClockP3();
        }

        /// <summary>
        /// "RL (IDR + D),Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 7 of Q.
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
        private void XRL(byte opCode, ushort addr)
        {
            int rlVal = ReadMemory(addr, false);
            ClockP3();
            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                rlVal <<= 1;
                rlVal++;
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                rlVal <<= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rlVal);
            ClockP3();
        }

        /// <summary>
        /// "RR (IDR + D),Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 0 of Q.
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
        private void XRR_Q(byte opCode, ushort addr)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rrVal = ReadMemory(addr, false);
            ClockP3();
            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                rrVal >>= 1;
                rrVal += 0x80;
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                rrVal >>= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rrVal);
            Registers[q] = (byte)rrVal;
            ClockP3();
        }

        /// <summary>
        /// "RR (IDR + D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
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
        /// C is data from bit 0 of Q.
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
        private void XRR(byte opCode, ushort addr)
        {
            int rrVal = ReadMemory(addr, false);
            ClockP3();
            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                rrVal >>= 1;
                rrVal += 0x80;
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                rrVal >>= 1;
            }
            ClockP1();
            WriteMemory(addr, (byte)rrVal);
            ClockP3();
        }

        private void XSET(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSET_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XRES(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XRES_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XBITN(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XBITN_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSRL(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSRL_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSLL(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSLL_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSRA(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSRA_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSLA(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }

        private void XSLA_R(byte arg1, ushort arg2)
        {
            throw new NotImplementedException();
        }
    }
}