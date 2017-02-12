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
            opCode = ReadMemoryM1(Registers.PC);
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
                SRA_Q,    SRA_Q,    SRA_Q,    SRA_Q,    SRA_Q,    SRA_Q,    SRA_HLi,  SRA_Q,    // 28..2F
                SLL_Q,    SLL_Q,    SLL_Q,    SLL_Q,    SLL_Q,    SLL_Q,    SLL_HLi,  SLL_Q,    // 30..37
                SRL_Q,    SRL_Q,    SRL_Q,    SRL_Q,    SRL_Q,    SRL_Q,    SRL_HLi,  SRL_Q,    // 38..3F

                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 40..47
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 48..4F
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 50..57
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 58..5F
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 60..67
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 68..6F
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 70..77
                BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_Q,   BITN_HLi, BITN_Q,   // 78..7F

                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // 80..87
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // 88..8F
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // 90..97
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // 98..9F
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // A0..A7
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // A8..AF
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // B0..B7
                RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_Q,   RESN_HLi, RESN_Q,   // B8..BF

                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // C0..C7
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // C8..CF
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // D0..D7
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // D8..DF
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // E0..E7
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // E8..EF
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q,   // F0.F7
                SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_Q,   SETN_HLi, SETN_Q    // F0..FF
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
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
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
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
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
            var rlcVal = ReadMemory(Registers.HL);
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
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
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
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
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
            var rrcVal = ReadMemory(Registers.HL);
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
            var rlVal = ReadMemory(Registers.HL);
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
            var rrVal = ReadMemory(Registers.HL);
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
            var q = (Reg8Index)(opCode & 0x07);
            int slaVal = Registers[q];
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            slaVal <<= 1;
            Registers[q] = (byte)slaVal;
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
            var slaVal = ReadMemory(Registers.HL);
            Registers.F = s_RlCarry0Flags[slaVal];
            slaVal <<= 1;
            ClockP4();
            WriteMemory(Registers.HL, slaVal);
            ClockP3();
        }

        /// <summary>
        /// "SRA Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register Q. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void SRA_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int sraVal = Registers[q];
            Registers.F = s_SraFlags[sraVal];
            sraVal = (sraVal >> 1) + (sraVal & 0x80);
            Registers[q] = (byte)sraVal;
        }

        /// <summary>
        /// "SRA (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents the memory address specified by the contents of HL. 
        /// The contents of bit 0 are copied to the Carry flag and the 
        /// previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SRA_HLi(byte opCode)
        {
            var sraVal = ReadMemory(Registers.HL);
            Registers.F = s_SraFlags[sraVal];
            sraVal = (byte)((sraVal >> 1) + (sraVal & 0x80));
            ClockP4();
            WriteMemory(Registers.HL, sraVal);
            ClockP3();
        }

        /// <summary>
        /// "SLL Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register Q. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void SLL_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int sllVal = Registers[q];
            Registers.F = s_RlCarry1Flags[sllVal];
            sllVal <<= 1;
            sllVal++;
            Registers[q] = (byte)sllVal;
        }

        /// <summary>
        /// "SLL (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents the memory address specified by the contents of HL. 
        /// The contents of bit 7 are copied to the Carry flag. Bit 0 is 
        /// set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SLL_HLi(byte opCode)
        {
            var sllVal = ReadMemory(Registers.HL);
            Registers.F = s_RlCarry1Flags[sllVal];
            sllVal <<= 1;
            sllVal++;
            ClockP4();
            WriteMemory(Registers.HL, sllVal);
            ClockP3();
        }

        /// <summary>
        /// "SRL Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of register Q are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of Q.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void SRL_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            int srlVal = Registers[q];
            Registers.F = s_RrCarry0Flags[srlVal];
            srlVal >>= 1;
            Registers[q] = (byte)srlVal;
        }

        /// <summary>
        /// "SRL (HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents the memory address specified by the contents of HL 
        /// are shifted right 1 bit position. The contents of bit 0 are 
        /// copied to the Carry flag, and bit 7 is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the source byte
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SRL_HLi(byte opCode)
        {
            var srlVal = ReadMemory(Registers.HL);
            Registers.F = s_RlCarry0Flags[srlVal];
            srlVal >>= 1;
            ClockP4();
            WriteMemory(Registers.HL, srlVal);
            ClockP3();
        }

        /// <summary>
        /// "BIT N,Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction tests bit N in register Q and sets the Z 
        /// flag accordingly.
        /// 
        /// S Set if N = 7 and tested bit is set.
        /// Z is set if specified bit is 0; otherwise, it is reset.
        /// H is set.
        /// P/V is Set just like ZF flag.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 1 | N | N | N | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void BITN_Q(byte opCode)
        {
            var q = (Reg8Index) (opCode & 0x07);
            var n = (byte) ((opCode & 0x38) >> 3);
            var srcVal = Registers[q];
            var testVal = srcVal & (1 << n);
            var flags = FlagsSetMask.H
                        | (Registers.F & FlagsSetMask.C)
                        | (srcVal & (FlagsSetMask.R3 | FlagsSetMask.R5));
            if (testVal == 0)
            {
                flags |= FlagsSetMask.Z | FlagsSetMask.PV;
            }
            if (n == 7 && testVal != 0)
            {
                flags |= FlagsSetMask.S;
            }
            Registers.F = (byte)flags;
        }

        /// <summary>
        /// "BIT N,(HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// This instruction tests bit b in the memory location specified by
        /// the contents of HL and sets the Z flag accordingly.
        /// 
        /// S Set if N = 7 and tested bit is set.
        /// Z is set if specified bit is 0; otherwise, it is reset.
        /// H is set.
        /// P/V is Set just like ZF flag.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 0 | 1 | N | N | N | 1 | 1 | 0 |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4, 4 (12)
        /// </remarks>
        private void BITN_HLi(byte opCode)
        {
            var srcVal = ReadMemory(Registers.HL);
            var n = (byte)((opCode & 0x38) >> 3);
            var testVal = srcVal & (1 << n);
            var flags = FlagsSetMask.H
                        | (Registers.F & FlagsSetMask.C)
                        | (srcVal & (FlagsSetMask.R3 | FlagsSetMask.R5));
            if (testVal == 0)
            {
                flags |= FlagsSetMask.Z | FlagsSetMask.PV;
            }
            if (n == 7 && testVal != 0)
            {
                flags |= FlagsSetMask.S;
            }
            flags = (byte)((flags & (FlagsResetMask.R3 | FlagsResetMask.R5)) 
                | (Registers.MH & (FlagsSetMask.R3 | FlagsSetMask.R5)));

            Registers.F = (byte)flags;
            ClockP4();
        }

        /// <summary>
        /// "RES N,Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Bit N in register Q is reset.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 0 | N | N | N | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void RESN_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            var n = (byte)((opCode & 0x38) >> 3);
            Registers[q] &= (byte)~(1 << n);
        }

        /// <summary>
        /// "RES N,(HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Bit N in the memory location addressed by the contents of
        /// HL is reset.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 0 | N | N | N | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RESN_HLi(byte opCode)
        {
            var memVal = ReadMemory(Registers.HL);
            var n = (byte)((opCode & 0x38) >> 3);
            memVal &= (byte)~(1 << n);
            ClockP4();
            WriteMemory(Registers.HL, memVal);
            ClockP3();
        }

        /// <summary>
        /// "RES N,Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Bit N in register Q is set.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 1 | N | N | N | Q | Q | Q |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4 (4)
        /// </remarks>
        private void SETN_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            var n = (byte)((opCode & 0x38) >> 3);
            Registers[q] |= (byte)(1 << n);
        }

        /// <summary>
        /// "SET N,(HL)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// Bit N in the memory location addressed by the contents of
        /// HL is set.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | CB prefix
        /// =================================
        /// | 1 | 1 | N | N | N | 1 | 1 | 0 |
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SETN_HLi(byte opCode)
        {
            var memVal = ReadMemory(Registers.HL);
            var n = (byte)((opCode & 0x38) >> 3);
            memVal |= (byte)(1 << n);
            ClockP4();
            WriteMemory(Registers.HL, memVal);
            ClockP3();
        }
    }
}