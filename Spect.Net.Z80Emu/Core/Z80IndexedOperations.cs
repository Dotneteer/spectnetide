using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides indexed CPU operations
    /// for execution with 0xDD or 0xFD prefix
    /// </summary>
    public partial class Z80
    {
        /// <summary>
        /// Indexed (0xDD or 0xFD-prefixed) operations jump table
        /// </summary>
        private Action<byte>[] _indexedOperations;

        /// <summary>
        /// Initializes the indexed operation execution tables
        /// </summary>
        private void InitializeIndexedOpsExecutionTable()
        {
            _indexedOperations = new Action<byte>[]
            {
                null,      LD_QQ_NN,  LD_QQi_A,   INC_QQ,    INC_Q,     DEC_Q,     LD_Q_N,    RLCA,      // 00..07
                EX_AF,     ADD_IX_RR, LD_A_RRi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    RRCA,      // 08..0F
                DJNZ,      LD_QQ_NN,  LD_QQi_A,   INC_QQ,    INC_Q,     DEC_Q,     LD_Q_N,    RLA,       // 10..17
                JR_E,      ADD_IX_RR, LD_A_RRi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    RRA,       // 18..1F
                JR_X_E,    LD_IX_NN,  LD_NNi_IX, INC_IX,    INC_XH,    DEC_XH,    LD_XH_N,   DAA,       // 20..27
                JR_X_E,    ADD_IX_RR, LD_IX_NNi, DEC_IX,    INC_XL,    DEC_XL,    LD_XL_N,   CPL,       // 28..2F
                JR_X_E,    LD_QQ_NN,  LD_NN_A,   INC_QQ,    INC_IXi,   DEC_IXi,   LD_IXi_NN, SCF,       // 30..37
                JR_X_E,    ADD_IX_RR, LD_A_NNi,  DEC_RR,    INC_Q,     DEC_Q,     LD_Q_N,    CCF,       // 38..3F

                null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_XH,   LD_R_XL,   LD_R_IXi,  LD_Rd_Rs,  // 40..47
                LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_Rd_Rs,  LD_R_XH,   LD_R_XL,   LD_R_IXi,  LD_Rd_Rs,  // 48..4F
                LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_Rd_Rs,  LD_R_XH,   LD_R_XL,   LD_R_IXi,  LD_Rd_Rs,  // 50..57
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  null,      LD_R_XH,   LD_R_XL,   LD_R_IXi,  LD_Rd_Rs,  // 58..5F
                LD_XH_R,   LD_XH_R,   LD_XH_R,   LD_XH_R,   null,      LD_XH_XL,  LD_R_IXi,  LD_XH_R,   // 60..67
                LD_XL_R,   LD_XL_R,   LD_XL_R,   LD_XL_R,   LD_XL_XH,  null,      LD_R_IXi,  LD_XL_R,   // 68..6F
                LD_IXi_R,  LD_IXi_R,  LD_IXi_R,  LD_IXi_R,  LD_IXi_R,  LD_IXi_R,  HALT,      LD_IXi_R,  // 70..77
                LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_Rd_Rs,  LD_R_XH,   LD_R_XL,   LD_R_IXi,  null,      // 78..7F

                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // 80..87
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // 88..8F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // 90..97
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // 98..9F
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // A0..A7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // A8..AF
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // B0..B7
                ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_R,   ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  ALU_A_R,  // B8..BF

                RET_X,     POP_RR,    JP_X_NN,   JP_NN,     CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // C0..C7
                RET_X,     RET,       JP_X_NN,   null,      CALL_X_NN, CALL_NN,   ALU_A_N,    RST_N,    // C8..CF
                RET_X,     POP_RR,    JP_X_NN,   OUT_NN_A,  CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // D0..D7
                RET_X,     EXX,       JP_X_NN,   IN_A_NN,   CALL_X_NN, null,      ALU_A_N,    RST_N,    // D8..DF
                RET_X,     POP_IX,    JP_X_NN,   EX_SPi_IX, CALL_X_NN, PUSH_IX,   ALU_A_N,    RST_N,    // E0..E7
                RET_X,     JP_IXi,    JP_X_NN,   EX_DE_HL,  CALL_X_NN, null,      ALU_A_N,    RST_N,    // E8..EF
                RET_X,     POP_RR,    JP_X_NN,   DI,        CALL_X_NN, PUSH_RR,   ALU_A_N,    RST_N,    // F0..F7
                RET_X,     LD_SP_IX,  JP_X_NN,   EI,        CALL_X_NN, null,      ALU_A_N,    RST_N,    // F8..FF
            };
        }

        /// <summary>
        /// "ADD IX,RR" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of RR register pair are added to the contents of IX,
        /// and the results are stored in IX.
        /// 
        /// S, Z, P/V is not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | R | R | 1 | 0 | 0 | 1 | 
        /// =================================
        /// RR: 00=BC, 01=DE, 10=IX, 11=SP
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void ADD_IX_RR(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD IX,NN" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 16-bit integer is loaded to IX.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 1 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 4, 3, 3 (14)
        /// </remarks>
        private void LD_IX_NN(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (NN),IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The low-order byte in IX is loaded to memory address (NN); 
        /// the upper order byte is loaded to the next highest address 
        /// (NN + 1).
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 
        /// =================================
        /// |           8-bit L             |
        /// =================================
        /// |           8-bit H             |
        /// =================================
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// </remarks>
        private void LD_NNi_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "INC IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX are incremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4, 6 (10)
        /// </remarks>
        private void INC_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "INC XH" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XH are incremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void INC_XH(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC XH" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XH are decremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void DEC_XH(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XH,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to XH
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 4, 3, (11)
        /// </remarks>
        private void LD_XH_N(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD IX,(NN)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of the address (NN) are loaded to the low-order
        /// portion of IX, and the contents of the next highest memory address
        /// (NN + 1) are loaded to the high-orderp ortion of IX.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 
        /// =================================
        /// |            8-bit L            |
        /// =================================
        /// |            8-bit H            |
        /// =================================
        /// T-States: 4, 4, 3, 3, 3, 3 (20)
        /// </remarks>
        private void LD_IX_NNi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX are decremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4, 6 (10)
        /// </remarks>
        private void DEC_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "INC XL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are incremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void INC_XL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC XL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are decremented.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void DEC_XL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XL,N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 8-bit integer N is loaded to XL
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 0 | 1 | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 4, 3, (11)
        /// </remarks>
        private void LD_XL_N(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "INC (IX+D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX are added to the two's-complement displacement
        /// integer, D, to point to an address in memory. The contents of this 
        /// address are then incremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if (IX+D) was 0x7F before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 0 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void INC_IXi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "DEC (IX+D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX are added to the two's-complement displacement
        /// integer, D, to point to an address in memory. The contents of this 
        /// address are then decremented.
        /// 
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4, otherwise, it is reset.
        /// P/V is set if (IX+D) was 0x80 before operation; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 0 | 1 | 
        /// =================================
        /// |            8-bit              |
        /// =================================
        /// T-States: 4, 4, 3, 5, 4, 3 (23)
        /// </remarks>
        private void DEC_IXi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (IX+D),N" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The n operand is loaded to the memory address specified by the sum
        /// of IX and the two's complement displacement operand D.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit D            |
        /// =================================
        /// |            8-bit N            |
        /// =================================
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void LD_IXi_NN(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD R,XH" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XH are moved to register specified by R
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | R | R | R | 1 | 0 | 0 | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_R_XH(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD R,XL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are moved to register specified by R
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | R | R | R | 1 | 0 | 0 | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_R_XL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD R,(IX+D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX summed with two's-complement displacement D
        /// is loaded to R
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | R | R | R | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit D            |
        /// =================================
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void LD_R_IXi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XH,R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of R are moved to XH
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | R | R | R | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XH_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XH,XL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are moved to XH
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XH_XL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XL,R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of R are moved to XL
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | R | R | R | 
        /// =================================
        /// R: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XL_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD XL,XH" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are moved to XH
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | 1 | 0 | 0 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XL_XH(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD (IX+D),R" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of R are loaded to the memory address specified
        /// by the contents of IX summed with D, a two's-complement displacement 
        /// integer.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | R | R | R | 
        /// =================================
        /// |            8-bit D            |
        /// =================================
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void LD_IXi_R(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and XH.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 0 | A | A | A | 1 | 0 | 0 | 
        /// =================================
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void ALU_A_XH(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and XL.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 0 | A | A | A | 1 | 0 | 0 | 
        /// =================================
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void ALU_A_XL(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and the 8/bit value at the (IX+D) address
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The flags are set according to the ALU operation rules.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 0 | A | A | A | 1 | 1 | 0 | 
        /// =================================
        /// A: 000=ADD, 001=ADC, 010=SUB, 011=SBC,
        ///    100=AND, 101=XOR, 110=OR, 111=CP 
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void ALU_A_IXi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "POP IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The top two bytes of the external memory last-in, first-out (LIFO)
        /// stack are popped to IX. SP holds the 16-bit address of the current 
        /// top of the Stack. This instruction first loads to the low-order 
        /// portion of IX the byte at the memory location corresponding to the 
        /// contents of SP; then SP is incremented and the contents of the 
        /// corresponding adjacent memory location are loaded to the high-order
        /// portion of IX. SP is incremented again.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 4, 3, 3 (14)
        /// </remarks>
        private void POP_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "EX (SP),IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The low-order byte in IX is exchanged with the contents of the 
        /// memory address specified by the contents of SP, and the 
        /// high-order byte of IX is exchanged with the next highest memory
        /// address (SP+1).
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 
        /// =================================
        /// T-States: 4, 4, 3, 4, 3, 5 (23)
        /// </remarks>
        private void EX_SPi_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "PUSH IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX are pushed to the external memory last-in, 
        /// first-out (LIFO) stack. SP holds the 16-bit address of the 
        /// current top of the Stack. This instruction first decrements SP 
        /// and loads the high-order byte of IX to the memory address 
        /// specified by SP; then decrements SP again and loads the low-order
        /// byte to the memory location corresponding to this new address
        /// in SP.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 0 | 1 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 5, 3, 3 (15)
        /// </remarks>
        private void PUSH_IX(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "JP (IX)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The PC is loaded with the contents of IX. The next instruction 
        /// is fetched from the location designated by the new contents of PC.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 1 | 1 | 0 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void JP_IXi(byte opCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "LD SP,IX" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The 2-byte contents of IX are loaded to SP.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 1 | 
        /// =================================
        /// T-States: 4, 6 (10)
        /// </remarks>
        private void LD_SP_IX(byte opCode)
        {
            throw new NotImplementedException();
        }
    }
}