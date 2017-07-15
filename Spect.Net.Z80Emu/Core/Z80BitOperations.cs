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
        private Action[] _bitOperations;

        /// <summary>
        /// Processes the operations with 0xCB prefix
        /// </summary>
        private void ProcessCBPrefixedOperations()
        {
            if (IndexMode == OpIndexMode.None)
            {
                var opMethod = _bitOperations[OpCode];
                opMethod?.Invoke();
                return;
            }

            Registers.MW = (ushort) ((IndexMode == OpIndexMode.IX ? Registers.IX : Registers.IY)
                                     + (sbyte) OpCode);
            ClockP1();
            OpCode = ReadMemory(Registers.PC);
            ClockP3();
            Registers.PC++;
            var xopMethod = _indexedBitOperations[OpCode];
            xopMethod?.Invoke(Registers.MW);
        }

        /// <summary>
        /// Initializes the bit operation execution tables
        /// </summary>
        private void InitializeBitOpsExecutionTable()
        {
            _bitOperations = new Action[]
            {
                RLC_B,    RLC_C,    RLC_D,    RLC_E,    RLC_H,    RLC_L,    RLC_HLi,  RLC_A,    // 00..07
                RRC_B,    RRC_C,    RRC_D,    RRC_E,    RRC_H,    RRC_L,    RRC_HLi,  RRC_A,    // 08..0F
                RL_B,     RL_C,     RL_D,     RL_E,     RL_H,     RL_L,     RL_HLi,   RL_A,     // 10..17
                RR_B,     RR_C,     RR_D,     RR_E,     RR_H,     RR_L,     RR_HLi,   RR_A,     // 18..1F
                SLA_B,    SLA_C,    SLA_D,    SLA_E,    SLA_H,    SLA_L,    SLA_HLi,  SLA_A,    // 20..27
                SRA_B,    SRA_C,    SRA_D,    SRA_E,    SRA_H,    SRA_L,    SRA_HLi,  SRA_A,    // 28..2F
                SLL_B,    SLL_C,    SLL_D,    SLL_E,    SLL_H,    SLL_L,    SLL_HLi,  SLL_A,    // 30..37
                SRL_B,    SRL_C,    SRL_D,    SRL_E,    SRL_H,    SRL_L,    SRL_HLi,  SRL_A,    // 38..3F

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
        /// "rlc b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register B are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0x00
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_B()
        {
            var rlcVal = Registers.B;
            Registers.B = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register C are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0x01
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_C()
        {
            var rlcVal = Registers.C;
            Registers.C = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register D are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0x02
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_D()
        {
            var rlcVal = Registers.D;
            Registers.D = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register E are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0x03
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_E()
        {
            var rlcVal = Registers.E;
            Registers.E = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register H are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0x04
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_H()
        {
            var rlcVal = Registers.H;
            Registers.H = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register L are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0x05
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_L()
        {
            var rlcVal = Registers.L;
            Registers.L = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rlc (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0x06
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RLC_HLi()
        {
            var rlcVal = ReadMemory(Registers.HL);
            Registers.F = s_RlcFlags[rlcVal];
            ClockP4();
            WriteMemory(Registers.HL, s_RolOpResults[rlcVal]);
            ClockP3();
        }

        /// <summary>
        /// "rlc a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register A are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag and also to bit 0.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0x07
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RLC_A()
        {
            var rlcVal = Registers.A;
            Registers.A = s_RolOpResults[rlcVal];
            Registers.F = s_RlcFlags[rlcVal];
        }

        /// <summary>
        /// "rrc b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register B are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0x08
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_B()
        {
            var rrcVal = Registers.B;
            Registers.B = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register C are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0x09
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_C()
        {
            var rrcVal = Registers.C;
            Registers.C = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register D are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0x0A
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_D()
        {
            var rrcVal = Registers.D;
            Registers.D = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register E are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0x0B
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_E()
        {
            var rrcVal = Registers.E;
            Registers.E = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register H are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0x0C
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_H()
        {
            var rrcVal = Registers.H;
            Registers.H = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register L are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0x0D
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_L()
        {
            var rrcVal = Registers.L;
            Registers.L = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rrc (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0x0E
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RRC_HLi()
        {
            var rrcVal = ReadMemory(Registers.HL);
            Registers.F = s_RrcFlags[rrcVal];
            ClockP4();
            WriteMemory(Registers.HL, s_RorOpResults[rrcVal]);
            ClockP3();
        }

        /// <summary>
        /// "rrc a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register A are rotated right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag and also 
        /// to bit 7.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0x0F
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RRC_A()
        {
            var rrcVal = Registers.A;
            Registers.A = s_RorOpResults[rrcVal];
            Registers.F = s_RrcFlags[rrcVal];
        }

        /// <summary>
        /// "rl b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register B are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0x10
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_B()
        {
            int rlVal = Registers.B;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.B = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.B = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register C are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0x11
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_C()
        {
            int rlVal = Registers.C;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.C = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.C = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register D are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0x12
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_D()
        {
            int rlVal = Registers.D;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.D = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.D = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register E are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0x13
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_E()
        {
            int rlVal = Registers.E;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.E = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.E = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register H are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0x14
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_H()
        {
            int rlVal = Registers.H;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.H = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.H = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register L are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0x15
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_L()
        {
            int rlVal = Registers.L;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.L = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.L = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rl (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0x16
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RL_HLi()
        {
            var rlVal = ReadMemory(Registers.HL);
            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                rlVal = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                rlVal = (byte)(rlVal << 1);
            }
            ClockP4();
            WriteMemory(Registers.HL, rlVal);
            ClockP3();
        }
        /// <summary>
        /// "rl a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of Register A are rotated left 1 bit position. The 
        /// contents of bit 7 are copied to the Carry flag, and the previous 
        /// contents of the Carry flag are copied to bit 0.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0x17
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RL_A()
        {
            int rlVal = Registers.A;

            if (Registers.CFlag)
            {
                Registers.F = s_RlCarry1Flags[rlVal];
                Registers.A = (byte)((rlVal << 1) + 1);
            }
            else
            {
                Registers.F = s_RlCarry0Flags[rlVal];
                Registers.A = (byte)(rlVal << 1);
            }
        }

        /// <summary>
        /// "rr b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register B are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0x18
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_B()
        {
            int rrVal = Registers.B;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.B = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.B = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register C are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0x19
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_C()
        {
            int rrVal = Registers.C;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.C = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.C = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register D are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0x1A
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_D()
        {
            int rrVal = Registers.D;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.D = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.D = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register E are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0x1B
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_E()
        {
            int rrVal = Registers.E;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.E = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.E = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register H are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0x1C
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_H()
        {
            int rrVal = Registers.H;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.H = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.H = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register L are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0x1D
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_L()
        {
            int rrVal = Registers.L;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.L = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.L = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "rr (hl)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of the memory address specified by the contents 
        /// of HL are rotated right 1 bit position through the Carry flag. 
        /// The contents of bit 0 are copied to the Carry flag and the 
        /// previous contents of the Carry flag are copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of (HL).
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0x1E
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void RR_HLi()
        {
            var rrVal = ReadMemory(Registers.HL);
            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                rrVal = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                rrVal = (byte)(rrVal >> 1);
            }
            ClockP4();
            WriteMemory(Registers.HL, rrVal);
            ClockP3();
        }

        /// <summary>
        /// "rr a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register A are rotated right 1 bit position 
        /// through the Carry flag. The contents of bit 0 are copied to the 
        /// Carry flag and the previous contents of the Carry flag are 
        /// copied to bit 7.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0x1F
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RR_A()
        {
            int rrVal = Registers.A;

            if (Registers.CFlag)
            {
                Registers.F = s_RrCarry1Flags[rrVal];
                Registers.A = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                Registers.F = s_RrCarry0Flags[rrVal];
                Registers.A = (byte)(rrVal >> 1);
            }
        }

        /// <summary>
        /// "sla b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register B. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0x20
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_B()
        {
            int slaVal = Registers.B;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.B = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register C. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 0x21
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_C()
        {
            int slaVal = Registers.C;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.C = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register D. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0x22
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_D()
        {
            int slaVal = Registers.D;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.D = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register E. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 0x23
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_E()
        {
            int slaVal = Registers.E;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.E = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register H. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 0x24
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_H()
        {
            int slaVal = Registers.H;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.H = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register L. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0x25
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_L()
        {
            int slaVal = Registers.L;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.L = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sla (hl)" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents the memory address specified by the contents of HL.
        /// The contents of bit 7 are copied to the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of (HL).
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 0x26
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SLA_HLi()
        {
            var slaVal = ReadMemory(Registers.HL);
            Registers.F = s_RlCarry0Flags[slaVal];
            slaVal <<= 1;
            ClockP4();
            WriteMemory(Registers.HL, slaVal);
            ClockP3();
        }

        /// <summary>
        /// "sla a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register A. The contents of bit 7 are copied to 
        /// the Carry flag.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 1 | 0x27
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLA_A()
        {
            int slaVal = Registers.A;
            Registers.F = s_RlCarry0Flags[(byte)slaVal];
            Registers.A = (byte)(slaVal << 1);
        }

        /// <summary>
        /// "sra b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register B. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0x28
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_B()
        {
            int sraVal = Registers.B;
            Registers.F = s_SraFlags[sraVal];
            Registers.B = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register C. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 1 | 0x29
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_C()
        {
            int sraVal = Registers.C;
            Registers.F = s_SraFlags[sraVal];
            Registers.C = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register D. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 0x2A
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_D()
        {
            int sraVal = Registers.D;
            Registers.F = s_SraFlags[sraVal];
            Registers.D = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register E. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 0x2B
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_E()
        {
            int sraVal = Registers.E;
            Registers.F = s_SraFlags[sraVal];
            Registers.E = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register H. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0x2C
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_H()
        {
            int sraVal = Registers.H;
            Registers.F = s_SraFlags[sraVal];
            Registers.H = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register L. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 0x2D
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_L()
        {
            int sraVal = Registers.L;
            Registers.F = s_SraFlags[sraVal];
            Registers.L = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sra (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 0x2E
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SRA_HLi()
        {
            var sraVal = ReadMemory(Registers.HL);
            Registers.F = s_SraFlags[sraVal];
            sraVal = (byte)((sraVal >> 1) + (sraVal & 0x80));
            ClockP4();
            WriteMemory(Registers.HL, sraVal);
            ClockP3();
        }

        /// <summary>
        /// "sra a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift right 1 bit position is performed on the 
        /// contents of register A. The contents of bit 0 are copied to the
        /// Carry flag and the previous contents of bit 7 remain unchanged.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 0 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 1 | 0x2F
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRA_A()
        {
            int sraVal = Registers.A;
            Registers.F = s_SraFlags[sraVal];
            Registers.A = (byte)((sraVal >> 1) + (sraVal & 0x80));
        }

        /// <summary>
        /// "sll b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register B. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0x30
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_B()
        {
            int sllVal = Registers.B;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.B = (byte) ((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register C. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 1 | 0x31
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_C()
        {
            int sllVal = Registers.C;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.C = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register D. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 0x32
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_D()
        {
            int sllVal = Registers.D;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.D = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register E. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 0 | 1 | 1 | 0x33
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_E()
        {
            int sllVal = Registers.E;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.E = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register H. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 0x34
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_H()
        {
            int sllVal = Registers.H;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.H = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register L. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 0x35
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_L()
        {
            int sllVal = Registers.L;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.L = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "sll (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0x36
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SLL_HLi()
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
        /// "sll a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// An arithmetic shift left 1 bit position is performed on the 
        /// contents of register A. The contents of bit 7 are copied to 
        /// the Carry flag. Bit 0 is set to 1.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 1 | 0x37
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SLL_A()
        {
            int sllVal = Registers.A;
            Registers.F = s_RlCarry1Flags[sllVal];
            Registers.A = (byte)((sllVal << 1) + 1);
        }

        /// <summary>
        /// "srl b" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register B are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0x38
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_B()
        {
            int srlVal = Registers.B;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.B = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl c" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register C are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 1 | 0x39
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_C()
        {
            int srlVal = Registers.C;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.C = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl d" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register D are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 0 | 0x3A
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_D()
        {
            int srlVal = Registers.D;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.D = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl e" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register E are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 0 | 1 | 1 | 0x3B
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_E()
        {
            int srlVal = Registers.E;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.E = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl h" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register H are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 0 | 0x3C
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_H()
        {
            int srlVal = Registers.H;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.H = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl l" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register L are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 0 | 1 | 0x3D
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_L()
        {
            int srlVal = Registers.L;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.L = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "srl (hl)" operation
        /// </summary>
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
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0 | 0x3E
        /// =================================
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void SRL_HLi()
        {
            var srlVal = ReadMemory(Registers.HL);
            Registers.F = s_RlCarry0Flags[srlVal];
            srlVal >>= 1;
            ClockP4();
            WriteMemory(Registers.HL, srlVal);
            ClockP3();
        }

        /// <summary>
        /// "srl a" operation
        /// </summary>
        /// <remarks>
        /// 
        /// The contents of register A are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 
        /// is reset.
        /// 
        /// S, Z, P/V are not affected.
        /// H, N are reset.
        /// C is data from bit 7 of the original register value.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 1 | 0xCB
        /// =================================
        /// | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0x3F
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SRL_A()
        {
            int srlVal = Registers.A;
            Registers.F = s_RrCarry0Flags[srlVal];
            Registers.A = (byte)(srlVal >> 1);
        }

        /// <summary>
        /// "BIT N,Q" operation
        /// </summary>
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
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void BITN_Q()
        {
            var q = (Reg8Index) (OpCode & 0x07);
            var n = (byte) ((OpCode & 0x38) >> 3);
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
        private void BITN_HLi()
        {
            var srcVal = ReadMemory(Registers.HL);
            var n = (byte)((OpCode & 0x38) >> 3);
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
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void RESN_Q()
        {
            var q = (Reg8Index)(OpCode & 0x07);
            var n = (byte)((OpCode & 0x38) >> 3);
            Registers[q] &= (byte)~(1 << n);
        }

        /// <summary>
        /// "RES N,(HL)" operation
        /// </summary>
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
        private void RESN_HLi()
        {
            var memVal = ReadMemory(Registers.HL);
            var n = (byte)((OpCode & 0x38) >> 3);
            memVal &= (byte)~(1 << n);
            ClockP4();
            WriteMemory(Registers.HL, memVal);
            ClockP3();
        }

        /// <summary>
        /// "RES N,Q" operation
        /// </summary>
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
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void SETN_Q()
        {
            var q = (Reg8Index)(OpCode & 0x07);
            var n = (byte)((OpCode & 0x38) >> 3);
            Registers[q] |= (byte)(1 << n);
        }

        /// <summary>
        /// "SET N,(HL)" operation
        /// </summary>
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
        private void SETN_HLi()
        {
            var memVal = ReadMemory(Registers.HL);
            var n = (byte)((OpCode & 0x38) >> 3);
            memVal |= (byte)(1 << n);
            ClockP4();
            WriteMemory(Registers.HL, memVal);
            ClockP3();
        }
    }
}