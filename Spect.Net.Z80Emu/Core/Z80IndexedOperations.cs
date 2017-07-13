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
                null,      LdBCNN,    LdBCiA,    IncBC,     IncB,      DecB,      LdBN,       Rlca,     // 00..07
                ExAF,      ADD_IX_QQ, LdABCi,    DecBC,     IncC,      DecC,      LdCN,       Rrca,     // 08..0F
                Djnz,      LdDENN,    LdDEiA,    IncDE,     IncD,      DecD,      LdDN,       Rla,      // 10..17
                JrE,       ADD_IX_QQ, LdADEi,    DecDE,     IncE,      DecE,      LdEN,       Rra,      // 18..1F
                JrXE,      LD_IX_NN,  LD_NNi_IX, INC_IX,    INC_XH,    DEC_XH,    LD_XH_N,    Daa,      // 20..27
                JrXE,      ADD_IX_QQ, LD_IX_NNi, DEC_IX,    INC_XL,    DEC_XL,    LD_XL_N,    Cpl,      // 28..2F
                JrXE,      LdSPNN,    LdNNA,     IncSP,     INC_IXi,   DEC_IXi,   LD_IXi_NN,  Scf,      // 30..37
                JrXE,      ADD_IX_QQ, LdNNiA,    DecSP,     IncA,      DecA,      LdAN,       Ccf,      // 38..3F

                null,      LdQdQs,    LdQdQs,    LdQdQs,    LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdQdQs,   // 40..47
                LdQdQs,    null,      LdQdQs,    LdQdQs,    LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdQdQs,   // 48..4F
                LdQdQs,    LdQdQs,    null,      LdQdQs,    LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdQdQs,   // 50..57
                LdQdQs,    LdQdQs,    LdQdQs,    null,      LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdQdQs,   // 58..5F
                LD_XH_Q,   LD_XH_Q,   LD_XH_Q,   LD_XH_Q,   null,      LD_XH_XL,  LD_Q_IXi,   LD_XH_Q,  // 60..67
                LD_XL_Q,   LD_XL_Q,   LD_XL_Q,   LD_XL_Q,   LD_XL_XH,  null,      LD_Q_IXi,   LD_XL_Q,  // 68..6F
                LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  HALT,       LD_IXi_Q, // 70..77
                LdQdQs,    LdQdQs,    LdQdQs,    LdQdQs,    LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   null,     // 78..7F

                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // 80..87
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // 88..8F
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // 90..97
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // 98..9F
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // A0..A7
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // A8..AF
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // B0..B7
                AluAQ,     AluAQ,     AluAQ,     AluAQ,     ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AluAQ,    // B8..BF

                RetX,      PopQQ,     JpXNN,    JpNN,      CallXNN,    PushQQ,    AluAN,      RstN,     // C0..C7
                RetX,      Ret,       JpXNN,    null,      CallXNN,    CallNN,    AluAN,      RstN,     // C8..CF
                RetX,      PopQQ,     JpXNN,    OutNA,     CallXNN,    PushQQ,    AluAN,      RstN,     // D0..D7
                RetX,      Exx,       JpXNN,    InAN,      CallXNN,    null,      AluAN,      RstN,     // D8..DF
                RetX,      POP_IX,    JpXNN,    EX_SPi_IX, CallXNN,    PUSH_IX,   AluAN,      RstN,     // E0..E7
                RetX,      JP_IXi,    JpXNN,    ExDEHL,    CallXNN,    null,      AluAN,      RstN,     // E8..EF
                RetX,      PopQQ,     JpXNN,    Di,        CallXNN,    PushQQ,    AluAN,      RstN,     // F0..F7
                RetX,      LD_SP_IX,  JpXNN,    Ei,        CallXNN,    null,      AluAN,      RstN      // F8..FF
            };
        }

        /// <summary>
        /// "ADD IX,QQ" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of QQ register pair are added to the contents of IX,
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
        /// | 0 | 0 | Q | Q | 1 | 0 | 0 | 1 | 
        /// =================================
        /// QQ: 00=BC, 01=DE, 10=IX, 11=SP
        /// T-States: 4, 4, 4, 3 (15)
        /// </remarks>
        private void ADD_IX_QQ(byte opCode)
        {
            var ixVal = GetIndexReg();
            Registers.MW = (ushort)(ixVal + 1);

            var qq = (Reg16Index) ((opCode & 0x30) >> 4);
            var qqVal = qq == Reg16Index.HL ? ixVal : Registers[qq];
            ClockP4();

            var result = qqVal + ixVal;
            Registers.F = (byte)(Registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV));
            Registers.F |= (byte)((byte)((result >> 8) & 0xFF) & (FlagsSetMask.R5 | FlagsSetMask.R3));
            Registers.F |= (byte)((((ixVal & 0x0FFF) + (qqVal & 0x0FFF)) >> 8) & FlagsSetMask.H);
            if ((result & 0x10000) != 0) Registers.F |= FlagsSetMask.C;

            SetIndexReg((ushort)result);
            ClockP3();
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
            SetIndexReg(Get16BitFromCode());
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
            var ixVal = GetIndexReg();
            var addr = Get16BitFromCode();
            Registers.MW = (ushort)(addr + 1);
            WriteMemory(addr, (byte)ixVal);
            ClockP3();
            WriteMemory(Registers.MW, (byte)(ixVal >> 8));
            ClockP3();
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
            SetIndexReg((ushort)(GetIndexReg() + 1));
            ClockP2();
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
            var ixVal = GetIndexReg();
            var hVal = AluIncByte((byte)(ixVal >> 8));
            SetIndexReg((ushort)(hVal << 8 | (ixVal & 0xFF)));
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
            var ixVal = GetIndexReg();
            var hVal = AluDecByte((byte)(ixVal >> 8));
            SetIndexReg((ushort)(hVal << 8 | (ixVal & 0xFF)));
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
            var val = Get8BitFromCode();
            SetIndexReg((ushort)(val << 8 | (GetIndexReg() & 0xFF)));
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
            var addr = Get16BitFromCode();
            Registers.MW = (ushort)(addr + 1);
            ushort val = ReadMemory(addr);
            ClockP3();
            val += (ushort)(ReadMemory(Registers.MW) << 8);
            ClockP3();
            SetIndexReg(val);
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
            SetIndexReg((ushort)(GetIndexReg() - 1));
            ClockP2();
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
            var ixVal = GetIndexReg();
            var lVal = AluIncByte((byte)(ixVal));
            SetIndexReg((ushort)(ixVal & 0xFF00 | lVal));
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
            var ixVal = GetIndexReg();
            var lVal = AluDecByte((byte)(ixVal));
            SetIndexReg((ushort)(ixVal & 0xFF00 | lVal));
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
            var val = Get8BitFromCode();
            SetIndexReg((ushort)(GetIndexReg() & 0xFF00 | val));
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
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            var memVal = ReadMemory(addr);
            ClockP3();
            memVal = AluIncByte(memVal);
            ClockP1();
            WriteMemory(addr, memVal);
            ClockP3();
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
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            var memVal = ReadMemory(addr);
            ClockP3();
            memVal = AluDecByte(memVal);
            ClockP1();
            WriteMemory(addr, memVal);
            ClockP3();
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
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var val = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP2();
            WriteMemory(addr, val);
            ClockP3();
        }

        /// <summary>
        /// "LD Q,XH" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XH are moved to register specified by Q
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | Q | Q | Q | 1 | 0 | 0 | 
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=H, 101=L, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_Q_XH(byte opCode)
        {
            var q = (Reg8Index)((opCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            Registers[q] = (byte) (ixVal >> 8);
        }

        /// <summary>
        /// "LD Q,XL" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of XL are moved to register specified by Q
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | Q | Q | Q | 1 | 0 | 0 | 
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_Q_XL(byte opCode)
        {
            var q = (Reg8Index)((opCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            Registers[q] = (byte)ixVal;
        }

        /// <summary>
        /// "LD Q,(IX+D)" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of IX summed with two's-complement displacement D
        /// is loaded to Q
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | Q | Q | Q | 1 | 1 | 0 | 
        /// =================================
        /// |            8-bit D            |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void LD_Q_IXi(byte opCode)
        {
            var q = (Reg8Index)((opCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            Registers[q] = ReadMemory(addr);
            ClockP3();
        }

        /// <summary>
        /// "LD XH,Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of Q are moved to XH
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 0 | Q | Q | Q | 
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XH_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(Registers[q] << 8 | ixVal & 0xFF));
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
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)((ixVal & 0xFF) << 8 | ixVal & 0xFF));
        }

        /// <summary>
        /// "LD XL,Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of Q are moved to XL
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 1 | 1 | 0 | 1 | Q | Q | Q | 
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4 (8)
        /// </remarks>
        private void LD_XL_Q(byte opCode)
        {
            var q = (Reg8Index) (opCode & 0x07);
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(ixVal & 0xFF00 | Registers[q]));
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
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(ixVal & 0xFF00 | (ixVal >> 8)));
        }

        /// <summary>
        /// "LD (IX+D),Q" operation
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <remarks>
        /// 
        /// The contents of Q are loaded to the memory address specified
        /// by the contents of IX summed with D, a two's-complement displacement 
        /// integer.
        /// 
        /// =================================
        /// | 1 | 1 | 0 | 1 | 1 | 1 | 0 | 1 | 
        /// =================================
        /// | 0 | 0 | 1 | 1 | 0 | Q | Q | Q | 
        /// =================================
        /// |            8-bit D            |
        /// =================================
        /// Q: 000=B, 001=C, 010=D, 011=E
        ///    100=N/A, 101=N/A, 110=N/A, 111=A
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void LD_IXi_Q(byte opCode)
        {
            var q = (Reg8Index)(opCode & 0x07);
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            WriteMemory(addr, Registers[q]);
            ClockP3();
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
            var ix = GetIndexReg();
            var op = (opCode & 0x38) >> 3;
            _AluAlgorithms[op]((byte)(ix >> 8), Registers.CFlag);
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
            var ix = GetIndexReg();
            var op = (opCode & 0x38) >> 3;
            _AluAlgorithms[op]((byte)(ix & 0xFF), Registers.CFlag);
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
        /// |            8-bit D            |
        /// =================================
        /// T-States: 4, 4, 3, 5, 3 (19)
        /// </remarks>
        private void ALU_A_IXi(byte opCode)
        {
            var ixVal = GetIndexReg();
            var offset = Get8BitFromCode();
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            var op = (opCode & 0x38) >> 3;
            _AluAlgorithms[op](ReadMemory(addr), Registers.CFlag);
            ClockP3();
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
            var val = Get16BitFromStack();
            SetIndexReg(val);
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
            var spOld = Registers.SP;
            var ix = GetIndexReg();
            var l = ReadMemory(spOld);
            ClockP3();
            WriteMemory(spOld, (byte)(ix & 0xFF));
            ClockP4();
            var h = ReadMemory(++spOld);
            ClockP3();
            WriteMemory(spOld, (byte)(ix >> 8));
            ClockP4();
            Registers.MW = (ushort)(h << 8 | l);
            SetIndexReg(Registers.MW);
            ClockP1();
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
            var ix = GetIndexReg();
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(ix >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(ix & 0xFF));
            ClockP3();
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
            Registers.PC = GetIndexReg();
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
            Registers.SP = GetIndexReg();
            ClockP2();
        }
    }
}