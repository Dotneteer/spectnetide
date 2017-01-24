using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides indexed CPU operations
    /// for execution with 0xDD or 0xFD prefix
    /// </summary>
    public partial class Z80
    {
        private Action<byte>[] _indexedOperations;

        private void InitializeIndexedExecutionTables()
        {
            _indexedOperations = new Action<byte>[]
            {
                null,      LD_RR_NN,  LD_RR_A,   INC_RR,    INC_R,     DEC_R,     LD_R_N,    RLCA,      // 00..07
                EX_AF,     ADD_IX_RR, LD_A_RRi,  DEC_RR,    INC_R,     DEC_R,     LD_R_N,    RRCA,      // 08..0F
                DJNZ,      LD_RR_NN,  LD_RR_A,   INC_RR,    INC_R,     DEC_R,     LD_R_N,    RLA,       // 10..17
                JR_E,      ADD_IX_RR, LD_A_RRi,  DEC_RR,    INC_R,     DEC_R,     LD_R_N,    RRA,       // 18..1F
                JR_X_E,    LD_IX_NN,  LD_NNi_IX, INC_IX,    INC_XH,    DEC_XH,    LD_XH_N,   DAA,       // 20..27
                JR_X_E,    ADD_IX_RR, LD_IX_NNi, DEC_IX,    INC_XL,    DEC_XL,    LD_XL_N,   CPL,       // 28..2F
                JR_X_E,    LD_RR_NN,  LD_NN_A,   INC_RR,    INC_IXi,   DEC_IXi,   LD_IXi_NN, SCF,       // 30..37
                JR_X_E,    ADD_IX_RR, LD_A_NNi,  DEC_RR,    INC_R,     DEC_R,     LD_R_N,    CCF,       // 38..3F
                // ---> Correct indexed operations table down 
                null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 40..47
                LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 48..4F
                LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 50..57
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 58..5F
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_R_HLi,   LD_Rd_Rs, // 60..67
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_R_HLi,   LD_Rd_Rs, // 68..6F
                LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  LD_HLi_R,  HALT,       LD_HLi_R, // 70..77
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_HLi,   null,     // 78..7F

                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 80..87
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 88..8F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 90..97
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // 98..9F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // A0..A7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // A8..AF
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // B0..B7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_HLi,  ALU_A_R,  // B8..BF

                RET_X,     POP_RR,    JP_X_NN,   JP_NN,     CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,   // C0..C7
                RET_X,     RET,       JP_X_NN,   null,      CALL_X_NN, CALL_NN,   ALU_A_N,    RST_N,   // C8..CF
                RET_X,     POP_RR,    JP_X_NN,   OUT_NN_A,  CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,   // D0..D7
                RET_X,     EXX,       JP_X_NN,   IN_A_NN,   CALL_X_NN, null,      ALU_A_N,    RST_N,   // D8..DF
                RET_X,     POP_RR,    JP_X_NN,   EX_SPi_HL, CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,   // E0..E7
                RET_X,     JP_HL,     JP_X_NN,   EX_DE_HL,  CALL_X_NN, null,      ALU_A_N,    RST_N,   // E8..EF
                RET_X,     POP_RR,    JP_X_NN,   DI,        CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,   // F0..F7
                RET_X,     LD_SP_HL,  JP_X_NN,   EI,        CALL_X_NN, null,      ALU_A_N,    RST_N,   // F8..FF
            };
        }

        private void LD_IXi_NN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void DEC_IXi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INC_IXi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_XL_N(byte obj)
        {
            throw new NotImplementedException();
        }

        private void DEC_XL(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INC_XL(byte obj)
        {
            throw new NotImplementedException();
        }

        private void DEC_IX(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_IX_NNi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_XH_N(byte obj)
        {
            throw new NotImplementedException();
        }

        private void DEC_XH(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INC_XH(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INC_IX(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_NNi_IX(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_IX_NN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void ADD_IX_RR(byte obj)
        {
            throw new NotImplementedException();
        }
    }
}