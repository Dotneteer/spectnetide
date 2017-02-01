// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides bit operations
    /// (OpCodes used with the 0xCB prefix)
    /// </summary>
    public partial class Z80
    {
        /// <summary>
        /// Bit (0xCB-prefixed) operations jump table
        /// </summary>
        private Action<byte>[] _bitOperations;

        /// <summary>
        /// Processes the operations with 0xCB prefix
        /// </summary>
        /// <param name="opCode">Operation code</param>
        private void ProcessCBPrefixedOperations(byte opCode)
        {
            if (IndexMode == OpIndexMode.None)
            {
                var opMethod = _bitOperations[opCode];
                opMethod?.Invoke(opCode);
                return;
            }

            Registers.MW = (ushort) ((IndexMode == OpIndexMode.IX ? Registers.IX : Registers.IY)
                                     + (sbyte) opCode);
            ClockP1();
            opCode = ReadMemory(Registers.PC, true);
            ClockP3();
            Registers.PC++;
            var xopMethod = _indexedBitOperations[opCode];
            xopMethod?.Invoke(opCode, Registers.MW);
        }

        /// <summary>
        /// Initializes the bit operation execution tables
        /// </summary>
        private void InitializeBitOpsExecutionTable()
        {
            _bitOperations = new Action<byte>[]
            {
                RLC_Q,    RLC_Q,    RLC_Q,    RLC_Q,    RLC_Q,    RLC_Q,    RLC_HLi,  RLC_Q,    // 00..07
                RRC_Q,    RRC_Q,    RRC_Q,    RRC_Q,    RRC_Q,    RRC_Q,    RRC_HLi,  RRC_Q,    // 08..0F
                RL_Q,     RL_Q,     RL_Q,     RL_Q,     RL_Q,     RL_Q,     RL_HLi,   RL_Q,     // 10..17
                RR_Q,     RR_Q,     RR_Q,     RR_Q,     RR_Q,     RR_Q,     RR_HLi,   RR_Q,     // 18..1F
                SLA_Q,    SLA_Q,    SLA_Q,    SLA_Q,    SLA_Q,    SLA_Q,    SLA_HLi,  SLA_Q,    // 20..27
                SRA_R,    SRA_R,    SRA_R,    SRA_R,    SRA_R,    SRA_R,    SRA_HLi,  SRA_R,    // 28..2F
                SLL_R,    SLL_R,    SLL_R,    SLL_R,    SLL_R,    SLL_R,    SLL_HLi,  SLL_R,    // 30..37
                SRL_R,    SRL_R,    SRL_R,    SRL_R,    SRL_R,    SRL_R,    SRL_HLi,  SRL_R,    // 38..3F

                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 40..47
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 48..4F
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 50..57
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 58..5F
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 60..67
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 68..6F
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 70..77
                BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_R,   BITN_HLi, BITN_R,   // 78..7F

                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // 80..87
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // 88..8F
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // 90..97
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // 98..9F
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // A0..A7
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // A8..AF
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // B0..B7
                RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_R,   RESN_HLi, RESN_R,   // B8..BF

                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // C0..C7
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // C8..CF
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // D0..D7
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // D8..DF
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // E0..E7
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // E8..EF
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R,   // F0.F7
                SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_R,   SETN_HLi, SETN_R    // F0..FF
            };
        }

        /// <summary>
        /// "RLC Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register Q are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void RLC_Q(byte opCode)
        {
            var q = (Reg8Index) (opCode & 0x07);
            int rlcVal = Registers[q];
            Registers.F = s_RlcFlags[rlcVal];
            rlcVal <<= 1;
            if ((rlcVal & 0x100) != 0)
            {
                rlcVal = (rlcVal | 0x01) & 0xFF;
            }
            Registers[q] = (byte)rlcVal;
        }

        /// <summary>
        /// "RLC (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory address specified by the contents 
        /// of HL are rotated left 1 bit position.The contents of bit 7 
        /// are copied to the Carry flag and also to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RLC_HLi(byte opCode)
        {
            var rlcVal = ReadMemory(Registers.HL, false);
            ClockP4();
            Registers.F = s_RlcFlags[rlcVal];
            rlcVal <<= 1;
            if ((rlcVal & 0x100) != 0)
            {
                rlcVal = (byte)((rlcVal | 0x01) & 0xFF);
            }
            ClockP4();
            WriteMemory(Registers.HL, rlcVal);
            ClockP3();
        }

        /// <summary>
        /// "RRC Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register Q are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void RRC_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rrcVal = Registers[q];
            Registers.F = s_RrcFlags[rrcVal];
            rrcVal = (byte) ((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            Registers[q] = (byte)rrcVal;
        }

        /// <summary>
        /// "RRC (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory address specified by the contents 
        /// of HL are rotated right 1 bit position. The contents of bit 0 
        /// are copied to the Carry flag and also to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RRC_HLi(byte opCode)
        {
            var rrcVal = ReadMemory(Registers.HL, false);
            ClockP4();
            Registers.F = s_RrcFlags[rrcVal];
            rrcVal = (byte)((rrcVal & 0x01) != 0 ? (rrcVal >> 1) | 0x80 : rrcVal >> 1);
            ClockP4();
            WriteMemory(Registers.HL, rrcVal);
            ClockP3();
        }

        /// <summary>
        /// "RL Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of Register Q are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void RL_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rlVal = Registers[q];

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
            Registers[q] = (byte)rlVal;
        }

        /// <summary>
        /// "RL (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory address specified by the contents 
        /// of HL are rotated left 1 bit position. The contents of bit 7 
        /// are copied to the Carry flag, and the previous contents of the 
        /// Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RL_HLi(byte opCode)
        {
            var rlVal = ReadMemory(Registers.HL, false);
            ClockP4();
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
            ClockP4();
            WriteMemory(Registers.HL, rlVal);
            ClockP3();
        }

        /// <summary>
        /// "RL Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register Q are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void RR_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int rrVal = Registers[q];

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
            Registers[q] = (byte)rrVal;
        }

        /// <summary>
        /// "RL (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the memory address specified by the contents 
        /// of HL are rotated right 1 bit position through the Carry flag. 
        /// The contents of bit 0 are copied to the Carry flag and the 
        /// previous contents of the Carry flag are copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RR_HLi(byte opCode)
        {
            var rrVal = ReadMemory(Registers.HL, false);
            ClockP4();
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
            ClockP4();
            WriteMemory(Registers.HL, rrVal);
            ClockP3();
        }

        /// <summary>
        /// "SLA Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register Q. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void SLA_Q(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "SLA (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents the memory address specified by the contents of HL.
        /// The contents of bit 7 are copied to the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SLA_HLi(byte opCode)
        {
            throw new NotImplementedException();
        }

        private void SETN_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SETN_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RESN_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RESN_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void BITN_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void BITN_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SRL_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SRL_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SLL_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SLL_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SRA_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SRA_R(byte obj)
        {
            throw new NotImplementedException();
        }
    }
}