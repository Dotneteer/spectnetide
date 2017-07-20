using System;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This partion of the class provides indexed CPU operations
    /// for execution with 0xDD or 0xFD prefix
    /// </summary>
    public partial class Z80Cpu
    {
        /// <summary>
        /// Indexed (0xDD or 0xFD-prefixed) operations jump table
        /// </summary>
        private Action[] _indexedOperations;

        /// <summary>
        /// Initializes the indexed operation execution tables
        /// </summary>
        private void InitializeIndexedOpsExecutionTable()
        {
            _indexedOperations = new Action[]
            {
                null,      LdBCNN,    LdBCiA,    IncBC,     IncB,      DecB,      LdBN,       Rlca,     // 00..07
                ExAF,      ADD_IX_QQ, LdABCi,    DecBC,     IncC,      DecC,      LdCN,       Rrca,     // 08..0F
                Djnz,      LdDENN,    LdDEiA,    IncDE,     IncD,      DecD,      LdDN,       Rla,      // 10..17
                JrE,       ADD_IX_QQ, LdADEi,    DecDE,     IncE,      DecE,      LdEN,       Rra,      // 18..1F
                JrNZ,      LD_IX_NN,  LD_NNi_IX, INC_IX,    INC_XH,    DEC_XH,    LD_XH_N,    Daa,      // 20..27
                JrZ,       ADD_IX_QQ, LD_IX_NNi, DEC_IX,    INC_XL,    DEC_XL,    LD_XL_N,    Cpl,      // 28..2F
                JrNC,      LdSPNN,    LdNNA,     IncSP,     INC_IXi,   DEC_IXi,   LD_IXi_NN,  Scf,      // 30..37
                JrC,       ADD_IX_QQ, LdNNiA,    DecSP,     IncA,      DecA,      LdAN,       Ccf,      // 38..3F

                null,      LdB_C,     LdB_D,     LdB_E,     LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdB_A,    // 40..47
                LdC_B,     null,      LdC_D,     LdC_E,     LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdC_A,    // 48..4F
                LdD_B,     LdD_C,     null,      LdD_E,     LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdD_A,    // 50..57
                LdE_B,     LdE_C,     LdE_D,     null,      LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   LdE_A,    // 58..5F
                LD_XH_Q,   LD_XH_Q,   LD_XH_Q,   LD_XH_Q,   null,      LD_XH_XL,  LD_Q_IXi,   LD_XH_Q,  // 60..67
                LD_XL_Q,   LD_XL_Q,   LD_XL_Q,   LD_XL_Q,   LD_XL_XH,  null,      LD_Q_IXi,   LD_XL_Q,  // 68..6F
                LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  LD_IXi_Q,  HALT,       LD_IXi_Q, // 70..77
                LdA_B,     LdA_C,     LdA_D,     LdA_E,     LD_Q_XH,   LD_Q_XL,   LD_Q_IXi,   null,     // 78..7F

                AddA_B,    AddA_C,    AddA_D,    AddA_E,    ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AddA_A,   // 80..87
                AdcA_B,    AdcA_C,    AdcA_D,    AdcA_E,    ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AdcA_A,   // 88..8F
                SubB,      SubC,      SubD,      SubE,      ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  SubA,     // 90..97
                SbcB,      SbcC,      SbcD,      SbcE,      ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  SbcA,     // 98..9F
                AndB,      AndC,      AndD,      AndE,      ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  AndA,     // A0..A7
                XorB,      XorC,      XorD,      XorE,      ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  XorA,     // A8..AF
                OrB,       OrC,       OrD,       OrE,       ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  OrA,      // B0..B7
                CpB,       CpC,       CpD,       CpE,       ALU_A_XH,  ALU_A_XL,  ALU_A_IXi,  CpA,      // B8..BF

                RetNZ,     PopBC,     JpNZ_NN,  JpNN,      CallNZ,     PushBC,    AluAN,      Rst00,    // C0..C7
                RetZ,      Ret,       JpZ_NN,   null,      CallZ,      CallNN,    AluAN,      Rst08,    // C8..CF
                RetNC,     PopDE,     JpNC_NN,  OutNA,     CallNC,     PushDE,    AluAN,      Rst10,    // D0..D7
                RetC,      Exx,       JpC_NN,   InAN,      CallC,      null,      AluAN,      Rst18,    // D8..DF
                RetPO,     POP_IX,    JpPO_NN,  EX_SPi_IX, CallPO,     PUSH_IX,   AluAN,      Rst20,    // E0..E7
                RetPE,     JP_IXi,    JpPE_NN,  ExDEHL,    CallPE,     null,      AluAN,      Rst28,    // E8..EF
                RetP,      PopAF,     JpP_NN,   Di,        CallP,      PushAF,    AluAN,      Rst30,    // F0..F7
                RetM,      LD_SP_IX,  JpM_NN,   Ei,        CallM,      null,      AluAN,      Rst38     // F8..FF
            };
        }

        /// <summary>
        /// "ADD IX,QQ" operation
        /// </summary>
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
        private void ADD_IX_QQ()
        {
            var ixVal = GetIndexReg();
            _registers.MW = (ushort)(ixVal + 1);

            var qq = (Reg16Index) ((OpCode & 0x30) >> 4);
            var qqVal = qq == Reg16Index.HL ? ixVal : _registers[qq];
            ClockP4();

            var result = qqVal + ixVal;
            _registers.F = (byte)(_registers.F & (FlagsSetMask.S | FlagsSetMask.Z | FlagsSetMask.PV));
            _registers.F |= (byte)((byte)((result >> 8) & 0xFF) & (FlagsSetMask.R5 | FlagsSetMask.R3));
            _registers.F |= (byte)((((ixVal & 0x0FFF) + (qqVal & 0x0FFF)) >> 8) & FlagsSetMask.H);
            if ((result & 0x10000) != 0) _registers.F |= FlagsSetMask.C;

            SetIndexReg((ushort)result);
            ClockP3();
        }

        /// <summary>
        /// "LD IX,NN" operation
        /// </summary>
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
        private void LD_IX_NN()
        {
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var nn = (ushort)(ReadMemory(_registers.PC) << 8 | l);
            ClockP3();
            _registers.PC++;
            SetIndexReg(nn);
        }

        /// <summary>
        /// "LD (NN),IX" operation
        /// </summary>
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
        private void LD_NNi_IX()
        {
            var ixVal = GetIndexReg();
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadMemory(_registers.PC) << 8 | l);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort)(addr + 1);
            WriteMemory(addr, (byte)ixVal);
            ClockP3();
            WriteMemory(_registers.MW, (byte)(ixVal >> 8));
            ClockP3();
        }

        /// <summary>
        /// "INC IX" operation
        /// </summary>
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
        private void INC_IX()
        {
            SetIndexReg((ushort)(GetIndexReg() + 1));
            ClockP2();
        }

        /// <summary>
        /// "INC XH" operation
        /// </summary>
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
        private void INC_XH()
        {
            var ixVal = GetIndexReg();
            var hVal = AluIncByte((byte)(ixVal >> 8));
            SetIndexReg((ushort)(hVal << 8 | (ixVal & 0xFF)));
        }

        /// <summary>
        /// "DEC XH" operation
        /// </summary>
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
        private void DEC_XH()
        {
            var ixVal = GetIndexReg();
            var hVal = AluDecByte((byte)(ixVal >> 8));
            SetIndexReg((ushort)(hVal << 8 | (ixVal & 0xFF)));
        }

        /// <summary>
        /// "LD XH,N" operation
        /// </summary>
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
        private void LD_XH_N()
        {
            var val = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            SetIndexReg((ushort)(val << 8 | (GetIndexReg() & 0xFF)));
        }

        /// <summary>
        /// "LD IX,(NN)" operation
        /// </summary>
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
        private void LD_IX_NNi()
        {
            var l = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ReadMemory(_registers.PC) << 8 | l);
            ClockP3();
            _registers.PC++;
            _registers.MW = (ushort)(addr + 1);
            ushort val = ReadMemory(addr);
            ClockP3();
            val += (ushort)(ReadMemory(_registers.MW) << 8);
            ClockP3();
            SetIndexReg(val);
        }

        /// <summary>
        /// "DEC IX" operation
        /// </summary>
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
        private void DEC_IX()
        {
            SetIndexReg((ushort)(GetIndexReg() - 1));
            ClockP2();
        }

        /// <summary>
        /// "INC XL" operation
        /// </summary>
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
        private void INC_XL()
        {
            var ixVal = GetIndexReg();
            var lVal = AluIncByte((byte)(ixVal));
            SetIndexReg((ushort)(ixVal & 0xFF00 | lVal));
        }

        /// <summary>
        /// "DEC XL" operation
        /// </summary>
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
        private void DEC_XL()
        {
            var ixVal = GetIndexReg();
            var lVal = AluDecByte((byte)(ixVal));
            SetIndexReg((ushort)(ixVal & 0xFF00 | lVal));
        }

        /// <summary>
        /// "LD XL,N" operation
        /// </summary>
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
        private void LD_XL_N()
        {
            var val = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            SetIndexReg((ushort)(GetIndexReg() & 0xFF00 | val));
        }

        /// <summary>
        /// "INC (IX+D)" operation
        /// </summary>
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
        private void INC_IXi()
        {
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
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
        private void DEC_IXi()
        {
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
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
        private void LD_IXi_NN()
        {
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var val = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP2();
            WriteMemory(addr, val);
            ClockP3();
        }

        /// <summary>
        /// "LD Q,XH" operation
        /// </summary>
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
        private void LD_Q_XH()
        {
            var q = (Reg8Index)((OpCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            _registers[q] = (byte) (ixVal >> 8);
        }

        /// <summary>
        /// "LD Q,XL" operation
        /// </summary>
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
        private void LD_Q_XL()
        {
            var q = (Reg8Index)((OpCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            _registers[q] = (byte)ixVal;
        }

        /// <summary>
        /// "LD Q,(IX+D)" operation
        /// </summary>
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
        private void LD_Q_IXi()
        {
            var q = (Reg8Index)((OpCode & 0x38) >> 3);
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            _registers[q] = ReadMemory(addr);
            ClockP3();
        }

        /// <summary>
        /// "LD XH,Q" operation
        /// </summary>
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
        private void LD_XH_Q()
        {
            var q = (Reg8Index)(OpCode & 0x07);
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(_registers[q] << 8 | ixVal & 0xFF));
        }

        /// <summary>
        /// "LD XH,XL" operation
        /// </summary>
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
        private void LD_XH_XL()
        {
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)((ixVal & 0xFF) << 8 | ixVal & 0xFF));
        }

        /// <summary>
        /// "LD XL,Q" operation
        /// </summary>
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
        private void LD_XL_Q()
        {
            var q = (Reg8Index) (OpCode & 0x07);
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(ixVal & 0xFF00 | _registers[q]));
        }

        /// <summary>
        /// "LD XL,XH" operation
        /// </summary>
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
        private void LD_XL_XH()
        {
            var ixVal = GetIndexReg();
            SetIndexReg((ushort)(ixVal & 0xFF00 | (ixVal >> 8)));
        }

        /// <summary>
        /// "LD (IX+D),Q" operation
        /// </summary>
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
        private void LD_IXi_Q()
        {
            var q = (Reg8Index)(OpCode & 0x07);
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            WriteMemory(addr, _registers[q]);
            ClockP3();
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and XH.
        /// </summary>
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
        private void ALU_A_XH()
        {
            var ix = GetIndexReg();
            var op = (OpCode & 0x38) >> 3;
            _AluAlgorithms[op]((byte)(ix >> 8), _registers.CFlag);
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and XL.
        /// </summary>
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
        private void ALU_A_XL()
        {
            var ix = GetIndexReg();
            var op = (OpCode & 0x38) >> 3;
            _AluAlgorithms[op]((byte)(ix & 0xFF), _registers.CFlag);
        }

        /// <summary>
        /// Executes one of the ADD, ADC, SUB, SBC, AND, XOR, OR, or CP
        /// operation for A and the 8/bit value at the (IX+D) address
        /// </summary>
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
        private void ALU_A_IXi()
        {
            var ixVal = GetIndexReg();
            var offset = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            var addr = (ushort)(ixVal + (sbyte)offset);
            ClockP5();
            var op = (OpCode & 0x38) >> 3;
            _AluAlgorithms[op](ReadMemory(addr), _registers.CFlag);
            ClockP3();
        }

        /// <summary>
        /// "POP IX" operation
        /// </summary>
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
        private void POP_IX()
        {
            ushort val = ReadMemory(_registers.SP);
            ClockP3();
            _registers.SP++;
            val += (ushort)(ReadMemory(_registers.SP) * 0x100);
            ClockP3();
            _registers.SP++;
            SetIndexReg(val);
        }

        /// <summary>
        /// "EX (SP),IX" operation
        /// </summary>
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
        private void EX_SPi_IX()
        {
            var spOld = _registers.SP;
            var ix = GetIndexReg();
            var l = ReadMemory(spOld);
            ClockP3();
            WriteMemory(spOld, (byte)(ix & 0xFF));
            ClockP4();
            var h = ReadMemory(++spOld);
            ClockP3();
            WriteMemory(spOld, (byte)(ix >> 8));
            ClockP4();
            _registers.MW = (ushort)(h << 8 | l);
            SetIndexReg(_registers.MW);
            ClockP1();
        }

        /// <summary>
        /// "PUSH IX" operation
        /// </summary>
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
        private void PUSH_IX()
        {
            var ix = GetIndexReg();
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte)(ix >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte)(ix & 0xFF));
            ClockP3();
        }

        /// <summary>
        /// "JP (IX)" operation
        /// </summary>
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
        private void JP_IXi()
        {
            _registers.PC = GetIndexReg();
        }

        /// <summary>
        /// "LD SP,IX" operation
        /// </summary>
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
        private void LD_SP_IX()
        {
            _registers.SP = GetIndexReg();
            ClockP2();
        }
    }
}