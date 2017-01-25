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
            var opMethod = IndexMode == OpIndexMode.None
                ? _bitOperations[opCode]
                : _indexedBitOperations[opCode];
            opMethod?.Invoke(opCode);

        }

        private void InitializeBitOpsExecutionTable()
        {
            _bitOperations = new Action<byte>[]
            {
                RLC_R,    RLC_R,    RLC_R,    RLC_R,    RLC_R,    RLC_R,    RLC_HLi,  RLC_R,    // 00..07
                RRC_R,    RRC_R,    RRC_R,    RRC_R,    RRC_R,    RRC_R,    RRC_HLi,  RRC_R,    // 08..0F
                RL_R,     RL_R,     RL_R,     RL_R,     RL_R,     RL_R,     RL_HLi,   RL_R,     // 10..17
                RR_R,     RR_R,     RR_R,     RR_R,     RR_R,     RR_R,     RR_HLi,   RR_R,     // 18..1F
                SLA_R,    SLA_R,    SLA_R,    SLA_R,    SLA_R,    SLA_R,    SLA_HLi,  SLA_R,    // 20..27
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

        private void SLA_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SLA_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RR_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RR_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RL_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RL_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RRC_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RRC_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RLC_HLi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RLC_R(byte obj)
        {
            throw new NotImplementedException();
        }
    }
}