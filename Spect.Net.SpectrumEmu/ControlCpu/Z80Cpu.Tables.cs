/* 
 *  Copyright 2007, 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System;
using System.Linq;


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        private readonly Action<byte>[] _opcodes;
        private readonly Action<byte>[] _opcodesFx;
        private readonly Action<byte>[] _opcodesEd;
        private readonly Action<byte>[] _opcodesCb;
        private readonly Action<byte, ushort>[] _opcodesFxCb;
        private readonly Action<byte>[] _alualg;
        private readonly Func<ushort>[] _pairGetters;
        private readonly Action<ushort>[] _pairSetters;
        private readonly Func<byte>[] _regGetters;
        private readonly Action<byte>[] _regSetters;



        private Action<byte>[] CreateOpcodes()
        {
            var opcodes = new Action<byte>[256]
            {
//              0        1         2         3         4          5        6         7          8       9        A         B        C          D         E         F
                null,    LDRRNNNN, LD_RR_A,  INCRR,    INCR,      DECR,    LDRNN,    RLCA,      EXAFAF, ADDHLRR, LDA_RR_,  DECRR,   INCR,      DECR,     LDRNN,    RRCA,   // 00..0F
                DJNZ,    LDRRNNNN, LD_RR_A,  INCRR,    INCR,      DECR,    LDRNN,    RLA,       JRNN,   ADDHLRR, LDA_RR_,  DECRR,   INCR,      DECR,     LDRNN,    RRA,    // 10..1F
                JRXNN,   LDRRNNNN, LD_NN_HL, INCRR,    INCR,      DECR,    LDRNN,    DAA,       JRXNN,  ADDHLRR, LDHL_NN_, DECRR,   INCR,      DECR,     LDRNN,    CPL,    // 20..2F
                JRXNN,   LDRRNNNN, LD_NN_A,  INCRR,    INC_HL_,   DEC_HL_, LD_HL_NN, SCF,       JRXNN,  ADDHLRR, LDA_NN_,  DECRR,   INCR,      DECR,     LDRNN,    CCF,    // 30..3F

                null,    LDRdRs,   LDRdRs,   LDRdRs,   LDRdRs,    LDRdRs,  LDR_HL_,  LDRdRs,    LDRdRs, null,    LDRdRs,   LDRdRs,  LDRdRs,    LDRdRs,   LDR_HL_,  LDRdRs, // 40..4F
                LDRdRs,  LDRdRs,   null,     LDRdRs,   LDRdRs,    LDRdRs,  LDR_HL_,  LDRdRs,    LDRdRs, LDRdRs,  LDRdRs,   null,    LDRdRs,    LDRdRs,   LDR_HL_,  LDRdRs, // 50..5F
                LDRdRs,  LDRdRs,   LDRdRs,   LDRdRs,   null,      LDRdRs,  LDR_HL_,  LDRdRs,    LDRdRs, LDRdRs,  LDRdRs,   LDRdRs,  LDRdRs,    null,     LDR_HL_,  LDRdRs, // 60..6F
                LD_HL_R, LD_HL_R,  LD_HL_R,  LD_HL_R,  LD_HL_R,   LD_HL_R, HALT,     LD_HL_R,   LDRdRs, LDRdRs,  LDRdRs,   LDRdRs,  LDRdRs,    LDRdRs,   LDR_HL_,  null,   // 70..7F
    
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,     ALUAR,  ALUAR,   ALUAR,    ALUAR,   ALUAR,     ALUAR,    ALUA_HL_, ALUAR,  // 80..8F
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,     ALUAR,  ALUAR,   ALUAR,    ALUAR,   ALUAR,     ALUAR,    ALUA_HL_, ALUAR,  // 90..9F
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,     ALUAR,  ALUAR,   ALUAR,    ALUAR,   ALUAR,     ALUAR,    ALUA_HL_, ALUAR,  // A0..AF
                ALUAR,   ALUAR,    ALUAR,    ALUAR,    ALUAR,     ALUAR,   ALUA_HL_, ALUAR,     ALUAR,  ALUAR,   ALUAR,    ALUAR,   ALUAR,     ALUAR,    ALUA_HL_, ALUAR,  // B0..BF

                RETX,    POPRR,    JPXNN,    JPNNNN,   CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,     RETX,   RET,     JPXNN,    null,    CALLXNNNN, CALLNNNN, ALUAN,    RSTNN,  // C0..CF
                RETX,    POPRR,    JPXNN,    OUT_NN_A, CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,     RETX,   EXX,     JPXNN,    INA_NN_, CALLXNNNN, null,     ALUAN,    RSTNN,  // D0..DF
                RETX,    POPRR,    JPXNN,    EX_SP_HL, CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,     RETX,   JP_HL_,  JPXNN,    EXDEHL,  CALLXNNNN, null,     ALUAN,    RSTNN,  // E0..EF
                RETX,    POPRR,    JPXNN,    DI,       CALLXNNNN, PUSHRR,  ALUAN,    RSTNN,     RETX,   LDSPHL,  JPXNN,    EI,      CALLXNNNN, null,     ALUAN,    RSTNN,  // F0..FF
            };
            return opcodes;
            // patch opcodes with optimized version
            //for (var cmd = 0; cmd < 0x100; cmd++)
            //{
            //    opcodes[cmd] =
            //        EmitOpcodeLdRegReg(cmd) ??
            //        EmitOpcodeJrXdisp(cmd) ??
            //        opcodes[cmd];
            //}
            //return opcodes;
        }

        private Action<byte>[] CreateOpcodesFx()
        {
            return new Action<byte>[256]
            {
//              0           1            2            3            4           5           6             7           8        9           A           B         C          D          E             F
                null,       LDRRNNNN,    LD_RR_A,     INCRR,       INCR,       DECR,       LDRNN,        RLCA,       EXAFAF,  FX_ADDIXRR, LDA_RR_,    DECRR,    INCR,      DECR,      LDRNN,        RRCA,   // 00..0F
                DJNZ,       LDRRNNNN,    LD_RR_A,     INCRR,       INCR,       DECR,       LDRNN,        RLA,        JRNN,    FX_ADDIXRR, LDA_RR_,    DECRR,    INCR,      DECR,      LDRNN,        RRA,    // 10..1F
                JRXNN,      FX_LDIXNNNN, FX_LD_NN_IX, FX_INCIX,    FX_INCH,    FX_DECH,    FX_LDHNN,     DAA,        JRXNN,   FX_ADDIXRR, FX_LDIX_N_, FX_DECIX, FX_INCL,   FX_DECL,   FX_LDLNN,     CPL,    // 20..2F
                JRXNN,      LDRRNNNN,    LD_NN_A,     INCRR,       FX_INC_IX_, FX_DEC_IX_, FX_LD_IX_NN,  SCF,        JRXNN,   FX_ADDIXRR, LDA_NN_,    DECRR,    INCR,      DECR,      LDRNN,        CCF,    // 30..3F

                null,       LDRdRs,      LDRdRs,      LDRdRs,      FX_LDRH,    FX_LDRL,    FX_LDR_IX_,   LDRdRs,     LDRdRs,  null,       LDRdRs,     LDRdRs,   FX_LDRH,   FX_LDRL,   FX_LDR_IX_,   LDRdRs, // 40..4F
                LDRdRs,     LDRdRs,      null,        LDRdRs,      FX_LDRH,    FX_LDRL,    FX_LDR_IX_,   LDRdRs,     LDRdRs,  LDRdRs,     LDRdRs,     null,     FX_LDRH,   FX_LDRL,   FX_LDR_IX_,   LDRdRs, // 50..5F
                FX_LDHR,    FX_LDHR,     FX_LDHR,     FX_LDHR,     null,       FX_LDHL,    FX_LDR_IX_,   FX_LDHR,    FX_LDLR, FX_LDLR,    FX_LDLR,    FX_LDLR,  FX_LDLH,   null,      FX_LDR_IX_,   FX_LDLR,// 60..6F
                FX_LD_IX_R, FX_LD_IX_R,  FX_LD_IX_R,  FX_LD_IX_R,  FX_LD_IX_R, FX_LD_IX_R, HALT,         FX_LD_IX_R, LDRdRs,  LDRdRs,     LDRdRs,     LDRdRs,   FX_LDRH,   FX_LDRL,   FX_LDR_IX_,   null,   // 70..7F

                ALUAR,      ALUAR,       ALUAR,       ALUAR,       FX_ALUAXH,  FX_ALUAXL,  FX_ALUA_IX_,  ALUAR,      ALUAR,   ALUAR,      ALUAR,      ALUAR,    FX_ALUAXH, FX_ALUAXL, FX_ALUA_IX_,  ALUAR,  // 80..8F
                ALUAR,      ALUAR,       ALUAR,       ALUAR,       FX_ALUAXH,  FX_ALUAXL,  FX_ALUA_IX_,  ALUAR,      ALUAR,   ALUAR,      ALUAR,      ALUAR,    FX_ALUAXH, FX_ALUAXL, FX_ALUA_IX_,  ALUAR,  // 90..9F
                ALUAR,      ALUAR,       ALUAR,       ALUAR,       FX_ALUAXH,  FX_ALUAXL,  FX_ALUA_IX_,  ALUAR,      ALUAR,   ALUAR,      ALUAR,      ALUAR,    FX_ALUAXH, FX_ALUAXL, FX_ALUA_IX_,  ALUAR,  // A0..AF
                ALUAR,      ALUAR,       ALUAR,       ALUAR,       FX_ALUAXH,  FX_ALUAXL,  FX_ALUA_IX_,  ALUAR,      ALUAR,   ALUAR,      ALUAR,      ALUAR,    FX_ALUAXH, FX_ALUAXL, FX_ALUA_IX_,  ALUAR,  // B0..BF

                RETX,       POPRR,       JPXNN,       JPNNNN,      CALLXNNNN,  PUSHRR,     ALUAN,        RSTNN,      RETX,    RET,        JPXNN,      null,     CALLXNNNN, CALLNNNN,  ALUAN,        RSTNN,  // C0..CF
                RETX,       POPRR,       JPXNN,       OUT_NN_A,    CALLXNNNN,  PUSHRR,     ALUAN,        RSTNN,      RETX,    EXX,        JPXNN,      INA_NN_,  CALLXNNNN, null,      ALUAN,        RSTNN,  // D0..DF
                RETX,       FX_POPIX,    JPXNN,       FX_EX_SP_HL, CALLXNNNN,  FX_PUSHIX,  ALUAN,        RSTNN,      RETX,    FX_JP_HL_,  JPXNN,      EXDEHL,   CALLXNNNN, null,      ALUAN,        RSTNN,  // E0..EF
                RETX,       POPRR,       JPXNN,       DI,          CALLXNNNN,  PUSHRR,     ALUAN,        RSTNN,      RETX,    FX_LDSPHL,  JPXNN,      EI,       CALLXNNNN, null,      ALUAN,        RSTNN,  // F0..FF
            };
        }

        private Action<byte>[] CreateOpcodesEd()
        {
            return new Action<byte>[256] 
			{
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 00..0F
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 10..1F
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 20..2F
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 30..3F
				ED_INRC, ED_OUTCR, ED_SBCHLRR, ED_LD_NN_RR, ED_NEG, ED_RETN, ED_IM, ED_LDXRA,  ED_INRC, ED_OUTCR, ED_ADCHLRR, ED_LDRR_NN_, ED_NEG, ED_RETN, ED_IM, ED_LDXRA, // 40..4F
				ED_INRC, ED_OUTCR, ED_SBCHLRR, ED_LD_NN_RR, ED_NEG, ED_RETN, ED_IM, ED_LDAXR,  ED_INRC, ED_OUTCR, ED_ADCHLRR, ED_LDRR_NN_, ED_NEG, ED_RETN, ED_IM, ED_LDAXR, // 50..5F
				ED_INRC, ED_OUTCR, ED_SBCHLRR, ED_LD_NN_RR, ED_NEG, ED_RETN, ED_IM, ED_RRD,    ED_INRC, ED_OUTCR, ED_ADCHLRR, ED_LDRR_NN_, ED_NEG, ED_RETN, ED_IM, ED_RLD,   // 60..6F
				ED_INRC, ED_OUTCR, ED_SBCHLRR, ED_LD_NN_RR, ED_NEG, ED_RETN, ED_IM, null,   ED_INRC, ED_OUTCR, ED_ADCHLRR, ED_LDRR_NN_, ED_NEG, ED_RETN, ED_IM, null,  // 70..7F
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 80..8F
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // 90..9F
				ED_LDI,  ED_CPI,  ED_INI,  ED_OUTI, null, null,  null, null,  ED_LDD,  ED_CPD,  ED_IND,  ED_OUTD, null, null, null, null,             // A0..AF
				ED_LDIR, ED_CPIR, ED_INIR, ED_OTIR, null, null,  null, null,  ED_LDDR, ED_CPDR, ED_INDR, ED_OTDR, null, null, null, null,             // B0..BF
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // C0..CF
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // D0..DF
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null,             // E0..EF
				null, null, null, null, null, null, null, null,  null, null, null, null, null, null, null, null              // F0..FF
			};
        }

        private Action<byte>[] CreateOpcodesCb()
        {
            return new Action<byte>[256]
            {
//              0       1       2       3       4       5       6         7        8       9       A       B       C       D       E         F
                CB_RLC, CB_RLC, CB_RLC, CB_RLC, CB_RLC, CB_RLC, CB_RLCHL, CB_RLC,  CB_RRC, CB_RRC, CB_RRC, CB_RRC, CB_RRC, CB_RRC, CB_RRCHL, CB_RRC,  // 00..0F
                CB_RL,  CB_RL,  CB_RL,  CB_RL,  CB_RL,  CB_RL,  CB_RLHL,  CB_RL,   CB_RR,  CB_RR,  CB_RR,  CB_RR,  CB_RR,  CB_RR,  CB_RRHL,  CB_RR,   // 10..1F
                CB_SLA, CB_SLA, CB_SLA, CB_SLA, CB_SLA, CB_SLA, CB_SLAHL, CB_SLA,  CB_SRA, CB_SRA, CB_SRA, CB_SRA, CB_SRA, CB_SRA, CB_SRAHL, CB_SRA,  // 20..2F
                CB_SLL, CB_SLL, CB_SLL, CB_SLL, CB_SLL, CB_SLL, CB_SLLHL, CB_SLL,  CB_SRL, CB_SRL, CB_SRL, CB_SRL, CB_SRL, CB_SRL, CB_SRLHL, CB_SRL,  // 30..3F
                CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  // 40..4F
                CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  // 50..5F
                CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  // 60..6F
                CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BIT, CB_BITHL, CB_BIT,  // 70..7F
                CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  // 80..8F
                CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  // 90..9F
                CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  // A0..AF
                CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RES, CB_RESHL, CB_RES,  // B0..BF
                CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  // C0..CF
                CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  // D0..DF
                CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  // E0..EF
                CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SET, CB_SETHL, CB_SET,  // F0..FF
            };
        }

        private Action<byte, ushort>[] CreateOpcodesFxCb()
        {
            return new Action<byte, ushort>[256]
            {
                FXCB_RLC,   FXCB_RLC,   FXCB_RLC,   FXCB_RLC,   FXCB_RLC,   FXCB_RLC,   FXCB_RLCIX, FXCB_RLC,    FXCB_RRC,   FXCB_RRC,   FXCB_RRC,   FXCB_RRC,   FXCB_RRC,   FXCB_RRC,   FXCB_RRCIX, FXCB_RRC,    // 00..0F
                FXCB_RL,    FXCB_RL,    FXCB_RL,    FXCB_RL,    FXCB_RL,    FXCB_RL,    FXCB_RLIX,  FXCB_RL,     FXCB_RR,    FXCB_RR,    FXCB_RR,    FXCB_RR,    FXCB_RR,    FXCB_RR,    FXCB_RRIX,  FXCB_RR,     // 10..1F
                FXCB_SLA,   FXCB_SLA,   FXCB_SLA,   FXCB_SLA,   FXCB_SLA,   FXCB_SLA,   FXCB_SLAIX, FXCB_SLA,    FXCB_SRA,   FXCB_SRA,   FXCB_SRA,   FXCB_SRA,   FXCB_SRA,   FXCB_SRA,   FXCB_SRAIX, FXCB_SRA,    // 20..2F
                FXCB_SLL,   FXCB_SLL,   FXCB_SLL,   FXCB_SLL,   FXCB_SLL,   FXCB_SLL,   FXCB_SLLIX, FXCB_SLL,    FXCB_SRL,   FXCB_SRL,   FXCB_SRL,   FXCB_SRL,   FXCB_SRL,   FXCB_SRL,   FXCB_SRLIX, FXCB_SRL,    // 30..3F
                FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  // 40..4F
                FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  // 50..5F
                FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  // 60..6F
                FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX, FXCB_BITIX,  // 70..7F
                FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    // 80..8F
                FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    // 90..9F
                FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    // A0..AF
                FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RES,   FXCB_RESIX, FXCB_RES,    // B0..BF
                FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    // C0..CF
                FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    // D0..DF
                FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    // E0..EF
                FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SET,   FXCB_SETIX, FXCB_SET,    // F0..FF
            };
        }

        private Action<byte>[] CreateAluAlg()
        {
            return new Action<byte>[8] 
            { 
                ALU_ADDR, ALU_ADCR, ALU_SUBR, ALU_SBCR, 
                ALU_ANDR, ALU_XORR, ALU_ORR, ALU_CPR, 
            };
        }



        #region Optimized emitters

        //private Action<byte> EmitOpcodeLdRegReg(int cmd)
        //{
        //    if (cmd < 0x40 || cmd >= 0x80)
        //    {
        //        return null;
        //    }
        //    var rsrc = cmd & 0x07;
        //    var rdst = (cmd & 0x38) >> 3;
        //    if (rsrc == rdst)
        //    {
        //        return rsrc == 6 ? HALT : (Action<byte>)null;
        //    }
        //    var getter = rsrc != 6 ? regs.CreateRegGetter(rsrc) :
        //        new Func<byte>(() =>
        //        {
        //            // LD R,(HL)
        //            // 7T (4, 3)
        //            var value = RDMEM(regs.HL); Tact += 3;
        //            return value;
        //        });
        //    var setter = rdst != 6 ? regs.CreateRegSetter(rdst) :
        //        new Action<byte>(arg =>
        //        {
        //            // LD (HL),R
        //            // 7T (4, 3)
        //            WRMEM(regs.HL, arg); Tact += 3;
        //        });
        //    return arg => setter(getter());
        //}

        //private Action<byte> EmitOpcodeJrXdisp(int cmd)
        //{
        //    if (cmd < 0x20 || cmd >= 0x40 || (cmd & 7) != 0)
        //    {
        //        return null;
        //    }
        //    // JR x,disp
        //    // false => 7T (4, 3)
        //    // true  => 12 (4, 3, 5)
        //    var cond = (cmd & 0x18) >> 3;
        //    var mask = s_conds[cond >> 1];
        //    var xor = (cond & 1) == 0 ? -1 : 0;
        //    return new Action<byte>(arg =>
        //        {
        //            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
        //            regs.PC++;
        //            var f = (regs.AF ^ xor) & mask;
        //            if (f == 0)
        //            {
        //                return;
        //            }
        //            RDNOMREQ(regs.PC); Tact++;
        //            RDNOMREQ(regs.PC); Tact++;
        //            RDNOMREQ(regs.PC); Tact++;
        //            RDNOMREQ(regs.PC); Tact++;
        //            RDNOMREQ(regs.PC); Tact++;
        //            regs.MW = (ushort)(regs.PC + drel);
        //            regs.PC = regs.MW;
        //        });
        //}

        #endregion Optimized emitters
    }
}
