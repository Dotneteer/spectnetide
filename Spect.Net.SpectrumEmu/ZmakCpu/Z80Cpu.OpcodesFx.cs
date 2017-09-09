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
 *  Description: Z80 CPU Emulator [DD/FD prefixed opcode part]
 *  Date: 13.04.2007
 * 
 */
using System;


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        #region FXxx ops...

        private void FX_LDSPHL(byte cmd)       // LD SP,IX 
        {
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            if (FX == CpuModeIndex.Ix)
                regs.SP = regs.IX;
            else
                regs.SP = regs.IY;
        }

        private void FX_EX_SP_HL(byte cmd)     // EX (SP),IX
        {
            // 23T (4, 4, 3, 4, 3, 5)
            
            var tmpsp = regs.SP;
            regs.MW = RDMEM(tmpsp); Tact += 3;
            tmpsp++;

            regs.MW += (ushort)(RDMEM(tmpsp) * 0x100); Tact += 3;
            RDNOMREQ(tmpsp); Tact++;

            if (FX == CpuModeIndex.Ix)
            {
                WRMEM(tmpsp, regs.XH); Tact += 3;
                tmpsp--;

                WRMEM(tmpsp, regs.XL); Tact += 3;
                WRNOMREQ(tmpsp); Tact++;
                WRNOMREQ(tmpsp); Tact++;
                regs.IX = regs.MW;
            }
            else
            {
                WRMEM(tmpsp, regs.YH); Tact += 3;
                tmpsp--;

                WRMEM(tmpsp, regs.YL); Tact += 3;
                WRNOMREQ(tmpsp); Tact++;
                WRNOMREQ(tmpsp); Tact++;
                regs.IY = regs.MW;
            }
        }

        private void FX_JP_HL_(byte cmd)       // JP (IX) 
        {
            if (FX == CpuModeIndex.Ix)
                regs.PC = regs.IX;
            else
                regs.PC = regs.IY;
        }

        private void FX_PUSHIX(byte cmd)       // PUSH IX
        {
            // 15 (4, 5, 3, 3)

            RDNOMREQ(regs.IR); Tact++;
            var val = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.SP--;

            WRMEM(regs.SP, (byte)(val >> 8)); Tact += 3;
            regs.SP--;

            WRMEM(regs.SP, (byte)val); Tact += 3;
        }

        private void FX_POPIX(byte cmd)        // POP IX
        {
            // 14T (4, 4, 3, 3)

            var val = (ushort)RDMEM(regs.SP);
            regs.SP++;
            Tact += 3;
            
            val |= (ushort)(RDMEM(regs.SP) << 8);
            regs.SP++;
            if (FX == CpuModeIndex.Ix)
                regs.IX = val;
            else
                regs.IY = val;
            Tact += 3;
        }

        private void FX_ALUAXH(byte cmd)       // ADD/ADC/SUB/SBC/AND/XOR/OR/CP A,XH
        {
            byte val;
            if (FX == CpuModeIndex.Ix)
                val = (byte)(regs.IX >> 8);
            else
                val = (byte)(regs.IY >> 8);
            _alualg[(cmd & 0x38) >> 3](val);
        }

        private void FX_ALUAXL(byte cmd)       // ADD/ADC/SUB/SBC/AND/XOR/OR/CP A,XL
        {
            byte val;
            if (FX == CpuModeIndex.Ix)
                val = (byte)regs.IX;
            else
                val = (byte)regs.IY;
            _alualg[(cmd & 0x38) >> 3](val);
        }

        private void FX_ALUA_IX_(byte cmd)     // ADD/ADC/SUB/SBC/AND/XOR/OR/CP A,(IX)
        {
            // 19T (4, 4, 3, 5, 3)
            var op = (cmd & 0x38) >> 3;

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;

            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            
            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);

            var val = RDMEM(regs.MW); Tact += 3;
            _alualg[op](val);
        }

        private void FX_ADDIXRR(byte cmd)      // ADD IX,RR
        {
            var rr = (cmd & 0x30) >> 4;
            
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            
            ushort rde;
            switch (rr)
            {
                case 0: rde = regs.BC; break;
                case 1: rde = regs.DE; break;
                case 2: rde = FX == CpuModeIndex.Ix ? regs.IX : regs.IY; break;
                case 3: rde = regs.SP; break;
                default: throw new ArgumentOutOfRangeException("(cmd & 0x30) >> 4");
            }
            if (FX == CpuModeIndex.Ix)
            {
                regs.MW = (ushort)(regs.IX + 1);
                regs.IX = ALU_ADDHLRR(regs.IX, rde);
            }
            else
            {
                regs.MW = (ushort)(regs.IY + 1);
                regs.IY = ALU_ADDHLRR(regs.IY, rde);
            }
        }

        private void FX_DECIX(byte cmd)        // DEC IX
        {
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            if (FX == CpuModeIndex.Ix)
                regs.IX--;
            else
                regs.IY--;
        }

        private void FX_INCIX(byte cmd)        // INC IX
        {
            RDNOMREQ(regs.IR); Tact++;
            RDNOMREQ(regs.IR); Tact++;
            if (FX == CpuModeIndex.Ix)
                regs.IX++;
            else
                regs.IY++;
        }

        private void FX_LDIX_N_(byte cmd)      // LD IX,(nnnn)
        {
            // 20 (4, 4, 3, 3, 3, 3)

            var adr = (ushort)RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;
            
            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            regs.MW = (ushort)(adr + 1);
            Tact += 3;

            var val = (ushort)RDMEM(adr);
            Tact += 3;
            
            val += (ushort)(RDMEM(regs.MW) * 0x100);
            if (FX == CpuModeIndex.Ix)
                regs.IX = val;
            else
                regs.IY = val;
            Tact += 3;
        }

        private void FX_LD_NN_IX(byte cmd)     // LD (nnnn),IX
        {
            // 20 (4, 4, 3, 3, 3, 3)
            
            var hl = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            var adr = (ushort)RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;
            
            adr += (ushort)(RDMEM(regs.PC) * 0x100);
            regs.PC++;
            regs.MW = (ushort)(adr + 1);
            Tact += 3;

            WRMEM(adr, (byte)hl);
            Tact += 3;

            WRMEM(regs.MW, (byte)(hl >> 8));
            Tact += 3;
        }

        private void FX_LDIXNNNN(byte cmd)     // LD IX,nnnn
        {
            // 14 (4, 4, 3, 3)

            var val = (ushort)RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;
            
            val |= (ushort)(RDMEM(regs.PC) << 8);
            regs.PC++;
            if (FX == CpuModeIndex.Ix)
                regs.IX = val;
            else
                regs.IY = val;
            Tact += 3;
        }

        private void FX_DEC_IX_(byte cmd)      // DEC (IX)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;

            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            
            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);

            var val = RDMEM(regs.MW); Tact += 3;
            RDNOMREQ(regs.MW); Tact++;
            val = ALU_DECR(val);
            WRMEM(regs.MW, val); Tact += 3;
        }

        private void FX_INC_IX_(byte cmd)      // INC (IX)
        {
            //23T (4, 4, 3, 5, 4, 3)

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;

            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);

            var val = RDMEM(regs.MW); Tact += 3;
            RDNOMREQ(regs.MW); Tact++;
            val = ALU_INCR(val);
            WRMEM(regs.MW, val); Tact += 3;
        }

        private void FX_LD_IX_NN(byte cmd)     // LD (IX),nn
        {
            // 19 (4, 4, 3, 5, 3)

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            regs.PC++;

            var val = RDMEM(regs.PC); Tact += 3;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);
            WRMEM(regs.MW, val); Tact += 3;
        }

        private void FX_LD_IX_R(byte cmd)      // LD (IX),R
        {
            // 19T (4, 4, 3, 5, 3)
            var r = cmd & 0x07;

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;

            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;

            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);
            WRMEM(regs.MW, _regGetters[r]()); Tact += 3;
        }

        private void FX_LDR_IX_(byte cmd)      // LD R,(IX)
        {
            // 19T (4, 4, 3, 5, 3)
            var r = (cmd & 0x38) >> 3;

            int drel = (sbyte)RDMEM(regs.PC); Tact += 3;
            
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            RDNOMREQ(regs.PC); Tact++;
            
            regs.PC++;
            regs.MW = FX == CpuModeIndex.Ix ? regs.IX : regs.IY;
            regs.MW = (ushort)(regs.MW + drel);

            _regSetters[r](RDMEM(regs.MW)); Tact += 3;
        }

        #endregion

        #region RdRs...

        private void FX_LDHL(byte cmd)
        {
            if (FX == CpuModeIndex.Ix)
                regs.XH = regs.XL;
            else
                regs.YH = regs.YL;
        }

        private void FX_LDLH(byte cmd)
        {
            if (FX == CpuModeIndex.Ix)
                regs.XL = regs.XH;
            else
                regs.YL = regs.YH;
        }

        private void FX_LDRL(byte cmd)
        {
            var r = (cmd & 0x38) >> 3;

            if (FX == CpuModeIndex.Ix)
                _regSetters[r](regs.XL);
            else
                _regSetters[r](regs.YL);
        }

        private void FX_LDRH(byte cmd)
        {
            var r = (cmd & 0x38) >> 3;
            
            if (FX == CpuModeIndex.Ix)
                _regSetters[r](regs.XH);
            else
                _regSetters[r](regs.YH);
        }

        private void FX_LDLR(byte cmd)
        {
            var r = cmd & 0x07;
            
            if (FX == CpuModeIndex.Ix)
                regs.XL = _regGetters[r]();
            else
                regs.YL = _regGetters[r]();
        }

        private void FX_LDHR(byte cmd)
        {
            var r = cmd & 0x07;
            
            if (FX == CpuModeIndex.Ix)
                regs.XH = _regGetters[r]();
            else
                regs.YH = _regGetters[r]();
        }

        private void FX_LDLNN(byte cmd)     // LD XL,nn
        {
            // 11T (4, 4, 3)
            
            if (FX == CpuModeIndex.Ix)
                regs.XL = RDMEM(regs.PC);
            else
                regs.YL = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;
        }

        private void FX_LDHNN(byte cmd)     // LD XH,nn
        {
            // 11T (4, 4, 3)
            
            if (FX == CpuModeIndex.Ix)
                regs.XH = RDMEM(regs.PC);
            else
                regs.YH = RDMEM(regs.PC);
            regs.PC++;
            Tact += 3;
        }

        private void FX_INCL(byte cmd)      // INC XL
        {
            if (FX == CpuModeIndex.Ix)
                regs.XL = ALU_INCR(regs.XL);
            else
                regs.YL = ALU_INCR(regs.YL);
        }

        private void FX_INCH(byte cmd)      // INC XH
        {
            if (FX == CpuModeIndex.Ix)
                regs.XH = ALU_INCR(regs.XH);
            else
                regs.YH = ALU_INCR(regs.YH);
        }

        private void FX_DECL(byte cmd)      // DEC XL
        {
            if (FX == CpuModeIndex.Ix)
                regs.XL = ALU_DECR(regs.XL);
            else
                regs.YL = ALU_DECR(regs.YL);
        }

        private void FX_DECH(byte cmd)      // DEC XH
        {
            if (FX == CpuModeIndex.Ix)
                regs.XH = ALU_DECR(regs.XH);
            else
                regs.YH = ALU_DECR(regs.YH);
        }

        #endregion
    }
}
