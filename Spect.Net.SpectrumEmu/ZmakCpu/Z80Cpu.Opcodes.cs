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
 *  Description: Z80 CPU Emulator [direct opcode part]
 *  Date: 13.04.2007
 * 
 */


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        #region direct/DD/FD

        private void INA_NN_(byte cmd)      // IN A,(N) [11T] 
        {
            // 11T (4, 3, 4)

            regs.MW = RDMEM(regs.PC++); Tact += 3;
            regs.MW += (ushort)(regs.A << 8);

            regs.A = RDPORT(regs.MW); Tact += 4;
            regs.MW++;
        }

        private void OUT_NN_A(byte cmd)     // OUT (N),A [11T]+ 
        {
            // 11T (4, 3, 4)

            regs.MW = RDMEM(regs.PC++); Tact += 3;
            regs.MW += (ushort)(regs.A << 8);

            WRPORT(regs.MW, regs.A); Tact += 4;
            regs.ML++;
        }

        private void DI(byte cmd)
        {
            IFF1 = false;
            IFF2 = false;
        }

        private void EI(byte cmd)
        {
            IFF1 = true;
            IFF2 = true;
            BINT = true;
        }

        private void LDSPHL(byte cmd)       // LD SP,HL 
        {
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            regs.SP = regs.HL;
        }

        private void EX_SP_HL(byte cmd)     // EX (SP),HL
        {
            // 19T (4, 3, 4, 3, 5)

            ushort tmpsp = regs.SP;
            regs.MW = RDMEM(tmpsp); Tact += 3;
            tmpsp++;

            regs.MW += (ushort)(RDMEM(tmpsp) * 0x100); Tact += 3;
            RDNOMREQ(tmpsp); Tact += 1;

            WRMEM(tmpsp, regs.H); Tact += 3;
            tmpsp--;

            WRMEM(tmpsp, regs.L); Tact += 3;
            WRNOMREQ(tmpsp); Tact++;
            WRNOMREQ(tmpsp); Tact++;
            regs.HL = regs.MW;
        }

        private void JP_HL_(byte cmd)       // JP (HL) 
        {
            regs.PC = regs.HL;
        }

        private void EXDEHL(byte cmd)       // EX DE,HL 
        {
            ushort tmp;
            tmp = regs.HL;      // ix префикс не действует!
            regs.HL = regs.DE;
            regs.DE = tmp;
        }

        private void EXAFAF(byte cmd)       // EX AF,AF' 
        {
            var tmp = regs.AF;
            regs.AF = regs._AF;
            regs._AF = tmp;
        }

        private void EXX(byte cmd)          // EXX 
        {
            var tmp = regs.BC;
            regs.BC = regs._BC;
            regs._BC = tmp;
            tmp = regs.DE;
            regs.DE = regs._DE;
            regs._DE = tmp;
            tmp = regs.HL;
            regs.HL = regs._HL;
            regs._HL = tmp;
        }

        #endregion


        #region logical

        private void RLCA(byte cmd)
        {
            regs.F = (byte)(CpuTables.Rlcaf[regs.A] | (regs.F & CpuFlags.SZP));
            int x = regs.A;
            x <<= 1;
            if ((x & 0x100) != 0) x |= 0x01;
            regs.A = (byte)x;
        }

        private void RRCA(byte cmd)
        {
            regs.F = (byte)(CpuTables.Rrcaf[regs.A] | (regs.F & CpuFlags.SZP));
            int x = regs.A;
            if ((x & 0x01) != 0) x = (x >> 1) | 0x80;
            else x >>= 1;
            regs.A = (byte)x;
        }

        private void RLA(byte cmd)
        {
            var carry = (regs.F & CpuFlags.C) != 0;
            regs.F = (byte)(CpuTables.Rlcaf[regs.A] | (regs.F & CpuFlags.SZP)); // use same table with rlca
            regs.A = (byte)(regs.A << 1);
            if (carry) regs.A |= 0x01;
        }

        private void RRA(byte cmd)
        {
            var carry = (regs.F & CpuFlags.C) != 0;
            regs.F = (byte)(CpuTables.Rrcaf[regs.A] | (regs.F & CpuFlags.SZP)); // use same table with rrca
            regs.A = (byte)(regs.A >> 1);
            if (carry) regs.A |= 0x80;
        }

        private void DAA(byte cmd)
        {
            regs.AF = CpuTables.Daaf[regs.A + 0x100 * ((regs.F & 3) + ((regs.F >> 2) & 4))];
        }

        private void CPL(byte cmd)
        {
            regs.A = (byte)~regs.A;
            regs.F = (byte)((regs.F & CpuFlags.NotF3F5) | CpuFlags.HN | (regs.A & CpuFlags.F3F5));
        }

        private void SCF(byte cmd)
        {
            //regs.F = (byte)((regs.F & (int)~(ZFLAGS.H | ZFLAGS.N)) | (regs.A & (int)(ZFLAGS.F3 | ZFLAGS.F5)) | (int)ZFLAGS.C);
            regs.F = (byte)((regs.F & CpuFlags.SZP) |
                (regs.A & CpuFlags.F3F5) |
                CpuFlags.C);
        }

        private void CCF(byte cmd)
        {
            //regs.F = (byte)(((regs.F & (int)~(ZFLAGS.N | ZFLAGS.H)) | ((regs.F << 4) & (int)ZFLAGS.H) | (regs.A & (int)(ZFLAGS.F3 | ZFLAGS.F5))) ^ (int)ZFLAGS.C);
            regs.F = (byte)((regs.F & CpuFlags.SZP) |
                ((regs.F & CpuFlags.C) != 0 ? CpuFlags.H : CpuFlags.C) | 
                (regs.A & CpuFlags.F3F5));
        }

        #endregion

        #region jmp/call/ret/jr

        private static readonly byte[] s_conds = new byte[4] 
        { 
            CpuFlags.Z, 
            CpuFlags.C, 
            CpuFlags.P, 
            CpuFlags.S 
        };

        private void DJNZ(byte cmd)      // DJNZ nn
        {
            // B==0 => 8T (5, 3)
            // B!=0 => 13T (5, 3, 5)

            RDNOMREQ(regs.IR); Tact++;

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            if (--regs.B != 0)
            {
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                regs.MW = (ushort)(regs.PC + drel);
                regs.PC = regs.MW;
            }
        }

        private void JRNN(byte cmd)      // JR nn
        {
            // 12T (4, 3, 5)

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            regs.MW = (ushort)(regs.PC + drel);
            regs.PC = regs.MW;
        }

        private void JRXNN(byte cmd)     // JR x,nn
        {
            // false => 7T (4, 3)
            // true  => 12 (4, 3, 5)
            var cond = (cmd & 0x18) >> 3;
            var mask = s_conds[cond >> 1];
            var f = regs.AF & mask;
            if ((cond & 1) != 0) f ^= mask;

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            regs.PC++;
            
            if (f == 0)
            {
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                RDNOMREQ(regs.PC); Tact++;
                regs.MW = (ushort)(regs.PC + drel);
                regs.PC = regs.MW;
            }
        }

        private void CALLNNNN(byte cmd)  // CALL
        {
            // 17T (4, 3, 4, 3, 3)

            regs.MW = RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            regs.MW += (ushort)(RDMEM(regs.PC) * 0x100); Tact += 3;
            RDNOMREQ(regs.PC); Tact++;
            regs.PC++;
            regs.SP--;

            WRMEM(regs.SP, (byte)(regs.PC >> 8)); Tact += 3;
            regs.SP--;

            WRMEM(regs.SP, (byte)regs.PC); Tact += 3;
            regs.PC = regs.MW;
        }

        private void CALLXNNNN(byte cmd) // CALL x,#nn
        {
            // false => 10T (4, 3, 3)
            // true  => 17T (4, 3, 4, 3, 3)
            var cond = (cmd & 0x38) >> 3;
            var mask = s_conds[cond >> 1];
            var f = regs.AF & mask;
            if ((cond & 1) != 0) f ^= mask;

            regs.MW = RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            regs.MW += (ushort)(RDMEM(regs.PC) * 0x100); Tact += 3;
            if (f == 0)
            {
                RDNOMREQ(regs.PC); Tact++;
            }
            regs.PC++;

            if (f == 0)
            {
                regs.SP--;

                WRMEM(regs.SP, (byte)(regs.PC >> 8)); Tact += 3;
                regs.SP--;

                WRMEM(regs.SP, (byte)regs.PC); Tact += 3;
                regs.PC = regs.MW;
            }
        }

        private void RET(byte cmd)       // RET
        {
            // 10T (4, 3, 3)

            regs.MW = RDMEM(regs.SP); Tact += 3;
            regs.SP++;

            regs.MW += (ushort)(RDMEM(regs.SP) * 0x100); Tact += 3;
            regs.SP++;
            regs.PC = regs.MW;
        }

        private void RETX(byte cmd)      // RET x
        {
            // false => 5T (5)
            // true  => 11T (5, 3, 3)
            var cond = (cmd & 0x38) >> 3;
            var mask = s_conds[cond >> 1];
            var f = regs.AF & mask;
            if ((cond & 1) != 0) f ^= mask;

            RDNOMREQ(regs.IR); Tact++;

            if (f == 0)
            {
                regs.MW = RDMEM(regs.SP); Tact += 3;
                regs.SP++;

                regs.MW += (ushort)(RDMEM(regs.SP) * 0x100); Tact += 3;
                regs.SP++;
                regs.PC = regs.MW;
            }
        }

        private void JPNNNN(byte cmd)    // JP nnnn
        {
            // 10T (4, 3, 3)

            regs.MW = RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            regs.MW += (ushort)(RDMEM(regs.PC) * 0x100); Tact += 3;
            regs.PC = regs.MW;
        }

        private void JPXNN(byte cmd)     // JP x,#nn ???
        {
            // 10T (4, 3, 3)
            var cond = (cmd & 0x38) >> 3;
            var mask = s_conds[cond >> 1];
            var f = regs.AF & mask;
            if ((cond & 1) != 0) f ^= mask;

            regs.MW = RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            regs.MW += (ushort)(RDMEM(regs.PC) * 0x100); Tact += 3;
            regs.PC++;

            if (f == 0)
                regs.PC = regs.MW;
        }

        private void RSTNN(byte cmd)     // RST #nn ?TIME?
        {
            // 11T (5, 3, 3)
            var rst = (ushort)(cmd & 0x38);

            RDNOMREQ(regs.IR); Tact++;
            regs.SP--;

            WRMEM(regs.SP, (byte)(regs.PC >> 8)); Tact += 3;
            regs.SP--;

            WRMEM(regs.SP, (byte)regs.PC); Tact += 3;
            regs.MW = rst;
            regs.PC = regs.MW;
        }

        #endregion

        #region push/pop

        private void PUSHRR(byte cmd)    // PUSH RR ?TIME?
        {
            // 11T (5, 3, 3)
            var rr = (cmd & 0x30) >> 4;

            RDNOMREQ(regs.IR); Tact += 1;
            ushort val = rr == CpuRegId.Sp ? regs.AF : _pairGetters[rr]();

            regs.SP--;
            WRMEM(regs.SP, (byte)(val >> 8)); Tact += 3;

            regs.SP--;
            WRMEM(regs.SP, (byte)val); Tact += 3;
        }

        private void POPRR(byte cmd)     // POP RR
        {
            // 10T (4, 3, 3)
            var rr = (cmd & 0x30) >> 4;

            var val = (ushort)RDMEM(regs.SP);
            regs.SP++;
            Tact += 3;

            val |= (ushort)(RDMEM(regs.SP) << 8);
            regs.SP++;
            if (rr == CpuRegId.Sp) regs.AF = val;
            else _pairSetters[rr](val);
            Tact += 3;
        }

        #endregion

        #region ALU

        private void ALUAN(byte cmd)
        {
            // 7T (4, 3)
            var op = (cmd & 0x38) >> 3;

            var val = RDMEM(regs.PC);
            regs.PC++;
            _alualg[op](val);
            Tact += 3;
        }

        private void ALUAR(byte cmd)     // ADD/ADC/SUB/SBC/AND/XOR/OR/CP A,R
        {
            var r = cmd & 0x07;
            var op = (cmd & 0x38) >> 3;

            _alualg[op](_regGetters[r]());
        }

        private void ALUA_HL_(byte cmd)     // ADD/ADC/SUB/SBC/AND/XOR/OR/CP A,(HL)
        {
            // 7T (4, 3)
            var op = (cmd & 0x38) >> 3;

            var val = RDMEM(regs.HL);
            _alualg[op](val);
            Tact += 3;
        }

        private void ADDHLRR(byte cmd)   // ADD HL,RR
        {
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            regs.MW = (ushort)(regs.HL + 1);
            regs.HL = ALU_ADDHLRR(regs.HL, _pairGetters[(cmd & 0x30) >> 4]());
        }

        #endregion

        #region loads

        private void LDA_RR_(byte cmd)   // LD A,(RR)
        {
            // 7T (4, 3)
            var rr = (cmd & 0x30) >> 4;

            var rrValue = _pairGetters[rr]();
            regs.A = RDMEM(rrValue);
            regs.MW = (ushort)(rrValue + 1);
            Tact += 3;
        }

        private void LDA_NN_(byte cmd)   // LD A,(nnnn)
        {
            // 13T (4, 3, 3, 3)

            ushort adr = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            Tact += 3;

            regs.A = RDMEM(adr);
            regs.MW = (ushort)(adr + 1);
            Tact += 3;
        }

        private void LDHL_NN_(byte cmd)   // LD HL,(nnnn)
        {
            // 16T (4, 3, 3, 3, 3)

            var adr = (ushort)RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            Tact += 3;

            var val = (ushort)RDMEM(adr);
            regs.MW = (ushort)(adr + 1);
            Tact += 3;

            val += (ushort)(RDMEM(regs.MW) * 0x100);
            regs.HL = val;
            Tact += 3;
        }

        private void LD_RR_A(byte cmd)   // LD (RR),A
        {
            // 7T (4, 3)
            var rr = (cmd & 0x30) >> 4;

            var rrValue = _pairGetters[rr]();
            WRMEM(rrValue, regs.A);
            regs.MH = regs.A;
            regs.ML = (byte)(rrValue + 1);
            Tact += 3;
        }

        private void LD_NN_A(byte cmd)   // LD (nnnn),A
        {
            // 13T (4, 3, 3, 3)

            ushort adr = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            Tact += 3;

            WRMEM(adr, regs.A);
            regs.MH = regs.A;
            regs.ML = (byte)(adr + 1);
            Tact += 3;
        }

        private void LD_NN_HL(byte cmd)   // LD (nnnn),HL
        {
            // 16T (4, 3, 3, 3, 3)

            ushort adr = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            Tact += 3;

            WRMEM(adr, regs.L);
            regs.MW = (ushort)(adr + 1);
            Tact += 3;

            WRMEM(regs.MW, regs.H);
            Tact += 3;
        }

        private void LDRRNNNN(byte cmd)  // LD RR,nnnn
        {
            // 10T (4, 3, 3)
            var rr = (cmd & 0x30) >> 4;

            ushort val = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            val |= (ushort)(RDMEM(regs.PC) << 8);
            regs.PC++;
            _pairSetters[rr](val);
            Tact += 3;
        }

        private void LDRNN(byte cmd)     // LD R,nn
        {
            // 7T (4, 3)
            var r = (cmd & 0x38) >> 3;

            _regSetters[r](RDMEM(regs.PC));
            regs.PC++;
            Tact += 3;
        }

        private void LD_HL_NN(byte cmd)     // LD (HL),nn
        {
            // 10T (4, 3, 3)

            var val = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;

            WRMEM(regs.HL, val);
            Tact += 3;
        }

        private void LDRdRs(byte cmd)     // LD R1,R2
        {
            var rsrc = cmd & 0x07;
            var rdst = (cmd & 0x38) >> 3;
            _regSetters[rdst](_regGetters[rsrc]());
        }

        private void LD_HL_R(byte cmd)    // LD (HL),R
        {
            // 7T (4, 3)
            var r = cmd & 0x07;

            WRMEM(regs.HL, _regGetters[r]());
            Tact += 3;
        }

        private void LDR_HL_(byte cmd)    // LD R,(HL)
        {
            // 7T (4, 3)
            var r = (cmd & 0x38) >> 3;
            
            _regSetters[r](RDMEM(regs.HL));
            Tact += 3;
        }

        #endregion

        #region INC/DEC

        private void DECRR(byte cmd)     // DEC RR
        {
            var rr = (cmd & 0x30) >> 4;

            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            _pairSetters[rr]((ushort)(_pairGetters[rr]() - 1));
        }

        private void INCRR(byte cmd)     // INC RR
        {
            var rr = (cmd & 0x30) >> 4;

            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            _pairSetters[rr]((ushort)(_pairGetters[rr]() + 1));
        }

        private void DECR(byte cmd)      // DEC R
        {
            var r = (cmd & 0x38) >> 3;

            _regSetters[r](ALU_DECR(_regGetters[r]()));
        }

        private void INCR(byte cmd)      // INC R
        {
            var r = (cmd & 0x38) >> 3;

            _regSetters[r](ALU_INCR(_regGetters[r]()));
        }

        private void DEC_HL_(byte cmd)      // DEC (HL)
        {
            // 11T (4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_DECR(val);

            WRMEM(regs.HL, val); Tact += 3;
        }

        private void INC_HL_(byte cmd)      // INC (HL)
        {
            // 11T (4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_INCR(val);

            WRMEM(regs.HL, val); Tact += 3;
        }

        #endregion

        private void HALT(byte cmd)
        {
            HALTED = true;
            regs.PC--;      // workaround for Z80 snapshot halt issue + comfortable debugging
        }
    }
}
