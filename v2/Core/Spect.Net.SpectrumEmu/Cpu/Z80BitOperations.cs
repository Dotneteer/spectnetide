// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This partion of the class provides bit operations
    /// (OpCodes used with the 0xCB prefix)
    /// </summary>
    public partial class Z80Cpu
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
            if (_indexMode == OpIndexMode.None)
            {
                var opMethod = _bitOperations[_opCode];
                opMethod?.Invoke();
                return;
            }

            _registers.WZ = (ushort) ((_indexMode == OpIndexMode.IX ? _registers.IX : _registers.IY)
                                     + (sbyte) _opCode);
            if (!UseGateArrayContention)
            {
                ReadMemory((ushort) (_registers.PC - 1));
            }
            ClockP1();
            _opCode = ReadCodeMemory();
            ClockP3();
            _registers.PC++;
            var xopMethod = _indexedBitOperations[_opCode];
            xopMethod?.Invoke(_registers.WZ);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_B()
        {
            var rlcVal = _registers.B;
            _registers.B = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_C()
        {
            var rlcVal = _registers.C;
            _registers.C = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_D()
        {
            var rlcVal = _registers.D;
            _registers.D = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_E()
        {
            var rlcVal = _registers.E;
            _registers.E = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_H()
        {
            var rlcVal = _registers.H;
            _registers.H = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_L()
        {
            var rlcVal = _registers.L;
            _registers.L = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void RLC_HLi()
        {
            var rlcVal = ReadMemory(_registers.HL);
            _registers.F = s_RlcFlags[rlcVal];
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, s_RolOpResults[rlcVal]);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RLC_A()
        {
            var rlcVal = _registers.A;
            _registers.A = s_RolOpResults[rlcVal];
            _registers.F = s_RlcFlags[rlcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_B()
        {
            var rrcVal = _registers.B;
            _registers.B = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_C()
        {
            var rrcVal = _registers.C;
            _registers.C = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_D()
        {
            var rrcVal = _registers.D;
            _registers.D = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_E()
        {
            var rrcVal = _registers.E;
            _registers.E = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_H()
        {
            var rrcVal = _registers.H;
            _registers.H = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_L()
        {
            var rrcVal = _registers.L;
            _registers.L = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void RRC_HLi()
        {
            var rrcVal = ReadMemory(_registers.HL);
            _registers.F = s_RrcFlags[rrcVal];
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, s_RorOpResults[rrcVal]);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RRC_A()
        {
            var rrcVal = _registers.A;
            _registers.A = s_RorOpResults[rrcVal];
            _registers.F = s_RrcFlags[rrcVal];
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_B()
        {
            int rlVal = _registers.B;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.B = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.B = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_C()
        {
            int rlVal = _registers.C;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.C = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.C = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_D()
        {
            int rlVal = _registers.D;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.D = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.D = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_E()
        {
            int rlVal = _registers.E;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.E = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.E = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_H()
        {
            int rlVal = _registers.H;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.H = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.H = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_L()
        {
            int rlVal = _registers.L;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.L = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.L = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void RL_HLi()
        {
            var rlVal = ReadMemory(_registers.HL);
            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                rlVal = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                rlVal = (byte)(rlVal << 1);
            }
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, rlVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RL_A()
        {
            int rlVal = _registers.A;

            if (_registers.CFlag)
            {
                _registers.F = s_RlCarry1Flags[rlVal];
                _registers.A = (byte)((rlVal << 1) + 1);
            }
            else
            {
                _registers.F = s_RlCarry0Flags[rlVal];
                _registers.A = (byte)(rlVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_B()
        {
            int rrVal = _registers.B;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.B = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.B = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_C()
        {
            int rrVal = _registers.C;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.C = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.C = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_D()
        {
            int rrVal = _registers.D;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.D = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.D = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_E()
        {
            int rrVal = _registers.E;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.E = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.E = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_H()
        {
            int rrVal = _registers.H;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.H = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.H = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_L()
        {
            int rrVal = _registers.L;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.L = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.L = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void RR_HLi()
        {
            var rrVal = ReadMemory(_registers.HL);
            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                rrVal = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                rrVal = (byte)(rrVal >> 1);
            }
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, rrVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RR_A()
        {
            int rrVal = _registers.A;

            if (_registers.CFlag)
            {
                _registers.F = s_RrCarry1Flags[rrVal];
                _registers.A = (byte)((rrVal >> 1) + 0x80);
            }
            else
            {
                _registers.F = s_RrCarry0Flags[rrVal];
                _registers.A = (byte)(rrVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_B()
        {
            int slaVal = _registers.B;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.B = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_C()
        {
            int slaVal = _registers.C;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.C = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_D()
        {
            int slaVal = _registers.D;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.D = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_E()
        {
            int slaVal = _registers.E;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.E = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_H()
        {
            int slaVal = _registers.H;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.H = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_L()
        {
            int slaVal = _registers.L;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.L = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void SLA_HLi()
        {
            var slaVal = ReadMemory(_registers.HL);
            _registers.F = s_RlCarry0Flags[slaVal];
            slaVal <<= 1;
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, slaVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLA_A()
        {
            int slaVal = _registers.A;
            _registers.F = s_RlCarry0Flags[(byte)slaVal];
            _registers.A = (byte)(slaVal << 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_B()
        {
            int sraVal = _registers.B;
            _registers.F = s_SraFlags[sraVal];
            _registers.B = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_C()
        {
            int sraVal = _registers.C;
            _registers.F = s_SraFlags[sraVal];
            _registers.C = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_D()
        {
            int sraVal = _registers.D;
            _registers.F = s_SraFlags[sraVal];
            _registers.D = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_E()
        {
            int sraVal = _registers.E;
            _registers.F = s_SraFlags[sraVal];
            _registers.E = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_H()
        {
            int sraVal = _registers.H;
            _registers.F = s_SraFlags[sraVal];
            _registers.H = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_L()
        {
            int sraVal = _registers.L;
            _registers.F = s_SraFlags[sraVal];
            _registers.L = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void SRA_HLi()
        {
            var sraVal = ReadMemory(_registers.HL);
            _registers.F = s_SraFlags[sraVal];
            sraVal = (byte)((sraVal >> 1) + (sraVal & 0x80));
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, sraVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRA_A()
        {
            int sraVal = _registers.A;
            _registers.F = s_SraFlags[sraVal];
            _registers.A = (byte)((sraVal >> 1) + (sraVal & 0x80));
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_B()
        {
            int sllVal = _registers.B;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.B = (byte) ((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_C()
        {
            int sllVal = _registers.C;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.C = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_D()
        {
            int sllVal = _registers.D;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.D = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_E()
        {
            int sllVal = _registers.E;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.E = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_H()
        {
            int sllVal = _registers.H;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.H = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_L()
        {
            int sllVal = _registers.L;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.L = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void SLL_HLi()
        {
            var sllVal = ReadMemory(_registers.HL);
            _registers.F = s_RlCarry1Flags[sllVal];
            sllVal <<= 1;
            sllVal++;
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, sllVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SLL_A()
        {
            int sllVal = _registers.A;
            _registers.F = s_RlCarry1Flags[sllVal];
            _registers.A = (byte)((sllVal << 1) + 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_B()
        {
            int srlVal = _registers.B;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.B = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_C()
        {
            int srlVal = _registers.C;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.C = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_D()
        {
            int srlVal = _registers.D;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.D = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_E()
        {
            int srlVal = _registers.E;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.E = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_H()
        {
            int srlVal = _registers.H;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.H = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_L()
        {
            int srlVal = _registers.L;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.L = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void SRL_HLi()
        {
            var srlVal = ReadMemory(_registers.HL);
            _registers.F = s_RlCarry0Flags[srlVal];
            srlVal >>= 1;
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, srlVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SRL_A()
        {
            int srlVal = _registers.A;
            _registers.F = s_RrCarry0Flags[srlVal];
            _registers.A = (byte)(srlVal >> 1);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void BITN_Q()
        {
            var q = (Reg8Index) (_opCode & 0x07);
            var n = (byte) ((_opCode & 0x38) >> 3);
            var srcVal = _registers[q];
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4
        /// </remarks>
        private void BITN_HLi()
        {
            var srcVal = ReadMemory(_registers.HL);
            var n = (byte)((_opCode & 0x38) >> 3);
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
            flags = (byte)((flags & (FlagsResetMask.R3 | FlagsResetMask.R5)) 
                | (_registers.WZh & (FlagsSetMask.R3 | FlagsSetMask.R5)));

            _registers.F = (byte)flags;
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void RESN_Q()
        {
            var q = (Reg8Index)(_opCode & 0x07);
            var n = (byte)((_opCode & 0x38) >> 3);
            _registers[q] &= (byte)~(1 << n);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void RESN_HLi()
        {
            var memVal = ReadMemory(_registers.HL);
            var n = (byte)((_opCode & 0x38) >> 3);
            memVal &= (byte)~(1 << n);
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, memVal);
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
        /// Contention breakdown: pc:4,pc+1:4
        /// </remarks>
        private void SETN_Q()
        {
            var q = (Reg8Index)(_opCode & 0x07);
            var n = (byte)((_opCode & 0x38) >> 3);
            _registers[q] |= (byte)(1 << n);
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
        /// Contention breakdown: pc:4,pc+1:4,hl:3,hl:1,hl(write):3
        /// Gate array contention breakdown: pc:4,pc+1:4,hl:4,hl(write):3
        /// </remarks>
        private void SETN_HLi()
        {
            var memVal = ReadMemory(_registers.HL);
            var n = (byte)((_opCode & 0x38) >> 3);
            memVal |= (byte)(1 << n);
            if (UseGateArrayContention)
            {
                ClockP4();
            }
            else
            {
                ClockP3();
                ReadMemory(_registers.HL);
                ClockP1();
            }
            WriteMemory(_registers.HL, memVal);
            ClockP3();
        }
    }
}